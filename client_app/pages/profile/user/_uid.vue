<template>
    <div class="user-page px-5">
        <v-card light color="white" max-width="250" max-height="250" elevation="2" class="user-card avatar-card">
            <h4 class="user-card__name"></h4>
            <p class="text">{{userObj.email}}</p>
            <p class="text">{{userObj.role}}</p>
            <form enctype="multipart/form-data" @submit.prevent="uploadAvatar">
            <h4>Click to change avatar</h4>
            <!-- Could add ValidationProvider here and make UiImageUpload accept files too -->
                <UiImageUpload @getFiles="avatar = $event" :errorText="error" :email="userObj.email" :maxSize="1024" class="user-card__actions"
                    namechanged="avatar">
                    <template v-slot:activator>
                        <v-avatar size="150px" v-ripple v-if="Object.keys(avatar).length === 0">
                            <img :src="$auth.user.picture" />
                        </v-avatar>
                        <v-avatar size="150px" v-ripple v-else-if="avatar.length > 0">
                            <img :src="avatar[0].image.imageUrl" />
                        </v-avatar>
                        <!-- <v-avatar size="150px" v-ripple v-if="Object.keys(avatar).length === 0 && avatarurl === null" class="grey lighten-3 mb-3">
                            Click to add avatar
                        </v-avatar> -->
                    </template>
                </UiImageUpload>
                <v-btn v-show="selectedAvatar" ref="uploadBtn" class="primary button--normal" type="submit" :loading="saving">
                    Save Avatar
                    <v-icon right dark>
                        mdi-cloud-upload
                    </v-icon>
                </v-btn>
            </form>
        </v-card>
        <v-card light elevation="2" class="user-card user-page__certifications">
            <h2>Certifications</h2>
            <p>{{$certs.state.message}}</p>
            <button type="button" class="button button--normal" v-if="$certs.state.certifications.length > 0" @click="deleteMode = !deleteMode">
                {{deleteMode ? 'Enter view mode' : 'Enter delete mode'}}
            </button>
            <div class="user-page__certifications--content">
                <div :ref="`certs-${i}`" class="user-page__certification" v-for="(cert, i) in $certs.state.certifications" :key="`cert-${i}`">
                    <h3>{{cert.idNumber}}</h3>
                    <img v-if="cert.hasOwnProperty('badge')" :src="cert.badge.imageUrl" class="user-page__certification--image" />
                    <p>{{cert.description}}</p>
                    <p>{{cert.expiration}}</p>
                    
                    <v-dialog transition="dialog-bottom-transition" max-width="300px" v-if="deleteMode">
                        <template v-slot:activator="{ on, attrs }">
                            <span class="button button--normal" v-bind="attrs" v-on="on">Delete this?</span>
                        </template>
                        <template v-slot:default="dialog">
                            <div class="modal__content">
                                <p>You are about to delete a certification. Are you sure you want to do this?</p>
                            </div>
                            <div class="modal__footer">
                                <v-btn text @click="deleteCert(cert, i)">Delete</v-btn>
                                <v-btn text @click="dialog.value = false">Close</v-btn>
                            </div>
                        </template>
                    </v-dialog>
                </div>
                <div class="user-page__certification user-page__certification--add">
                    <button type="button" class="button button--normal" @click="isAdding = !isAdding">Add a Certification</button>
                    <ValidationObserver ref="form" v-slot="{handleSubmit}" v-if="isAdding">
                        <form class="form" @submit.prevent="handleSubmit(addCertification)">
                            <ValidationProvider v-slot="{errors, ariaMsg}" name="ID" rules="required" class="form__input-group form__input-group--normal">
                                <label for="id" class="form__label">ID Number</label>
                                <input id="idNumber" type="text" class="form__input" v-model="idNumber" />
                                <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                            </ValidationProvider>
                            <ValidationProvider v-slot="{errors, ariaMsg}" name="Description" class="form__input-group form__input-group--normal">
                                <label for="description" class="form__label">Description</label>
                                <input id="description" type="text" class="form__input" v-model="description" />
                                <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                            </ValidationProvider>
                            <div class="form__input-group form__input-group--normal">
                                <label for="expDate" class="form__label">Expiration Date</label>
                                <imask-input id="expDate" @complete="expDate = $event" :lazy="false" :blocks="dateMask.blocks" :mask="dateMask.mask" :format="dateMask.format" 
                                    :parse="dateMask.parse" :pattern="dateMask.pattern" class="form__input" />
                            </div>
                            <ValidationProvider v-slot="{errors, ariaMsg}" name="Badge" class="upload-group upload-group--sm" >
                                <input type="hidden" v-model="badge[0]" />
                                <UiImageUpload @getFiles="badge = $event" :maxSize="2048" :email="userObj.email" :errorText="error">
                                    <template v-slot:activator>
                                        <img v-if="badge.length > 0" class="file-listing__preview" :src="badge[0].image.imageUrl" />
                                        <button type="button" class="button button--normal">Add image</button>
                                    </template>
                                </UiImageUpload>
                                <v-btn :loading="saving" v-show="badge.length > 0" ref="uploadBtn" class="primary button--normal" type="button" @click="uploadBadge">
                                    Save image
                                    <v-icon right dark>mdi-cloud-upload</v-icon>
                                </v-btn>
                                
                                <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                            </ValidationProvider>
                            <div v-show="$certs.state.message !== ''">{{$certs.state.message}}</div>
                            <button type="submit" class="button button--normal">{{ $certs.state.submitting ? 'Submitting' : 'Submit' }}</button>
                        </form>
                    </ValidationObserver>
                </div>
            </div>
        </v-card>
        <div class="block-group user-page__reports">
            <button @click="refreshReports" class="button--normal">Refresh</button>
            <p v-if="$userReports.state.loading">Fetching reports...</p>
            <LayoutReports v-else :reports="$userReports.state.reports" theme="dark" flex />
        </div>
    </div>
</template>
<script>
import axios from 'axios';
import 'animate.css'
import { ref, computed, onMounted, defineComponent, useStore, useFetch, watch, useContext } from '@nuxtjs/composition-api'
import useUsers from "@/composable/users";
import { dateMask } from "@/data/masks";
import { compress } from 'image-conversion';
export default defineComponent({
    middleware: ['auth'],
    setup(props, { root, refs }) {
        const { $gcs, $auth, $certs, $userReports, $api } = useContext()
        const store = useStore()
        //const { fetchUser, userObj } = useUsers()
        const saving = ref(false)
        const saved = ref(false)
        const avatar = ref({})
        const error = ref('')
        const selectedAvatar = ref(false)
        const avatarurl = computed(() => store.getters['users/getAvatar'])
        const userObj = computed(() => $auth.user)
        const idNumber = ref(""); const description = ref(""); const expDate = ref(""); const badge = ref([]);
        const isAdding = ref(false); const deleteMode = ref(false);
        const submitting = computed(() => $certs.state.submitting)
        const tempImage = ref({}) //has imageUrl and name property


        const refreshReports = async () => {
            await $userReports.fetchReports(userObj.value.email)
        }
        const compressing = async (upload) => {
            var compressedFiles = []
            var formData = new FormData()
            const res = await compress(upload.image.image, {
                quality: .7,
                scale: .8
            })
            let compressedImg = new File([res], upload.image.imageName, {
                type: res.type
            })
            formData.append("single", compressedImg)
            
            //Object.assign(compressedFile, {formData, imageName: upload.image.imageName, image: compressedImg, imageUrl: upload.image.imageUrl})
            compressedFiles.push({formData, compressedImg})
            return compressedFiles
        }
        const animateCSS = (element, animation, prefix = 'animate__') =>
            new Promise((resolve, reject) => {
                const animationName = `${prefix}${animation}`;
                var node;
                if (refs[element]._isVue) {
                    node = refs[element].$el
                } else if (refs[element].length === 1) {
                    node = refs[element][0]
                } else {
                    node = refs[element]
                }
                node.classList.add(`${prefix}animated`, animationName);
                function handleAnimationEnd(event) {
                    node.classList.remove(`${prefix}animated`, animationName);
                    resolve('Animation ended');
                }

                node.addEventListener('animationend', handleAnimationEnd);
            });
        const uploadAvatar = async () => {
            await uploadFile().then((image) => {
                store.commit('users/setAvatar', image.imageUrl)
                $api.$put(`/api/employees/update/${userObj.value.sub}`, {picture: image.imageUrl, auth_id: userObj.value.sub}).then((res) => {
                    console.log("Updated user")
                })
                saving.value = false
            })
            animateCSS('uploadBtn', 'hinge').then((message) => {
                avatar.value = {}
                selectedAvatar.value = false
            })
        }
        const uploadBadge = async() => {
            await uploadCertImage()
            animateCSS('uploadBtn', 'hinge').then((message) => {
            })
        }
        async function addCertification() {
            const post = {
                idNumber: idNumber.value,
                description: description.value,
                expDate: expDate.value,
                user: userObj.value.email,
                badge: tempImage.value
            }
            $certs.addCert(post, userObj.value)
            badge.value = []
        }
        async function uploadFile() {
            saving.value = true
            return new Promise((resolve, reject) => {
                compressing(avatar.value[0]).then((result) => {
                    console.log(result)
                    result[0].formData.append('user', userObj.value.email)
                    result[0].formData.append('name', 'avatar')
                    axios.post(`${process.env.gsutil}/upload/avatar`, result[0].formData, {
                        headers: {
                            'Content-Type': 'multipart/form-data',
                            authorization: $auth.strategy.token.get()
                        }
                    }).then((res) => {
                        resolve(res.data)
                    }).catch((err) => {
                        error.value = err
                        reject(err)
                    })
                })
            })
        }
        async function uploadCertImage() {
            badge.value[0].formData.append('user', userObj.value.email)
            saving.value = true
            return new Promise((resolve, reject) => {
                $fire.auth.currentUser.getIdToken().then((idToken) => {
                    axios.post(`${process.env.gsutil}/upload/cert`, badge.value[0].formData, {
                        headers: {
                            'Content-Type': 'multipart/form-data',
                            authorization: `Bearer ${idToken}`
                        }
                    }).then((res) => {
                        tempImage.value = {
                            imageUrl: res.data.imageUrl,
                            name: res.data.imageName
                        }
                        resolve(res.data)
                        saving.value = false
                    }).catch((err) => {
                        error.value = err
                        reject(err)
                    })
                }).catch((error) => {
                    console.log(`Not authenticated: ${error}`)
                })
            })
        }
        const deleteCert = async (obj, key) => {
            await $certs.deleteCert(obj)
            animateCSS(`certs-${key}`, 'rollOut').then((message) => {
            })        
        }
        
        /* watch(badge, (val) => {
            fetchUser(user.value)
        }) */
        watch(submitting, (val) => {
            isAdding.value = val
        })
        watch(avatar, (val) => {
            if (val.length > 0) {
                selectedAvatar.value = true
                animateCSS('uploadBtn', 'fadeInRight')
            }
        })
        //$certs.fetchCerts(userObj.value)
        /* fetchUser($fire.auth.currentUser.email).then((result) => {
            $certs.fetchCerts(userObj.value)
        }) */
        
        return {
            //loading,
           // reports: computed(() => reports.value),
            uploadAvatar,
            userObj,
            uploadFile,
            avatar,
            saving,
            saved,
            error,
            idNumber, description, expDate, badge,
            submitting,
            refreshReports,
            addCertification,
            dateMask,
            selectedAvatar,
            isAdding, deleteMode,
            deleteCert,
            uploadBadge
        }
    }
})
</script>
<style lang="scss">
.user-page {
    max-width:1200px;
    margin:40px 0;
    display:grid;
    grid-template-columns:1fr 1fr;
    row-gap:40px;
    grid-template-areas: 'avatar certifications'
        'reports reports';
    @include respond(tabletLargeMax) {
        grid-template-areas: 'certifications certifications'
            'avatar .'
            'reports reports';
    }
    &__reports {
        grid-area:reports;
    }
    &__certifications {
        grid-area:certifications;
        &--content {
            display:flex;
            flex-wrap: wrap;
            column-gap:30px;
        }
    }
    &__avatar-card {
        grid-area: avatar;
    }
    &__certification {
        width:200px;
        &--image {
            width:100%;
            height:150px;
            object-fit:contain;
        }
    }
}
.user-card {
    padding:10px 15px;
}
</style>