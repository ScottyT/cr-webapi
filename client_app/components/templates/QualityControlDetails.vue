<template>
    <section :class="notPdf ? '' : 'pdf-content'">
        <h2 class="text-center">{{company}}</h2>
        <h2 class="text-center">Quality Control Evaluation Report</h2>
        <div class="pdf-item report-details__section">
            <div class="report-details__data">
                <label class="form__label">ID #</label>
                <span>{{report.id}}</span>
            </div>
            <div class="report-details__data">
                <label class="form__label">Job ID #</label>
                <span>{{report.JobId}}</span>
            </div>
            <div class="report-details__data">
                <label class="form__label">Address</label>
                <span>{{report.location.address}}</span>
            </div>
            <div class="report-details__data">
                <label class="form__label">City, State, Zip</label>
                <span>{{report.location.cityStateZip}}</span>
            </div>
        </div>
        <div class="pdf-item report-details__section">
            <h3>Check the Box When Completed and Executed Document is in Hand & Filed, Complete the list Fully</h3>
            <div class="report-details__listing">
                <ul class="grid grid--three-column">
                    <li v-for="(item, i) in report.completedDocs" :key="`docs-${i}`">{{item}}</li>
                </ul>
            </div>
        </div>
        <div class="pdf-item report-details__section">
            <h3>Check the Box to Verify Quality Services. If a box is not check please note, and address appropriately</h3>
            <div class="report-details__listing" v-for="(group) in services" :key="group">
                <h3>{{group.label}}</h3>
                <ul>
                    <li v-for="(item, i) in report.completedServices[group.id].checked" :key="`team-${i}`">{{item}}</li>
                </ul>
            </div>
        </div>
        <div class="pdf-item report-details__section flex-column">
            <h3>Quality Control Task List</h3>
            <div class="report-details__listing">
                <ul class="grid grid--two-column">
                    <li v-for="(item, i) in report.taskList" :key="`task-${i}`">{{item}}</li>
                </ul>
            </div>
        </div>
        <div class="pdf-item report-details__section">
            <h3>Customer Review</h3>
            <div class="report-details__listing">
                <ul class="grid grid--two-column">
                    <li v-for="(item, i) in report.customerReview" :key="`review-${i}`">{{item}}</li>
                </ul>
            </div>
        </div>
        <div class="pdf-item report-details__section">
            <h4>Evaluation Logs</h4>
            <div class="report-details__data">
                <label class="form__label">Time of Evaluation</label>
                <span>{{report.evalTime}}</span>
            </div>
            <div class="report-details__data">
                <label class="form__label">Date of Evaluation</label>
                <span>{{report.evalDate}}</span>
            </div>
        </div>
        <div class="pdf-item report-details__section">
            <div class="report-details__data">
                <label class="report-details__data-label">Customer</label>
                <div class="report-details__data--row">
                    <div>
                        <label>First:</label>
                        <span>{{report.customer.first}}</span>
                    </div>
                    <div>
                        <label>Last:</label>
                        <span>{{report.customer.last}}</span>
                    </div>
                </div>
            </div>
            <div class="report-details__data">
                <h4>Customer Signature</h4>
                <div class="report-details__signature"><img :src="report.customerSig" /></div>
            </div>
            <div class="report-details__data">
                <h4>Sign Date</h4>
                <span>{{report.signDate}}</span>
            </div>
        </div>
    </section>
</template>
<script>
import { defineComponent, toRefs, ref, onMounted, computed } from '@nuxtjs/composition-api'

export default defineComponent({
    props: {
        notPdf:Boolean,
        company: String,
        report: Object
    },
    setup(props, { refs }) {
        const { report } = toRefs(props)
        const services = ref([])
        /* const signature = computed(() => {
            var sig = report.value && report.value.verifySign ? report.value.verifySign : null;
            if (sig !== null) {
                return report.value.verifySign
            }
            return ""
        }) */

        function filterServices() {
            for (const key in report.value.completedServices) {
                services.value.push({label: report.value.completedServices[key].label, id: key})
            }
        }

        onMounted(filterServices)

        return {
            services
        }
    },
})
</script>
