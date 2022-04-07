<template>
    <div v-if="Object.keys(rep).length === 0">
        <h1>No report</h1>
    </div>
    <section slot="pdf-content" v-else>
        <div class="text-center pdf-first-section">
            <h1 class="text-center">{{company}}</h1>
            <h2 class="text-center">Contracting Service - Scope of Work</h2>
            <p>Insurance Repair, Renovation, or Restoration & Betterment Agreement <br/> <strong>THIS SCOPE OF WORK REFERENCED IN CONTRACTING SERVICES AGREEMENT</strong><br/>
            This Scope of Work Agreement (hereinafter, this “Agreement”) is entered into below by:
            Guardian Restoration Incorporated (Hereinafter, “Guardian”) 
            The Owner/Persons of legal authority (Hereinafter, “Customer”)
            Pertaining to services engaged at the following described property below </p>
                    <p>The Customer, in consideration of the work to be performed, hereby engages and hires Guardian Restoration Inc. (“Guardian”) to provide Repair Services and Betterments (as defined below), and in consideration of this Agreement, Guardian Restoration agrees to provide the Repair Services and Betterments, subject to the following terms and conditions:<br />
            The scope of work that Guardian estimates is necessary to fully restore and/or replace any damage to the Property is set forth below:</p>
        </div>
        <div class="report-details__section">
            <h3>ROOF SPECIFICATIONS/ SCHEMATICS</h3>
            <div class="report-details__data" v-for="(item, i) in roofSpecsSection" :key="`roof-specs-${i}`">
                <label>{{item.label}}:</label><span v-if="item.hasOwnProperty('sub')"><i>{{item.sub}}</i></span>
                <h4>{{item.value}}</h4>
            </div>
            <div><i>(All Flashings are replaced at $5 a LFT upcharge)</i></div>
            <div><i>($30 a Layer above 1 layer tear off upcharge)</i></div>
        </div>
        <div class="report-details__section">
            <h3>BETTERMENTS</h3>
            <div class="report-details__section report-details__section--row">
                <span>Upgrade to Ridge vent:</span>
                <span>Y &nbsp; $10xLF</span>
                <span class="form__input--currency">
                    <span>$</span><span>{{report.ridgeVentPrice}}</span>
                </span>
                <div class="report-details__data report-details__data--row">
                    <label for="ridgeVentType" class="form__label">Vent Type:  </label>
                    <div>{{report.ridgeVentType}}</div>
                </div>
                <div class="report-details__data report-details__data--row">
                    <label for="lfOfVents" class="form__label">#/LFT of Vents:  </label>
                    <div>{{report.lfOfVents}}</div>
                </div>
            </div>
            <div class="report-details__section report-details__section--row">
                <p>Upgrade to Valley Metal:</p>
                <p>Y &nbsp; $5xLF</p>
                <span class="form__input--currency">
                    <span>$</span><span>{{report.valleyMetalPrice}}</span>
                </span>
            </div>
            <div class="report-details__section report-details__section--row">
                <p>Upgrade to Ice and Water:</p>
                <p>Y &nbsp; $3.75xLF</p>
                <span class="form__input--currency">
                    <span>$</span><span>{{report.iceWaterPrice}}</span>
                </span>
            </div>
            <div class="report-details__section report-details__section--row">
                <p>Upgrade to Drip Edge:</p>
                <p>Y &nbsp; $1.75xLF</p>
                <span class="form__input--currency">
                    <span>$</span><span>{{report.dripEdgePrice}}</span>
                </span>
            </div>
            <div class="report-details__section report-details__section--row">
                <p>Upgrade to Synthetic:</p>
                <p>Y &nbsp; $10xSQ</p>
                <span class="form__input--currency">
                    <span>$</span><span>{{report.syntheticPrice}}</span>
                </span>
                <p>Felt Weight: 15#</p>
            </div>
            <div class="report-details__section report-details__section--row">
                <p>Upgrade to Lead Plumbing Boots:</p>
                <p>Y &nbsp; $10xPC</p>
                <span class="form__input--currency">
                    <span>$</span><span>{{report.plumbingBootsPrice}}</span>
                </span>
                <div class="report-details__data report-details__data--row">
                    <label class="form__label">Type of boots:</label>
                    <div>{{report.bootsType}}</div>
                </div>
                <div class="report-details__data report-details__data--row">
                    <label class="form__label">Size of boots:</label>
                    <div>{{report.bootsSize}}</div>
                </div>
                <div class="report-details__data report-details__data--row">
                    <label class="form__label" for="numOfBoots">Number of boots:</label>
                    <div>{{report.numOfBoots}}</div>
                </div>
            </div>
        </div>
        <div class="report-details__seciton">
            <h3>JOBSITE INFORMATION</h3>
            <div class="report-details__data report-details__data--row border-underline" v-for="(item, i) in report.jobsiteInfo" :key="`jobsiteinfo-${i}`">
                <label>{{item.question}}</label>
                <div>{{item.value}}</div>
            </div>
        </div>
        <div class="report-details__section">
            <h3>ADDITIONS & SUBSTRACTIONS</h3>
            <div class="report-details__data" v-for="(item, i) in additionsAndSubtractions" :key="`additions-${i}`">
                <label><strong>{{item.label}}</strong><span v-if="item.hasOwnProperty('sub')">{{item.sub}}</span></label>
                <div class="report-details__textarea">{{item.value}}</div>
            </div>
        </div>
        <div class="report-details" v-for="(item, i) in report.bettermentDocuments" :key="`betterment-docs-${i}`">
            <h3>{{item.heading}}</h3>
            <div>{{item.value}}</div>
            <div class="border-underline" v-for="(payment, j) in item.paymentArr" :key="`payment-${j}`">
                <label>{{payment.label}}</label>
                <span class="form__input--currency">
                    <span>$</span><span>{{payment.value}}</span>
                </span>
            </div>
            <div class="border-underline">
                <label>Representitive Print</label>
                <div>{{item.repPrint}}</div>
            </div>
            <div class="border-underline">
                <label>Representitive Sign</label>
                <div class="report-details__signature">
                    <img :src="item.repSign" v-if="item.repSign.data !== ''" />
                    <div v-else>N/A</div>
                </div>
            </div>
            <div class="border-underline">
                <label>Date:</label>
                <div>{{item.date}}</div>
            </div>
        </div>
        <div class="report-details__section flex-column">
            <h4>PROJECT STATEMENT OF ACCOUNT TO-DATE(A-F)</h4>
            <ul class="report-details__list">
                <li v-for="(item, i) in report.projStatement" :key="`prostatement-${i}`">
                    <label>{{item.label}}: </label>
                    <span class="form__input--currency">
                        <span>$</span><span>{{item.value}}</span>
                    </span>
                </li>
                <div class="report-details__section">
                    <div class="report-details__data report-details__data--row">
                        <h4>INSURANCE SCOPE OF WORK (1-4)</h4>
                        <strong>Xactimate Export Date: </strong>
                        <div>{{report.xactimateExportDate}}</div>
                    </div>
                </div>
                <ul class="flex-list-num report-details__list--sublist">
                    <li class="grid grid--two-column border-underline" v-for="(item, j) in report.insuranceScopeOfWork" :key="`insurance-scope-${j}`">
                        <label>{{item.label}}</label>
                        <span class="form__input--currency">
                            <span>$</span><span>{{item.value}}</span>
                        </span>
                    </li>
                    <div class="report-details__list">
                        <h4>BETTERMENT SCOPE OF WORK (5-8)(pages 1-6 Above)</h4>
                        <li class="grid grid--two-column border-underline"
                            v-for="(item, j) in report.bettermentDocuments" :key="`betterment-scope-${j}`">
                            <label>{{item.heading}}</label>
                            <span class="form__input--currency">
                                <span>$</span><span>{{item.paymentArr[0].value !== "" ? item.paymentArr[0].value : "N/A"}}</span>
                            </span>
                        </li>
                    </div>
                </ul>
                <div class="report-details__section report-details__section--column">
                    <h4>SUBTOTALS</h4>
                    <div class="report-details__data">
                        <li v-for="(item, i) in report.subtotalsArr" :key="`subtotalsarr-${i}`">
                            <label>{{item.label}}: </label>
                            <span class="form__input--currency">
                                <span>$</span><span>{{item.value}}</span>
                            </span>
                        </li>
                    </div>
                </div>
            </ul>
        </div>
        <div class="report-details__section report-details__section--column">
            <div class="border-underline">
                <label>Total: </label>
                <span class="form__input--currency">
                    <span>$</span><span>{{report.finalTotal}}</span>
                </span>
            </div>
            <div class="border-underline">
                <label>Payment: </label>
                <span class="form__input--currency">
                    <span>$</span><span>{{report.finalPayment}}</span>
                </span>
            </div>
            <div class="border-underline">
                <label>CURRENT BALANCE – <strong>PROMISE TO PAY</strong> – BALANCE IS SUBJECT TO CHANGE HEREIN: </label>
                <span class="form__input--currency">
                    <span>$</span><span>{{report.finalCurrentBalance}}</span>
                </span>
            </div>
        </div>
        <div class="report-details__section grid grid--two-column">
            <div>
                <div class="report-details__data">
                    <label>Customer Print: </label>
                    <div>{{report.finalCusPrint}}</div>
                </div>
                <div class="report-details__data">
                    <label>Customer License Number: </label>
                    <div>{{report.cusLicenseNumber}}</div>
                </div>
            </div>
            <div>
                <div class="report-details__data">
                    <label>Customer Signature: </label>
                    <div class="report-details__signature">
                        <img v-if="report.hasOwnProperty('finalCusSign')" :src="report.finalCusSign" />
                        <div v-else>N/A</div>
                    </div>
                </div>
                <div class="report-details__data">
                    <label>Date: </label>
                    <div>{{report.signDate}}</div>
                </div>
            </div>
        </div>
    </section>
</template>
<script>
import { defineComponent,onMounted,ref, toRefs, watch } from '@nuxtjs/composition-api'
import useReports from "@/composable/reports"
export default defineComponent({
    props: {
        rep: Object,
        company: String,
        abbreviation: String
    },
    setup(props) {
        const { rep } = toRefs(props)
        const { getReport, report } = useReports()
        const roofSpecsSection = ref([
            {label: 'Total Squares (including waste)', value: rep.value.totalSquares, sub:'(a roof above 30 squares possibly could take up to two days to complete)'},
            {label: 'Three Tab', value: rep.value.threeTab},
            {label: 'Upgrade to Architectual Shingles', value: rep.value.shingles},
            {label: 'Other', value: rep.value.otherCosts},
            {label: 'Other costs', value: rep.value.otherPrice, currency: true},
            {label: 'Shingle Color', value: rep.value.shingleColor},
            {label: 'Shingle Brand', value: rep.value.shingleBrand},
            {label: 'Shingle Grade', value: rep.value.shingleGrade},
            {label: 'Pitch 5/12-8/12', value: rep.value.pitch[0].value},
            {label: 'Pitch 9/12-12/12', value: rep.value.pitch[1].value},
            {label: 'Chimney Flashing', value: rep.value.chimneyFlashing},
            {label: 'Number of Chimneys', value: rep.value.numberOfChimneys},
            {label: 'Decking included', value: rep.value.decking, sub:'(Additional decking will be supplied and added to owner’s contract at $200.00 for Remove & Replace)'},
            {label: 'Step Flashing LF', value: rep.value.stepFlashing},
            {label: 'Counter Flashing LF', value: rep.value.counterFlashing},
            {label: 'Wall Flashing', value: rep.value.wallFlashing},
            {label: 'Tear Off', value: rep.value.tearOff},
            {label: '# of Layers', value: rep.value.numOfLayers}
        ])
        const additionsAndSubtractions = ref([
            {
                label: 'IS THERE ANY INTERIOR WORK?  Detail: ', 
                sub: '(colors of products, grade of product, size of products) PLEASE SUMMARIZE ANY BETTERMENTS, LABEL & ATTACH AGREEMENT)',
                value: rep.value.anyInteriorWork
            },
            {
                label: 'IS THERE ANY EXTERIOR WORK?  Detail: ', 
                sub: '(colors of products, grade of product, size of products) PLEASE SUMMARIZE ANY BETTERMENTS, LABEL & ATTACH AGREEMENT)',
                value: rep.value.anyExteriorWork
            },
            {
                label: 'IS THERE ANY WORK GUARDIAN WILL NOT BE PERFORMING? Detail: ', 
                value: rep.value.anyGuardianWork
            }
        ])
        getReport(`${rep.value.ReportType}/${rep.value.formType}/${rep.value.JobId}`).fetchReport()

        return {
            roofSpecsSection,
            additionsAndSubtractions,
            report,
            getReport
        }
    },
})
</script>
<style lang="scss">
.form {
    &__betterments {
        display:flex;        
        flex-wrap:wrap;
    }
}
</style>