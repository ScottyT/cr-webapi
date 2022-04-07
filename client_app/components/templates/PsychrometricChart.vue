<template>
    <div class="chart__container">
        <client-only>
            <scatter-chart class="chart__scatter" v-if="dataLoaded" :chartdata="chartData" :options="options" :plugins="[plugin]" :width="width" :height="height" />
        </client-only>
        
        <div class="d-flex chart__bottom" v-if="dataLoaded">
            <button class="button--normal" v-show="dayOfJob !== undefined" @click="addData(dayOfJob)">Create Chart</button>
            <ValidationProvider v-show="!multipleCharts" vid="Chart type" name="Chart type" rules="required" class="form__input-group form__input-group--normal">
                <input type="hidden" v-model="readingsType" />
                <label class="form__label">Readings type: </label>
                <i class="form__select--icon icon--angle-down mdi" aria-label="icon"></i>
                <select class="form__input" v-model="readingsType">
                    <option disabled value="">Please select an area </option>
                    <option>Affected</option>
                    <option>Unaffected</option>
                    <option>Exterior</option>
                </select>
            </ValidationProvider>
        </div>
        
    </div>
</template>
<script>
import { computed, defineComponent, inject, onMounted, ref, toRefs, watch } from "@nuxtjs/composition-api";
import genericFuncs from '@/composable/utilityFunctions'
import useReports from '@/composable/reports'
export default defineComponent({
    props: {
        xaxes: String,
        yaxes: String,
        existingChart: Array,
        dayOfJob: String,
        jobid: String,
        width: Number,
        height: Number,
        dataLoaded: Boolean,
        multipleCharts: Boolean
    },
    setup(props, {root, emit}) {
        const { xaxes, yaxes, existingChart, multipleCharts, jobid, dayOfJob, dataLoaded } = toRefs(props)
        let jobId = root.$route.params.slug
        const buttonDisabled = ref(false)
        const { getRandomUnique, groupByKey } = genericFuncs()
        const chartData = ref([])
        const colors = ref(['#157f27', '#900C3F', '#0A9C8F', '#FF5733', '#EB1F28', '#343434', '#C70039'])
        const loaded = ref(false)
        const tempData = ref({})
        const readingsType = ref("")
        const readingsGroup = ref({})
        const options = ref({
            title: {
                display: true,
                text: "",
                fontSize: 30
            },
            scales: {
                xAxes: [{
                    display:true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Dry Bulb Temperature in Fahrenheit',
                        fontColor: '#181818',
                        fontSize: 14,
                        fontFamily: "'Roboto', sans-serif"
                    },
                    suggestedMin: 20,
                    suggestedMax: 120,
                    ticks: {
                        min:20,
                        max: 120,
                        stepSize: 5
                    },
                    gridLines: {
                        color: 'rgba(0, 0, 0, 1)'
                    }
                }],
                yAxes: [{
                    display:true,
                    id: 'humidityRatio',
                    scaleLabel: {
                        display:true,
                        labelString: 'Humidity Ratio - Grains of Moisture Per Pound of Dry Air (GGP)',
                        fontColor: '#181818',
                        fontSize: 14,
                        fontFamily: "'Roboto', sans-serif"
                    },
                    suggestedMin: 0,
                    suggestedMax: 210,
                    ticks: {
                        min: 0,
                        max: 210,
                        stepSize: 10
                    },
                    gridLines: {
                        color: 'rgba(0, 0, 0, 1)'
                    },
                    position: 'right'
                }, {
                    id: 'vaporPressure',
                    type: 'linear',
                    position:'left',
                    scaleLabel: {
                        display: true,
                        labelString: 'Vapor Pressure - Inches of Mercury',
                        fontColor: '#181818',
                        fontSize: 14,
                        fontFamily: "'Roboto', sans-serif"
                    },
                    ticks: {
                        min: 0,
                        max: 1.4,
                        stepSize: .1
                    }
                }]
            },
            responsive: true,
            //maintainAspectRatio: false
        })
        const plugin = ref({
            id: 'bgColor',
            beforeDraw: (chart) => {
                const ctx = chart.canvas.getContext('2d')
                ctx.save()
                ctx.globalCompositeOperation = 'destination-over'
                ctx.fillStyle = 'white'
                ctx.fillRect(0, 0, chart.width, chart.height);
                ctx.restore();
            }
        })
        const { getReportPromise, loading } = useReports()
        const getExistingChart = async (jobid) => {
            return new Promise((resolve) => {
                loaded.value = false
                getReportPromise(`psychrometric-chart/${jobid}`).then((result) => {
                    if (result.error) {
                        return
                    }
                    var tempArr = []
                    for (var i = 0; i < result.data.jobProgress.length; i++) {
                        var data = {
                            x: result.data.jobProgress[i].info.dryBulbTemp,
                            y: result.data.jobProgress[i].info.humidityRatio
                        }
                        var dataset = { 
                            readingsType: result.data.jobProgress[i].readingsType, 
                            pointRadius: 5, data: [data], 
                            label: result.data.jobProgress[i].date, 
                            backgroundColor: result.data.jobProgress[i].color 
                        }
                        tempArr.push(dataset)
                        chartData.value.push(dataset)
                    // readingsGroup.value = groupByKey(dataset, 'readingsType')
                    }
                    resolve(true)
                    readingsGroup.value = groupByKey(tempArr, 'readingsType')
                    loaded.value = true
                })
            })
            
        }
        function addData(label) {
            var newData = {
                x: parseInt(xaxes.value),
                y: parseFloat(yaxes.value)
            }
            var existingDotColor = existingChart.value.map((e) => { return e.backgroundColor })
            var dotColor = getRandomUnique(existingChart.value, existingDotColor, colors.value)
            var dataset = { pointRadius: 5, data: [newData], readingsType: readingsType.value, label: label, backgroundColor: dotColor };
            chartData.value.forEach((item, i) => {
                var dataIndex = chartData.value.findIndex(el => el.label === dayOfJob.value)
                if (dataIndex > -1) {
                    chartData.value[dataIndex] = dataset
                } else {
                    //chartData.value.pop()
                    chartData.value.push(dataset)
                }
            })
            /* if (chartData.value.length === 1) {  // Object.keys(tempData.value).length === 0            
                chartData.value[0] = dataset
            } else {
                chartData.value.pop()
                chartData.value.push(dataset)
            } */
            emit('addData', {data: newData, date: label, title: readingsType.value, color: dataset.backgroundColor})
            buttonDisabled.value = true
            tempData.value = dataset
            if (chartData.value.length === 0) {
                chartData.value.push(dataset)
            }
        }
        watch(jobid, (val) => {
            chartData.value = []
            getExistingChart(val)
            emit('existingChart', chartData.value)
        })
        watch(readingsType, (val) => {
            loaded.value = true
            options.value.title.text = val
            emit("sendChartType", val)
            for (const property in readingsGroup.value) {
                if (property === val) {
                    console.log(property)
                    chartData.value = readingsGroup.value[property]
                    return
                } else {
                    chartData.value = []
                }
            }
        })
        if (multipleCharts.value) {
            chartData.value = existingChart.value
        }
        return { 
            chartData, options, plugin, loaded, addData, tempData, buttonDisabled, loading, readingsType, readingsGroup
        }
    }
})
</script>
<style lang="scss" scoped>
.chart {
    position:relative;
    background-color:$color-white;
    padding:35px;

    &__image {
        max-width:860px;
        width:100%;
        position:absolute;
        left:50%;
        transform:translateX(-50%);
        height:600px;
    }

    &__scatter {
        max-width:860px;
        //height:62vh;
        position:relative;
        &--thin {
            width:100px;
            //max-width:63px;
            max-height:636px;
            top:23px;
        }
    }

    &__label {
        position:absolute;
        top:-5px;
        left:50%;
        transform:translateX(-50%);
        color:$color-black;
        font-size:14px;
    }
    
    &__bottom {
        position:relative;
        button {
            height:50px;
        }
    }

    &__calculations {
        display:flex;
        flex-wrap:wrap;
        justify-content:space-around;
    }
}

.pdf-item {
    .chart__scatter {
        max-width:700px;
    }
}
</style>