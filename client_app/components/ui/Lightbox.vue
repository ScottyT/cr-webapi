<template>
    <div class="slider__images">        
        <div class="folder-contents__content" @click.stop="!selecting ? clickedImage(item) : null" v-for="(item, i) in images" :key="i">
            <slot name="image" v-bind:image="item"></slot>
        </div>
        <v-dialog v-model="sliderDialog" :overlay-opacity=".7" content-class="slider no-box-shadow overflow-hidden" max-width="600px">
            <UiDataPagination :items="images" :itemsPerPage="imagesPerPage" :visiblePageNumbers="5" itemStyle="data-list--flex" :curPage="initImage + 1" themeMode="image-slider"
            imageSlider>
                <template v-slot:content="slotProps">
                    <v-icon class="slider__close" size="36px" @click="sliderDialog = false" right>mdi-close</v-icon>
                    <img :src="slotProps.report.imageUrl" class="slider__image" />
                </template>
            </UiDataPagination>
        </v-dialog>
    </div>
</template>
<script>
import { defineComponent, ref, computed, toRefs, watch, watchEffect } from '@nuxtjs/composition-api'

export default defineComponent({
    props: {
        images: Array,
        imagesPerPage: Number,
        selecting: Boolean
    },
    setup(props, { emit }) {
        const { images } = toRefs(props)
        const sliderDialog = ref(false)
        const activeImage = ref({})
        const initImage = computed(() => {
            if (Object.keys(activeImage.value).length === 0) return
            return images.value.findIndex(el => el.name === activeImage.value.name)
        })
        function clickedImage(image) {
            activeImage.value = image
            sliderDialog.value = true
        }
        return {
            initImage,
            clickedImage,
            sliderDialog
        }
    },
})
</script>
<style lang="scss">
.slider {
    position:relative;
    &__images {
        display:flex;
        flex-wrap:wrap;
        row-gap: 40px;
        column-gap: 40px;
    }
    &__image {
        @include respond(mobileLargeMax) {
           // object-fit: contain;
        }
    }
    &__close {
        width:36px;
        top:0px;
        right:0;
        z-index:99999;
        background-color:transparent;
        border:none;
        width:36px;
    }
}
</style>