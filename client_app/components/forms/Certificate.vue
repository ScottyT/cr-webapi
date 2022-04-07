<template>
    <div class="form-wrapper">
            <!-- <h1 class="text-center">{{company}}</h1> -->
            <h2 class="text-center">Certificate of Completion</h2>
            <ValidationObserver ref="form" v-slot="{errors}">
                <v-dialog width="400px" v-model="errorDialog">
                    <div class="modal__error">
                    <div v-for="(error, i) in errors" :key="`error-${i}`">
                        <h3 class="form__input--error">{{ error[0] }}</h3>
                    </div>
                    </div>
                </v-dialog>
                <h2>{{message}}</h2>
                <h3 class="alert alert--error" v-for="(error, i) in errorMessage" :key="`server-errors-${i}`">{{error}}</h3>
                <form ref="form" class="form" @submit.prevent="submitForm" v-if="!submitted">
                    <fieldset v-if="currentStep === 1">
                        <div class="form__form-group">
                            <ValidationProvider name="Job Id" rules="required" v-slot="{errors}" class="form__input-group form__input-group--normal">
                                <input type="hidden" v-model="selectedJobId" />
                                <label for="selectJobId" class="form__label">Job ID</label>
                                <i class="form__select--icon icon--angle-down mdi" aria-label="icon"></i>
                                <select class="form__input" v-model="selectedJobId">
                                    <option disabled value="">Please select a Job Id</option>
                                    <option v-for="(item, i) in $store.state.reports.jobids" :key="`jobid-${i}`">{{item}}</option>
                                </select>
                                <span class="form__input--error">{{ errors[0] }}</span>
                            </ValidationProvider>
                        </div>
                        <p>This Assignment of Claim Agreement (hereinafter referred to as “Assignment” and/or “Agreement”) and Mitigation

                            Contract (hereinafter referred to as “Contract”) is entered into by and between:
                            {{company}} (hereinafter referred to as “{{abbreviation}}” and/or “Insured”)

                            and

                            The Owner/Persons of legal authority (hereinafter referred to as “Property Representative”)
                            of the property more commonly known as and identified by the following address:
                        </p>
                        <div class="form__form-group">
                            <ValidationProvider rules="required" class="form__input-group form__input-group--long" v-slot="{ errors }" name="Subject property">
                                <input type="text" class="form__input" v-model="subjectProperty" />
                                <span class="txt--subtext mt-2">(hereinafter referred to as “Subject Property”)</span>
                                <span class="form__input--error">{{ errors[0] }}</span>
                            </ValidationProvider>
                            <UiGeoCoder @sendAddress="settingSubjectProperty($event)" mapid="map" geocoderid="subjectProperty" geocoderef="geocoderProperty" mapView />
                        </div>
                        <p><strong>{{company}} has completed the Assignment of Benefit Agreement and Mitigation Contract in full</strong></p>
                        <p>SHARE: Property Representative will send a copy of this Agreement, Contract, Certificate of Completion, Xactimate and
                            the {{abbreviation}} W9 to the insurance company, the insurance company’s representatives and mortgage institutions to allow {{abbreviation}}
                            to communicate, share and exchange through reasonable introduction and/or through means found proper in the daily course
                            of business information including but not limited to the services that {{abbreviation}} has provided, currently is providing and/or may
                            be required to provide in the future.
                        </p>
                        <p>
                            DIRECTION OF PAYMENT: Property Representative hereby authorizes and unequivocally instructs direct payment of
                            any benefits or proceeds for services rendered pursuant to the Assignment of Claim Agreement and Mitigation Contract and
                            that such payment be made payable directly and solely to {{abbreviation}} and sent exclusively to {{abbreviation}} from the insurance company
                            including but not limited to all invoices, tasks, billable hours and billable units partially and /or completely provided by
                            {{abbreviation}}.
                        </p>
                        <div class="form__form-group form__form-group--inline form__form-group--info-box">
                            <span class="form__input-group--inline">
                                <div class="form__input-group--section">
                                    <label for="InsuredDeductible" class="form__label">Insured Deductible</label>
                                    <span class="currency-input">$</span><input type="text" id="InsuredDeductible" class="form__input form__input--short"
                                        v-model="deductible" />
                                </div>
                            </span>
                            <span class="form__input-group--inline">
                                <div class="form__input-group--section">
                                    <label for="InsuredEndDate" class="form__label">Insured: Agreed “Term” of Service Minimum End Date</label>
                                    <UiDatePicker dateId="InsuredEndDate" dialogId="dialogEndDate" @date="insuredEndDateFormatted = $event" />
                                </div>
                            </span>
                            <span class="form__input-group--inline">
                                <div class="form__input-group--section">
                                    <label for="insuredPayment1" class="form__label">Insured Payment 1)</label>
                                    <span class="form__input--currency">
                                        <span>$</span><input type="text" class="form__input form__input--short" v-model="insuredPayment.firstStep" />
                                    </span>
                                </div>
                                <div class="form__input-group--inline form__input-group--date-group">
                                    <label for="insuredDay1" class="form__label">Day (1) Date</label>
                                    <UiDatePicker dateId="insuredDay1" dialogId="insuredPayDay1" @date="insuredPayment.day1DateFormatted = $event" />
                                </div>
                            </span>
                            <span class="form__input-group--inline">
                                <div>
                                    <label for="insuredPayment2" class="form__label">Insured Payment 2)</label>
                                    <span class="form__input--currency">
                                        <span>$</span><input id="insuredPayment2" type="text" class="form__input form__input--short" v-model="insuredPayment.secondStep" readonly />
                                    </span>
                                </div>
                                <div class="form__input-group--inline form__input-group--date-group">
                                    <label for="insuredDay5" class="form__label">Day (5) Date (upon pickup)</label>
                                    <UiDatePicker dateId="insuredDay5" dialogId="insuredPayDay5" @date="insuredPayment.day5DateFormatted = $event" />
                                </div>
                            </span>
                            <span class="form__input-group--inline">
                                <div>
                                    <label class="form__label">Insured Payment 3) = “Available Proceeds” Per terms of this agreement</label>
                                </div>
                            </span>
                            <v-spacer></v-spacer>
                            <span class="form__input-group--inline">
                                <label class="form__label">Insured Payment 4) = Succeeding Day of “Available Proceeds” for Remaining Balance</label>
                            </span>
                        </div>
                        <div class="form__form-group form__form-group--inline form__form-group--info-box">
                            <span class="form__input-group--inline form__input-group--date-group">
                                <label for="NonInsuredEndDate" class="form__label">Non-Insured or Still Pending Coverage: Agreed “Term” of Service Minimum End Date</label>
                                <UiDatePicker dateId="NonInsuredEndDate" dialogId="dialognoninsuredEndDate" @date="nonInsuredPayment.endDateFormatted = $event" />
                            </span>
                            <span class="form__input-group--inline form__input-group--date-group">
                                <label for="NonInsuredDay1Date" class="form__label">Non-Insured or Still Pending Coverage: Payment 1) = $750.00 Day (1) Date</label>
                                <UiDatePicker dateId="NonInsuredDay1Date" dialogId="nonInsuredDay1" @date="nonInsuredPayment.day1DateFormatted = $event" />
                            </span>
                            <span class="form__input-group--inline form__input-group--date-group">
                                <label for="NonInsuredDay5Date" class="form__label">Non-Insured or Still Pending Coverage: Payment 2) = $750.00 Day (5) Date</label>
                                <UiDatePicker dateId="NonInsuredDay5Date" dialogId="nonInsuredDay5" @date="nonInsuredPayment.day5DateFormatted = $event" />
                            </span>
                            <span class="form__input-group--inline">
                                <label for="NonInsuredDay5Date" class="form__label">Non-Insured or Still Pending Coverage: Payment 3) = Succeeding Day of the Term for Remaining Balance</label>                           
                            </span>
                        </div>
                        <ValidationProvider rules="required" name="Rating" v-slot="{errors}" class="form__form-group">
                            <span><strong>Please Circle: RATE YOUR EXPIERENCE WITH OUR SERVICES:</strong></span>
                            <ul class="form__form-group--inline mt-3">
                                <li v-for="(item, i) in ratings" :key="`ratings-${i}`" class="form__input--radio">
                                    <label :for="item">{{item}}</label>
                                    <input :id="item" type="radio" v-model="selectedRating" :value="item" />
                                </li>
                            </ul>
                            <span class="form__input--error">{{ errors[0] }}</span>
                        </ValidationProvider>
                        <p>I am satisfied with the completion of all mitigation services and all other related services performed on the Subject Property by {{company}}.</p>
                        <div class="form__form-group">
                            <span class="form__input-group--inline">
                                <ValidationProvider rules="required" name="Representative" v-slot="{errors}" class="form__input-group form__input-group--long">
                                    <label for="repPrint" class="form__label">Representative (Print)</label>
                                    <input id="repPrint" type="text" class="form__input" v-model="repPrint" />
                                    <span class="form__input--error">{{ errors[0] }}</span>
                                </ValidationProvider>
                                <div class="form__input-group form__input-group--short">
                                    <label for="timeRepPrint" class="form__label">Time</label>
                                    <v-dialog ref="timeRepDialog" v-model="repTimeModal" :return-value.sync="repPrintTime" light persistent max-width="320px">
                                        <template v-slot:activator="{ on, attrs }">
                                            <input id="NonInsuredDay5Date" v-model="repPrintTimeFormatted" v-bind="attrs" class="form__input form__input--short" v-on="on" readonly />
                                        </template>
                                        <v-time-picker v-model="repPrintTime" scrollable>
                                            <v-spacer></v-spacer>
                                            <v-btn text color="#fff" @click="repTimeModal = false">Cancel</v-btn>
                                            <v-btn text color="#fff" @click="$refs.timeRepDialog.save(repPrintTime)">OK</v-btn>
                                        </v-time-picker>
                                    </v-dialog>
                                </div>
                            </span>
                            <LazyUiSignaturePadModal width="700px" height="219px" dialog :initial="false" sigType="customer" inputId="repSigPad" 
                                :sigData="repSign" sigRef="repSigPad" name="Representative Signature" />
                            <span class="form__input-group--inline">
                                <div class="form__input-group form__input-group--short">
                                    <label for="dateRepSign" class="form__label">Date</label>
                                    <UiDatePicker dateId="dateRepSign" dialogId="dialogRepSign" @date="repSignDateFormatted = $event" />
                                </div>
                            </span>
                            <LazyUiSignaturePadModal v-model="empSig" width="700px" height="219px" :initial="false" :sigData="teamMemberSig" inputId="teamMemberSig" sigType="employee" 
                                sigRef="teamMemberSigPad" name="Team Member Signature" />
                            <div class="form__input-group form__input-group--short">
                                <label for="dateTeamSign" class="form__label">Date</label>
                                <UiDatePicker dateId="dateTeamSign" dialogId="dialogTeamSign" @date="teamSignDateFormatted = $event" />
                            </div>
                            <div class="form__input-group">
                                <label for="testimonial" class="form__label">Would you care to give a Short Testimonial Note:</label>
                                <textarea id="testimonial" v-model="testimonial" class="form__input--textarea form__input"></textarea>                              
                            </div>
                            <ValidationProvider name="Payment option" rules="required" v-slot="{errors}" class="form__input-group mt-3">
                                <p class="form__label">Which payment method will you use?</p>
                                <ul class="form__form-group--inline">
                                    <li v-for="(item, i) in paymentOptions" :key="`payment-${i}`" class="form__input--radio">
                                        <label :for="item">{{item}}</label>
                                        <input :id="item" type="radio" v-model="paymentOption" :value="item" />
                                    </li>
                                </ul><br/>
                                <span class="form__input--error">{{ errors[0] }}</span>
                            </ValidationProvider>
                            <ValidationProvider v-if="paymentOption === 'Card'" name="Existing credit card" rules="required" v-slot="{errors}" class="form__input-group mt-3">
                                <p class="form__label">Are you using an existing credit/debit card?</p>
                                <ul class="form__form-group--inline">
                                    <li class="form__input--radio">
                                        <label for="Yes">Yes</label>
                                        <input id="Yes" type="radio" v-model="existingCreditCard" value="Yes" />
                                    </li>
                                    <li class="form__input--radio">
                                        <label for="No">No</label>
                                        <input id="No" type="radio" v-model="existingCreditCard" value="No" />
                                    </li>
                                </ul>
                                <span class="form__input--error">{{ errors[0] }}</span>
                            </ValidationProvider>
                            <ValidationProvider v-if="existingCreditCard === 'Yes' && paymentOption === 'Card'" name="Card to link" rules="required" v-slot="{errors}"
                                class="form__input-group form__input-group--long">
                                <input type="hidden" v-model="cardToUse" />
                                <label class="form__label">Card number</label>
                                <i class="form__select--icon icon--angle-down mdi" aria-label="icon"></i>
                                <select class="form__input" v-model="cardToUse">
                                    <option disabled value="">Please select a credit/debit card</option>
                                    <option v-for="(item, i) in cardNumbers" :key="`cardnumbers-${i}`">{{item}}</option>
                                </select>
                                <span class="form__input--error">{{ errors[0] }}</span>                         
                            </ValidationProvider>
                        </div>
                    </fieldset>
                    <LazyFormsCreditCard v-if="currentStep >= 2 && paymentOption == 'Card'" ref="creditCardForm"  :partOfLastSection="true" :jobId="selectedJobId" :repPrint="repPrint"
                        @submit="submitForm" @cardSubmit="cardSubmittedValue" company="Water Emergency Services Incorporated" abbreviation="WESI" />
                    <v-btn type="submit" v-if="currentStep === 1 && (paymentOption == 'Card' && existingCreditCard == 'No')">Next</v-btn>
                    <button type="submit" :class="cardSubmitted || (paymentOption !== 'Card' || existingCreditCard !== 'No') ? 'button button--normal' : 'button--disabled'">
                        {{ submitting ? 'Submitting' : 'Submit' }}
                    </button>
                </form>
            </ValidationObserver>
        </div>
</template>
<script>
import goTo from 'vuetify/es5/services/goto'
import {mapGetters, mapActions} from 'vuex'
export default {
    props: ['company', 'abbreviation'],
    data: (vm) => ({
        currentStep:1,
        message: '',
        errorMessage: [],
        submitted: false,
        submitting: false,
        subjectProperty: '',
        deductible: null,
        insuredEndDateModal: false,
        insuredEndDate: new Date().toISOString().substr(0, 10),
        insuredEndDateFormatted: vm.formatDate(new Date().toISOString().substr(0, 10)),
        insuredPayment: {
            day1Modal: false,
            day5Modal: false,
            firstStep: null,
            secondStep:null,
            day1Date: new Date().toISOString().substr(0, 10),
            day1DateFormatted: vm.formatDate(new Date().toISOString().substr(0, 10)),
            day5Date: new Date().toISOString().substr(0, 10),
            day5DateFormatted: vm.formatDate(new Date().toISOString().substr(0, 10))
        },
        nonInsuredPayment: {
            endDateModal: false,
            day1Modal: false,
            day5Modal: false,
            endDate: new Date().toISOString().substr(0, 10),
            endDateFormatted: vm.formatDate(new Date().toISOString().substr(0, 10)),
            day1Date: new Date().toISOString().substr(0, 10),
            day1DateFormatted: vm.formatDate(new Date().toISOString().substr(0, 10)),
            day5Date: new Date().toISOString().substr(0, 10),
            day5DateFormatted: vm.formatDate(new Date().toISOString().substr(0, 10)),
        },
        ratings: [
            "A+", "A", "A-", "B+", "B", "B-", "C+", "C", "C-"
        ],
        selectedRating: "",
        repPrint: "",
        repSign: {
            data: '',
            isEmpty: true
        },
        repTimeModal: false,
        repPrintTime: null,
        repPrintTimeFormatted: vm.formatTime(new Date().toTimeString().substr(0, 5)),
        repSignModal: false,
        repSignDate: new Date().toISOString().substr(0, 10),
        repSignDateFormatted: vm.formatDate(new Date().toISOString().substr(0, 10)),
        teamMemberSig: {
            data: '',
            isEmpty: true
        },
        empSig: "",
        teamSignModal: false,
        teamSignDate: new Date().toISOString().substr(0, 10),
        teamSignDateFormatted: vm.formatDate(new Date().toISOString().substr(0, 10)),
        testimonial:"",
        usingCreditCard: false,
        cusSig: {
            data: '',
            isEmpty: true
        },
        cusSigModal: false,
        cusSigDate: new Date().toISOString().substr(0, 10),
        cusSigDateFormatted: vm.formatDate(new Date().toISOString().substr(0, 10)),
        selectedJobId: "",
        paymentOptions: ["Cash", "Card"],
        paymentOption: "",
        errorDialog: false,
        existingCreditCard: "",
        cardToUse: "",
        cardObj: {},
        cardSubmitted: false
    }),
    computed: {
        ...mapGetters({getUser: "users/getUser", getReports: "reports/getReports", getCards:'reports/getCards'}),
        insuredDay1() {
            return this.insuredPayment.day1Date;
        },
        insuredDay5() {
            return this.insuredPayment.day5Date
        },
        nonInsuredEndDate() {
            return this.nonInsuredPayment.endDate;
        },
        nonInsuredDay1() {
            return this.nonInsuredPayment.day1Date;
        },
        nonInsuredDay5() {
            return this.nonInsuredPayment.day5Date;
        },
        insuredPay1: {
            get() {
                var pay = this.deductible * .50
                if (pay) { this.insuredPayment.firstStep = pay }
                else { this.insuredPayment.firstStep = "N/A" }
            },
            set(val) {
                this.insuredPayment.firstStep = val
            }
        },
        insuredPay2: {
            get() {
                var pay = this.deductible * .50
                if (pay) { this.insuredPayment.secondStep = pay }
                else { this.insuredPayment.secondStep = "N/A" }
            },
            set(val) {
                this.insuredPayment.secondStep = val
            }
        },
        certificates() {
            return this.getReports.filter((v) => {
                return v.formType === "coc"
            })
        },
        cardNumbers() {
            return this.getCards.map((v) => {
                return v.cardNumber
            })
        }
    },
    watch: {
        insuredEndDate(val) {
            this.insuredEndDateFormatted = this.formatDate(val)
        },
        insuredDay1(val) {
            this.insuredPayment.day1DateFormatted = this.formatDate(val)
        },
        insuredDay5(val) {
            this.insuredPayment.day5DateFormatted = this.formatDate(val)
        },
        nonInsuredEndDate(val) {
            this.nonInsuredPayment.endDateFormatted = this.formatDate(val)
        },
        nonInsuredDay1(val) {
            this.nonInsuredPayment.day1DateFormatted = this.formatDate(val)
        },
        nonInsuredDay5(val) {
            this.nonInsuredPayment.day5DateFormatted = this.formatDate(val)
        },
        repPrintTime(val) {
            this.repPrintTimeFormatted = this.formatTime(val)
        },
        repSignDate(val) {
            this.repSignDateFormatted = this.formatDate(val)
        },
        teamSignDate(val) {
            this.teamSignDateFormatted = this.formatDate(val)
        },
        cusSigDate(val) {
            this.cusSigDateFormatted = this.formatDate(val)
        },
        selectedJobId(val) {
            this.$api.$get(`/api/reports/details/dispatch/${val}`).then((res) => {
                this.subjectProperty = res.location.address + ", " + res.location.cityStateZip
            }).catch(err => {
                this.errorMessage = err
            })
        }
    },
    methods: {
        selectedJob(option) {
            this.selectedJobId = option;
        },
        formatDate(dateReturned) {
            if (!dateReturned) return null
            const [year, month, day] = dateReturned.split('-')
            return `${month}/${day}/${year}`
        },
        cardSubmittedValue(...params) {
            const {isSubmit, cardNumber} = params[0]
            this.cardObj = params
            this.cardToUse = cardNumber
            this.cardSubmitted = isSubmit
        },
        formatTime(timeReturned) {
            if (!timeReturned) return null
            
            const pieces = timeReturned.split(':')
            let hours
            let minutes
            if (pieces.length === 2) {
                hours = parseInt(pieces[0], 10)
                minutes = parseInt(pieces[1], 10)
            }
            const newFormat = hours >= 12 ? 'PM' : 'AM'
            hours = hours % 12
            // To display "0" as "12"
            hours = hours || 12
            minutes = minutes < 10 ? '0' + minutes : minutes
            return `${hours}:${minutes} ${newFormat}`
        },
        parseDate(date) {
            if (!date) return null
            const [month, day, year] = date.split('/')
            return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`
        },
        currencyFormat($event) {
            let keyCode = ($event.keyCode ? $event.keyCode : $event.which);
            // only allow number and one dot
            if ((keyCode < 48 || keyCode > 57) && (keyCode !== 46 || this.deductible.indexOf('.') != -1)) { // 46 is dot
                $event.preventDefault();
            }
            // restrict to 2 decimal places
            if(this.deductible!=null && this.deductible.indexOf(".")>-1 && (this.deductible.split('.')[1].length > 1)){
                $event.preventDefault();
            }
        },
        acceptNumber() {
            var x = this.cardholderName.phoneNumber.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/)
            this.cardholderName.phoneNumber = !x[2] ?
            x[1] :
            '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '')
        },
        maskDate() {
            var x = this.expirationDate.replace(/\D/g, '').match(/(\d{0,2})(\d{0,2})/)
            console.log(x)
            this.expirationDate = !x[2] ? x[1] : x[1] + '/' + x[2]
        },
        goToStep(step) {
            if (step < 1) {
                return;
            }
            if (step > 2) {
                this.submitForm();
                return;
            }
            this.currentStep = step
        },
        submitForm() {  
            this.errorMessage = []
            const certificates = this.certificates.map((v) => {
                return v.JobId
            })
            this.$refs.form.validate().then(success => {
                if (!success) {
                    this.errorDialog = true
                    this.submitting = false
                    this.submitted = false
                    return goTo(0)
                }
                if (!certificates.includes(this.selectedJobId)) {
                    if ((this.currentStep === 1 && this.paymentOption !== 'Card' || this.existingCreditCard !== 'No') ||
                        this.currentStep === 2) {
                        this.submitting = true
                        this.onSubmit()
                        return;
                    }
                    this.currentStep++;
                } else {
                    this.errorMessage.push("Duplicate Job ID is not allowed")
                    return goTo(0)
                }
                
            })            
        },
        onSubmit() {
            this.message = ''
            this.errorMessage = []
            let insuredPayment1Arr = {
              insuredPay: this.insuredPayment.firstStep,
              day1Date: this.insuredPayment.day1DateFormatted
            };
            let insuredPayment2Arr = {
              insuredPay: this.insuredPayment.secondStep,
              day5Date: this.insuredPayment.day5DateFormatted
            };
            const post = {
              JobId: this.selectedJobId,
              ReportType: "wesi-coc",
              formType: "coc",
              contractingCompany: "Water Emergency Services",
              subjectProperty: this.subjectProperty,
              deductible: this.deductible,
              insuredMinEndDate: this.insuredEndDateFormatted,
              insuredPayment1: insuredPayment1Arr,
              insuredPayment2: insuredPayment2Arr,
              nonInsuredMinEndDate: this.nonInsuredPayment.endDateFormatted,
              nonInsuredPayment1: this.nonInsuredPayment.day1DateFormatted,
              nonInsuredPayment2: this.nonInsuredPayment.day5DateFormatted,
              rating: this.selectedRating,
              representative: this.repPrint,
              repSignTime: this.repPrintTimeFormatted,
              representativeSign: this.repSign.data,
              repSignDate: this.repSignDateFormatted,
              teamSign: Object.keys(this.empSig).length !== 0,
              teamSignDate: this.teamSignDateFormatted,
              testimonial: this.testimonial,
              paymentOption: this.paymentOption,
              teamMember: this.getUser,
              cardNumber: this.cardToUse
            };
            this.$api.$post("/api/reports/wesi-coc/new", post, {
                params: {
                    jobid: selectedJobId.value
                }
            }).then((res) => {
                if (res.error) {
                    this.errorMessage = res.message
                    return
                }
                this.message = res
                this.submitted = true
                setTimeout(() => {
                    this.message = ""
                    window.location = "/"
                }, 3000)
            }).catch((err) => {
                this.errorMessage.push(err)
            })
        },
        settingSubjectProperty(params) {
            this.subjectProperty = params.address
        }
    }
}
</script>