<template>
    <p v-if="$fetchState.pending">Fetching reports...</p>
    <div class="pa-6 reports-wrapper" v-else>
        <button class="button--normal" type="button" @click="$fetch">Refresh</button>
        <div class="block-group" v-for="(item, i) in groupedReports" :key="i">
            <h3 class="reports-wrapper__heading">{{item[0].heading}}</h3>
            <LazyLayoutReports :reports="item" theme="light" />
        </div>
    </div>
</template>
<script>
import { defineComponent, ref, useStore, computed, onMounted, useContext } from '@nuxtjs/composition-api';
import useReports from '@/composable/reports';
export default defineComponent({
    middleware: 'auth',
    setup(props, {root}) {
        const store = useStore()
        const { $axios } = useContext()
        const fetching = ref(false)
        const certs = ref([])
        const isOnline = computed(() => root.$nuxt.isOnline)
        const { getReports, groupedReports, filterConditions, error, reports } = useReports()
        //Here change dispatch and rapid-response formType to initialForms when deleting the old test reports and updating job 21-0002
        filterConditions(["initialForms", "chart-report", "upholstery-form", "case-report", "logs-report", "sketch-report", "quality-control"])
        getReports(true, true, true)
        const fetchCerts = async () => {
            certs.value = await $axios.$get(`${process.env.apiUrl}/api/certifications`);
        }
        onMounted(fetchCerts)
        return { groupedReports, fetching, error, reports, certs }
    }
})
</script>