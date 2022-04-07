<template>
    <div class="form-wrapper">
        <h1 class="text-center">{{company}}</h1>
        <h2 class="text-center">Assignment of Benefits & Mitigation Contract</h2>
        <ValidationObserver ref="form" v-slot="{errors, handleSubmit}">
            <h2>{{message}}</h2>
            <v-dialog width="400px" v-model="errorDialog">
                <div class="modal__error">
                    <div v-for="(error, i) in errors" :key="`error-${i}`">
                        <h3 class="form__input--error">{{ error[0] }}</h3>
                    </div>
                </div>
            </v-dialog>
            <form class="form" @submit.prevent="handleSubmit(submitForm)" v-if="!submitted">
                <div class="form__form-group">
                    <ValidationProvider vid="JobId" v-slot="{errors, ariaMsg}" name="Job ID" class="form__input-group form__input-group--normal">
                        <input type="hidden" v-model="selectedJobId" />
                        <label class="form__label" for="jobid">Job ID:</label>
                        <i class="form__select--icon icon--angle-down mdi" aria-label="icon"></i>
                        <select id="jobid" class="form__input" v-model="selectedJobId">
                            <option disabled value="">Please select a Job ID</option>
                            <option v-for="(item, i) in $store.state.reports.jobids" :key="`jobids-${i}`">{{item}}</option>
                        </select>
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <div class="text-left">
                        <p>
                            This Assignment of Claim Agreement (hereinafter referred to as “Assignment” and/or
                            “Agreement”) and Restoration Contract
                            (hereinafter referred to as “Contract”) is entered into by and between:
                        </p>
                        <p>
                            {{company}} (hereinafter referred to as “{{abbreviation}}” and/or “Insured”) and The
                            Owner/Persons of legal authority (hereinafter referred to as “Customer”)
                            of the property more commonly known as and identified by the following address:
                        </p>
                        <div class="form__form-group">
                            <ValidationProvider rules="required" class="form__input-group form__input-group--long" v-slot="{ errors }" name="Subject property">
                                <input type="text" class="form__input" v-model="subjectProperty" />           
                                <span class="txt--subtext mt-2">(hereinafter referred to as “Subject Property”)</span>
                                <span class="form__input--error">{{ errors[0] }}</span>
                            </ValidationProvider>
                            <UiGeoCoder @sendAddress="subjectProperty = $event" mapid="map" geocoderid="subjectProperty" geocoderef="geocoderProperty" mapView />
                        </div>
                    </div>
                </div>
                <ol class="form__form-group--listing-num">
                    <li>
                        <span class="font-weight-bold">Assignment of Benefits to {{company}}:</span>
                        <ol>
                            <li>
                                Customer understands and agrees to assign all insurance rights, benefits, proceeds and any causes of action under 
                                applicable insurance policies to {{abbreviation}} for services rendered or to be rendered for said Subject Property identified 
                                As Part 1 and Part 2, Contracting Agreement & Scope of Work and Betterments. Both parties understand and agree 
                                that {{abbreviation}} will replace the Customer as the Insured on the applicable insurance policies and as such will be 
                                entitled to all insurance rights, benefits, proceeds, and any causes of action thereunder in its capacity as the legally 
                                recognized Insured.
                            </li>
                            <li>
                                By executing this document Customer agrees and intends for all rights, benefits, and proceeds for services for said 
                                Subject Property identified above to be assigned solely and exclusively to {{abbreviation}} in its capacity as the Insured.
                            </li>
                            <li>
                                In this regard, Customer waives all privacy rights if any related to all insurance information and agrees to provide 
                                any and all such reasonable and necessary information and documents to {{abbreviation}} to assist {{abbreviation}} in receiving 
                                payment for all services performed pursuant to this Agreement including but not limited to insurance policies, 
                                insurance policy numbers, endorsements, riders, correspondence, invoices, proof of coverage, full disclosure of any 
                                previous insurance claims if any and/or any other insurance policy information that {{abbreviation}} may need to establish, 
                                pursue and perfect its rights as the Insured and so as to exercise its rights thereunder to any benefits, proceeds, 
                                payments, causes of actions and/or any other related rights. Section III. of this Agreement is hereby incorporated by 
                                reference.
                            </li>
                            <li>
                                The Property Representative makes this Assignment in consideration of and for {{abbreviation}}’s agreement to perform 
                                labor, provide services, supply materials and perform such services under this Contract and including the additional 
                                consideration of and for {{abbreviation}}’s not requiring full payment at the time of service.
                            </li>
                            <li>
                                Property Representative hereby unequivocally directs the insurance carrier(s) to release all information requested by 
                                {{abbreviation}} in its capacity as the Insured, its representatives, and/or its attorney for the purpose of obtaining actual 
                                benefits to be paid by the insurance carrier(s) for services rendered or to be rendered for the Subject Property 
                                identified above.
                            </li>
                        </ol>
                    </li>
                </ol>
                <div class="form__form-group">
                    <LazyUiSignaturePadModal width="700px" height="219px" dialog :initial="false" sigType="customer" inputId="cusSign1" :sigData="cusSign1" 
                        sigRef="cusSign1" name="Customer signature" />
                    <ValidationProvider name="Sign Date" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--short">
                        <label for="cusSign1" class="form__label">Customer Sign Date</label>
                        <imask-input id="cusSign1" :value="cusSignDate1" class="form__input" :mask="Date" :pattern="dateMask.pattern" :blocks="dateMask.blocks" :format="dateMask.format" 
                            :parse="dateMask.parse" :autofix="true" :overwrite="true" :lazy="false" @complete="cusSignDate1 = $event" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                </div>
                <div class="form__form-group">
                    <LazyUiSignaturePadModal width="700px" height="219px" notrequired dialog :initial="false" sigType="customer" inputId="cusSign2" :sigData="cusSign2" 
                        sigRef="cusSign2" name="Customer signature" />
                    <ValidationProvider name="Sign Date" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--short">
                        <label for="cusSign2" class="form__label">Customer Sign Date</label>
                        <imask-input id="cusSign2" :value="cusSignDate2" class="form__input" :mask="Date" :pattern="dateMask.pattern" :blocks="dateMask.blocks" :format="dateMask.format" 
                            :parse="dateMask.parse" :autofix="true" :overwrite="true" :lazy="false" @complete="cusSignDate2 = $event" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                </div>
                <div class="form__form-group">
                    <ValidationProvider name="Witness" rules="required" v-slot="{errors,ariaMsg}" class="form__input-group form__input-group--long">
                        <label class="form__label" for="witness">Witness</label>
                        <input type="text" class="form__input" id="witness" v-model="witness" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider name="Witness Sign Date" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--short">
                        <label for="withnessDate" class="form__label">Witness Date</label>
                        <imask-input id="witnessDate" :value="witnessDate" class="form__input" :mask="Date" :pattern="dateMask.pattern" :blocks="dateMask.blocks" :format="dateMask.format" 
                            :parse="dateMask.parse" :autofix="true" :overwrite="true" :lazy="false" @complete="witnessDate = $event" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                </div>
                <button type="submit" class="button button--normal">{{ submitting ? 'Submitting' : 'Submit' }}</button>
            </form>
        </ValidationObserver>
    </div>
</template>
<script>
import { computed, defineComponent, ref, useContext, useStore, watch } from '@nuxtjs/composition-api'
import { dateMask } from "@/data/masks"
import useReports from '@/composable/reports'
export default defineComponent({
    props: {
        company: String,
        abbreviation: String
    },
    setup(props, { refs }) {
        const store = useStore()
        const { $api } = useContext()
        const { getReportPromise } = useReports()
        const errorDialog = ref(false)
        const submitted = ref(false)
        const submitting = ref(false)
        const message = ref("")
        const selectedJobId = ref("")
        const subjectProperty = ref("")
        const cusSign1 = ref({
            isEmpty: true,
            data: ''
        })
        const cusSign2 = ref({
            isEmpty: true,
            data: ''
        })
        const cusSignDate1 = ref("")
        const cusSignDate2 = ref("")
        const witness = ref("")
        const witnessDate = ref("")

        const user = computed(() => store.getters["users/getUser"])
        const setError = (arr, key) => {
            return arr.filter(obj => obj.param === key).map(v => v.msg)
        }
        
        async function submitForm() {
            message.value = ""
            if (cusSign1.value.isEmpty) {
                cusSign1.value.data = "N/A"
            }
            if (cusSign1.value.isEmpty) {
                cusSign1.value.data = "N/A"
            }
            const post = {
                JobId: selectedJobId.value,
                ReportType: "guardian-aob",
                formType: "aob",
                contractingCompany: "Guardian Restoration",
                subjectProperty: subjectProperty.value,
                cusSign1: cusSign1.value.data,
                cusSign2: cusSign2.value.data,
                cusSignDate1: cusSignDate1.value,
                cusSignDate2: cusSignDate2.value,
                witness: witness.value,
                witnessDate: witnessDate.value,
                teamMember: user.value
            };
            await refs.form.validate().then(success => {
                if (!success) {
                    submitting.value = false
                    submitted.value = false
                    errorDialog.value = true
                    return
                }
                submitting.value = true
                $api.$post("/api/reports/guardian-aob/new", post, {
                    params: {
                        jobid: selectedJobId.value
                    }
                }).then((res) => {
                    if (res.error) {
                        errorDialog.value = true
                        submitting.value = false
                        refs.form.setErrors({
                            JobId: [res.message]
                        })
                        return;
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

        watch(selectedJobId, (val) => {
            getReportPromise(`dispatch/${val}`).then((result) => {
                subjectProperty.value = result.location.address + ", " + result.location.cityStateZip
            })
        })
        
        return {
            errorDialog, submitted, submitting, message,
            selectedJobId,
            subjectProperty,
            cusSign1, cusSign2,
            cusSignDate1, cusSignDate2,
            witness, witnessDate,
            submitForm,
            dateMask
        }
    },
})
</script>
