<template>
    <p v-if="$nuxt.isOffline">You must be online to view a report</p>
    <div class="report-details-wrapper" v-else>
        <v-btn dark depressed :loading="clickedOn === 0" class="button--normal" @click="generateReport(0)">Download PDF</v-btn>
        <p v-if="Object.keys(report).length === 0">Fetching content...</p>
        <div v-else><UiBreadcrumbs page="field-jacket" :displayStrip="false" />
        <span v-if="reportType === 'rapid-response'">
            <client-only>
                <vue-html2pdf :pdf-quality="2" pdf-content-width="800px" :html-to-pdf-options="htmlToPdfOptions" :paginate-elements-by-height="1300" :enable-download="false" 
                    @hasDownloaded="hasDownloaded($event)" @beforeDownload="beforeDownload($event)" :manual-pagination="false" :show-layout="false" :preview-modal="true" ref="html2Pdf0">
                    <LazyLayoutResponseReportDetails company="Water Emergency Services Incorporated" reportName="Rapid Response Report" :notPdf="false" 
                        :rep="$store.state.reports.report" slot="pdf-content" />
                </vue-html2pdf>
            </client-only>
            <LazyLayoutResponseReportDetails :notPdf="true" reportName="Rapid Response Report" :rep="$store.state.reports.report" />
        </span>
        <span v-if="reportType === 'dispatch'">
            <client-only>
                <vue-html2pdf :pdf-quality="2" pdf-content-width="100%" :html-to-pdf-options="htmlToPdfOptions" :paginate-elements-by-height="800" :manual-pagination="false"
                    :show-layout="false" :preview-modal="true" :enable-download="false" @hasDownloaded="hasDownloaded($event)" @beforeDownload="beforeDownload($event)" ref="html2Pdf0">
                    <LazyLayoutReportDetails :notPdf="false" reportName="Dispatch Report" :report="$store.state.reports.report" slot="pdf-content" />
                </vue-html2pdf>
            </client-only>
            <LazyLayoutReportDetails :notPdf="true" reportName="Dispatch Report" :report="$store.state.reports.report" />
        </span>
        
        <span v-if="reportType === 'upholstery-form'">
            <client-only>
                <vue-html2pdf :pdf-quality="2" pdf-content-width="100%" :html-to-pdf-options="htmlToPdfOptions" :paginate-elements-by-height="1000" :manual-pagination="false"
                    :show-layout="false" :preview-modal="true" :enable-download="false" @hasDownloaded="hasDownloaded($event)" @beforeDownload="beforeDownload($event)" ref="html2Pdf0">
                    <LazyLayoutUpholsteryDetails slot="pdf-content" :report="report" :notPdf="false" />
                </vue-html2pdf>
            </client-only>
            <LazyLayoutUpholsteryDetails :report="report" notPdf />
        </span>
        <span v-if="reportType === 'case-file-containment' || reportType === 'case-file-technician'">
            <client-only>
                <vue-html2pdf :pdf-quality="2" pdf-content-width="100%" :html-to-pdf-options="htmlToPdfOptions" :paginate-elements-by-height="800" :manual-pagination="false"
                    :show-layout="false" :preview-modal="true" :enable-download="false" @hasDownloaded="hasDownloaded($event)" @beforeDownload="beforeDownload($event)" ref="html2Pdf0">
                    <LazyLayoutCaseFileDetails :form_name="report.ReportType" :notPdf="false" :report="report" slot="pdf-content" />
                </vue-html2pdf>
            </client-only>
            <LazyLayoutCaseFileDetails :form_name="report.ReportType" :notPdf="true" :report="report" />
        </span>
        <span v-if="report.formType === 'sketch-report'">
            <h1><span v-uppercase>{{report.ReportType}}</span> for job {{jobId}}</h1>
            <client-only>
                <vue-html2pdf :pdf-quality="2" pdf-content-width="100%" :html-to-pdf-options="htmlToPdfOptions" :paginate-elements-by-height="900" :manual-pagination="false"
                 :show-layout="false" @hasDownloaded="hasDownloaded($event)" :preview-modal="false" ref="html2Pdf0">
                    <PdfSketch :reportName="report.ReportType" :reportType="reportType" :report="report" company="Water Emergency Services Incorporated" slot="pdf-content" />
                </vue-html2pdf>
            </client-only>
        </span>
        <span v-if="report.formType === 'logs-report' && report.ReportType !== 'moisture-map' && report.ReportType !== 'personal-content-inventory'">
            <h1><span v-uppercase>{{report.ReportType}}</span> for job {{jobId}}</h1>
            <client-only>
                <vue-html2pdf :pdf-quality="2" pdf-content-width="100%" :html-to-pdf-options="htmlToPdfOptions" :paginate-elements-by-height="1400" :manual-pagination="false"
                 :show-layout="false" :enable-download="false" @hasDownloaded="hasDownloaded($event)" @beforeDownload="beforeDownload($event)" :preview-modal="true" ref="html2Pdf0">
                    <PdfLogs :reportName="report.ReportType" :reportType="reportType" :report="report" company="Water Emergency Services Incorporated" slot="pdf-content" />
                </vue-html2pdf>
            </client-only>        
        </span>
        <span v-if="report.ReportType === 'personal-content-inventory'">
            <h1><span v-uppercase>{{report.ReportType}}</span> for job {{jobId}}</h1>
            <client-only>
                <vue-html2pdf :pdf-quality="2" pdf-content-width="100%" :html-to-pdf-options="htmlToPdfOptions" :paginate-elements-by-height="800" :manual-pagination="false"
                 :show-layout="false" :enable-download="false" @hasDownloaded="hasDownloaded($event)" @beforeDownload="beforeDownload($event)" :preview-modal="true" ref="html2Pdf0">
                    <LayoutContentInventoryDetails slot="pdf-content" :reportName="report.ReportType" :report="report" company="Water Emergency Services Incorporated" />
                </vue-html2pdf>
            </client-only>
            <LayoutContentInventoryDetails slot="pdf-content" :reportName="report.ReportType" :report="report" company="Water Emergency Services Incorporated" />
        </span>
        <span v-if="report.ReportType === 'moisture-map'">
            <h1><span v-uppercase>{{report.ReportType}}</span> for job {{jobId}}</h1>
            <client-only>
                <vue-html2pdf :pdf-quality="2" pdf-content-width="100%" :html-to-pdf-options="htmlToPdfOptions" :paginate-elements-by-height="800" :manual-pagination="false"
                 :show-layout="false" :enable-download="false" @hasDownloaded="hasDownloaded($event)" @beforeDownload="beforeDownload($event)" :preview-modal="true" ref="html2Pdf0">
                    <LayoutMoistureMapDetails slot="pdf-content" :reportName="report.ReportType" :report="report" company="Water Emergency Services Incorporated" />
                </vue-html2pdf>
            </client-only>
        </span>
        <span v-if="report.ReportType === 'psychrometric-chart'">
            <h1><span v-uppercase>{{report.ReportType}}</span> for job {{jobId}}</h1>
            <client-only>
                <vue-html2pdf :pdf-quality="2" pdf-content-width="100%" :html-to-pdf-options="htmlToPdfOptions" :paginate-elements-by-height="1000" :manual-pagination="false"
                 :show-layout="false" :preview-modal="true" ref="html2Pdf0" @hasDownloaded="hasDownloaded($event)">
                    <PdfChart :report="report" slot="pdf-content" />
                </vue-html2pdf>
            </client-only>
            <!-- <PdfChart :report="report" /> -->
        </span>
        <span v-if="report.ReportType === 'quality-control'">
            <h1><span v-uppercase>{{report.ReportType}}</span> for job {{jobId}}</h1>
            <client-only>
                <vue-html2pdf :pdf-quality="2" pdf-content-width="100%" :html-to-pdf-options="htmlToPdfOptions" :paginate-elements-by-height="1000" :manual-pagination="false"
                    :show-layout="false" :preview-modal="true" :enable-download="false" @hasDownloaded="hasDownloaded($event)" @beforeDownload="beforeDownload($event)" ref="html2Pdf0">
                    <LayoutQualityControlDetails slot="pdf-content" :report="report" />
                </vue-html2pdf>
            </client-only>
            <LayoutQualityControlDetails notPdf :report="report" />
        </span></div>
    </div>
</template>
<script>
import { defineComponent, ref, onMounted, useAsync, watch, computed, provide, useStore, useContext } from '@nuxtjs/composition-api';
import useReports from '@/composable/reports';
export default defineComponent({
    //layout: 'dashboard-layout',
    setup(props, {root, refs}) {
        const store = useStore()
        const { $auth, $gcs } = useContext()
        const { beforeDownload } = useReports()
        const company = ref("")
        const clickedOn = ref(false)
        const reportType = root.$route.params.type
        const jobId = root.$route.params.slug
        const report = computed(() => store.getters["reports/getReport"]);
        const fetchSignature = (signType, email) => { store.dispatch("users/getSigOrInitialImage", {signType, email}); }
        const htmlToPdfOptions = computed(() => {
            return {
                margin:[20, 10, 20, 10],
                filename: `${report.value.ReportType}-${report.value.JobId}`,
                image: {
                    type: "jpeg",
                    quality: 0.95
                },
                html2canvas: {
                    scale: 2,
                    useCORS: true,
                    logging: true,
                    removeContainer: true
                },
                jsPDF: {
                    unit: 'px',
                    format: 'a4',
                    orientation: 'portrait',
                    hotfixes: ['px_scaling']
                }
            }
        })

        const fetchingReport = () => { 
            store.dispatch("reports/fetchReport", { authUser: $auth.user, path: `${reportType}/${jobId}` }).then(() => {
                if (store.getters["reports/getReport"].hasOwnProperty('evaluationLogs')) {
                    store.dispatch("reports/formatEvalTimes")
                }
                store.dispatch("users/getSigOrInitialImage", {signType: "signature.jpg", email: store.getters["reports/getReport"].teamMember.email})
            }) 
        }
        function generateReport(key) {
            clickedOn.value = key
            refs["html2Pdf"+key].generatePdf()
        }
        function hasDownloaded(blob) {
            uploadPdf(blob).then(() => {
                console.log("uploaded pdf")
            })
            clickedOn.value = null
        }
        async function uploadPdf(file) {

            let formData = new FormData();
            const pdf = new File([file], `${reportType}-${jobId}.pdf`, {
                type: 'application/pdf'
            });
            formData.append('multiFiles', pdf) // The method in the gsutil web api looks for a form file named multiFiles
            formData.append('path', 'pdfs/')
            $gcs.$post(`/upload`, formData)
        }
        onMounted(fetchingReport)
        return {
            report,
            reportType,
            jobId,
            company,
            htmlToPdfOptions,
            clickedOn,
            beforeDownload,
            generateReport,
            hasDownloaded
        }
    },
})
</script>