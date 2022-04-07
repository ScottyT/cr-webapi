<template>
    <div class="form-wrapper">
        <h1 class="text-center">{{company}}</h1>
        <h3 class="text-center">ATMOSPHERIC READINGS</h3>
        <ValidationObserver ref="form" v-slot="{errors}">
            <v-dialog width="400px" v-model="errorDialog">
                <div class="modal__error">
                    <div v-for="(error, i) in errors" :key="`error-${i}`">
                        <h3 class="form__input--error">{{ error[0] }}</h3>
                    </div>
                </div>
            </v-dialog>
            <p class="font-weight-bold">{{submittedMessage}}</p>
            <h3 class="alert alert--error">{{errorMessage}}</h3>
            <form ref="form" class="form" @submit.prevent="onSubmit" v-if="!submitted">
                <div class="form__form-group">
                    <ValidationProvider vid="selectedJobId" rules="required" v-slot="{errors, ariaMsg}" name="Job ID" class="form__input-group form__input-group--normal">
                        <input type="hidden" v-model="selectedJobId" />
                        <label class="form__label">Job ID:</label>
                        <v-overlay :value="isLoading" v-show="isLoading" light>
                            <v-progress-circular
                                indeterminate
                                size="64"
                                ></v-progress-circular>
                        </v-overlay>
                        <i class="form__select--icon icon--angle-down mdi" aria-label="icon"></i>
                        <select class="form__input" v-model="selectedJobId">
                            <option disabled value="">Please select a Job ID</option>
                            <option v-for="(item, i) in $store.state.reports.jobids" :key="`jobids-${i}`">{{item}}</option>
                        </select>
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider vid="startDate" name="Initial Starting Date" class="form__input-group form__input-group--short">
                        <label for="initDate" class="form__label">Initial Starting Date:</label>
                        <input type="hidden" v-model="initDate" />
                        <v-dialog ref="initDateDialog" v-model="initDateModal" :return-value.sync="initDate" persistent width="400px">
                            <template v-slot:activator="{on, attrs}">
                                <input id="initDate" v-model="initDateFormatted" v-bind="attrs" class="form__input" v-on="on" @blur="initDate = parseDate(initDateFormatted)"
                                   readonly />
                            </template>
                            <v-date-picker v-model="initDate" scrollable>
                                <v-spacer></v-spacer>
                                <v-btn text color="#fff" @click="initDateModal = false">Cancel</v-btn>
                                <v-btn text color="#fff" @click="$refs.initDateDialog.save(initDate)">OK</v-btn>
                            </v-date-picker>
                        </v-dialog>
                    </ValidationProvider>
                    <ValidationProvider vid="endDate" rules="required" v-slot="{errors, ariaMsg}" name="End Date" class="form__input-group form__input-group--short">
                        <label for="enddate" class="form__label">End Date:</label>
                        <input type="hidden" v-model="endDate" />
                        <v-dialog ref="endDateDialog" v-model="endDateModal" :return-value.sync="endDate" persistent width="400px">
                            <template v-slot:activator="{on, attrs}">
                                <input id="enddate" v-model="endDateFormatted" v-bind="attrs" class="form__input" v-on="on" readonly />
                            </template>
                            <v-date-picker v-model="endDate" scrollable :min="initDate">
                                <v-spacer></v-spacer>
                                <v-btn text color="#fff" @click="endDateModal = false">Cancel</v-btn>
                                <v-btn text color="#fff" @click="$refs.endDateDialog.save(endDate)">OK</v-btn>
                            </v-date-picker>
                        </v-dialog>
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider vid="curDate" rules="required" v-slot="{errors, ariaMsg}" name="This field" class="form__input-group form__input-group--short">
                        <input type="hidden" v-model="currentDate" />
                        <label class="form__label">Selected date for editing</label>
                        <UiDatePicker :d="initDateFormatted" :minDate="initDateFormatted" :maxDate="endDateFormatted" dateId="curDate" dialogId="curDateDialog" @date="currentDate = $event" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                </div>
                <div class="form__table reading-logs">
                    <div class="form__table form__table--rows">
                        <div class="form__table--cols">
                            <div class="form__label">Description</div>
                        </div>
                        <div class="form__table--cols" v-for="n in dateRanges" :key="n">
                            <div>{{n}}</div>
                        </div>
                    </div>
                    <div class="form__table form__table--rows">
                        <div class="form__table--cols">
                            <div class="form__label">Tech ID #</div>
                        </div>
                        <div class="form__table--cols" v-for="n in 7" :key="n">
                            <input type="text" class="form__input" readonly v-model="getUser.id" />
                        </div>
                    </div>
                    <div class="form__table form__table--rows" v-for="(row, i) in readingsArr" :key="`row-${i}`">
                        <div class="form__table--cols">
                            <div class="form__label">{{row.text}}</div>
                        </div>
                        <div class="form__table--cols" v-for="(item, j) in row.day" :key="`col-${j}`">
                            <input type="text" :tabindex="j" v-on="(row.identifier === 'dryBulbTemp' || row.identifier === 'humidityRatio' || row.identifier === 'dewPoint') ? {
                                    input: ($event) => calculationsDp($event, row.identifier, row.label, item.date)
                                }:{}" v-model="item.value" class="form__input" />
                        </div>
                    </div>
                    <div class="form__table form__table--rows row-heading">
                        <div class="form__table--cols">
                            <h3>Water Loss Classification</h3>
                        </div>
                    </div>
                    <div class="form__table--rows form__table" v-for="(row, i) in lossArr" :key="`classrow-${i}`">
                        <div class="form__table--cols">
                            <div class="form__label">{{row.text}}</div>
                        </div>
                        <div class="form__table--cols" v-for="(item, j) in row.day" :key="`col-${j}`">
                            <input type="number" :tabindex="j" v-model="item.value" class="form__input" />
                        </div>
                    </div>
                    <div class="form__table form__table--rows row-heading">
                        <div class="form__table--cols">
                            <h3>Water Category</h3>
                        </div>                       
                    </div>
                    <div class="form__table--rows form__table" v-for="(row, i) in catArr" :key="`catrow-${i}`">
                        <div class="form__table--cols">
                            <div class="form__label">{{row.text}}</div>
                        </div>
                        <div class="form__table--cols" v-for="(item, j) in row.day" :key="`col-${j}`">
                            <input type="number" :tabindex="j" v-model="item.value" class="form__input" />
                        </div>
                    </div>
                </div>
                <div class="form__form-group">
                    <ValidationProvider vid="notes" name="Notes" v-slot="{errors, ariaMsg}" class="form__input-text-area">
                        <label class="form__label" for="notes">Notes:</label>
                        <textarea id="notes" v-model="notes" class="form__input form__input--textarea"></textarea>
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                </div>
                <div class="form__button-wrapper">
                    <button class="button button--normal" type="submit">{{ submitting ? 'Submitting' : 'Submit' }}</button>
                    <!-- <v-dialog width="400px" v-model="warningDialog">
                        <template v-slot:activator="{on, attrs}">
                            <button v-show="reportFetched" type="button" v-bind="attrs" class="button button--normal" v-on="on">Update</button>
                        </template>
                        <div class="modal__content">
                            <p>You are about to overwrite this report. This is irreversable. Are you sure you want to submit?</p>                       
                        </div>
                        <div class="modal__footer">
                            <button class="button--normal" type="submit">Yes</button>
                            <button class="button--normal" type="button" @click="warningDialog = false">Cancel</button>
                        </div>
                    </v-dialog> -->
                </div>
            </form>
        </ValidationObserver>
    </div>
</template>
<script>
// Humidity Ratio = GGP
import {mapActions, mapGetters} from 'vuex';
import goTo from 'vuetify/es5/services/goto'
import genericFuncs from '@/composable/utilityFunctions'
export default {
    name: "AtmosphericReadings",
    data: (vm) => ({
        readingsArr:[
            {text: "Affected Temperature (fahrenheit)", label: "Affected", identifier: "dryBulbTemp", day: [
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
            {text: "Exterior Temperature", label: "Exterior", identifier: "dryBulbTemp", day: [
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
            {text: "Exterior RH", label: "Exterior", identifier: "relativeHumidity", day: [
                {text: "day1", value: ""},
                {text: "day2",value: ""},
                {text: "day3",value: ""},
                {text: "day4",value: ""},
                {text: "day5",value: ""},
                {text: "day6",value: ""},
                {text: "day7",value: ""}
            ]},
            {text: "Exterior Pressure", label: "Exterior", identifier: "airPressure", day: [
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
            ]},  
        ],
        lossArr: [
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
        ],
        catArr: [
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
        ],
        errorDialog: false,
        submittedMessage: "",
        submitting: false,
        submitted: false,
        errorMessage: "",
        selectedJobId: "",
        initDate: new Date().toISOString().substr(0, 10),
        initDateFormatted: vm.formatDate(new Date().toISOString().substr(0, 10)),
        endDate: vm.addDays(new Date(), 6).toISOString().substr(0, 10),
        endDateFormatted: vm.formatDate(vm.addDays(new Date(), 6).toISOString().substr(0, 10)),
        initDateModal: false,
        endDateModal: false,
        notes: "",
        reportFetched: false,
        warningDialog: false,
        psychrometricData: {
            Affected: {
                info: {
                    dryBulbTemp: "",
                    humidityRatio: "",
                    dewPoint: "",
                    vaporPressure: "",
                    relativeHumidity: ""
                },
                date: "",
                color: "",
                readingsType: "Affected"
            },
            Exterior: {
                info: {
                    dryBulbTemp: "",
                    relativeHumidity: "",
                    humidityRatio: "",
                    dewPoint: ""
                },
                date: "",
                color: "",
                readingsType: "Exterior"
            },
            Unaffected: {
                info: {
                    dryBulbTemp: "",
                    humidityRatio: "",
                    dewPoint: "",
                    vaporPressure: "",
                    relativeHumidity: ""
                },
                date: "",
                color: "",
                readingsType: "Unaffected"
            }
        },
        colors: ['#157f27', '#900C3F', '#0A9C8F', '#FF5733', '#EB1F28', '#343434', '#C70039'],
        hPa: .1805,
        beta: 17.62,
        temp: 243.12,
        vapor: "",
        tempInC: "",
        Dp: "",
        readingsType: {},
        fetchedPsychometric: {},
        currentDate: new Date().toISOString().substr(0, 10),
        error: false,
        reportId: "",
        isLoading: false
    }),
    props: ['company', 'abbreviation'],
    head() {
        return {
            title: "Atmospheric Readings"
        }
    },
    watch: {
        initDate(val) {
            this.initDateFormatted = this.formatDate(val)
            this.endDateFormatted = this.formatDate(this.addDays(val, 6).toISOString().substr(0, 10))
        },
        endDate(val) {
            this.endDateFormatted = this.formatDate(val)
        },
        selectedJobId(val) {
            this.dates = []
            this.isLoading = true
            this.$api.$get(`/api/reports/details/atmospheric-readings/${val}`).then((res) => {
                this.isLoading = false
                this.reportFetched = true
                this.initDateFormatted = res.startDate
                this.$emit("date", res.startDate)
                this.endDateFormatted = res.endDate
                this.readingsArr = res.readingsLog
                this.lossArr = res.lossClassification
                this.catArr = res.categoryData
                this.notes = res.notes
                this.reportId = res.Id
            }).catch((error) => {
                this.isLoading = false
                this.initDateFormatted = this.formatDate(new Date().toISOString().substr(0, 10))
                this.endDateFormatted = this.formatDate(this.addDays(new Date(), 6).toISOString().substr(0, 10))
                this.readingsArr.forEach((item, i) => {
                    item.day.forEach((d, j) => {
                        d.value = ""
                    })
                })
                this.lossArr.forEach((item, i) => {
                    item.day.forEach((d, j) => {
                        d.value = ""
                    })
                })
                this.catArr.forEach((item, i) => {
                    item.day.forEach((d, j) => {
                        d.value = ""
                    })
                })
                this.notes = ""
                this.reportFetched = false
                this.reportId = ""
            })
            this.$api.$get(`/api/reports/details/psychrometric-chart/${val}`).then((res) => {
                this.fetchedPsychometric = res
                this.readingsType = genericFuncs().groupByKey(res.jobProgress, "readingsType")
                this.initDate = new Date(res.jobProgress[0].date).toISOString().substr(0, 10)
                this.endDate = this.addDays(new Date(res.jobProgress[0].date).toISOString().substr(0, 10), 6).toISOString().substr(0, 10)
            }).catch((error) => {
                this.error = true
                this.fetchedPsychometric = {}
            })
        },
        currentDate(val) {
           // this.readingsType = genericFuncs().groupByKey(this.fetchedPsychometric.jobProgress, "readingsType")
           for (const key in this.readingsType) {
               this.readingsType[key]
           }
           /* Array.from(this.readingsType).filter((el) => {
               for (const key in el) {
                   console.log(key)
                   return key.date == val
               }
           }) */
        }
    },
    computed: {
        ...mapGetters({getReports: 'reports/getReports', getUser: 'users/getUser'}),
        groupingData() {
            return genericFuncs().groupByKey(this.readingsArr, 'label')
        },
        dateIndex() {
            return this.dateRanges.indexOf(this.currentDate)
        },
        dateRanges() {
            var datediff = new Date(this.endDateFormatted) - new Date(this.initDateFormatted)
            var daysDiff = datediff /  (1000 * 60 * 60 * 24);
            var startDateDay = new Date(this.initDateFormatted).getDate()
            var dates = []
            for (let i = 0; i <= daysDiff; i++) {
                var date = new Date(this.initDateFormatted)
                var day = date.setDate(startDateDay + i)
                var formattedDate = this.formatDate(new Date(day).toISOString().substr(0, 10))
                dates.push(formattedDate)
            }
            this.readingsArr.forEach((item, i) => {
                for (let j = 0; j < item.day.length; j++) {
                    item.day[j].date = dates[j]
                }
            })
            return dates
        },
        vaporToGGP() {
            var nums = []
            var counter = 0
            var ggp = 0
            while (counter <= 1.4) {
                counter = counter + .01
                ggp = ggp + 1.5
                nums.push({vaporPressure: genericFuncs().round(counter, 2), humidityRatio: ggp.toString()})
            }
            return nums
        }
    },
    methods: {
        ...mapActions({
            fetchReport: 'reports/fetchReport'
        }),
        calculationsDp(e, param, label, date) {
            var dateIndex = this.dateRanges.indexOf(date)
            var affectedTemp = this.groupingData["Affected"].find(el => el.identifier === "dryBulbTemp" && el.label === "Affected")
            var exteriorTemp = this.groupingData["Exterior"].find(el => el.identifier === "dryBulbTemp" && el.label === "Exterior")
            var unaffectedTemp = this.groupingData["Unaffected"].find(el => el.identifier === "dryBulbTemp" && el.label === "Unaffected")
            var affectedRH = this.groupingData["Affected"].find(el => el.identifier === "relativeHumidity" && el.label === "Affected")
            var exteriorRH = this.groupingData["Exterior"].find(el => el.identifier === "relativeHumidity" && el.label === "Exterior")
            var unaffectedRH = this.groupingData["Unaffected"].find(el => el.identifier === "relativeHumidity" && el.label === "Unaffected")
            var affectedDp = this.groupingData["Affected"].find(el => el.identifier === "dewPoint" && el.label === "Affected")
            var exteriorDp = this.groupingData["Exterior"].find(el => el.identifier === "dewPoint" && el.label === "Exterior")
            var unaffectedDp = this.groupingData["Unaffected"].find(el => el.identifier === "dewPoint" && el.label === "Unaffected")
            var affectedVapor = this.groupingData["Affected"].find(el => el.identifier === "vaporPressure" && el.label === "Affected")
            var exteriorVapor = ""
            var unaffectedVapor = this.groupingData["Unaffected"].find(el => el.identifier === "vaporPressure" && el.label === "Unaffected")
            var tempInCAffected = genericFuncs().convertToC(affectedTemp.day[dateIndex].value)
            var tempInCExterior = genericFuncs().convertToC(exteriorTemp.day[dateIndex].value)
            var tempInCUnaffected = genericFuncs().convertToC(unaffectedTemp.day[dateIndex].value)
            
            for (const property in this.groupingData) {
                this.groupingData[property].forEach((item, i) => {
                    if (param === "dryBulbTemp" && label === "Affected") {
                        this.psychrometricData["Affected"].info.dryBulbTemp = e.target.value
                    }
                    if (param === "dryBulbTemp" && label === "Exterior") {
                        this.psychrometricData["Exterior"].info.dryBulbTemp = e.target.value
                    }
                    if (param === "dryBulbTemp" && label === "Unaffected") {
                        this.psychrometricData["Unaffected"].info.dryBulbTemp = e.target.value
                    }
                    if (param === "humidityRatio" && label === "Affected") {
                        let vapor = e.target.value
                        affectedVapor.day[dateIndex].value = this.calcVapor(vapor, "Affected")
                    }
                    if (param === "humidityRatio" && label === "Unaffected") {
                        unaffectedVapor.day[dateIndex].value = this.calcVapor(e.target.value, "Unaffected")
                    }
                    if (param === "humidityRatio" && label === "Exterior") {
                        exteriorVapor = this.calcVapor(e.target.value, "Exterior")
                    }
                    if (item.identifier === "dewPoint" && label === "Affected") {
                        var ln = affectedVapor.day[dateIndex].value / this.hPa
                        var dewPointF = genericFuncs().convertToF((this.temp * Math.log(ln)) / (this.beta - Math.log(ln)))
                        affectedDp.day[dateIndex].value = genericFuncs().round(dewPointF, 3)
                    }
                    if (item.identifier === "dewPoint" && label === "Unaffected") {
                        var ln = unaffectedVapor.day[dateIndex].value / this.hPa
                        var dewPointF = genericFuncs().convertToF((this.temp * Math.log(ln)) / (this.beta - Math.log(ln)))
                        unaffectedDp.day[dateIndex].value = genericFuncs().round(dewPointF, 3)
                    }
                    if (item.identifier === "dewPoint" && label === "Exterior") {
                        var ln = exteriorVapor / this.hPa
                        var dewPointF = genericFuncs().convertToF((this.temp * Math.log(ln)) / (this.beta - Math.log(ln)))
                        exteriorDp.day[dateIndex].value = genericFuncs().round(dewPointF, 3)
                    }  
                    if (item.identifier === "relativeHumidity" && label === "Affected") {
                        var satVapor = this.hPa * Math.exp((this.beta * tempInCAffected) / (this.temp + tempInCAffected))
                        affectedRH.day[dateIndex].value = `${(affectedVapor.day[dateIndex].value / satVapor * 100).toFixed(2)}%`
                    }
                    if (item.identifier === "relativeHumidity" && label === "Unaffected") {
                        var satVapor = this.hPa * Math.exp((this.beta * tempInCUnaffected) / (this.temp + tempInCUnaffected))
                        unaffectedRH.day[dateIndex].value = `${(unaffectedVapor.day[dateIndex].value / satVapor * 100).toFixed(2)}%`
                    }
                    if (item.identifier === "relativeHumidity" && label === "Exterior") {
                        var satVapor = this.hPa * Math.exp((this.beta * tempInCExterior) / (this.temp + tempInCExterior))
                        exteriorRH.day[dateIndex].value = `${(exteriorVapor / satVapor * 100).toFixed(2)}%`
                    }
                })
            }
        },
        calcVapor(ggp) {
            const closest = this.vaporToGGP.reduce((a,b) => {
                return Math.abs(b.humidityRatio - ggp) < Math.abs(a.humidityRatio - ggp) ? b : a;
            })
            return closest.vaporPressure
        },
        disableDates(curindex) {
            var dateOfRow = new Date(this.dateRanges[curindex])
            dateOfRow.setHours(0,0,0,0)
            var today = new Date(this.currentDate)
            today.setHours(0,0,0,0)
            if (dateOfRow.getTime() !== today.getTime()) {
                return true
            } else {
                return false
            }
        },
        addDays(d, days) {
            const date = new Date(d);
            date.setDate(date.getDate() + days);
            return date;
        },
        formatDate(dateReturned) {
            if (!dateReturned) return null
            const [year, month, day] = dateReturned.split('-')
            return `${month}/${day}/${year}`
        },
        parseDate(date) {
            if (!date) return null
            const [month, day, year] = date.split('/')
            return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`
        },
        onSubmit() {
            this.submittedMessage = ""
            this.submitting = true
            const reports = this.getReports.filter((v) => {
                return v.ReportType === 'atmospheric-readings'
            })
            const jobids = reports.map((v) => {
                return v.JobId
            })
            
            const post = {
                JobId: this.selectedJobId,
                ReportType: "atmospheric-readings",
                startDate: this.initDateFormatted,
                endDate: this.endDateFormatted,
                formType: "logs-report",
                readingsLog: this.readingsArr,
                lossClassification: this.lossArr,
                categoryData: this.catArr,
                notes: this.notes,
                teamMember: this.getUser
            };
            
            this.$refs.form.validate().then(success => {
                if (!success) {
                    this.submitting = false
                    this.errorDialog = true
                    return goTo(0)
                }
                
                this.submitPsychrometic().then(result => {
                    this.$api.$put(`/api/reports/atmospheric-readings/${this.selectedJobId}/update`, post).then((res) => {
                        this.submittedMessage = res.message
                        this.submitting = false
                        this.submitted = true
                        setTimeout(() => {
                            window.location = "/"
                        }, 3000)
                    }).catch(err => {
                        this.errorDialog = true
                        this.submitting = false
                        if (err.response) {
                            console.error(err.response.data)
                        }
                    })
                })
            })
        },
        submitPsychrometic() {
            return new Promise((resolve) => {
                for (const property in this.groupingData) {
                    this.groupingData[property].forEach((item) => {
                        var filteredResults = item.day.filter(d => d.date === this.currentDate && d.value !== "")
                        if (filteredResults.length > 0) {
                            this.psychrometricData[property].info[item.identifier] = filteredResults[0].value
                            this.psychrometricData[property].date = filteredResults[0].date
                            this.psychrometricData[property].color = this.colors[this.dateIndex]
                        }
                    })
                }
                var promises = []
                for (const property in this.psychrometricData) {
                    if (this.psychrometricData[property].color === "") {
                        // this is to make sure not to submit empty data
                        delete this.psychrometricData[property]
                    }
                    const psychrometricPost = {
                        JobId: this.selectedJobId,
                        teamMember: this.getUser,
                        jobProgress: this.psychrometricData[property],
                        formType: 'chart-report',
                        ReportType: 'psychrometric-chart'
                    }
                    promises.push(psychrometricPost)
                }
                Promise.all(promises).then(result => {
                    let filteredArr = result.filter(r => r.jobProgress !== undefined)
                    filteredArr.forEach((item) => {
                        let updateReading = this.readingsType[item.jobProgress.readingsType] !== undefined
                        //var newDateProgress = this.readingsType[item.jobProgress.readingsType].find(el => el.date === jobProgressDate) === undefined
                        
                        if (updateReading) {
                            this.$api.$post(`/api/reports/psychrometric-chart/update-progress`, item).then((res) => {
                                this.message = res
                            })
                        } else {
                            this.$api.$post(`/api/reports/psychrometric-chart/update-chart`, item).then((res) => {
                                this.message = res
                            })
                        }
                    })
                    resolve("chart posting is done")
                    this.submittedMessage = "Psychrometric chart data sent"
                })
            })
        }
    }
}
</script>
<style lang="scss">
.reading-logs {
    grid-template-rows:60px repeat(18, 1fr);
}
</style>