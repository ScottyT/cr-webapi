<template>

    <section slot="pdf-content" class="pdf-content">
        <div class="text-center pdf-first-section">
            <h1 class="text-center">{{company}}</h1>
            <h2 class="text-center">Contracting Service Agreement</h2>
            <p>Insurance Repair, Renovation, or Restoration & Betterment Agreement
                This Contracting Service Agreement (hereinafter, this “Agreement”) is entered into below by:
                {{company}} (Hereinafter, “{{abbreviation}}”) &
                The Owner/Persons of legal authority (Hereinafter, “Customer”)
                Pertaining to services engaged at the following described property below
                This Agreement is one of two Agreements:
                One: Contracting Services Agreement &
                Two: Scope of Work
            </p>
        </div>
        <div class="report-details__section">
            <div class="grid grid--two-column">
                <div>
                    <div class="report-details__data">
                        <label>Job Name:</label>
                        <h4>{{report.jobName}}</h4>
                    </div>
                    <div class="report-details__data">
                        <label>Job Address:</label>
                        <h4>{{report ? null : report.jobLocation.address}}</h4>
                    </div>
                    <div class="report-details__data">
                        <label>City, State, Zip:</label>
                        <h4>{{report ? null : report.jobLocation.cityStateZip}}</h4>
                    </div>
                    <div class="report-details__data">
                        <label>Contact Liaison:</label>
                        <h4>{{report.contactLiaison}}</h4>
                    </div>
                    <div class="report-details__data">
                        <label>Contact Email:</label>
                        <h4>{{report.contactEmail}}</h4>
                    </div>
                    <div class="report-details__data">
                        <label>Emergency Liaison:</label>
                        <h4>{{report.emergencyLiaison}}</h4>
                    </div>
                    <div class="report-details__data">
                        <label>Emergency Phone:</label>
                        <h4>{{report.emergencyPhone}}</h4>
                    </div>
                    <div class="report-details__data">
                        <label>Fax Number:</label>
                        <h4>{{report.faxNumber}}</h4>
                    </div>
                </div>
                <div>
                    <div class="report-details__data">
                        <label>Billing Address:</label>
                        <h4>{{report ? null : report.billingLocation.address}}</h4>
                    </div>
                    <div class="report-details__data">
                        <label>Billing City, State, Zip:</label>
                        <h4>{{report ? null : report.billingLocation.cityStateZip}}</h4>
                    </div>
                    <div class="report-details__data">
                        <label>Mortgage Company:</label>
                        <h4>{{report.mortgageCompany}}</h4>
                    </div>
                    <div class="report-details__data">
                        <label>Insurance Company:</label>
                        <h4>{{report.insuranceCompany}}</h4>
                    </div>
                    <div class="report-details__data">
                        <label>Date of Loss:</label>
                        <h4>{{report.dateOfLoss}}</h4>
                    </div>
                    <div class="report-details__data">
                        <label>Claim Number:</label>
                        <h4>{{report.claimNumber}}</h4>
                    </div>
                    <div class="report-details__data">
                        <label>Policy Number:</label>
                        <h4>{{report.policyNumber}}</h4>
                    </div>
                    <div class="report-details__data">
                        <label>Policy Length:</label>
                        <h4>{{report.policyDate}}</h4>
                    </div>
                </div>
            </div>
        </div>
        <div class="report-details__section">
            <p>
                (SHARE) Customer will send a copy of this Agreement, Certificate of Completion, Xactimate and the
                {{abbreviation}} W9
                to the Insurance Company, the Insurance Company’s Representatives, and Mortgage Institutions to allow
                {{abbreviation}} to
                communicate, share, and exchange through reasonable introduction &/or through means found proper in the
                daily
                course of business, information including but not limited to the services that {{abbreviation}} has
                provided, currently is
                providing, &/or may be required to be provide in the future.
            </p>
            <p>
                (DIRECTION OF PAYMENT) Customer composedly recognizes to administer authorization and enact direction of
                payment immediately upon execution of this agreement to {{abbreviation}} from the Insurance Company
                including but not
                limited to all invoices, tasks, billable hours, and billable units partially &/or completely provided by
                {{abbreviation}}.
            </p>
            <p>
                (LIABILITY) <span class="bold-text underline-text">The Customer understands this agreement is to Repair, Renovate, or Rebuild property.</span> The
                Representative understands {{abbreviation}} does not offer services outside this agreement.
                {{abbreviation}} is not responsible for
                prior conditions, or damages resulting from, including but not limited to mold, hazardous materials, and
                structural
                damage before, during, or after authorization of this Agreement in-directly or directly caused by
                {{abbreviation}}.
            </p>
            <p>
                (GOOD FAITH) It is the responsibility of the Customer to act in “good faith.” Any actions or behaviors
                deemed
                inappropriate by the Customer, {{abbreviation}} reserves right of the Available Proceeds of this
                Agreement and Customer’s
                Insurance Company Policy. Property Representative agrees that {{abbreviation}} alone will document,
                scope and
                independently account a-side the Insurance Company concerning the replacement cost and/or actual cost
                value of the
                work, replacements and/or losses, and damages and/or services to the property.
            </p>
            <p>
                (DIRECTION) Customer will advise the Insurance Company (and each agent, adjuster, independent adjuster,
                building
                consultant, expert, engineer, or any other party acting on behalf thereof) of this Agreement, and
                formally Direct the
                Insurance Company to {{abbreviation}} if contacted regarding actions &/or services authorized in this
                Agreement.
            </p>
        </div>
        <div class="report-details__section">
            
        
            <p>
                (CERTIFICATE OF COMPLETION) Customer agrees to sign the {{abbreviation}} Certificate of Completion, upon
                the conclusion of the project.
            </p>
            <p>
                <strong>(Authority of Parties) Customer is the owner of or duly authorized agent of the owner with
                    authority to bond
                    the Representative of the property for this Agreement. Property Representative covenants that
                    {{abbreviation}} can
                    rely on this authority and this appointment in all dealings with the Insurance Company, insurance
                    agents,
                    insurance adjusters, tenants and invitees of the Customer and the Property, and contractors and
                    vendors of
                    Company.</strong>
            </p>
            <p>
                (Customer Cooperation Required) To the extent that are available, the Customer will provide
                {{abbreviation}} with all
                reasonable and necessary documents, including copies of all insurance policies, endorsements, riders,
                and
                correspondence including invoices and proof of coverage which may assist {{abbreviation}}.
            </p>
            <p>
                (Fraud) In most states, it is a crime to not pay Customer’s deductible. Property Representative
                understands that
                Available Proceeds include the Customer’s deductible. Customer may be able to find a contractor less
                expensive than
                {{abbreviation}}. Company settlements reflect {{abbreviation}} Cost. Using {{abbreviation}} accounting
                to personally gain or secondary
                gain in anyway is a misrepresentation of the Customer’s true cost to the Insurance Company. The Customer
                understands that misrepresenting information to the Insurance Company is a Crime. {{abbreviation}}
                reserves the right to
                notify any authority of such malicious behavior.
            </p>
            <p>
                (Timeline) In many cases, the process of getting the Insurance Company to accept the claim and its
                responsibility will
                take time, considerable effort, and refusal at first should not be assumed to be a final decision. The
                goal is to make the
                Customer “whole”. In many cases this is a very long costly process. Customer agrees the length of this
                process is
                understood.
            </p>
            <p>
                (No Guarantee of Coverage) {{abbreviation}} is not able to guarantee that the Customer will have
                insurance coverage for all
                mitigation, services, repair, restoration, and renovation needed. Coverage is determined under the
                contract that exists
                between the Insurance Company and the Customer. {{abbreviation}} has no control over this contract and,
                therefore, cannot
                guarantee that any particular loss or damage is covered by insurance or that the amount of coverage
                available will
                permit needed services. Customer understands in which any services herein this Agreement are not covered
                by The
                Insurance Company the Customer is responsible for said payment, by said terms herein this Agreement.
            </p>
            <p>
                (“Available Proceeds”) is defined as the potential amount of funds which {{abbreviation}} estimates the
                Replacement Cost
                Value, using the third-party software Xactimate, including: the deductible – interest & depreciation,
                equals. This
                amount can vary greatly depending on the skill and thoroughness of the adjustor, analyst, and
                {{abbreviation}}’s position.
                The goal is to make the Representative “Whole”.
            </p>
            <p>
                (Indemnification) The Property Representative will hold harmless and indemnify {{abbreviation}} against
                any and all claims
                and actions arising out of the participation of this Agreement including, without limitation, Expenses,
                judgements,
                fines, settlements and other amounts actually and reasonably incurred in connection with any liability,
                suit, action,
                loss, or damage arising or resulting from {{abbreviation}} participation in the services provided in
                this agreement. Under this
                agreement, indemnification will be unlimited as to the amount.
            </p>
        </div>
        <div class="report-details__section">
            
                <ul class="report-details__listing-num">
                    <strong>(Cancellation Rights or Privileges)</strong>
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
                </ul>
                <ul class="report-details__listing-num">
                    <strong>(DEFAULT)</strong>
                    <li>If Company is not paid by the Customer within (3) days of receipt of the Available Proceeds, Customer
agrees that all unpaid amounts will bear interest at the rate of 15% per annum (or maximum rated permitted
by law, if lower)</li>
                    <li>Entitlement to the interest will commence (3) days following Customer’s receipt of the Available Proceeds
from the Insurance Company.</li>
                    <li>Should default be made in payment of this contract, and if placed in the hand of an attorney for collection,
any and all reasonable attorney fees, and legal filings fees shall be paid by the Customer.</li>
                </ul>
        </div>
        <div class="report-details__section">
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
        <div class="pdf-detail">
            <div class="report-details__data">
                <label>Customer Print:</label>
                <div style="width:100%; border-bottom:1px solid black;">
                    <h4>Amanda Powers</h4>
                </div>
            </div>
            
            <div class="grid grid--two-column">
                <div class="report-details__data">
                    <label>Customer Signature:</label>
                    <div class="report-details__signature">
                        <img :src="report.cusSign" />
                    </div>
                </div>
                <div class="report-details__data">
                    <label>Date:</label>
                    <h4>{{report.signDate}}</h4>
                </div>
            </div>
        </div>
    </section>
</template>
<script>
import { defineComponent, onMounted, ref, toRefs, useContext } from '@nuxtjs/composition-api'
import useReports from '~/composable/reports'
export default defineComponent({
    props: {
        jobid: String,
        reportType: String
    },
    setup(props) {
        const { jobid, reportType } = toRefs(props)
        const report = ref({})
        const { $api } = useContext();
        const abbreviation = ref("")
        const company = ref("")
        
        const fetchingReport = () => {
            $api.$get(`/api/reports/details/${reportType.value}/${jobid.value}`).then((res) => {
                abbreviation.value = res.ReportType === 'guardian-contracting-agreement'?'Guardian':'WESI'
                report.value = res
                company.value = res.contractingCompany
            })
        }
        
        fetchingReport()
        return {
            report,
            abbreviation,
            company
        }
    },
})
</script>
<style lang="scss">
.pdf-content {
    .report-details__listing-num {
        list-style-position: inside;
    }
}
.bold-text {
    font-weight:700;
}
.underline-text {
    display:inline-block;
    border-bottom:1px solid black;
    line-height:15px;
}
</style>