<template>
    <div :class="`form-wrapper ${$vuetify.breakpoint.width < 991 ? 'flex-column' : ''}`">
        <LayoutPsychrometricChart class="chart" @addData="newChartData" :existingChart="chartdata" :dayOfJob="date" :jobid="selectedJobId" :xaxes="dryBulbTemp" :yaxes="humidityRatio"
            :width="860" :height="700" :dataLoaded="loaded" @existingChart="chartdata.push($event)" @sendChartType="readingsType = $event" />
        <h2 v-show="submittedMessage !== ''">{{submittedMessage}}</h2>
        <ValidationObserver ref="form" v-slot="{ errors }" v-if="!submitted">
            <v-dialog width="400px" v-model="errorDialog">
                <div class="modal__error">
                    <div v-for="(error, i) in errors" :key="`error-${i}`">
                        <h3 class="form__input--error">{{ error[0] }}</h3>
                    </div>
                </div>
            </v-dialog>
            <form class="form form__psychrometric-chart" @submit.prevent="onSubmit">
                <div class="d-flex">
                    <ValidationProvider vid="JobId" name="Job ID" v-slot="{errors}" rules="required" class="form__input-group form__input-group--normal">
                        <input type="hidden" v-model="selectedJobId" />
                        <label class="form__label">Job ID: </label>
                        <i class="form__select--icon icon--angle-down mdi" aria-label="icon"></i>
                        <select class="form__input" v-model="selectedJobId">
                            <option disabled value="">Please select a Job id</option>
                            <option v-for="(item, i) in $store.state.reports.jobids" :key="`jobid-${i}`">{{item}}</option>
                        </select>
                        <span class="form__input--error">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider v-slot="{ errors }" name="Date" rules="required" class="form__input-group form__input-group--normal">
                        <label class="form__label">Date of reading</label>
                        <input type="text" v-model="date" v-mask="'##/##/####'" class="form__input" />
                        <span class="form__input--error">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider v-show="Object.keys(jobProgress).length !== 0" v-slot="{errors}" class="form__input-group form__input-group--short" 
                        name="Vapor Pressure" rules="required|between:.0,1.4">
                        <label class="form__label">Vapor Pressure - inHg</label>
                        <input type="number" step="0.01" min="0" max="1.40"  v-model="vaporPressure" class="form__input" />
                        <span class="form__input--error">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider v-slot="{ errors }" name="Init date" rules="required" class="form__input-group form__input-group--normal">
                        <label class="form__label">Init Date</label>
                        <input type="text" @change="addDays($event, 6)" v-model="initDate" v-mask="'##/##/####'" class="form__input" />
                        <span class="form__input--error">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider v-slot="{ errors }" name="End date" class="form__input-group form__input-group--normal">
                        <label class="form__label">End Date</label>
                        <input type="text" v-model="endDate" v-mask="'##/##/####'" class="form__input" />
                        <span class="form__input--error">{{ errors[0] }}</span>
                    </ValidationProvider>
                </div>
                <div class="chart-inputs__calculations">
                    <div class="chart-inputs__calculations-row">
                        <h4 class="chart-inputs__calculations-col">Temperature</h4>
                        <ValidationProvider rules="between:20,120" name="" class="chart-inputs__temp">
                            <input @accept="changeInputVal($event, 'dryBulbTemp')" v-imask="{mask: Number}" min="20" max="120" :value="dryBulbTemp" class="chart-inputs__temp--number"/>&deg;F
                        </ValidationProvider>
                    </div>
                    <div class="chart-inputs__calculations-row">
                        <h4 class="chart-inputs__calculations-col">Humidity Ratio</h4>
                        <ValidationProvider rules="between:0,210" name="" class="chart-inputs__temp">
                            <input @accept="changeInputVal($event, 'humidityRatio')" v-imask="{mask: Number}" min="0" max="210" :value="humidityRatio" class="chart-inputs__temp--humidity"/> humidity ratio
                        </ValidationProvider>
                    </div>
                    <div class="chart-inputs__calculations-row">
                        <h4 class="chart-inputs__calculations-col">Relative humidity</h4>
                        <div class="chart-inputs__calculations-col">{{RH}}</div>
                    </div>
                    <div class="chart-inputs__calculations-row">
                        <h4 class="chart-inputs__calculations-col">Dew Point</h4>
                        <div class="chart-inputs__calculations-col">{{dewPoint}}&deg;F</div>
                    </div>
                </div>
                <div class="chart-inputs">
                    <div class="chart-inputs__col">
                        <div class="form__input-group">
                            <label class="form__label">Dry Bulb Temperature - &#176;F</label>
                            <UiRangeSlider slideRef="dryBulbTemp" htmlId="dryBulbTemp" minNum="20" maxNum="120" @sendInputVal="dryBulbTemp = $event" :parentInput="dryBulbTemp" />
                        </div>
                    </div>
                    <div class="chart-inputs__col">
                        <div class="form__input-group">
                            <label class="form__label">Humidity Ratio - Grains of Moisture Per Pound of Dry Air</label>
                            <UiRangeSlider slideRef="humidityRatio" htmlId="humidityRatio" minNum="0" maxNum="210" @sendInputVal="humidityRatio = $event" :parentInput="humidityRatio" />
                        </div>
                    </div>
                </div>
                <button type="submit" class="button button--normal">{{ submitting ? 'Submitting' : 'Submit' }}</button>
            </form>
        </ValidationObserver>
    </div>
</template>
<script>
import { defineComponent, computed, ref, useStore, reactive, watch, provide, useContext } from '@nuxtjs/composition-api'
import useReports from '@/composable/reports';
import genericFuncs from '@/composable/utilityFunctions'
export default defineComponent({
    setup(props, { refs, root }) {
        var jobId = root.$route.params.id
        const { getReportPromise, loading } = useReports()
        const { $api } = useContext()
        const { convertToF, round, convertToC, groupByKey } = genericFuncs()
        const store = useStore()
        const dryBulbTemp = ref('20')
        const humidityRatio = ref('0')
        const dewPoint = ref('')
        const vaporPressure = ref('')
        const submittedMessage = ref("")
        const loaded = ref(false)
        const submitted = ref(false)
        const submitting = ref(false)
        const errorDialog = ref(false)
        const selectedJobId = ref('')
        const chartdata = ref([])
        const chart = ref([])
        const initDate = ref('')
        const endDate = ref('')
        const jobProgress = ref({})
        const atmosReadingsLog = ref([{text: "Affected Temperature (fahrenheit)", label: "Affected", identifier: "dryBulbTemp", day: [
                {text: "day1", date: "", value: ""},
                {text: "day2",date: "",value: ""},
                {text: "day3",date: "",value: ""},
                {text: "day4",date: "",value: ""},
                {text: "day5",date: "",value: ""},
                {text: "day6",date: "",value: ""},
                {text: "day7",date: "",value: ""}
            ]},
            {text: "Affected Vapor Pressure", label: "Affected", identifier: "vaporPressure", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Affected Dew Point (fahrenheit)", label: "Affected", identifier: "dewPoint", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Affected Relative Humidity", label: "Affected", identifier: "relativeHumidity", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Affected GPP", label: "Affected", identifier: "humidityRatio", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Comparative Temperature", label: "Unaffected", identifier: "dryBulbTemp", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Comparative Vapor Pressure", label: "Unaffected", identifier: "vaporPressure", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Comparative Dew Point (fahrenheit)", label: "Unaffected", identifier: "dewPoint", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Comparative Humidity", label: "Unaffected", identifier: "relativeHumidity", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            
            {text: "Comparative GGP", label: "Unaffected", identifier: "humidityRatio", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            
            {text: "Exterior RH", label: "Exterior", identifier: "relativeHumidity", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Exterior Temperature", label: "Exterior", identifier: "dryBulbTemp", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Exterior GGP", label: "Exterior", identifier: "humidityRatio", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Exterior Dew Point (fahrenheit)", label: "Exterior", identifier: "dewPoint", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Contributing Event", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Precipitation (inches)", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Wind (mph)", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]}])
        const lossArr = ref([
                {text: "Class 1", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
                {text: "Class 2", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
                {text: "Class 3", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
                {text: "Class 4", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]}
        ])
        const catArr = ref([
            {text: "Category Water 1", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Category Water 2", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Category Water 3", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Category Water 4", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]}
        ])
        const pHa = ref(.1805)  // pHa of vapor pressure converted to inHg
        const beta = ref(17.62)
        const temp = ref(243.12) // In Celsius
        const readingsType = ref("")
        const atmosFetched = ref(false)

        const user = computed(() => store.getters['users/getUser'])
        const date = ref("")
        const EW = computed(() => {
            var tempInC = convertToC(dryBulbTemp.value)
            return pHa.value * Math.exp((beta.value * tempInC) / (temp.value + tempInC))
        })
        const RH = computed(() => {
            var r = vaporPressure.value / EW.value * 100
            return `${r.toFixed(2)}%`
        })
        const groupedDataByLabel = computed(() => {
            return groupByKey(atmosReadingsLog.value, 'label')
        })
        const groupedDataById = computed(() => {
            return groupByKey(atmosReadingsLog.value, "identifier")
        })
        
        const changeInputVal = (e, dataname) => {
            const maskRef = e.detail
            if (dataname === 'dryBulbTemp') dryBulbTemp.value = maskRef.value
            if (dataname === 'humidityRatio') humidityRatio.value = maskRef.value
        }
        const getAtmosphericReadings = async (jobid) => {
            chartdata.value = []
            atmosFetched.value = false
            await getReportPromise(`atmospheric-readings/${jobid}`).then((result) => {
                atmosReadingsLog.value = result.readingsLog
                initDate.value = result.startDate
                endDate.value = result.endDate
                atmosFetched.value = true
            }).catch(() => {
                atmosFetched.value = false
            })
        }
        const changeAtmosArr = () => {
            var datediff = new Date(endDate.value) - new Date(initDate.value)
            var daysDiff = datediff /  (1000 * 60 * 60 * 24);
            var startDateDay = new Date(initDate.value).getDate()
            var dates = []
            for (var i = 0; i <= daysDiff; i++) {
                var date = new Date(initDate.value)
                var day = date.setDate(startDateDay + i)
                var formattedDate = formatDate(new Date(day).toISOString().substr(0, 10))
                dates.push(formattedDate)
            }
            atmosReadingsLog.value.forEach((item, i) => {
                for (var j = 0; j < item.day.length; j++) {
                    item.day[j].date = dates[j]
                }
            })
        }
        function addDays(e, days) {
            const date = new Date(e.target.value)
            date.setDate(date.getDate() + days);
            var formattedDate = formatDate(date.toISOString().substr(0, 10))
            endDate.value = formattedDate
        }
        function formatDate(dateReturned) {
            if (!dateReturned) return null
            const [year, month, day] = dateReturned.split('-')
            return `${month}/${day}/${year}`
        }
        //This gets called when you click create chart
        function newChartData(...params) {
            const { data, date, color, title } = params[0]
            var formSubmissionData = {
                info: {
                    dryBulbTemp: data.x,
                    humidityRatio: data.y,
                    dewPoint: dewPoint.value + '%',
                    vaporPressure: vaporPressure.value,
                    relativeHumidity: RH.value
                },
                readingsType: title,
                date: date,
                color: color
            }
            jobProgress.value = formSubmissionData
            loaded.value = false
            setTimeout(() => {
                loaded.value = true
            }, 500)
        }
        function calcDewPoint(v) {
            var ln = v / pHa.value
            var Dp = (temp.value * Math.log(ln)) / (beta.value - Math.log(ln))
            dewPoint.value = round(convertToF(Dp), 3)
        }
        function submitAtmosReadings() {
            const findByIdentifier = (array, key) => {
                let result = array.filter((obj) => { 
                    return obj.identifier === key && obj !== undefined
                })
                return result
            }
            var groupedDataById = groupByKey(atmosReadingsLog.value, "identifier")
            var groupedDataByLabel = groupByKey(atmosReadingsLog.value, "label")
            var promises = []
            return new Promise((resolve, reject) => {
                for (const property in groupedDataByLabel) {
                    const dateIndex = groupedDataByLabel[property][0].day.findIndex(el => el.date === date.value)
                    for (const key in groupedDataById) {
                        var obj = findByIdentifier(groupedDataByLabel[readingsType.value], key)
                        console.log(obj)
                        switch (key) {
                            case "dryBulbTemp":
                                if (obj.length > 0) obj[0].day[dateIndex].value = dryBulbTemp.value
                                break;
                            case "vaporPressure":
                                if (obj.length > 0) obj[0].day[dateIndex].value = vaporPressure.value
                                break;
                            case "humidityRatio":
                                if (obj.length > 0) obj[0].day[dateIndex].value = humidityRatio.value
                                break;
                            case "dewPoint":
                                if (obj.length > 0) obj[0].day[dateIndex].value = dewPoint.value
                                break;
                            case "relativeHumidity":
                                if (obj.length > 0) obj[0].day[dateIndex].value = RH.value                            
                                break;
                        }
                        promises.push(obj)
                    }
                }
                Promise.all(promises).then((result) => {
                    const post = {
                        JobId: selectedJobId.value,
                        ReportType: "atmospheric-readings",
                        startDate: initDate.value,
                        endDate: endDate.value,
                        formType: "logs-report",
                        readingsLog: atmosReadingsLog.value,
                        notes: "",
                        lossClassification: lossArr.value,
                        categoryData: catArr.value,
                        teamMember: user.value
                    }
                    $api.$post(`/api/reports/atmospheric-readings/${selectedJobId.value}/update`, post).then((res) => {
                        resolve("finished updating atmos")
                    })
                })
            })
        }

        function onSubmit() {
            const post = {
                JobId: selectedJobId.value,
                teamMember: user.value,
                jobProgress: jobProgress.value,
                formType: 'chart-report',
                ReportType: 'psychrometric-chart'
            };
            refs.form.validate().then(success => {
                if (!success) {
                    submitting.value = false
                    submitted.value = false
                    errorDialog.value = true
                    return;
                }
                submitAtmosReadings(idToken).then((result) => {
                    submitting.value = true
                    if (atmosFetched.value) {
                        $api.$post(`/api/reports/psychrometric-chart/update-progress`, post).then((res) => {
                            submittedMessage.value = res
                            submitting.value = false
                            submitted.value = true
                            setTimeout(() => {
                                window.location = "/"
                            }, 3000)
                        })
                        return;
                    }
                    $api.$post(`/api/reports/psychrometric-chart/update-chart`, post).then((res) => {
                        submittedMessage.value = res
                        submitting.value = false
                        submitted.value = true
                        setTimeout(() => {
                            window.location = "/"
                        }, 3000)
                    })
                }).catch(err => {
                    console.error(err)
                })
            })
        }
        watch(readingsType, (val) => {
            chartdata.value[0] = []
            loaded.value = true
            var filtered = chartdata.value[0].filter((item) => {
                return item.readingsType === val
            })
            chart.value = filtered
            // populate the card with the job info
        })
        watch(selectedJobId, (val) => {
            loaded.value = true
            getAtmosphericReadings(val)
            //getExistingChart(val)
        })
        watch(vaporPressure, (val) => {
            calcDewPoint(val)
            jobProgress.value.info["dewPoint"] = dewPoint.value
            jobProgress.value.info["vaporPressure"] = vaporPressure.value
            jobProgress.value.info["relativeHumidity"] = RH.value
        })
        watch(endDate, (val) => {
            changeAtmosArr()
        })

        return {
            selectedJobId,
            errorDialog,
            submitting,
            submitted,
            readingsType,
            submittedMessage,
            user,
            dryBulbTemp,
            humidityRatio,
            dewPoint,
            vaporPressure,
            chart,
            chartdata,
            atmosReadingsLog,
            lossArr,
            catArr,
            date,
            initDate,
            endDate,
            groupedDataByLabel,
            groupedDataById,
            loading: computed(() => loading.value),
            loaded,
            jobProgress,
            EW, RH,
            changeInputVal,
            newChartData,
            onSubmit,
            changeAtmosArr,
            addDays,
            atmosFetched
        }
    },
})
</script>
<style lang="scss">
.chart-inputs {
    display:flex;
    justify-content: space-between;
    flex-wrap:wrap;
    @include respond(tabletLargeMax) {
        flex-direction:column;
    }

    &__input {
        input[type=number] {
            padding:3px;
            text-align:right;
            box-shadow:0px 0px 3px 2px rgba(0, 0, 0, .25);
        }
    }

    &__col {
        display:flex;
    }

    &__calculations {
        display:grid;
        grid-template-rows:1fr 1fr 1fr 1fr;
        max-width:420px;
        border-radius:4px;
        box-shadow:2px 4px 36px 3px rgba(0, 0, 0, 20%);
    }
    &__calculations-row {
        display:flex;
        flex-direction:row;
        padding:7px 18px;
        justify-content:space-between;
        align-items:center;
        &:not(:last-child) {
            border-bottom:1px solid $color-grey;
        }
    }
    &__temp {
        font-size:1em;
        &--number {
            font-size:1.3em;
            font-weight:bold;
            width:40px;
            text-align:right;
            padding-right:4px;
        }
        &--humidity {
            font-size:1.3em;
            font-weight:bold;
            width:54px;
            text-align:right;
            padding-right:4px;
        }
    }
}
</style>