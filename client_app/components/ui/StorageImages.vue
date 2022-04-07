<template>
<!-- Mainly used for PDF -->
    <section>
        <!-- <slot name="header"></slot> -->
        <h3>{{subPath}}</h3>
        <div class="report-details__image" v-for="(image, i) in images" :key="`image-${i}`"><img :src="image.imageUrl" /></div>
        <span v-if="error">{{errorMessage}}</span>
    </section>
</template>
<script>
import { defineComponent, onMounted, watch, ref, toRefs, useContext } from '@nuxtjs/composition-api'
import useReports from '@/composable/reports'
export default defineComponent({
    props: {
        subPath: String,
        path: String,
        jobid: String,
        imageArr: Array
    },
    setup(props, { refs }) {
        const { jobid, subPath, path } = toRefs(props)
        const { getReportImages, error, errorMessage, images } = useReports()
        getReportImages(jobid.value, path.value, subPath.value, "").fetchImages()
        
        return {
            images, error, errorMessage
        }
    },
})
</script>
