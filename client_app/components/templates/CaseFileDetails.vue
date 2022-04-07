<template>
<section class="pdf-content" slot="pdf-content">
        <h2 v-uppercase>{{ form_name }}</h2>
        <div class="pdf-item report-details__section">
            <div class="report-details__data">
                <label>Job ID:</label>
                <span>{{report.JobId}}</span>
            </div>
            <div class="report-details__data">
                <label>Team Lead ID #:</label>
                <span>{{report.id}}</span>
            </div>
            <div class="report-details__data">
                <label>Date:</label>
                <span>{{report.date}}</span>
            </div>
            <div class="report-details__data">
                <label>Address:</label>
                <span class="report-details__data-field">{{report.location.address}}</span>
            </div>
            <div class="report-details__data">
                <label>City, State, Zip:</label>
                <span class="report-details__data-field">{{report.location.cityStateZip}}</span>
            </div>
        </div>
        <div class="report-details__section--case-file">
            <div class="report-details__data" v-if="report && report.contentCleaningInspection !== undefined">
                <!-- content cleaning section -->
                <h4>{{report.contentCleaningInspection[0].group}}</h4>
                <ul>                   
                    <li v-for="(item, i) in report.contentCleaningInspection" :key="`cleaning-${i}`">
                        {{item.label}}
                    </li>
                </ul>
            </div>
            <div class="report-details__data" v-if="report && report.selectedTmpRepairs !== undefined">
                <!-- tmp repairs -->
                <h4>TMP Repairs</h4>
                <ul>
                    <li v-for="(item, i) in report.selectedTmpRepairs" :key="`tmp-repairs-${i}`">
                        {{item}}
                    </li>
                </ul>
            </div>
            <div class="report-details__data" v-if="report && report.selectedContent !== undefined">
                <ul>
                    <li v-for="(item, i) in groupByKey(report.selectedContent, 'group')" :key="`content-${i}`">
                        <h4>{{i}}</h4>
                        <ul>
                            <li v-for="(subitem, i) in item" :key="`contentoff-${i}`">
                                {{subitem.label}}
                                
                            </li>
                        </ul>
                    </li>                   
                </ul>               
            </div>
            <div class="report-details__data" v-if="report && report.selectedStructualDrying !== undefined">
                <ul>
                    <li v-for="(item, i) in groupByKey(report.selectedStructualDrying, 'group')" :key="i">
                        <h4>{{i}}</h4>
                        <ul>
                            <li v-for="subitem in item" :key="subitem.label">
                                {{subitem.label}}
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>
            <div class="report-details__data" v-if="report && report.selectedStructualCleaning !== undefined">
                <ul>
                    <li v-for="(item, i) in groupByKey(report.selectedStructualCleaning, 'group')" :key="i">
                        <h4>{{i}}</h4>
                        <ul>
                            <li v-for="subitem in item" :key="subitem.label">
                                {{subitem.label}}
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>
            <div class="report-details__data" v-if="report && report.contentCleaningInspection !== undefined">
                <ul>
                    <li v-for="(item, i) in groupByKey(report.contentCleaningInspection, 'group')" :key="i">
                        <h4>{{i}}</h4>
                        <ul>
                            <li v-for="subitem in item" :key="subitem.label">
                                {{subitem.label}}
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>
            <div class="report-details__data" v-if="report && report.waterRestorationInspection !== undefined">
                <ul>
                    <li v-for="(item, i) in groupByKey(report.waterRestorationInspection, 'group')" :key="i">
                        <h4>{{i}}</h4>
                        <ul>
                            <li v-for="subitem in item" :key="subitem.label">
                                {{subitem.label}}
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>
            <div class="report-details__data" v-if="report && report.waterRemediationAssesment !== undefined">
                <ul>
                    <li v-for="(item, i) in groupByKey(report.waterRemediationAssesment, 'group')" :key="i">
                        <h4>{{i}}</h4>
                        <ul>
                            <li v-for="subitem in item" :key="subitem.label">
                                {{subitem.label}}
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>
            <div class="report-details__data" v-if="report && report.overviewScopeOfWork !== undefined">
                <ul>
                    <li v-for="(item, i) in groupByKey(report.overviewScopeOfWork, 'group')" :key="i">
                        <h4>{{i}}</h4>
                        <ul>
                            <li v-for="subitem in item" :key="subitem.label">
                                {{subitem.label}}
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>
            <div class="report-details__data" v-if="report && report.specializedExpert !== undefined">
                <ul>
                    <li v-for="(item, i) in groupByKey(report.specializedExpert, 'group')" :key="i">
                        <h4>{{i}}</h4>
                        <ul>
                            <li v-for="subitem in item" :key="subitem.label">
                                {{subitem.label}}
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>
            <div class="report-details__data" v-if="report && report.scopeOfWork !== undefined">
                <ul>
                    <li v-for="(item, i) in groupByKey(report.scopeOfWork, 'group')" :key="i">
                        <h4>{{i}}</h4>
                        <ul>
                            <li v-for="subitem in item" :key="subitem.label">
                                {{subitem.label}}
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>
            <div class="report-details__data" v-if="report && report.projectWorkPlans !== undefined">
                <ul>
                    <li v-for="(item, i) in groupByKey(report.projectWorkPlans, 'group')" :key="i">
                        <h4>{{i}}</h4>
                        <ul>
                            <li v-for="subitem in item" :key="subitem.label">
                                {{subitem.label}}
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>
        </div>
        <div class="pdf-item report-details__section--block" v-if="report.notes && report.notes !== ''">
            <h4>Notes</h4>
            <span>{{report.notes}}</span>
        </div>
        <div class="pdf-item report-details__section">
            <div class="report-details__checklist">
                <h4>Evaluation Logs</h4>
                <ul>
                    <li v-for="(evalLog, i) in report.evaluationLogs" :key="`eval-${i}`">
                        <label class="form__label">{{ evalLog && evalLog.label ? evalLog.label : null}}</label>
                        <span>{{ evalLog && evalLog.value ? evalLog.value : null}}</span>
                    </li>
                </ul>
            </div>
            <div class="report-details__data">
                <h4>Team Member Signiture:</h4>
                <div v-if="report.verifySig">
                    <div class="report-details__data--cusSig" :style="'background-image:url('+$store.state.users.signature+')'"></div>
                </div>
            </div>
            <div class="report-details__data">
                <h4>Work completed after hours:</h4>
                <span>{{report.afterHoursWork}}</span>
            </div>
        </div>
        <UiStorageImages class="report-details__section--pictures" :imageArr="images" :jobid="report.JobId" subPath="" :path="imageFolder" />
        
        </section>
</template>
<script>
import { computed, defineComponent, onMounted, ref, toRefs } from "@nuxtjs/composition-api";
import genericFuncs from '@/composable/utilityFunctions'
export default defineComponent({
    props: {
        report: Object,
        notPdf: Boolean,
        form_name: String
    },
    setup(props, {root}) {
        const { report } = toRefs(props)
        const { groupByKey } = genericFuncs()
        const images = ref([])
        const imageFolders = ref([])
        const signature = computed(() => {
            var sig = report.value && report.value.verifySign ? report.value.verifySign : null;
            if (sig !== null) {
                return report.value.verifySign
            }
            return ""
        })
        const imageFolder = computed(() => {
            switch (report.value.ReportType) {
                case "case-file-technician":
                    return "tech-images"
                case "case-file-containment":
                    return "containment-images"
            }
        })
        return {
            images,
            signature,
            imageFolder,
            imageFolders,
            groupByKey
        }
    }
})
</script>