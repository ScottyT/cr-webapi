<template>
    <div class="pdf-content" slot="pdf-content">
        <h1 class="text-center">{{company}}</h1>
        <h2 class="text-center" v-uppercase>{{reportName}}</h2>
        <div class="report-details__section">
            <div class="report-details__data" v-for="(item, key) in introSection" :key="key">
                <h4 v-uppercase>{{key}}:</h4>
                <span>{{item}}</span>
            </div>
        </div>
        <div class="report-details__section content-inventory">
            <h3>Inventory</h3>
            <div class="content-inventory__table">
                <div class="content-inventory__row">
                    <div class="content-inventory__cols" v-for="item in report.inventory[0].cols" :key="item.id">{{item.label}}</div>
                </div>
                <div class="content-inventory__row" v-for="(row, i) in report.inventory" :key="`row-${i}`">
                    <div class="content-inventory__cols">{{report.inventory[i].item_num}}</div>
                    <div class="content-inventory__cols" v-for="(col, j) in row.cols.filter(el => el.id !== 'item_num')" :key="`col${j}-row${i}`">
                        <span v-if="(typeof col.value === 'string') && col.id !== 'image'">{{col.value}}</span>
                        <span v-if="typeof col.value === 'number'">${{col.value}}</span>
                        <span v-if="typeof col.value === 'boolean' && col.value === true">Yes</span>
                        <span v-if="typeof col.value === 'boolean' && col.value === false">No</span>
                        <img :src="col.value" v-if="col.id === 'image'" />
                    </div>
                </div>
            </div>
        </div>
        <div class="report-details__section">
            <div class="report-details__data">
                <h4>Total inventory amount</h4>
                <span>${{report.totalAmount}}</span>
            </div>
            <div class="report-details__data">
                <h4>Technician Signature</h4>
                <div class="report-details__signature" v-if="report.techSig"><img :src="$store.state.users.signature" /></div>
            </div>
            <div class="report-details__data">
                <h4>Customer Signature</h4>
                <div class="report-details__signature"><img :src="report.cusSig" /></div>
            </div>
        </div>
    </div>
</template>
<script>
import { computed, defineComponent, onMounted, toRefs, useContext } from '@nuxtjs/composition-api'

export default defineComponent({
    props: {
        company: String,
        reportName: String,
        report: Object
    },
    setup(props) {
        const { report } = toRefs(props)
        const { $axios } = useContext()
        const introSection = computed(() => {
            return Object.fromEntries(Object.entries(report.value).filter(([key]) => 
                key.includes('JobId') || key.includes('customer') || key.includes('claimNumber') || key.includes('insurance') || key.includes('dateCompleted') 
            ))
        })

        return {
            introSection
        }
    },
})
</script>
<style lang="scss" scoped>
.content-inventory {
    display:flex;
    flex-direction: column;
    max-width:750px;
    &__row {
        display:grid;
        grid-template-columns:60px 130px 1fr 81px 81px 77px 70px 1fr 1fr;
        flex-direction: row;
    }
    &__table {
        display:grid;
        row-gap:10px;
    }
}
</style>