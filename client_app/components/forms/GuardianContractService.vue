<template>
    <div class="form-wrapper">
        <h1 class="text-center">{{company}}</h1>
        <h2 class="text-center">Contracting Service Agreement</h2>
        <p>Insurance Repair, Renovation, or Restoration & Betterment Agreement
This Contracting Service Agreement (hereinafter, this “Agreement”) is entered into below by:
{{company}} (Hereinafter, “{{abbreviation}}”) &
The Owner/Persons of legal authority (Hereinafter, “Customer”)
Pertaining to services engaged at the following described property below
This Agreement is one of two Agreements:
One: Contracting Services Agreement &
Two: Scope of Work</p>
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
                </div>
                <div class="d-flex flex-wrap">
                    <div class="form__form-group flex-column form__form-group--narrow">
                        <h3>Job Information</h3>
                        <div class="pl-5">
                            <ValidationProvider name="Job name" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--full-width">
                                <label for="jobName" class="form__label">Job Name</label>
                                <input type="text" id="jobName" v-model="jobName" class="form__input" />
                                <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                            </ValidationProvider>
                            <div class="flex-row d-flex">
                                <ValidationProvider name="Job address" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--full-width">
                                    <label for="jobAddress" class="form__label">Job Address</label>
                                    <input type="text" v-model="location.address" v-if="location.address !== ''" class="form__input" />
                                    <UiGeoCoder v-else @sendAddress="settingLocation($event)" :mapView="false" v-model="location.address" geocoderid="customerLocation" geocoderef="customerLocationRef" />
                                    <!-- <input type="text" id="jobAddress" v-model="location.address" class="form__input" /> -->
                                    <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                                </ValidationProvider>
                                <ValidationProvider name="City, State, Zip" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--full-width">
                                    <label for="cityStateZip" class="form__label">City, State, Zip</label>
                                    <input type="text" id="cityStateZip" v-model="location.cityStateZip" class="form__input" />
                                    <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                                </ValidationProvider>
                            </div>
                            <div class="form__input-group form__input-group--full-width">
                                <label for="mortgageCompany" class="form__label">Mortgage Company</label>
                                <input id="mortgageCompany" v-model="mortgageCompany" type="text" class="form__input" />
                            </div>
                            <div class="flex-row">
                                <div class="form__input-group form__input-group--full-width">
                                    <label for="insuranceCompany" class="form__label">Insurance Company</label>
                                    <input id="insuranceCompany" v-model="insuranceCompany" type="text" class="form__input" />
                                </div>
                                <div class="form__input-group form__input-group--full-width">
                                    <label for="dateOfLoss" class="form__label">Date Of Loss</label>
                                    <input id="dateOfLoss" v-model="dateOfLoss" type="text" class="form__input" />
                                </div>
                            </div>
                            <div class="form__input-group form__input-group--full-width">
                                <label for="claimNumber" class="form__label">Claim Number</label>
                                <input id="claimNumber" v-model="claimNumber" type="text" class="form__input" />
                            </div>
                            <div class="d-flex flex-row">
                                <div class="form__input-group form__input-group--full-width">
                                    <label for="policyDate" class="form__label">Policy Date</label>
                                    <v-dialog content-class="date-pickers__range" max-width="700px" ref="policyDateDialog" v-model="policyDateModal" persistent 
                                        :return-value.sync="policyDateFormatted" >
                                        <template v-slot:activator="{ on, attrs }">
                                            <input type="text" id="appointment" v-model="policyDateFormatted" class="form__input" readonly v-bind="attrs" v-on="on" />
                                        </template>
                                        <v-date-picker class="date-picker date-picker-start" v-if="policyDateModal" v-model="policyDateStart" :max="policyDateEnd">
                                            <v-spacer></v-spacer>
                                        </v-date-picker>
                                        <v-date-picker class="date-picker date-picker-end" v-if="policyDateModal" v-model="policyDateEnd" :min="policyDateStart">
                                            <v-spacer></v-spacer>
                                            <v-btn text color="#fff" @click="policyDateModal = false">Cancel</v-btn>
                                            <v-btn text color="#fff" @click="$refs.policyDateDialog.save(policyDateEnd)">OK</v-btn>
                                        </v-date-picker>
                                    </v-dialog>
                                </div>
                                <div class="form__input-group form__input-group--full-width">
                                    <label for="policyNumber" class="form__label">Policy Number</label>
                                    <input id="policyNumber" v-model="policyNumber" type="text" class="form__input" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="form__form-group form__form-group--block form__form-group--narrow">
                        <h3>Contact Information</h3>
                        <div class="pl-5">
                            <div class="flex-row flex-wrap d-flex">
                                <div class="form__input-group form__input-group--normal">
                                    <label for="contactLiaison" class="form__label">Contact Liaison</label>
                                    <input id="contactLiaison" v-model="contactLiaison" type="text" class="form__input" />
                                </div>
                                <div class="form__input-group form__input-group--short">
                                    <label for="contactPhone" class="form__label">Contact Phone</label>
                                    <input id="contactPhone" :value="contactPhone" v-imask="phoneMask" @complete="contactPhone = $event.detail.value" type="text" class="form__input" />
                                </div>
                                <div class="form__input-group form__input-group--long">
                                    <label for="contactEmail" class="form__label">Contact Email</label>
                                    <input id="contactEmail" v-model="contactEmail" type="text" class="form__input" />
                                </div>
                            </div>
                            
                            <div class="d-flex flex-row">
                                <div class="form__input-group form__input-group--full-width">
                                    <label for="emergencyLiaison" class="form__label">Emergency Liaison</label>
                                    <input id="emergencyLiaison" v-model="emergencyLiaison" type="text" class="form__input" />
                                </div>
                                <div class="form__input-group form__input-group--full-width">
                                    <label for="emergencyPhone" class="form__label">Emergency Phone</label>
                                    <input id="emergencyPhone" :value="emergencyPhone" v-imask="phoneMask" @complete="emergencyPhone = $event.detail.value" type="text" class="form__input" />
                                </div>
                            </div>
                            <div class="d-flex flex-row">
                                <ValidationProvider name="Billing Address" class="form__input-group form__input-group--full-width">
                                    <label for="billingAddress" class="form__label">Billing Address</label>
                                    <input type="hidden" v-model="billingLocation.address" />
                                    <UiGeoCoder @sendAddress="settingBillingLocation($event)" :mapView="false" v-model="billingLocation.address" geocoderid="billingLocationRef" 
                                        geocoderef="geocoder2Property" />
                                </ValidationProvider>
                                <div class="form__input-group form__input-group--full-width">
                                    <label for="billingLocation" class="form__label">Billing City, State, Zip</label>
                                    <input id="billingLocation" v-model="billingLocation.cityStateZip" type="text" class="form__input" />
                                </div>
                            </div>
                            <div class="form__input-group form__input-group--full-width">
                                <label for="faxNumber" class="form__label">Fax Number</label>
                                <input type="text" id="faxNumber" class="form__input" v-model="faxNumber" />
                            </div>
                        </div>
                    </div>
                
                </div>
                <div class="form__form-group">
                    <span>Property Representative Understands and agrees to the following:</span><br/>
                    <p>
                        (SHARE) Customer will send a copy of this Agreement, Certificate of Completion, Xactimate and the {{abbreviation}} W9
                        to the Insurance Company, the Insurance Company’s Representatives, and Mortgage Institutions to allow {{abbreviation}} to
                        communicate, share, and exchange through reasonable introduction &/or through means found proper in the daily
                        course of business, information including but not limited to the services that {{abbreviation}} has provided, currently is
                        providing, &/or may be required to be provide in the future.
                    </p>
                    <p>
                        (DIRECTION OF PAYMENT) Customer composedly recognizes to administer authorization and enact direction of
                        payment immediately upon execution of this agreement to {{abbreviation}} from the Insurance Company including but not
                        limited to all invoices, tasks, billable hours, and billable units partially &/or completely provided by {{abbreviation}}.
                    </p>
                    <p>
                        (LIABILITY) <strong><u>The Customer understands this agreement is to Repair, Renovate, or Rebuild property.</u></strong> The
                        Representative understands {{abbreviation}} does not offer services outside this agreement. {{abbreviation}} is not responsible for
                        prior conditions, or damages resulting from, including but not limited to mold, hazardous materials, and structural
                        damage before, during, or after authorization of this Agreement in-directly or directly caused by {{abbreviation}}.
                    </p>
                    <p>
                        (GOOD FAITH) It is the responsibility of the Customer to act in “good faith.” Any actions or behaviors deemed
                        inappropriate by the Customer, {{abbreviation}} reserves right of the Available Proceeds of this Agreement and Customer’s
                        Insurance Company Policy. Property Representative agrees that {{abbreviation}} alone will document, scope and
                        independently account a-side the Insurance Company concerning the replacement cost and/or actual cost value of the
                        work, replacements and/or losses, and damages and/or services to the property.
                    </p>
                    <p>
                        (DIRECTION) Customer will advise the Insurance Company (and each agent, adjuster, independent adjuster, building
                        consultant, expert, engineer, or any other party acting on behalf thereof) of this Agreement, and formally Direct the
                        Insurance Company to {{abbreviation}} if contacted regarding actions &/or services authorized in this Agreement.
                    </p>
                    <p>
                        (CERTIFICATE OF COMPLETION) Customer agrees to sign the {{abbreviation}} Certificate of Completion, upon the conclusion of the project.
                    </p>
                    <p>
                        <strong>(Authority of Parties) Customer is the owner of or duly authorized agent of the owner with authority to bond
                            the Representative of the property for this Agreement. Property Representative covenants that {{abbreviation}} can
                            rely on this authority and this appointment in all dealings with the Insurance Company, insurance agents,
                            insurance adjusters, tenants and invitees of the Customer and the Property, and contractors and vendors of
                            Company.</strong>
                    </p>
                    <p>
                        (Customer Cooperation Required) To the extent that are available, the Customer will provide {{abbreviation}} with all
reasonable and necessary documents, including copies of all insurance policies, endorsements, riders, and
correspondence including invoices and proof of coverage which may assist {{abbreviation}}.
                    </p>
                    <p>
                        (Fraud) In most states, it is a crime to not pay Customer’s deductible. Property Representative understands that
Available Proceeds include the Customer’s deductible. Customer may be able to find a contractor less expensive than
{{abbreviation}}. Company settlements reflect {{abbreviation}} Cost. Using {{abbreviation}} accounting to personally gain or secondary
gain in anyway is a misrepresentation of the Customer’s true cost to the Insurance Company. The Customer
understands that misrepresenting information to the Insurance Company is a Crime. {{abbreviation}} reserves the right to
notify any authority of such malicious behavior.
                    </p>
                    <p>
                        (Timeline) In many cases, the process of getting the Insurance Company to accept the claim and its responsibility will
take time, considerable effort, and refusal at first should not be assumed to be a final decision. The goal is to make the
Customer “whole”. In many cases this is a very long costly process. Customer agrees the length of this process is
understood.
                    </p>
                    <p>
                        (No Guarantee of Coverage) {{abbreviation}} is not able to guarantee that the Customer will have insurance coverage for all
mitigation, services, repair, restoration, and renovation needed. Coverage is determined under the contract that exists
between the Insurance Company and the Customer. {{abbreviation}} has no control over this contract and, therefore, cannot
guarantee that any particular loss or damage is covered by insurance or that the amount of coverage available will
permit needed services. Customer understands in which any services herein this Agreement are not covered by The
Insurance Company the Customer is responsible for said payment, by said terms herein this Agreement.
                    </p>
                    <p>
                        (“Available Proceeds”) is defined as the potential amount of funds which {{abbreviation}} estimates the Replacement Cost
Value, using the third-party software Xactimate, including: the deductible – interest & depreciation, equals. This
amount can vary greatly depending on the skill and thoroughness of the adjustor, analyst, and {{abbreviation}}’s position.
The goal is to make the Representative “Whole”.
                    </p>
                    <p>
                        (Indemnification) The Property Representative will hold harmless and indemnify {{abbreviation}} against any and all claims
and actions arising out of the participation of this Agreement including, without limitation, Expenses, judgements,
fines, settlements and other amounts actually and reasonably incurred in connection with any liability, suit, action,
loss, or damage arising or resulting from {{abbreviation}} participation in the services provided in this agreement. Under this
agreement, indemnification will be unlimited as to the amount.
                    </p>
                </div>
                <div class="form__form-group">
                    <ol>
                        <span class="font-weight-bold">(Cancellation Rights or Privileges)</span>
                        <li>{{abbreviation}} services include company expertise, research, inventory, investigation, appraisals, calculations,
software, and other services necessary to obtain accurate estimates and provide said services. The value of
these services is not separately invoiced and is beyond simple estimation. The parties agree that the provision
of these services entitles {{abbreviation}} to compensation of an amount not yet known but, for simplicity and to
resolve uncertainty, and in liquidation, is agreed to Twenty-five percent (33%) of the Available Proceeds.
{{abbreviation}} Shall be entitled to this amount if the Customer, for any reason or no reason, breaches or does not
employ {{abbreviation}} for any services herein.</li>
                        <li>If this Agreement is canceled or breached later than three (3) days by the Customer from the execution,
Property Representative shall pay {{abbreviation}} thirty-five percent (35%) of the insurance proceeds or $10,000.00
whichever is greater, as liquidated damages, not as penalty, and Company agrees to accept such as a
reasonable and just compensation from said cancellation.</li>
                        <li>For cancellation to be effective, notice must be sent via certified mail to {{abbreviation}}.</li>
                        <li>Customer understands this thirty-five percent (35%) is for the priority mechanisms, services, travel time, and
resources that immediately go into effect when this Agreement is engaged.</li>
                        <li>Additionally, {{abbreviation}} may enforce its’ right to payment by other means including, but not limited to the
filing of liens against the property of the Customer involved in this Agreement, reporting to the appropriate
credit reporting agencies, and any other legal remedies available by Law.</li>
                    </ol>
                </div>
                <div class="form__form-group">
                    <ol>
                        <span class="font-weight-bold">(DEFAULT)</span>
                        <li>If Company is not paid by the Customer within (3) days of receipt of the Available Proceeds, Customer
agrees that all unpaid amounts will bear interest at the rate of 15% per annum (or maximum rated permitted
by law, if lower)</li>
                        <li>Entitlement to the interest will commence (3) days following Customer’s receipt of the Available Proceeds
from the Insurance Company.</li>
                        <li>Should default be made in payment of this contract, and if placed in the hand of an attorney for collection,
any and all reasonable attorney fees, and legal filings fees shall be paid by the Customer.</li>
                    </ol>
                </div>
                <div class="form__form-group">
                    <p>
                        The Customer is responsible for payment of all services, fees, rentals, treatments, betterments, and service minimums
provided regardless of Insurance Company Coverage or Non-Coverage. Whether the Available Proceeds are made
payable to the Customer or to {{abbreviation}} and Property Representative or {{abbreviation}} and the bank holding a mortgage on
the Property. Property Representative agrees that, to the extent possible, the Insurance Company shall deliver the
Available Proceeds to {{abbreviation}}. If Available Proceeds are not delivered to {{abbreviation}}, Representative agrees to pay
the Payment to {{abbreviation}} within (3) days of Property Representative’s receipt of the Available Proceeds. Property
Representative will, within (3) days of receipt of Available Proceeds endorse the Available Proceeds to {{abbreviation}} for
payment and {{abbreviation}} return the remaining Available Proceeds to Property Representative, if any
                    </p>
                    <p>
                        Except set forth in the Agreement entered by the parties, this agreement is the entire agreement between {{abbreviation}}
and the Customer with respect to the subject matter hereof and replaces any prior agreements between them,
whether verbal or written. Should any provision of this agreement be deemed invalid or unenforceable, the parties
request any court making such determination, to revise the provision at issue so that it will be valid and enforceable
to the broadest extent possible, and so that all the remaining provisions will remain valid and in full force and effect.
This agreement can only be amended or changed in writing signed by an officer of {{abbreviation}}. No waiver hereunder
will be binding unless in writing signed by the waiving party. This agreement may not be assigned except with the
written permission of {{abbreviation}}. The maximum liability for {{abbreviation}} in any event of default under this agreement is
the service minimum of the program, $1500.00. Any Salvage resulting from work under this agreement is the
property of {{abbreviation}}, including building materials, debris removed, or personal content abandoned including but not
limited to, for non-payment. The person executing this agreement for {{abbreviation}} must obtain the approval of an
officer of {{abbreviation}} for this agreement to be effective
                    </p>
                </div>
                <div class="form__form-group form__form-group--narrow">
                    <ValidationProvider name="Customer print" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--full-width">
                        <label for="customerPrint" class="form__label">Customer Print</label>
                        <input type="text" id="customerPrint" v-model="customer" class="form__input" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                </div>
                <div class="form__form-group form__form-group--narrow">
                    <LazyUiSignaturePadModal width="700px" height="219px" notrequired dialog :initial="false" sigType="customer" inputId="cusSign" :sigData="cusSign" 
                        sigRef="cusSign" name="Customer signature" />
                    <ValidationProvider name="Sign Date" rules="required" v-slot="{errors, ariaMsg}">
                        <label for="signDate" class="form__label">Date</label>
                        <imask-input id="signDate" :value="signDate" class="form__input" :mask="dateMask.mask" :pattern="dateMask.pattern" :blocks="dateMask.blocks" :format="dateMask.format" 
                            :parse="dateMask.parse" :autofix="true" :overwrite="true" :lazy="false" @complete="signDate = $event" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                </div>
                <button type="submit" class="button button--normal">{{ submitting ? 'Submitting' : 'Submit' }}</button>
            </form>
        </ValidationObserver>
    </div>
</template>
<script>
import { defineComponent, useStore, useContext, ref, watch, computed } from '@nuxtjs/composition-api'
import useReports from "@/composable/reports"
import { dateMask } from "@/data/masks"
export default defineComponent({
    props: {
        company: String,
        abbreviation: String
    },
    setup(props, { refs }) {
        const store = useStore()
        const { $api } = useContext()
        const user = computed(() => store.getters["users/getUser"])
        const reports = computed(() => store.getters["reports/getReports"])
        const { getReportPromise } = useReports()
        const selectedJobId = ref("")
        const errorDialog = ref(false)
        const submitted = ref(false)
        const submitting = ref(false)
        const jobName = ref("")
        const message = ref("")
        const location = ref({
            address: "",
            cityStateZip: ""
        })
        const billingLocation = ref({
            address: "",
            cityStateZip: ""
        })
        const contactLiaison = ref("")
        const contactPhone = ref("")
        const contactEmail = ref("")
        const emergencyLiaison = ref("")
        const emergencyPhone = ref("")
        const insuranceCompany = ref("")
        const policyNumber = ref("")
        const dateOfLoss = ref("")
        const claimNumber = ref("")
        const mortgageCompany = ref("")
        const faxNumber = ref("")
        const policyDateStart = ref(null)
        const policyDateEnd = ref(null)
        const policyDateModal = ref(false)
        const policyDateFormatted = ref("") //ref(formatDateRange(new Date().toISOString().substr(0, 10), new Date().toISOString().substr(0, 10)))
        const customer = ref("")
        const signDate = ref("")
        const cusSign = ref({
            isEmpty: true,
            data: ''
        })
        const phoneMask = ref({
            mask: '(000) 000-0000'
        })

        function formatDateRange(dateStart, dateEnd) {
            if (!dateStart && !dateEnd) return null
            if (dateStart> dateEnd) return "Invalid date range"
            const [yearStart, monthStart, dayStart] = dateStart.split('-')
            const [yearEnd, monthEnd, daysEnd] = dateEnd.split('-')
            return `${monthStart}/${dayStart}/${yearStart} - ${monthEnd}/${daysEnd}/${yearEnd}`
        }
        function getJob(jobid) {
            getReportPromise(`rapid-response/${jobid}`).then((result) => {
                jobName.value = result.cusLastName + '-' + jobid
                location.value.address = result.location.address
                location.value.cityStateZip = result.location.cityStateZip
                contactLiaison.value = result.ContactName.first + " " + result.ContactName.last
                contactPhone.value = result.phoneNumber
                contactEmail.value = result.emailAddress
                insuranceCompany.value = result.InsuranceCompany
                policyNumber.value = result.PolicyNumber
                dateOfLoss.value = result.DateOfLoss
                claimNumber.value = result.ClaimNumber
            })
        }

        async function submitForm () {
            message.value = ""
            var contractingRep = reports.value.filter((v) => {
                return v.ReportType === 'guardian-contracting-agreement'
            })
            const contractingRepId = contractingRep.map((v) => {
                return v.JobId
            })
            const post = {
                JobId: selectedJobId.value,
                ReportType: "guardian-contracting-agreement",
                formType: "contracting-agreement",
                contractingCompany: "Guardian Restoration",
                teamMember: user.value,
                jobName: jobName.value,
                jobLocation: location.value,
                contactLiaison: contactLiaison.value,
                contactPhone: contactPhone.value,
                contactEmail: contactEmail.value,
                emergencyLiaison: emergencyLiaison.value,
                emergencyPhone: emergencyPhone.value,
                billingLocation: billingLocation.value,
                faxNumber: faxNumber.value,
                mortgageCompany: mortgageCompany.value,
                insuranceCompany: insuranceCompany.value,
                dateOfLoss: dateOfLoss.value,
                claimNumber: claimNumber.value,
                policyNumber: policyNumber.value,
                policyDate: policyDateFormatted.value,
                customerPrint: customer.value,
                cusSign: cusSign.value.data,
                signDate: signDate.value
            }
            await refs.form.validate().then(success => {
                if (!success) {
                    submitting.value = false
                    submitted.value = false
                    errorDialog.value = true
                    return
                }
                if (!contractingRepId.includes(selectedJobId.value)) {
                    submitting.value = true
                    $api.$post("/api/reports/guardian-contracting-agreement/new", post, {
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
                    })
                } else  {
                    refs.form.setErrors({
                        JobId: ['Job ID already exists']
                    })
                }
            })
        }
        function settingLocation(params) {
            location.value.address = params.street
            location.value.cityStateZip = params.cityStateZip
        }
        function settingBillingLocation(params) {
            billingLocation.value.address = params.street
            billingLocation.value.cityStateZip = params.cityStateZip
        }

        watch(selectedJobId, (val) => {
            getJob(val)
        })
        watch(policyDateFormatted, (val) => {
            policyDateFormatted.value = formatDateRange(policyDateStart.value, policyDateEnd.value)
        })

        return {
            errorDialog, submitted, submitting, message, selectedJobId,
            jobName,
            location,
            contactLiaison, contactPhone, contactEmail, emergencyLiaison, emergencyPhone,
            insuranceCompany, policyNumber, dateOfLoss, claimNumber, mortgageCompany, faxNumber,
            billingLocation,
            policyDateModal, policyDateStart, policyDateEnd, policyDateFormatted,
            customer,
            signDate,
            cusSign,
            phoneMask,
            dateMask,
            submitForm,
            settingLocation, settingBillingLocation,
            reports
        }
    },
})
</script>