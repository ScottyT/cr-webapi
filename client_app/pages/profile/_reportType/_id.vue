<template>
    <!-- <p v-if="$fetchState.pending">Fetching content...</p> -->
    <span v-if="reportType === 'psychrometric-chart'">
        <h1>{{formName}} for job {{jobId}}</h1>
        <LazyLayoutPsychrometricChart buttonDisabled class="chart" />
    </span>
    <span v-else><div class="pa-6 report-details-wrapper" v-if="$nuxt.isOnline && error">
        <SavedLogReports :formName="formName" :reportType="reportType" :company="companyName" />
    </div>
    <div class="pa-6 report-details-wrapper" v-else-if="$nuxt.isOnline && !error">
        <SavedLogReports :company="companyName" :reportType="reportType" :formName="formName" />
    </div>   
    <div class="pa-6 report-details-wrapper" v-else>
        <SavedLogReports :formName="formName" :reportType="reportType" :company="companyName" :report="savedreports" />
    </div></span>
</template>
<script>
import { computed, defineComponent, ref, useStore } from '@nuxtjs/composition-api';
import useReports from '@/composable/reports';
export default defineComponent({
    setup(props, {root}) {
        const store = useStore()
        let authUser = root.$fire.auth.currentUser
        const formName = ref(""); const company = ref(""); const offlinereport = ref({});
        const reps = computed(() =>store.getters['indexDb/getSavedReports'])
        const savedreports = computed(() => {           
            return reps.value.find(
                report => report.ReportType === root.$route.params.reportType && report.JobId === root.$route.params.id
            )
        })
        let reportType = root.$route.params.reportType
        let jobId = root.$route.params.id
        const { report, error } = useReports()
        switch (reportType) {
            case "atmospheric-readings":
                formName.value = "Atmospheric Readings"
                break;
            case "quantity-inventory-logs":
                formName.value = "Unit Quantity and Equipment Inventory"
                break;
            case "moisture-map":
                formName.value = "Moisture Readings Map Readings"
                    break;
            default:
                formName.value = "Sketch Form"
        }
        if (root.$nuxt.isOnline) {
            //getReport(`${reportType}/${jobId}`, authUser).fetchReport()
        }
        
        if (report.error !== '') {
            report.value = savedreports.value
        }
        
        return {
            formName, 
            companyName: company.value, 
            jobId, 
            report,
            offlinereport, 
            savedreports, 
            reportType,
            error: computed(() => error.value)
        }
    }
})
</script>