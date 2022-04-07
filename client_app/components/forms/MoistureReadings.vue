<template>
    <div class="form-wrapper">
        <h1 class="text-center">{{company}}</h1>
        <h3 class="text-center">MOISTURE READING MAP READINGS</h3>
        <v-overlay :value="isLoading" v-show="isLoading" light>
            <v-progress-circular indeterminate size="64"></v-progress-circular>
        </v-overlay>
        <ValidationObserver ref="form" v-slot="{ errors }">
            <v-dialog width="400px" v-model="errorDialog">
                <div class="modal__error">
                    <div v-for="(error, i) in errors" :key="`error-${i}`">
                        <h3 class="form__input--error">{{ error[0] }}</h3>
                    </div>
                </div>
            </v-dialog>
            <p class="font-weight-bold">{{submittedMessage}}</p>
            <form class="form" v-if="!submitted" @submit.prevent="submitForm">
                <div class="form__form-group">
                    <ValidationProvider vid="JobId" v-slot="{errors, ariaMsg}" name="Job ID" class="form__input-group form__input-group--normal">
                        <input type="hidden" v-model="selectedJobId" />
                        <label class="form__label">Job ID:</label>
                        <i class="form__select--icon icon--angle-down mdi" aria-label="icon"></i>
                        <select class="form__input" v-model="selectedJobId">
                            <option disabled value="">Please select a Job ID</option>
                            <option v-for="(item, i) in $store.state.reports.jobids" :key="`jobids-${i}`">{{item}}</option>
                        </select>
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider vid="evalDate" name="Initial date of evaluation" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--short">
                        <label class="form__label" for="evalDate">Initial Date of Evaluation</label>
                        <input type="hidden" v-model="initialEvalDateFormatted" />
                        <v-dialog ref="initEvalDateDialog" v-model="initEvalDateModal" :return-value.sync="initialEvalDate" persistent width="500px">
                            <template v-slot:activator="{on, attrs}">
                                <input id="evalDate" v-model="initialEvalDateFormatted" v-bind="attrs" readonly class="form__input" v-on="on"
                                    @blur="initialEvalDate = parseDate(initialEvalDateFormatted)" />
                            </template>
                            <v-date-picker v-model="initialEvalDate" scrollable>
                                <v-spacer></v-spacer>
                                <v-btn text color="#fff" @click="initEvalDateModal = false">Cancel</v-btn>
                                <v-btn text color="#fff" @click="$refs.initEvalDateDialog.save(initialEvalDate)">OK</v-btn>
                            </v-date-picker>
                        </v-dialog>
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <div class="form__input-group form__input-group--long">
                        <label for="location" class="form__label">Location</label>
                        <UiGeoCoder @sendAddress="settingLocation($event)" :mapView="false" geocoderid="geocoder" geocoderef="geocoder" />
                    </div>
                    <ValidationProvider vid="address" name="Address" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--long">
                        <label for="address" class="form__label">Address</label>
                        <input id="address" v-model="location.address" name="Address" type="text" class="form__input form__input--long" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider vid="cityStateZip" name="City, State, and Zip" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--long">
                        <label for="citystatezip" class="form__label">City, State, Zip</label>
                        <input id="citystatezip" v-model="location.cityStateZip" type="text" class="form__input form__input--long" />
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                </div>
                <div class="form__form-group area-sub-group">
                    <div class="form__input-group form__input-group--long" v-for="(subarea, i) in subAreas" :key="`subarea-${i}`">
                        <label class="form__label">Area Sub-{{i+1}}</label>
                        <input :id="`areaSub${i}`" class="form__input" v-model="subarea.areaName" @click="openTable(i)" />
                        <transition name="table-slide">
                            <div class="area-sub-group__table" v-if="active === i">
                                <div class="area-sub-group__table--row">
                                    <div class="area-sub-group__table--cols">
                                        <label class="form__label">Date:</label>
                                    </div>
                                    <div class="area-sub-group__table--cols" v-for="(col, k) in subarea.areaCols" :key="`col-${k}`">
                                        <label class="form__label">Area {{col}} %</label>
                                    </div>
                                </div>
                                <div class="area-sub-group__table--row" v-for="(row, k) in subAreas[i].areas" :key="`row-${k}`">
                                    <div class="area-sub-group__table--cols">
                                        <input type="text" v-mask="'##/##/####'" v-model="row.date" class="form__input" />
                                    </div>
                                    <div class="area-sub-group__table--cols" v-for="(area, l) in row.area" :key="`input-${l}`">
                                        <span class="number-input"><input type="number" maxlength="3" v-model="row.area[l].val" class="form__input" /></span>                                       
                                    </div>
                                    <button class="button--normal" type="button" @click="addColumn(row.area, i, k)">+</button>
                                </div>
                                <button class="button--normal" type="button" @click="addRow(`row${subarea.areas.length}`, `sub${i}`, i, 'area-readings')">Add row</button>
                                <!-- <button class="button--normal" type="button" @click="addColumn(subarea.areas[0].area, i)">Add column</button> -->
                            </div>
                        </transition>
                    </div>
                    <button class="button--normal" type="button" @click="addSub(`sub${subAreas.length}`, subAreas.length)">Add new sub area</button>
                </div>
                <div class="form__form-group area-sub-group">
                    <div class="form__input-group form__input-group--long">
                        <label for="baseLine" class="form__label">Base Line Comparitive Readings (Non-Affected)</label>
                        <div class="area-sub-group__table">
                            <div class="area-sub-group__table--row">
                                <div class="area-sub-group__table--cols">
                                    <label class="form__label">Date:</label>
                                </div>
                                <div class="area-sub-group__table--cols" v-for="(subarea, i) in subAreas" :key="i">
                                    <label class="form__label">Area Sub-{{i+1}} %</label>
                                </div>
                            </div>
                            <div class="area-sub-group__table--row" v-for="(row, j) in baseline" :key="`base-${j}`">
                                <div class="area-sub-group__table--cols">
                                    <input type="text" v-mask="'##/##/####'" v-model="row.date" class="form__input" />
                                </div>
                                <div class="area-sub-group__table--cols" v-for="(col, i) in row.subareas"
                                     :key="`cols-${i}`">
                                    <span class="number-input"><input type="text" maxlength="3" v-model="col.val"
                                               class="form__input" /></span>
                                </div>
                            </div>
                            <button class="button--normal" type="button" 
                                @click="addRow(`baseline${baseline.length - 1}`,`baseline${baseline.length - 1}`, baseline.length, 'baseline-readings')">Add row</button>
                        </div>
                    </div>
                </div>
                <LayoutMoistureCompare :width="860" :height="700" :jobid="selectedJobId" :existingChart="baseline" />
                <!-- <button type="button" class="button--normal" @click="updateChart">Update</button> -->
                <div class="form__section">
                    <ValidationProvider vid="notes" name="Notes" rules="required" v-slot="{errors, ariaMsg}" class="form__input--input-group">
                        <label class="form__label" for="notes">Notes:</label>
                        <textarea id="notes" class="form__input form__input--textarea" v-model="notes"></textarea>
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                </div>
                <div class="form__form-group">
                    <UiFilesUpload :singleImage="false" :subDir="`moisture-images`" :rootPath="selectedJobId" @uploadDone="uploaded = $event" @sendDownloadUrl="uploadedFiles = $event" />
                </div>
                <button type="submit" class="button button--normal" v-show="!reportFetched">{{ submitting ? 'Submitting' : 'Submit' }}</button>
                <v-dialog width="400px" v-model="warningDialog">
                    <template v-slot:activator="{on, attrs}">
                        <button v-show="reportFetched" type="button" v-bind="attrs" class="button button--normal" v-on="on">Update</button>
                    </template>
                    <div class="modal__content">
                        <p>You are about to overwrite this report. This is irreversable. Are you sure you want to submit?</p>                       
                    </div>
                    <div class="modal__footer">
                        <button class="button--normal" type="submit" @click="submitForm">Yes</button>
                        <button class="button--normal" type="button" @click="warningDialog = false">Cancel</button>
                    </div>
                </v-dialog>
            </form>
        </ValidationObserver>
    </div>
</template>
<script>
import {mapActions, mapGetters} from 'vuex';
import genericFuncs from '@/composable/utilityFunctions'
import { dateMask } from "@/data/masks";
export default {
    name: "MoistureReadings",
    data: (vm) => ({
        errorDialog: false,
        uploading:false,
        submittedMessage: "",
        errorMessage: "",
        submitted: false,
        submitting: false,
        selectedJobId: "",
        uploadedFiles:[],
        initialEvalDate: new Date().toISOString().substr(0, 10),
        initialEvalDateFormatted: vm.formatDate(new Date().toISOString().substr(0, 10)),
        initEvalDateModal: false,
        location: {
            address: null,
            city: null,
            cityStateZip: null,
        },
        areaCols: ["A"],
        subAreas: [
            {
                areaCols: ["A"],
                subLabel: 'sub0',
                areaName: "",
                reading: "",
                areas: [
                    {
                        date: "",
                        label: "row0",
                        area: [
                            {
                                label: 'A',
                                val: ''
                            }
                        ]
                    }
                ],
            }
        ],
        baseline: [
            {
                date: '',
                subareas: [{label: "sub1", val: ""}],
                label: 'baseline0'
            }
        ],
        uploadProgress: "",
        uploadMessage: "",
        filesUpload: [],
        notes: '',
        disabled: false,
        tables:[false, false, false],
        active: null,
        reportFetched: false,
        warningDialog: false,
        loaded: false,
        uploaded:false,
        isLoading: false,
        dateMask: dateMask
    }),
    props:['company', 'abbreviation'],
    head() {
        return {
            title: "Moisture Readings"
        }
    },
    computed: {
        ...mapGetters({getReports:'reports/getReports', getUser:'users/getUser'}),
        date() {
            const today = new Date()
            return (today.getMonth() + 1)+'-'+today.getUTCDate()+'-'+today.getFullYear();
        }
    },
    watch: {
        initialEvalDate(val) {
            this.initialEvalDateFormatted = this.formatDate(val)
        },
        selectedJobId(val) {
            this.loaded = true
            this.isLoading = true
            this.$api.$get(`/api/reports/details/moisture-map/${val}`).then((res) => {
                this.reportFetched = true
                this.isLoading = false
                this.initialEvalDateFormatted = res.initialEvalDate
                this.location = res.location
                this.subAreas = res.readingsRow
                this.baseline = res.baselineReadings
                this.notes = res.notes
                var areas = genericFuncs().groupByKey(res.readingsRow, 'label')
                for (const property in areas) {
                    this[property] = areas[property]
                }
            }).catch(err => {
                this.isLoading = false
                var initReport = this.$store.state.reports.all.find(obj => obj.ReportType === 'dispatch')
                if (initReport !== undefined) {
                    this.location.address = initReport.location.address
                    this.location.cityStateZip = initReport.location.cityStateZip
                } else {
                    this.location = {
                        address: null,
                        city: null,
                        cityStateZip: null,
                    }
                }
                this.initialEvalDateFormatted = this.formatDate(new Date().toISOString().substr(0, 10))
                this.subAreas = [{
                    areaCols: ["A"],
                    subLabel: 'sub0',
                    areaName: "",
                    reading: "",
                    areas: [{
                        date: "",
                        label: "row0",
                        area: [{
                            label: 'A',
                            val: ''
                        }]
                    }],
                }]
                this.baseline = [{
                    date: '',
                    subareas: [{
                        label: "sub1",
                        val: ""
                    }],
                    label: 'baseline0'
                }]
                this.notes = ""
                this.reportFetched = false
                return;
            })
        }
    },
    methods: {
        updateChart() {
            this.$emit('updatingChart', this.baseline)
            this.loaded = false
            setTimeout(() => {
                this.loaded = true
            }, 500)
        },
        addDays(d, days) {
            if (d === undefined) return
            const date = new Date(d);
            
            date.setDate(date.getDate() + days);
            return date;
        },
        formatDate(dateReturned) {
            if (!dateReturned) return null
            const [year, month, day] = dateReturned.split('-')
            return `${month}/${day}/${year}`
        },
        addRow(area, subarea, subIndex, areaType) {
            switch (areaType) {
                case "area-readings":
                    var sub = this.subAreas.find(el => el.subLabel === subarea)
                    if (sub.areas[sub.areas.length - 1].date === "") {
                        alert("You must add a date for sub area row.")
                        break;
                    }
                    var cols = []
                    this.subAreas[subIndex].areaCols.forEach((col) => {
                        cols.push({label: col, val: ''})
                    })
                    var nextDate = this.formatDate(this.addDays(sub.areas[sub.areas.length - 1].date, 1).toISOString().substr(0,10))
                    sub.areas.push({ date: nextDate,label: area,area: cols })
                    break;
                case "baseline-readings":
                    var baselineRow = this.baseline.find(el => el.label === area)
                    if (baselineRow.date === "") {
                        alert("You must add a date for sub area row.")
                        break;
                    }
                    var cols = []
                    baselineRow.subareas.forEach((item) => {
                        cols.push({label: `sub${subIndex+1}`, val: ""})
                    })
                    this.baseline.push({date: "", label: `baseline${subIndex}`, subareas: cols})
                    break;
            }           
        },
        addSub(sub, index) {
            this.subAreas.push({
                subLabel: sub,
                areaName: "",
                areaCols: ["A"],
                areas: [
                    {
                        date: "",
                        label: "row0",
                        area: [
                            {
                                label: 'A',
                                val: ''
                            }
                        ]
                    }
                ],
            })
            var subColLabels = []
            this.subAreas.forEach((item) => {
                subColLabels.push(item.subLabel)
            })
            this.baseline.forEach((col) => {
                col.subareas.push({label: subColLabels[index], val: ""})
            })
        },
        addColumn(colArr, subIndex, rowIndex) {
            var char2 = new Array(27)
            var char1 = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'.split('');
            char2 = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'.split('')
            char2.unshift('')
            
            var result = []
            for (var i=0;i<char2.length;i++) {
                for (var j=0;j<char1.length;j++) {
                    result.push(char2[i] + char1[j])
                }
            }
            
            var colToAdd = result[colArr.length]
            if (!this.subAreas[subIndex].areaCols.includes(colToAdd)) {
                this.subAreas[subIndex].areaCols.push(colToAdd)
            }
            if (!this.areaCols.includes(colToAdd)) {
                this.areaCols.push(colToAdd)
            }

            this.subAreas[subIndex].areas[rowIndex].area.push({label: result[colArr.length], val: ""})
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
        submitForm() {
            this.submitting = true
            this.submittedMessage = ""
            const reports = this.getReports.filter((v) => {
                return v.ReportType === 'moisture-map'
            })
            const jobids = reports.map((v) => {
                return v.JobId
            })
            
            const post = {
                JobId: this.selectedJobId,
                ReportType: "moisture-map",
                formType: "logs-report",
                initialEvalDate: this.initialEvalDateFormatted,
                baselineReadings: this.baseline,
                readingsRow: this.subAreas,
                location: this.location,
                notes: this.notes,
                teamMember: this.getUser
            };
            this.$refs.form.validate().then(success => {
                if (!success) {
                    this.submitting = false
                    this.errorDialog = true
                    return;
                }
                this.$api.$put(`/api/reports/moisture-map/${this.selectedJobId}/update`, post).then((res) => {
                    this.submittedMessage = res
                    this.submitting = false
                    this.submitted = true
                    setTimeout(() => {
                        window.location = "/"
                    }, 3000)
                }).catch(err => {
                    if (err.response) {
                        this.errorMessage = err.response.data
                        this.errorDialog = true
                        this.submitting = false
                    }
                })
            })
        },
        openTable(index) {
            this.active = index
        },
        settingLocation(params) {
            this.location.cityStateZip = params.cityStateZip
            this.location.address = params.address
        }
    }
}
</script>
<style lang="scss">
.moisture-map {
    @include respond(tabletLargeMax) {
        font-size:.95em;
    }
    .form__table--rows {
        grid-template-columns:100px repeat(12, 1fr);
    }
    input[type=text] {
        margin-bottom:0;
        @include respond(tabletLandscapeMax) {
            font-size:.9em;
            padding:2px 4px;
        }
    }
    .number-input {
        display:inline-block;
        @include respond(tabletLarge) {
            width:53px;
            input {
                padding:2px 4px;
            }
        }
        width:38px;
    }
}
.area-sub-group {
    flex-direction:column;
    &__table {
        width: 700px;
        height:auto;
        padding:10px 0;
        &--row {
            display:grid;
            grid-template-columns: 118px repeat(auto-fill, minmax(62px, 1fr));
            column-gap:15px;
            &:not(:first-child) {
                margin:7px 0;
            }
        }
    }
}
</style>