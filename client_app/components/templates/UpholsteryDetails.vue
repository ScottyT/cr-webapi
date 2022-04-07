<template>
    <section :class="notPdf ? '' : 'pdf-content'">
        <h2>{{company}}</h2>
        <h2>Upholstery Cleaning Pre-Inspection Form</h2>
        <div class="pdf-item report-details__section">
            <div class="report-details__data">
                <label class="form__label">Customer</label>
                <span>{{report.Customer}}</span>
            </div>
            <div class="report-details__data">
                <label class="form__label">Technician</label>
                <span>{{report.Technician}}</span>
            </div>
            <div class="report-details__data">
                <label class="form__label">Job ID</label>
                <span>{{report.JobId}}</span>
            </div>
            <div class="report-details__data">
                <label class="form__label">Date</label>
                <span>{{report.date}}</span>
            </div>
            <div class="report-details__data">
                <label class="form__label">Address</label>
                <span>{{report.address}}</span>
            </div>
            <div class="report-details__data">
                <label class="form__label">Phone Number</label>
                <span>{{report.phoneNumber}}</span>
            </div>
            <div class="report-details__data">
                <label class="form__label">Age of Fabrics</label>
                <span>{{report.ageOfFabric}}</span>
            </div>
        </div>
        <div class="pdf-item report-details__section">
            <div class="report-details__listing" v-for="group in report.categoryData" :key="group.id">
                <h4>{{group.label}}</h4>
                <ul>
                    <li v-for="(item, i) in group.checked" :key="`checked-${i}`">
                        {{item}}
                    </li>
                </ul>
            </div>
        </div>
        <div class="pdf-item report-details__section auth-group">
            <div class="report-details__data">
                <label class="form__label">Limitations and Authorization Limitations</label>
                <span>{{report.limitations}}</span>
            </div>
            <div class="report-details__data">
                <label class="form__label">Technician Signature</label>
                <div class="report-details__signature" v-if="report.techSig">
                    <img :src="$store.state.users.signature" />
                </div>
            </div>
            <div class="report-details__data">
                <label class="form__label">Customer Signature</label>
                <div class="report-details__signature"><img :src="report.customerSig" /></div>
            </div>
            <div class="report-details__data">
                <label class="form__label>">Date</label>
                <span>{{report.date}}</span>
            </div>
            <div class="report-details__data">
                <label class="form__label">Acknowledgement</label>
                <p class="red--text">I am aware of the above limitations regarding the furniture noted. I authorize Company Name to clean my furniture subject to the above limitations.</p>
            </div>
        </div>
    </section>
</template>
<script>
import { defineComponent } from '@nuxtjs/composition-api'

export default defineComponent({
    props: {
        company: String,
        report:Object,
        notPdf: Boolean
    },
    setup() {
        
    },
})
</script>
<style lang="scss">
.pdf-content {
    ul {
        padding-left:30px;
    }
    .form__label {
        font-weight:900;
        font-size:1em;
    }
    .report-details__signature {
        height:80px;
        width:200px;
        img {
            object-fit: contain;
        }
    }
    .red--text {
        color:red;
    }
}
.auth-group {
    display:grid;
    padding:0 15px;
    grid-template-columns:1fr 1fr 1fr;
    grid-template-rows:1fr 1fr;
}
</style>