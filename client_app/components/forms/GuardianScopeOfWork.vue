<template>
    <div class="form-wrapper form-wrapper__scope-of-work">
        <h1 class="text-center">{{company}}</h1>
        <h2 class="text-center">Contracting Service - Scope of Work</h2>
        <p>Insurance Repair, Renovation, or Restoration & Betterment Agreement <br/> <strong>THIS SCOPE OF WORK REFERENCED IN CONTRACTING SERVICES AGREEMENT</strong><br/>
        This Scope of Work Agreement (hereinafter, this “Agreement”) is entered into below by:
Guardian Restoration Incorporated (Hereinafter, “Guardian”) 
 The Owner/Persons of legal authority (Hereinafter, “Customer”)
Pertaining to services engaged at the following described property below. </p>
        <p>The Customer, in consideration of the work to be performed, hereby engages and hires Guardian Restoration Inc. (“Guardian”) to provide Repair Services and Betterments (as defined below), and in consideration of this Agreement, Guardian Restoration agrees to provide the Repair Services and Betterments, subject to the following terms and conditions:<br />
The scope of work that Guardian estimates is necessary to fully restore and/or replace any damage to the Property is set forth below:</p>
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
                <div class="form__form-group--column form__form-group">
                    <h3>ROOF SPECIFICATIONS/ SCHEMATICS</h3>
                    <div class="flex-wrap d-flex flex-row form__form-group--col-spacing">
                        <div class="form__input-group form__input-group--long">
                            <label for="totalSquares" class="form__label">Total Squares (including waste)</label>
                            <input id="totalSquares" type="text" class="form__input" v-model="totalSquares" />
                            <span class="txt--subtext">(a roof above 30 squares possibly could take up to two days to complete)</span>
                        </div>
                        <div class="form__input-group">
                            <label for="threeTab" class="form__label">Three Tab</label>
                            <UiRadioButtonsList v-model="threeTab" :radioArr="yesNoArr" radioGroup="threeTabs" />
                        </div>
                        <div class="form__input-group">
                            <label for="shingles" class="form__label">Upgrade to Architectual Shingles</label>
                            <UiRadioButtonsList v-model="shingles" :radioArr="yesNoArr" radioGroup="shingles" />
                        </div>
                        <div class="form__input-group form__input-group--inline">
                            <label>$50xSQ</label>
                            <input type="text" class="form__input" v-model="upgradeShinglesCost" />
                        </div>
                        <div class="form__input-group">
                            <label class="form__label">Other</label>
                            <UiRadioButtonsList v-model="otherCosts" :radioArr="yesNoArr" radioGroup="OtherCosts" />
                        </div>
                        <span class="form__input--currency">
                            <span>$</span><input type="text" class="form__input form__input--short" v-model="otherPrice" />
                        </span>
                        <div class="form__input-group form__input-group--normal">
                            <label for="shingleColor" class="form__label">Shingle Color</label>
                            <input id="shingleColor" class="form__input" v-model="shingleColor" />
                        </div>
                        <div class="form__input-group form__input-group--normal">
                            <label for="shingleGrade" class="form__label">Shingle Grade</label>
                            <input id="shingleGrade" class="form__input" v-model="shingleGrade" />
                        </div>
                        <div class="form__input-group form__input-group--normal">
                            <label for="shingleBrand" class="form__label">Shingle Brand</label>
                            <input id="shingleBrand" class="form__input" v-model="shingleBrand" />
                        </div>
                        <div class="grid grid--two-column">
                            <h4 class="grid__col-span--two-col">Pitch</h4>
                            <div class="form__input-group form__input-group--normal" v-for="(item, i) in pitch" :key="`pitch-${i}`">
                                <label class="form__label" :for="item.label">{{item.label}}</label>
                                <input type="text" :id="item.label" v-model="item.value" class="form__input" />
                            </div>
                        </div>
                        <div class="form__input-group">
                            <label for="chimneyFlashing" class="form__label">Chimney Flashing</label>
                            <UiRadioButtonsList class="d-flex justify-space-between" v-model="chimneyFlashing" :radioArr="chimneyRadioArr" radioGroup="chimneyFlashing" />
                        </div>
                        <div class="form__input-group">
                            <label for="#ofChimney" class="form__label">Number of Chimney</label>
                            <UiRadioButtonsList class="form__radio-list--row" v-model="numberOfChimneys" :radioArr="numRadioArr" radioGroup="numOfChimney" />
                        </div>
                        <div class="form__input-group form__input-group--long">
                            <label class="form__label" for="decking">Decking Included</label>
                            <input type="text" id="decking" class="form__input" v-model="decking" />
                            <span class="txt--subtext">(Additional decking will be supplied and added to owner’s contract at $200.00 for Remove & Replace)</span>
                        </div>
                        <div class="d-flex flex-column">
                            <div class="d-flex flex-row flex-wrap form__form-group--col-spacing">
                                <div class="form__input-group form__input-group--normal">
                                    <label class="form__label" for="stepFlashing">Step Flashing</label>LF
                                    <input type="text" id="stepFlashing" v-model="stepFlashing" class="form__input" />
                                </div>
                                <div class="form__input-group form__input-group--normal">
                                    <label class="form__label" for="counterFlashing">Counter Flashing</label>LF
                                    <input type="text" id="counterFlashing" v-model="counterFlashing" class="form__input" />
                                </div>
                                <div class="form__input-group form__input-group--normal">
                                    <label class="form__label" for="wallFlashing">Wall Flashing</label>LF
                                    <input type="text" id="wallFlashing" v-model="wallFlashing" class="form__input" />
                                </div>
                            </div>
                            <span class="txt--subtext">(All Flashings are replaced at $5 a LFT upcharge)</span>
                        </div>
                        <div class="grid grid--two-columns">
                            <div class="form__input-group no-min-height">
                                <label class="form__label">Tear off?</label>
                                <UiRadioButtonsList class="form__radio-list--row" v-model="tearOff" :radioArr="yesNoArr" radioGroup="tearOff" />
                            </div>
                            <div class="form__input-group no-min-height">
                                <label class="form__label">Number of Layers</label>
                                <UiRadioButtonsList class="form__radio-list--row" v-model="numOfLayers" :radioArr="numRadioArr" radioGroup="numOfLayers" />
                            </div>
                            <span class="txt--subtext grid__col-span--two-col">($30 a Layer above 1 layer tear off upcharge)</span>
                        </div>
                    </div>
                </div>
                <div class="form__form-group--column">
                    <h3>BETTERMENTS</h3>
                    <div class="form__input-group--col-spacing form__betterments">
                        <span>Upgrade to Ridge vent:</span>
                        <span>Y &nbsp; $10xLF</span>
                        <span class="form__input--currency">
                            <span>$</span><input type="text" class="form__input form__input--short" v-model="ridgeVentPrice" />
                        </span>
                        <div>
                            <label for="ridgeVentType" class="form__label">Vent Type</label>
                            <input type="text" id="ridgeVentType" class="form__input form__input--normal" v-model="ridgeVentType" />
                        </div>
                        <div>
                            <label for="lfOfVents" class="form__label">#/LFT of Vents</label>
                            <input type="number" id="lfOfVents" class="form__input form__input--normal"  v-model="lfOfVents" />
                        </div>
                    </div>
                    <div class="form__input-group--col-spacing form__betterments">
                        <p>Upgrade to Valley Metal:</p>
                        <p>Y &nbsp; $5xLF</p>
                        <span class="form__input--currency">
                            <span>$</span><input type="text" class="form__input form__input--short" v-model="valleyMetalPrice" />
                        </span>
                    </div>
                    <div class="form__input-group--col-spacing form__betterments">
                        <p>Upgrade to Ice and Water:</p>
                        <p>Y &nbsp; $3.75xLF</p>
                        <span class="form__input--currency">
                            <span>$</span><input type="text" class="form__input form__input--short" v-model="iceWaterPrice" />
                        </span>
                    </div>
                    <div class="form__input-group--col-spacing form__betterments">
                        <p>Upgrade to Drip Edge:</p>
                        <p>Y &nbsp; $1.75xLF</p>
                        <span class="form__input--currency">
                            <span>$</span><input type="text" class="form__input form__input--short" v-model="dripEdgePrice" />
                        </span>
                    </div>
                    <div class="form__input-group--col-spacing form__betterments">
                        <p>Upgrade to Synthetic:</p>
                        <p>Y &nbsp; $10xSQ</p>
                        <span class="form__input--currency">
                            <span>$</span><input type="text" class="form__input form__input--short" v-model="syntheticPrice" />
                        </span>
                        <p>Felt Weight: 15#</p>
                    </div>
                    <div class="form__input-group--col-spacing form__betterments">
                        <p>Upgrade to Lead Plumbing Boots:</p>
                        <p>Y &nbsp; $10xPC</p>
                        <span class="form__input--currency">
                            <span>$</span><input type="text" class="form__input form__input--short" v-model="plumbingBootsPrice" />
                        </span>
                        <div>
                            <label class="form__label">Type of boots</label>
                            <UiRadioButtonsList class="form__radio-list--row"
                                                :radioArr="[{label: 'RB', value: 'Rubber Boot'},{label:'LB',value:'Lead Boot'}]"
                                                radioGroup="TypeBoots" v-model="bootsType" />
                        </div>
                        <div>
                            <label class="form__label">Size of boots</label>
                            <UiRadioButtonsList class="form__radio-list--row"
                                                :radioArr="[{label: '2', value: '2'},{label:'3',value:'3'}]"
                                                radioGroup="SizeBoots" v-model="bootsSize" />
                        </div>
                        <div>
                            <label class="form__label" for="numOfBoots">Number of boots</label>
                            <input id="numOfBoots" type="number" class="form__input form__input--short" v-model="numOfBoots" />
                        </div>
                    </div>
                </div>
                <div class="form__form-group--column">
                    <h3>JOBSITE INFORMATION</h3>
                    <ul class="form__form-group--listing-num">
                        <li v-for="(item, i) in jobsiteInfo" :key="`question-${i}`">
                            <label :for="`item-${i}`">{{item.question}}</label>
                            <textarea :id="`item-${i}`" rows="6" class="form__input form__input--textarea form__input--textarea-small" v-model="item.value"></textarea>
                        </li>
                    </ul>
                </div>
                <div class="form__form-group--column form__form-group form__form-group--col-spacing">
                    <h3>ADDITIONS & SUBTRACTIONS</h3>
                    <div class="form__input-group">
                        <label class="form__label">IS THERE ANY INTERIOR WORK?</label>
                        <span><strong>Detail:</strong>(colors of products, grade of product, size of products)</span>
                        <p><em>PLEASE SUMMARIZE ANY BETTERMENTS, LABEL & ATTACH AGREEMENT</em></p>
                        <textarea class="form__input form__input--textarea" v-model="anyInteriorWork"></textarea>
                    </div>
                    <div class="form__input-group">
                        <label class="form__label">IS THERE ANY EXTERIOR WORK?</label>
                        <span><strong>Detail:</strong>(colors of products, grade of product, size of products)</span>
                        <p><em>PLEASE SUMMARIZE ANY BETTERMENTS, LABEL & ATTACH AGREEMENT</em></p>
                        <textarea class="form__input form__input--textarea" v-model="anyExteriorWork"></textarea>
                    </div>
                    <div class="form__input-group">
                        <label class="form__label">IS THERE ANY WORK GUARDIAN WILL NOT BE PERFORMING?</label>
                        <p><strong>Detail:</strong></p>
                        <textarea class="form__input form__input--textarea" v-model="anyGuardianWork"></textarea>
                    </div>
                </div>
                <div v-for="(betterment, i) in bettermentDocuments" :key="`betterment-${i}`" class="form__form-group--column">
                    <h3>{{betterment.heading}}</h3>
                    <p>Representitive has initial all documents</p>
                    <div class="flex-wrap d-flex">
                        <div class="form__input-group"
                            v-for="payment in betterment.paymentArr" :key="`${betterment.id}-${payment.id}`">
                            <label class="form__label" :for="`${betterment.id}-${payment.id}`">
                                {{payment.label}}
                                <span v-if="payment.id === 'currentBalance'"
                                    v-html="'<strong>PROMISE TO PAY</strong> - BALANCE IS SUBJECT TO CHANGE'"></span>
                            </label>
                            <span class="form__input--currency">
                                <span>$</span><input :id="`${betterment.id}-${payment.id}`" type="text"
                                    class="form__input form__input--short" v-model="payment.value" />
                            </span>
                        </div>
                        <div class="form__input-group form__input-group--long">
                            <label :for="`${betterment.id}-rep-print`" class="form__label">Representitive Print</label>
                            <input type="text" class="form__input" :id="`${betterment.id}-rep-print`" v-model="betterment.repPrint"/>
                        </div>
                        <LazyUiSignaturePadModal width="650px" height="219px" inputId="repSig" :sigData="initialData" v-model="betterment.repSign" name="Representative signature" 
                            :dialog="false" :initial="false" sigRef="repSignaturePad" sigType="customer" />
                        <div class="form__input-group form__input-group--short">
                            <label class="form__label">Date</label>
                            <imask-input :id="`${betterment.id}-signdate`" :value="betterment.date" class="form__input" :mask="dateMask.mask" :pattern="dateMask.pattern" :blocks="dateMask.blocks" 
                                :format="dateMask.format" :parse="dateMask.parse" :autofix="true" :overwrite="true" :lazy="false" @complete="betterment.date = $event" />
                        </div>
                    </div>
                </div>
                <div class="form__form-group--column">
                    <h3>PROJECT STATEMENT OF ACCOUNT TO-DATE(A-F)</h3>
                    <ul class="border-style form__form-group--listing">
                        <li class="pt-2 pb-2" v-for="(item, i) in projStatement" :key="`projStatement-${i}`">
                            <label :for="`projStatement-${i}`" class="form__label">{{item.label}}</label>
                            <span class="form__input--currency">
                                <span>$</span><input :id="`projStatement-${i}`" type="text" class="form__input form__input--short" v-model="item.value" />
                            </span>
                        </li>
                        <div class="form__form-group--column">
                            <div class="form__form-group--col-spacing d-flex flex-wrap">
                                <h3>INSURANCE SCOPE OF WORK (1-4)</h3>
                                <div class="form__input-group form__input-group--short no-min-height">
                                    <label class="form__label">Xactimate Export Date</label>
                                    <imask-input id="xactimateSignDate" :value="xactimateExportDate" class="form__input form__input--short" :mask="dateMask.mask" :pattern="dateMask.pattern" :blocks="dateMask.blocks" 
                                        :format="dateMask.format" :parse="dateMask.parse" :autofix="true" :overwrite="true" :lazy="false" @complete="xactimateExportDate = $event" />
                                </div>
                            </div>
                            
                            <ul class="border-style form__form-group--listing flex-list-num">
                                <li class="grid grid--two-column pb-2 pt-2" v-for="(item, i) in insuranceScopeOfWork" :key="`iScopeOfWork-${i}`">
                                    <label :for="`insurance-scope-${i}`" class="form__label">{{item.label}}</label>
                                    <span class="form__input--currency">
                                        <span>$</span><input :id="`insurance-scope-${i}`" type="text" class="form__input form__input--short" v-model="item.value" />
                                    </span>
                                </li>
                                <div class="form__form-group--column">
                                    <h4>BETTERMENT SCOPE OF WORK(5-8)</h4>
                                    <ul class="border-style form__form-group--listing">
                                        <li class="grid grid--two-column pt-2 pb-2" v-for="(item, i) in bettermentDocuments" :key="`bettermentScope-${i}`">
                                            <label :for="`bettermentScope-${i}`" class="form__label">{{item.heading}}</label>
                                            <span class="form__input--currency">
                                                <span>$</span><input :id="`bettermentScope-${i}`" type="text" class="form__input form__input--short" v-model="item.paymentArr[0].value" />
                                            </span>
                                        </li>
                                    </ul>
                                </div>
                            </ul>
                        </div>
                        <h3 class="pt-3">Subtotals</h3>
                        <li class="pt-2 pb-2" v-for="(item, i) in subtotalsArr" :key="`subtotalsArr-${i}`">
                            <label :for="`subtotalsArr-${i}`" class="form__label">{{item.label}}</label>
                            <span class="form__input--currency">
                                <span>$</span><input :id="`subtotalsArr-${i}`" type="text" class="form__input form__input--short" v-model="item.value" />
                            </span>
                        </li>
                    </ul>
                    
                    <div class="form__form-group d-flex flex-wrap">
                        <div class="form__input-group form__input-group--normal">
                            <label class="form__label" for="finalTotal">Total</label>
                            <span class="form__input--currency">
                                <span>$</span><input id="finalTotal" type="text" class="form__input form__input--short" v-model="finalTotal" />
                            </span>
                        </div>
                        <div class="form__input-group form__input-group--normal">
                            <label class="form__label" for="finalPayment">Payment</label>
                            <span class="form__input--currency">
                                <span>$</span><input id="finalPayment" type="text" class="form__input form__input--short" v-model="finalPayment" />
                            </span>
                        </div>
                        <div class="form__input-group">
                            <label class="form__label" for="finalCurrentBalance">CURRENT BALANCE – <strong>PROMISE TO PAY</strong> – BALANCE IS SUBJECT TO CHANGE HEREIN</label>
                            <span class="form__input--currency">
                                <span>$</span><input id="finalCurrentBalance" type="text" class="form__input form__input--short" v-model="finalCurrentBalance" />
                            </span>
                        </div>
                    </div>
                </div>
                <div class="form__form-group">
                    <div>
                        <div class="form__input-group form__input-group--long">
                            <label for="cusPrint" class="form__label">Representitive Print</label>
                            <input type="text" class="form__input" id="cusPrint" v-model="finalCusPrint"/>
                        </div>
                        <div class="form__input-group form__input-group--long">
                            <label class="form__label" for="cusLicenseNumber">Customer License Number</label>
                            
                            <imask-input id="cusLicenseNumber" @complete="cusLicenseNumber = $event" class="form__input" :mask="licenseMask.mask" 
                                :prepare="licenseMask.prepare" :value="cusLicenseNumber" />
                        </div>
                    </div>
                    <div>
                        <LazyUiSignaturePadModal width="650px" height="219px" inputId="repSig" :sigData="initialData" v-model="finalCusSign" name="Customer signature" 
                            :dialog="$vuetify.breakpoint.width < 750" :initial="false" sigRef="cusSigPad" sigType="customer" />
                        <div class="form__input-group form__input-group--short">
                            <label class="form__label">Date</label>
                            <imask-input id="signDate" :value="signDate" class="form__input" :mask="dateMask.mask" :pattern="dateMask.pattern" :blocks="dateMask.blocks" 
                                :format="dateMask.format" :parse="dateMask.parse" :autofix="true" :overwrite="true" :lazy="false" @complete="signDate = $event" />
                        </div>
                    </div>
                </div>
                <button type="submit" class="button button--normal">{{ submitting ? 'Submitting' : 'Submit' }}</button>
            </form>
        </ValidationObserver>
    </div>
</template>
<script>
import { defineComponent, ref, useContext, useStore, computed } from '@nuxtjs/composition-api'
import { dateMask, driversLicenseMask } from "@/data/masks"
export default defineComponent({
    props: {
        company: String,
        abbreviation: String
    },
    setup(props, context) {
        const store = context.root.$store
        const { $fire, $api } = useContext()
        const reports = computed(() => store.getters["reports/getReports"])
        const user = computed(() => store.getters["users/getUser"])
        const submitting = ref(false);
        const initialData = ref({ data: '', isEmpty: true });
        const yesNoArr = ref([{label: 'Yes', value: 'Yes'}, {label: 'No', value: 'No'}]);
        const chimneyRadioArr = ref([{ label: 'LG', value: 'LG'}, { label: 'SM', value: 'SM' }])
        const numRadioArr = ref([
            {label: 'One', value: '1'},
            {label: 'Two', value: '2'},
            {label: 'Three', value: '3'}
        ])
        const message = ref(""); const errorDialog = ref(false); const submitted = ref(false);
        const selectedJobId = ref("")
        const totalSquares = ref("")
        const threeTab = ref("")
        const upgradeShinglesCost = ref(""); const otherCosts = ref(""); const otherPrice = ref("");
        const shingles = ref(""); const shingleColor = ref(""); const shingleGrade = ref(""); const shingleBrand = ref("");
        const pitch = ref([
            { label:"5/12-8/12", value:"" }, { label:"9/12-12/12", value:"" }
        ])
        const chimneyFlashing = ref(""); const numberOfChimneys = ref("")
        const decking = ref("");
        const stepFlashing = ref(""); const counterFlashing = ref(""); const wallFlashing = ref("");
        const tearOff = ref("");
        const numOfLayers = ref("");
        const lfOfVents = ref("");
        const ridgeVentPrice = ref(""); const ridgeVentType = ref(""); const valleyMetalPrice = ref(""); const iceWaterPrice = ref("");
        const dripEdgePrice = ref(""); const syntheticPrice = ref("");
        const plumbingBootsPrice = ref(""); const bootsType = ref(""); const bootsSize = ref(""); const numOfBoots = ref("");
        const jobsiteInfo = ref([
            {question: 'Is there an AC unit located on the property?', value:''},
            {question: 'Is there outside electrical available?', value:''},
            {question: 'Can you see the rafters, or decking in any area contractor will be roofing?', value:''},
            {question: 'Is there a screened in porch or sunroom on the home?', value:''},
            {question: 'Do you have any gutter protection systems?', value:''},
            {question: 'Is there any interior damage not related to the project?', value:''},
            {question: 'Is there any exterior damage not related to this project?', value:''},
            {question: 'Most people prefer to have the shingles delivered to their driveways, is there a good location to have the supplier deliver them?', value:''}
        ])
        const anyInteriorWork = ref(""); const anyExteriorWork = ref(""); const anyGuardianWork = ref("");
        const bettermentDocuments = ref([
            {heading: 'ROOF BETTERMENT SCOPE OF WORK', value: '', id: 'roof', paymentArr: [
                {label: 'Total', value: 'N/A', id:'total'}, {label: 'Payment', value: 'N/A', id:'payment'}, {label: 'Current Balance - ', value: 'N/A', id:'currentBalance'}
            ], date: 'N/A', repPrint: 'N/A', repSign: {data:'', isEmpty: true}},
            {heading: 'INTERIOR BETTERMENT SCOPE OF WORK', value: '', id: 'interior', paymentArr: [
                {label: 'Total', value: 'N/A', id:'total'}, {label: 'Payment', value: 'N/A', id:'payment'}, {label: 'Current Balance - ', value: 'N/A', id:'currentBalance'}
            ], date: 'N/A', repPrint: 'N/A', repSign: {data:'', isEmpty: true}},
            {heading: 'EXTERIOR BETTERMENT SCOPE OF WORK', value: '', id: 'exterior', paymentArr: [
                {label: 'Total', value: 'N/A', id:'total'}, {label: 'Payment', value: 'N/A', id:'payment'}, {label: 'Current Balance - ', value: 'N/A', id:'currentBalance'}
            ], date: 'N/A', repPrint: 'N/A', repSign: {data:'', isEmpty: true}},
            {heading: 'MISC. ADDITION/CHANGE ORDER BETTERMENT SCOPE OF WORK', value: '', id: 'misc', paymentArr: [
                {label: 'Total', value: 'N/A', id:'total'}, {label: 'Payment', value: 'N/A', id:'payment'}, {label: 'Current Balance - ', value: 'N/A', id:'currentBalance'}
            ], date: 'N/A', repPrint: 'N/A', repSign: {data:'', isEmpty: true}}
        ])
        const projStatement = ref([
            {label:'Mitigation Total (Customer Understands mitigation is owed to Mitigator separate of this scope)', value:''},
            {label:'Temporary Repair 1', value:''},
            {label:'Temporary Repair 2', value:''},
            {label:'Content Total', value:''},
            {label:'Storage Total', value:''},
            {label:'CURRENT STATEMENT OF ACCOUNT BALANCE', value:''}
        ])
        const xactimateExportDate = ref("")
        const insuranceScopeOfWork = ref([
            {label:'Net Claim/ACV(Actual  Case Value)', value:''},
            {label:'Depreciation', value:''},
            {label:'Deductible', value:''},
            {label:'Total Insurance Estimate', value:''}
        ])
        const subtotalsArr = ref([
            {label:'CURRENT STATEMENT OF ACCOUNT BALANCE', value:''},
            {label:'Insurance Scope of Work Total', value:''},
            {label:'Betterment Scope of Work Total', value:''},
            {label:'Previous Payment', value:''},
            {label:'Promotions', value:''},
            {label:'Credits', value:''},
            {label:'SUBTOTAL', value:''}
        ])
        const finalTotal = ref("")
        const finalPayment = ref("")
        const finalCurrentBalance = ref("")
        const finalCusPrint = ref("")
        const finalCusSign = ref({
            data: '',
            isEmpty: true
        })
        const signDate = ref("")
        const cusLicenseNumber = ref("")

        async function submitForm() {
            message.value = ""
            var scopeRep = reports.value.filter((v) => {
                return v.ReportType === 'guardian-scope-of-work'
            })
            const scopeRepId = scopeRep.map((v) => {
                return v.JobId
            })

            const post = {
                JobId: selectedJobId.value,
                ReportType: "guardian-scope-of-work",
                formType: "scope-of-work",
                contractingCompany: "Guardian Restoration",
                teamMember: user.value,
                totalSquares: totalSquares.value,
                threeTab: threeTab.value,
                otherCosts: otherCosts.value,
                otherPrice: otherPrice.value,
                shingles: shingles.value,
                shingleColor: shingleColor.value,
                shingleGrade: shingleGrade.value,
                shingleBrand: shingleBrand.value,
                pitch: pitch.value,
                chimneyFlashing: chimneyFlashing.value,
                numberOfChimneys: numberOfChimneys.value,
                decking: decking.value,
                stepFlashing: stepFlashing.value,
                counterFlashing: counterFlashing.value,
                wallFlashing: wallFlashing.value,
                tearOff: tearOff.value,
                numOfLayers: numOfLayers.value,
                lfOfVents: lfOfVents.value,
                ridgeVentPrice: ridgeVentPrice.value,
                ridgeVentType: ridgeVentType.value,
                valleyMetalPrice: valleyMetalPrice.value,
                iceWaterPrice: iceWaterPrice.value,
                dripEdgePrice: dripEdgePrice.value,
                syntheticPrice: syntheticPrice.value,
                plumbingBootsPrice: plumbingBootsPrice.value,
                bootsType: bootsType.value,
                bootsSize: bootsSize.value,
                numOfBoots: numOfBoots.value,
                jobsiteInfo: jobsiteInfo.value,
                anyInteriorWork: anyInteriorWork.value,
                anyExteriorWork: anyExteriorWork.value,
                anyGuardianWork: anyGuardianWork.value,
                bettermentDocuments: bettermentDocuments.value,
                projStatement: projStatement.value,
                xactimateExportDate: xactimateExportDate.value,
                insuranceScopeOfWork: insuranceScopeOfWork.value,
                subtotalsArr: subtotalsArr.value,
                finalTotal: finalTotal.value,
                finalPayment: finalPayment.value,
                finalCurrentBalance: finalCurrentBalance.value,
                finalCusPrint: finalCusPrint.value,
                finalCusSign: finalCusSign.value.data,
                signDate: signDate.value,
                cusLicenseNumber: cusLicenseNumber.value
            }

            await context.refs.form.validate().then(success => {
                if (!success) {
                    submitting.value = false
                    submitted.value = false
                    errorDialog.value = true
                    return
                }
                if (!scopeRepId.includes(selectedJobId)) {
                    $api.$post("/api/guardian-scope-of-work/new", post).then((res) => {
                        submitted.value = true
                        submitting.value = false
                        message.value = res
                    }).cathc((err) => {
                        if (err.response) {
                            submitting.value = false
                            console.error(err.response.data)
                        }
                    })
                } else {
                    context.refs.form.setErrors({
                        JobId: ['Job ID already exists']
                    })
                }
            })
        }

        return {
            submitting,
            initialData,
            dateMask,
            yesNoArr, numRadioArr, chimneyRadioArr,
            message, errorDialog, submitted,
            selectedJobId,
            totalSquares,
            threeTab,
            upgradeShinglesCost, otherCosts, otherPrice,
            shingles, shingleColor, shingleGrade, shingleBrand,
            pitch,
            chimneyFlashing, numberOfChimneys,
            decking,
            stepFlashing, counterFlashing, wallFlashing,
            tearOff, numOfLayers,
            lfOfVents,
            ridgeVentPrice, ridgeVentType, valleyMetalPrice, iceWaterPrice,
            dripEdgePrice, syntheticPrice,
            plumbingBootsPrice, bootsType, bootsSize, numOfBoots,
            jobsiteInfo,
            anyInteriorWork, anyExteriorWork, anyGuardianWork,
            bettermentDocuments,
            projStatement, xactimateExportDate, insuranceScopeOfWork, subtotalsArr,
            finalTotal, finalPayment, finalCurrentBalance, finalCusPrint, finalCusSign,
            signDate, cusLicenseNumber,
            licenseMask: driversLicenseMask,
            submitForm
        }
    },
})
</script>
<style lang="scss">
.form {
    &__betterments {
        display:grid;        
        grid-template-rows:repeat(auto-fit, minmax(72px, 1fr));
        grid-template-columns:174px 103px 165px repeat(auto-fit, minmax(100px, 1fr));
        align-items:center;
        &:not(:last-child) {
            padding-bottom:10px;
        }
    }
}
</style>