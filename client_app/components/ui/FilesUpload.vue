<template>
<!-- This is for firebase uploads only that are done client-side v-for="(item, i) in uploadimages" :key="i" -->
    <div>
        <UiImageUpload @getFiles="filePreviews($event)" :multipleFiles="!singleImage" :namechanged="nameChanged" :maxSize="5000" 
            :uploadFieldName="fieldName" class="image-upload" >
            <template v-slot:activator>
                <span class="button--normal button" v-if="fieldName !== 'avatar'">{{buttonName !== undefined ? buttonName : 'Add Image'}}</span>
            </template>
        </UiImageUpload>
        <div class="file-listing-wrapper" v-if="inlinePreview && (!singleImage && uploadimages.length > 0)" ref="previewImages">
            <div v-for="(file, key) in uploadimages[0].imagesArr" class="file-listing" :key="`images-${key}`">
                <img class="file-listing__preview" :src="file.imageUrl"  />
                <span class="file-listing__remove-file" @click="removeFile(key)" tag="i">
                    <span class="file-listing__remove-file--leg1 file-listing__remove-file--leg"></span>
                    <span class="file-listing__remove-file--leg2 file-listing__remove-file--leg"></span>
                </span>
            </div>
        </div>
        <div class="file-listing-wrapper" v-else-if="inlinePreview && (singleImage && uploadimages.length > 0)">
            <img class="file-listing__preview" :src="uploadimages[0].image.imageUrl" />
        </div>
        <v-slide-x-transition v-if="!autoUpload">
            <v-btn dark @click="compressAndUpload(uploadimages)" :loading="uploading" v-if="(uploadimages.length > 0 || Object.keys(uploadimages).length > 0) && $nuxt.isOnline">
                {{ uploading ? 'Uploading' : 'Upload'}}
            </v-btn>
        </v-slide-x-transition>
        <span class="form__input--error">{{ errorMessage }}</span>
        <h3 v-show="uploadMessage !== ''">{{uploadMessage}}</h3>
    </div>
</template>
<script>
import { toRefs, watch, ref, useContext, computed } from '@nuxtjs/composition-api'
import {compress} from 'image-conversion';
export default {
    props: {
        singleImage: {
            type: Boolean,
            required: true
        },
        changeImageName: String,
        fieldName: String,
        subDir: String,
        path: String,
        rootPath: String,
        errors: Array,
        inlinePreviews: {
            type: Boolean,
            default: true
        },
        files: Array,
        autoUpload: Boolean,
        buttonName: String,
        delimiter: String,
        isStorage: Boolean
    },
    setup(props, { refs, emit, root }) {
        const { singleImage, rootPath, changeImageName, subDir, errors, inlinePreviews, files, autoUpload, delimiter, path, isStorage } = toRefs(props)
        const { $auth, $gcs } = useContext()
        const uploadimages = ref([])
        const uploadMessage = ref('')
        const filesUploaded = ref([])
        const uploading = ref(false)
        const errorMessage = ref('')
        const errorDialog = ref(false)
        const showUpload = ref(false)
        const errorslist = computed(() => errors.value)
        const currentFolder = computed(() => { return `${path.value !== '' ? '/'+path.value : ''}${subPath.value !== '' ? '/'+subPath.value : ''}`.trimEnd() })
        const nameChanged = computed(() => {
            return changeImageName.value
        })
        const uploadPath = computed(() => {
            if (!isStorage.value) {
                if (path.value === '') {
                    return rootPath.value + "/"
                }
                return rootPath.value + "/" + path.value + "/"
            } else {
                var pathArr = root.$route.path.split("/")
                var pathString = pathArr.slice(2).join("/")
                if (path.value === '') {
                    pathString = pathString + "/"
                }
                return pathString
            }
        });
        const removeFile = (key) => {
            const files = Array.from(uploadimages.value)
            files.splice(key, 1)
            uploadimages.value = files
        }
        const compressing = async (uploadarr) => {
            var compressedFiles = []
            var images = []
            var formData = new FormData()
            if (!singleImage.value) {
                formData.append('path', uploadPath.value)
                await Promise.all(uploadarr[0].imagesArr.map(async (file) => {
                    const res = await compress(file.image, {quality: .7, scale:.8})
                    let compressedImg = new File([res], file.imageName, {
                        type: res.type
                    })
                    formData.append("multiFiles", compressedImg)
                    images.push({image: compressedImg, imageName: file.imageName})
                }))
            } 
            if (singleImage.value) {
                formData.append('path', uploadPath.value)
                const res = await compress(uploadarr[0].image.image, {quality: .7, scale:.8})
                let compressedImg = new File([res], uploadarr[0].image.imageName, {
                    type: res.type
                })
                formData.append("multiFiles", compressedImg)
                images.push({image: compressedImg, imageName: uploadarr[0].image.imageName})
            }
            
            compressedFiles.push({formData, images})
            return compressedFiles
        }
        const compressAndUpload = async (uploadarr) => {
            uploadMessage.value = "Images are uploading..."
            // Create another one for single images
           //uploadarr[0].formData.append('path', uploadPath.value)
            compressing(uploadarr).then((result) => {
                $gcs.$post(`/upload`, result[0].formData, {
                    params: {
                        folder: path.value,
                        subfolder: path.value + "/" + subDir.value,
                        delimiter: "/"
                    }
                }).then((res) => {
                    uploadMessage.value = res.message
                    if (!inlinePreviews.value) {
                        emit("afterUpload", res.files)
                        return
                    }
                    if (!singleImage.value) uploadimages.value = []
                    //emit('sendDownloadUrl', {imageUrl: res.data.downloadUrl,imageName: uploadarr[0].imagesArr.imageName})
                })
            }).catch((err) => {
                console.log(err)
            })
        }
        
        function filePreviews(params) {
            const promises = []
            if (singleImage.value) {
                emit('sendPreviews', params)
                uploadimages.value = []
            }
            params.forEach((item, i) => {
                uploadimages.value.push(item)
                promises.push(item)
            })
            if (!inlinePreviews.value) {
                Promise.all(promises).then(result => {
                    emit('sendPreviews', uploadimages.value)
                })
            }
            if (errorMessage.value === '') {
                
            }           
        }
        function setDownloadUrls(param) {
            uploadimages.value.push(param)
        }
        watch(uploadimages, (val) => {
            if (autoUpload.value && rootPath.value !== '') {
                compressAndUpload(val)
            } else if (rootPath.value === '') {
                errorMessage.value = "Job ID is required"
            }
        })
        return {
            errorslist,
            uploading,
            showUpload,
            uploadimages,
            uploadMessage,
            inlinePreview: inlinePreviews.value,
            single: singleImage.value,
            nameChanged,
            errorMessage,
            filesUploaded,
            filePreviews,
            setDownloadUrls,
            removeFile,
            uploadPath,
            compressAndUpload
        }
    }
}
</script>
<style lang="scss">
.upload-group {
    width: 100%;
    padding: 10px;
    box-shadow: -1px 0 11px rgba(0, 0, 0, 0.2);
    display:block;
    &--sm {
        max-width: 300px;
    }
    &--lg {
        max-width:600px;
    }
}
.file-listing {
    &__remove-file {
        position: absolute;
        background-color: red;
        cursor: pointer;
        top: -5px;
        width: 22px;
        height:21px;
        right: 11px;
        border-radius: 50%;
        &--leg {
            background-color:$color-white;
            top:9px;
            width:17px;
            height:2px;
            position:absolute;
            left:3px;
            }
        &--leg1 {     
            transform:rotate(45deg);     
        }
        &--leg2 {
            transform:rotate(-45deg);
        }
    }
}
</style>