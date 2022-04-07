<template>
    <div class="form-wrapper form-wrapper__qc-eval">
        <h1 class="text-center">{{company}}</h1>
        <h2 class="text-center">Quality Control Evaluation Report</h2>
        <h3 v-if="serverMessage !== ''">{{serverMessage}}</h3>
        <ValidationObserver ref="form" v-slot="{errors, handleSubmit}">
            <h2>{{message}}</h2>
            <v-dialog width="400px" v-model="errorDialog">
                <div class="modal__error">
                    <div v-for="(error, i) in errors" :key="`error-${i}`">
                        <h3 class="form__input--error">{{ error }}</h3>
                    </div>
                </div>
            </v-dialog>
            <form class="form" @submit.prevent="handleSubmit(submitForm)" v-if="!submitted">
                <div class="form__form-group">
                    <ValidationProvider v-slot="{errors, ariaMsg}" name="Job ID" class="form__input-group form__input-group--normal">
                        <input type="hidden" v-model="selectedJobId" />
                        <label class="form__label">Job ID:</label>
                        <i class="form__select--icon icon--angle-down mdi" aria-label="icon"></i>
                        <select class="form__input" v-model="selectedJobId">
                            <option disabled value="">Please select a Job ID</option>
                            <option v-for="(item, i) in $store.state.reports.jobids" :key="`jobids-${i}`">{{item}}</option>
                        </select>
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider name="Address" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--very-long">
                        <label for="address" class="form__label">Address</label>
                        <input id="address" v-model="location.address" name="Address" type="text" class="form__input" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider name="City, state, zip" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--very-long">
                        <label for="cityStateZip" class="form__label">City, State, Zip</label>
                        <input id="cityStateZip" v-model="location.cityStateZip" name="CityStateZip" type="text" class="form__input" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider vid="Name" v-slot="{errors, ariaMsg}" name="Name" class="form__input-group form__input-group--long">
                        <label for="name" class="form__label">Customer</label>
                        <input id="name" class="form__input" type="text" v-model="name" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                </div>
                <div class="form__form-group--block">
                    <h2>Check the Box When Completed and Executed Document is in Hand & Filed, Complete the list Fully</h2>
                    <div>
                        <input id="selectAllDocs" type="checkbox" @click="selectAll('documentArr', 'completedDocs', 0)" v-model="allSelected[0]" />
                        <label for="selectAllDocs" class="form__label form__label--listing">Select all</label>
                    </div>
                    <div class="grid grid--default">
                        <div class="form__form-group--info-box" v-for="(group, i) in documentArr" :key="`doc-${i}`">
                            <ul class="form__form-group--listing form__form-group--listing-no-style">
                                <li v-for="(item, j) in group" :key="`item-${j}`">
                                    <input type="checkbox" :id="item" v-model="completedDocs" :value="item" />
                                    <label class="form__label form__label--listing" :for="item">{{item}}</label>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div class="form__form-group--block">
                    <h2>Check the Box to Verify Quality Services. If a box is not check please note, and address appropriately</h2>
                    <div class="grid grid--default">
                        <div class="form__form-group--info-box" v-for="(group, i) in qualityServicesArr" :key="`doc-${i}`">
                            <h3>{{group.label}}</h3>
                            <div>
                                <input :id="group.id" type="checkbox" @click="selectAll(group, 'completedServices')" v-model="group.selected" />
                                <label :for="group.id" class="form__label form__label--listing">Select all</label>
                            </div>
                            <ul class="form__form-group--listing form__form-group--listing-no-style">
                                <li v-for="(item, j) in group.data" :key="`item-${j}`">
                                    <span v-if="completedServices.hasOwnProperty(group.id)">
                                        <input type="checkbox" :id="item" v-model="completedServices[group.id].checked" :value="item" />
                                    </span>
                                    <label class="form__label form__label--listing" :for="item">{{item}}</label>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div class="form__form-group--block">
                    <div v-for="(group, i) in secondFormData" :key="`group-${i}`">
                        <h2>{{group.label}}</h2>
                        <div>
                            <input :id="group.id" type="checkbox" @click="selectAll(group, 'selectedData')" v-model="group.selected" />
                            <label :for="group.id" class="form__label form__label--listing">Select all</label>
                        </div>
                        <ul class="form__form-group--listing form__form-group--listing-no-style grid grid--default">
                            <li v-for="(item, i) in group.data" :key="`doc-${i}`">
                                <input v-if="selectedData.hasOwnProperty(group.id)" type="checkbox" :id="item" v-model="selectedData[group.id].checked" :value="item" />
                                <label class="form__label form__label--listing" :for="item">{{item}}</label>
                            </li>
                        </ul>
                    </div>
                </div>
                <div class="form__form-group">
                    <ValidationProvider name="Time of Evaluation" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--short">
                        <label class="form__label">Time of Evaluation</label>
                        <imask-input v-model="evalTime" :lazy="false" :mask="timeMask.mask" :blocks="timeMask.blocks" class="form__input" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider vid="evalDate" name="Date of Evaluation" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--short">
                        <label for="evalDate" class="form__label">Date of Evaluation</label>
                        <input type="hidden" v-model="evalDate" />
                        <imask-input id="evalDate" @complete="formatEvalDate" :lazy="false" :blocks="dateMask.blocks" :mask="dateMask.mask" :format="dateMask.format" 
                            :parse="dateMask.parse" :pattern="dateMask.pattern" class="form__input" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider name="Customer first name" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--normal">
                        <label class="form__label">Customer First Name</label>
                        <input type="text" v-model="customer.first" class="form__input" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider name="Customer last name" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--normal">
                        <label class="form__label">Customer Last Name</label>
                        <input type="text" v-model="customer.last" class="form__input" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                </div>
                <div class="form__form-group">
                    <LazyUiSignaturePadModal dialog v-model="customerSig" width="700px" sigType="customer" height="219px" inputId="cusSig" :initial="false" :sigData="cusSig" 
                        sigRef="cusSigPad" name="Customer Signature" />
                    <ValidationProvider  name="Sign Date" rules="required|length:10" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--short">
                        <label for="signDate" class="form__label">Sign date</label>
                        <input type="hidden" v-model="signDate" />
                        <imask-input id="signDate" @complete="formatSignDate" :lazy="false" :blocks="dateMask.blocks" :mask="dateMask.mask" :format="dateMask.format" 
                            :parse="dateMask.parse" :pattern="dateMask.pattern" class="form__input" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                </div>
                <button type="submit" class="button button--normal">{{ submitting ? 'Submitting' : 'Submit' }}</button>
            </form>
        </ValidationObserver>
    </div>
</template>
<script>
import { computed, defineComponent, onMounted, ref, useContext, useStore, watch } from '@nuxtjs/composition-api'
import useReports from "@/composable/reports"
import genericFuncs from "@/composable/utilityFunctions"
import { timeMask, dateMask } from "@/data/masks";
export default defineComponent({
    props: {
        company: String
    },
    setup(props, { refs, parent }) {
        const data = parent.$children[0]
        const { getReportPromise } = useReports()
        const { groupByKey } = genericFuncs()
        const store = useStore()
        const { $api } = useContext()
        const submitted = ref(false)
        const submitting = ref(false)
        const message = ref("")
        const serverMessage = ref("")
        const errorDialog = ref(false)
        const selectedJobId = ref("")
        const name = ref("")
        const location = ref({
            address: "",
            cityStateZip: ""
        })
        const customer = ref({
            first: '',
            last: ''
        })
        const cusSig = ref({
            isEmpty: true,
            data: ""
        })
        const documentArr = ref([
            [
                "Executed Dispatch Report", "Executed Response Report", "Executed Mitigation Contract", "Executed Containment Report", "Executed Technician Report", "Executed Content Loss Report", "Executed Completion Contract","Executed Xactimate Note","Executed Promise to Pay"
            ],
            [
                "Atmospheric Condition Report", "Psychrometric Chart Report", "Moisture Readings", "Moisture Location Map", "Equipment Map", "Equipment Inventory Report", "Billable Tasks Report","Hourly Tasks Report","Water Category Report"
            ],
            [
                "Sketch & Detailed Measurements", "Daily Attendance Logs","Rapid Response Photographs", "Daily Task Logs", "Containment Daily Photographs", "Technician Daily Photographs", "Quality Control Photographs", "Job File Receipts Collected","Job File General History Log"
            ]
        ])
        const qualityServicesArr = ref([
            {
                selected: false,
                id:"response",
                label: "Response Team",
                data: ["Timely & Punctual", "Polite & Courteous", "Documented Sufficiently", "Articulated All Concerns", "Effectively Evaluated Conditions", "Answered Concerns Clearly"]
            },
            {
                selected: false,
                id:"containment",
                label: "Containment Team",
                data: ["Prepared & Equipped", "Customer Service Oriented", "Material Removal Appropriate", "Property Swept & Clean", "Debris Removed from Property", "Containment Effectively Tasked"]
            },
            {
                selected: false,
                id:"technician",
                label: "Technician Team",
                data: ["Clearly Explained Procedures", "Professional & Experienced", "Used Appropriate Equipment", "Knowledgably Monitored Loss", "Efficiently Improved Conditions", "Chemically Treated All Areas"]
            },
        ])
        const secondFormData = ref([
            {
                selected: false,
                id: "taskList",
                label: "Quality Control Task List",
                data: ["All Documents Executed", "All Reports & Logs Completed", "All Industry Safety Regulations have been Followed", "All Materials Measured, Photographed, & Loaded", "All Affected Areas Treated Properly", 
                    "All Equipment Removed from Project", "All Equipment is in Working Condition", "All Equipment, Tools, & Property Returned to WESI", "Property is Swept, Cleaned, & Free of Debris", "Property is Mitigated & in a Repair Ready Condition"
                ]
            },
            {
                selected: false,
                id: "customerReview",
                label: "Customer Review (Check all that apply)",
                data: [
                    "You Would Recommend this Company to a Friend or Colleague",
                    "You Would Consider this Company First for Similar Services in the Future",
                    "You Would Express a Positive Review to Someone Who Asked About Our Companies Services",
                    "You Feel Satisfied in Our Company’s Performance on Your Property"
                ]
            }
        ])
        const completedDocs = ref([])
        const completedServices = ref({})
        const selectedData = ref({})
        const evalTime = ref("")
        const evalDate = ref("")
        const customerSig = ref("")
        const signDate = ref("")
        const allSelected = ref([false])
        const user = computed(() => store.getters["users/getUser"])

        function selectAll(arr, selectArr) {
            const selectarr = selectArr.toString()
            if (arr === "documentArr") {
                const arrString = arr.toString()
                data[selectarr] = []
                if (!allSelected.value[0]) {
                    for (const key in data[arrString]) {
                        data[arrString][key].forEach((item) => {
                            data[selectarr].push(item)
                        })
                    }
                }
            }
            if (selectArr === "completedServices" || selectArr === "selectedData") {
                data[selectarr][arr.id].checked = []
                if (!arr.selected) {
                    for (const key in data[selectarr]) {
                        if (key === arr.id) {
                            data[selectarr][key].checked = arr.data
                        }
                    }
                }
            }
        }
        function groupedData(arr) {
            var groupData = groupByKey(arr, "id")
            var selectedGroupData = {}
            for (const key in groupData) {
                var obj = { id: key, checked: [], label: groupData[key][0].label}
                selectedGroupData[key] = obj
            }
            return selectedGroupData
        }
        function getExistingReport(jobid) {
            getReportPromise(`dispatch/${jobid}`).then((result) => {
                location.value.address = result.location.address
                location.value.cityStateZip = result.location.cityStateZip
                name.value = result.callerName.last
            }).catch(err => {
                location.value.address = ""
                location.value.cityStateZip = ""
                name.value = ""
                errorDialog.value = true
                if (err.response) {
                    serverMessage.value = err.response.data
                }
            })
        }
        async function submitForm() {
            message.value = ""
            const post = {
                JobId: selectedJobId.value,
                ReportType: "quality-control",
                formType: "eval-report",
                teamMember: user.value,
                id: user.value.id,
                location: location.value,
                completedDocs: completedDocs.value,
                completedServices: completedServices.value,
                taskList: selectedData.value["taskList"].checked,
                customerReview: selectedData.value["customerReview"].checked,
                evalTime: evalTime.value,
                evalDate: evalDate.value,
                customer: customer.value,
                customerSig: customerSig.value,
                signDate: signDate.value
            }
            await refs.form.validate().then(success => {
                if (!success) {
                    errorDialog.value = true
                    submitting.value = false
                    submitted.value = false
                    return;
                }
                $api.$post(`/api/reports/quality-control/new`, post, {params: {jobid: selectedJobId.value}}).then((res) => {
                    if (res.error) {
                        errorDialog.value = true
                        submitting.value = false
                        refs.form.setErrors({
                            JobId: [res.message]
                        })
                        return
                    }
                    submitted.value = true
                    submitting.value = false
                    message.value = res
                    setTimeout(() => {
                        window.location = "/"
                    }, 3000)
                })
            })
        }
        function formatSignDate(e) {
            signDate.value = new Date(e).toISOString().substr(0, 10)
        }
        function formatEvalDate(e) {
            evalDate.value = new Date(e).toISOString().substr(0, 10)
        }

        completedServices.value = groupedData(qualityServicesArr.value)
        selectedData.value = groupedData(secondFormData.value)
        watch(selectedJobId, (val) => {
            getExistingReport(val)
        })

        return {
            dateMask, timeMask,
            message,
            selectedJobId,
            errorDialog,
            location,
            name,
            documentArr,
            qualityServicesArr,
            secondFormData,
            completedDocs,
            completedServices,
            selectedData,
            evalTime,
            evalDate,
            customer,
            cusSig, customerSig,
            signDate,
            submitting, submitted,
            allSelected,
            submitForm,
            formatSignDate,
            formatEvalDate,
            selectAll
        }
    }
})
</script>
