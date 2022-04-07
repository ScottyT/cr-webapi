<template>
    <div class="chart__container">
        <div v-if="!onPdf">
            <button type="button" class="button--normal" @click="populateChart(existingChart)">Update</button>
        </div>
        <v-overlay absolute :value="loading" v-show="!onPdf">
            <v-progress-circular indeterminate size="64"></v-progress-circular>
        </v-overlay>
        <client-only>
            <bar-chart class="chart__bar" v-if="!loading" :width="width" :height="height" :bardata="chartData" :baroptions="options" />
        </client-only>
    </div>
</template>
<script>
import { defineComponent, toRefs, watch, ref, computed } from '@nuxtjs/composition-api'
import useReports from '@/composable/reports'
import genericFuncs from '@/composable/utilityFunctions'
export default defineComponent({
    props: {
        width: Number,
        height: Number,
        jobid: String,
        existingChart: Array,
        onPdf: Boolean
    },
    setup(props, { root }) {
        const { jobid, existingChart, onPdf } = toRefs(props)
        const loaded = ref(true)
        const { getReportPromise, loading } = useReports()
        const { groupByKey, namedColor } = genericFuncs()
        const options = ref({
            title: {
                display: true,
                text: "Baseline Comparative",
                fontSize: 30
            },
            scales: {
                yAxes: [{
                    display:true,
                    scaleLabel: {
                        display:true,
                        labelString: "Moisture Readings in %",
                        fontSize: 14
                    },
                    suggestedMin: 0,
                    suggestedMax: 100,
                    ticks: {
                        min: 0
                    }
                }]
            },
            responsive: true
        })
        const chartData = ref({
            labels: [],
            datasets:[]
        })
        const baselineReadings = ref([])
        const baselineDates = ref([])

        async function getExistingMoistureReport(jobid) {
            getReportPromise(`moisture-map/${jobid}`).then((result) => {
                console.log(result.data)
                var baselineData = {
                    label: '',
                    data:[]
                }
                baselineReadings.value = result.data.readingsRow.filter(el => el.label === 'baseline')
                baselineReadings.value.forEach((item) => {
                    baselineData.label = item.date
                })
                chartData.value.labels = baselineReadings.value.map(el => el.date)
                loading.value = true
            })
            setTimeout(() => {
                loading.value = false
            }, 500)
        }
        function populateChart(v) {
            loading.value = true
            setTimeout(() => {
                loading.value = false
            }, 500)
            var tempArr = []
            var datasets = []
            var labels = []
            v.forEach((item) => {
                for (var i=0;i<item.subareas.length;i++) {                
                    tempArr.push({ data: item.subareas[i].val, index: i })
                }
                labels.push(item.date)
            })
            var groupedByIndex = groupByKey(tempArr, "index")
            for (const property in groupedByIndex) {
                var data = []
                var color = ""
                groupedByIndex[property].forEach((item) => {
                    data.push(item.data)
                })
                /* if (property % 2 == 0) color = "rgba(202, 21, 21, .5)"
                else if (property % 2 !== 0) color = "rgba(42, 91, 199, .5)" */
                color = namedColor(property)
                datasets.push({label: `Area Sub-${groupedByIndex[property][0].index + 1}`, data: data, backgroundColor: color, barPercentage: .95,
                    maxBarThickness: 150})
            }
            chartData.value.labels = labels
            chartData.value.datasets = datasets
        }

        watch(jobid, (val) => {
           // getExistingMoistureReport(val)
        })
        watch(existingChart, (val) => {
            populateChart(val)
        })
        if (onPdf.value) {
            populateChart(existingChart.value)
        }
        return {
            options,
            chartData,
            loading,
            baselineReadings,
            baselineDates,
            loaded,
            populateChart
        }
    },
})
</script>
<style lang="scss" scoped>
.chart {
    position:relative;
    padding:35px;

    &__bar {
        max-width:880px;
    }
}
</style>