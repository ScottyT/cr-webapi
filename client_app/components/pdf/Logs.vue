<template>
    <section class="pdf-content" slot="pdf-content">
            <h1 class="text-center">{{company}}</h1>
            <h2 class="text-center" v-uppercase>{{reportName}}</h2>
            <h2 v-show="updateMessage !== ''">{{updateMessage}}</h2>
            
            <div class="report-details__section" style="margin-bottom:10px;">
                <div class="report-details__data">
                    <label>Job ID: </label>
                    <span>{{report.JobId}}</span>
                </div>
                <div class="report-details__data">
                    <label>Initial Start Date: </label>
                    <span>{{report.startDate}}</span>
                </div>
                <div class="report-details__data">
                    <label>End Date: </label>
                    <span>{{report.endDate}}</span>
                </div>
                <div class="pdf-item__inline address-box" v-if="report.hasOwnProperty('location')">
                    <div class="pdf-item__data">
                        <label>Address:</label>
                        <span>{{report.location.address}}</span>
                    </div>
                    <div class="pdf-item__data">
                        <label>City, State, Zip:</label>
                        <span>{{report.location.cityStateZip}}</span>
                    </div>
                </div>
            </div>
            <div class="pdf-item__row" v-if="report.notes !== null">
                <label>Notes: </label>
                <div class="pdf-item__textbox">{{report.notes}}</div>
            </div>
            <div class="pdf-item__table inventory-logs" v-if="report.ReportType === 'quantity-inventory-logs'">
                <div class="pdf-item__table pdf-item__table--rows">
                    <div class="pdf-item__table--cols">
                        <div>Description</div>
                    </div>
                    <div class="pdf-item__table--cols" v-for="n in 7" :key="n">
                        <p class="pdf-item__table--data-heading">Day {{n}}</p>
                    </div>
                </div>
                <div class="pdf-item__table pdf-item__table--rows">
                    <div class="pdf-item__table--cols">
                        <label>Tech ID #</label>
                    </div>
                    <div class="pdf-item__table--cols" v-for="n in 7" :key="`techids-col-${n}`">
                        <input type="number" class="pdf-item__table--data" readonly v-model="report.teamMember.id" />
                    </div>
                </div>
                <div class="pdf-item__table pdf-item__table--rows" v-for="(row, i) in report.quantityData" :key="`unitquanitity-${i}`">
                    <div class="pdf-item__table--cols">
                        <label>{{row.text}}</label>
                    </div>
                    <div class="pdf-item__table--cols" v-for="(col, j) in row.day" :key="`unit-col-${j}`">
                        <input type="number" class="pdf-item__table--data" v-model="report.quantityData[i].day[j].value" />
                    </div>
                </div>
                <div class="pdf-item__table pdf-item__table--rows" v-for="(row, i) in report.checkData" :key="`checkbox-${i}`">
                    <div class="pdf-item__table--cols">
                        <label>{{row.text}}</label>
                    </div>
                    <div class="pdf-item__table--cols" v-for="(col, j) in row.day" :key="`checkbox-col-${j}`">
                        <input type="checkbox" class="pdf-item__table--data" v-model="report.checkData[i].day[j].value" />
                    </div>
                </div>
                <div class="pdf-item__table pdf-item__table--rows" v-for="(row, i) in report.serviceArr" :key="`service-${i}`">
                    <div class="pdf-item__table--cols">
                        <label>{{row.text}}</label>
                    </div>
                    <div class="pdf-item__table--cols" v-for="(col, j) in row.day" :key="`service-col-${j}`">
                        <input type="checkbox" class="pdf-item__table--data" v-model="report.serviceArr[i].day[j].value" />
                    </div>
                </div>
               <!--  <div class="pdf-item__table pdf-item__table--rows" v-for="(row, i) in report.categoryData" :key="`category-${i}`">
                    <div class="pdf-item__table--cols">
                        <label>{{row.text}}</label>
                    </div>
                    <div class="pdf-item__table--cols" v-for="(col, j) in row.day" :key="`category-col-${j}`">
                        <input type="text" class="pdf-item__table--data" v-model="report.categoryData[i].day[j].value" />
                    </div>
                </div> -->
            </div>
            <div class="pdf-item__table logs-pdf" v-if="report.ReportType === 'atmospheric-readings'">
                <div class="pdf-item__table pdf-item__table--rows">
                    <div class="pdf-item__table--cols">
                        <div>Description</div>
                    </div>
                    <div class="pdf-item__table--cols" v-for="n in 7" :key="n">
                        <p class="pdf-item__table--data-heading">Day {{n}}</p>
                    </div>
                </div>
                <div class="pdf-item__table pdf-item__table--rows">
                    <div class="pdf-item__table--cols">
                        <label>Tech ID #</label>
                    </div>
                    <div class="pdf-item__table--cols" v-for="n in 7" :key="`techids-col-${n}`">
                        <input type="number" class="pdf-item__table--data" readonly v-model="report.teamMember.id" />
                    </div>
                </div>
                <div class="pdf-item__table pdf-item__table--rows" v-for="(row, i) in report.readingsLog" :key="`row-${i}`">
                    <div class="pdf-item__table--cols">
                        <label>{{row.text}}</label>
                    </div>
                    <div class="pdf-item__table--cols" v-for="(col, j) in row.day" :key="`cols-${j}`">
                        <input type="text" class="pdf-item__table--data" v-model="report.readingsLog[i].day[j].value" />
                    </div>
                </div>
            </div>
            <div class="pdf-item__table logs-pdf" v-if="report.ReportType === 'atmospheric-readings'">
                <div class="pdf-item__table pdf-item__table--rows row-heading">
                    <div class="pdf-item__table--cols">
                        <h3>Water Loss Classification</h3>
                    </div>
                </div>
                
                <div class="pdf-item__table pdf-item__table--rows" v-for="(row, i) in report.lossClassification" :key="`loss-${i}`">
                    <div class="pdf-item__table--cols">
                        <label>{{row.text}}</label>
                    </div>
                    <div class="pdf-item__table--cols" v-for="(col, j) in row.day" :key="`loss-cols-${j}`">
                        <input type="number" class="pdf-item__table--data" v-model="report.lossClassification[i].day[j].value" />
                    </div>
                </div>
                <div class="pdf-item__table pdf-item__table--rows row-heading">
                    <div class="pdf-item__table--cols">
                        <h3>Water Category</h3>
                    </div>
                </div>
                <div class="pdf-item__table pdf-item__table--rows" v-for="(row, i) in report.categoryData" :key="`water-cat-${i}`">
                    <div class="pdf-item__table--cols">
                        <label>{{row.text}}</label>
                    </div>
                    <div class="pdf-item__table--cols" v-for="(col, j) in row.day" :key="`cat-cols-${j}`">
                        <input type="number" class="pdf-item__table--data" v-model="report.categoryData[i].day[j].value" />
                    </div>
                </div>
            </div>
    </section>
</template>
<script>
import genericFuncs from '@/composable/utilityFunctions'
export default {
    props: ['formType', 'reportName', 'reportType', 'report', 'company'],
    data() {
        return {
            editing: false,
            updateMessage: '',
            updated: false,
            errorMessage: "",
            areaCols: ["A", "B", "C", "D", "E", "F", "G", "H", "I", "SUB-1", "SUB-2", "SUB-3"],
            loaded: false,
            baseline: [],
            images: []
        }
    },
    computed: {
        currentDate() {
            const today = new Date()
            var formattedDate = this.formatDate(new Date(today).toISOString().substr(0, 10))
            return formattedDate
        }
    },
    methods: {
        updateReport() {
            // use indexDb for offline support here
            this.$axios.$post(`/api/logs-report/${this.reportType}/${this.report.JobId}/update`, this.newdata).then((res) => {
                if (res.errors) {
                    this.errorMessage = res.errors
                    return;
                }
                this.updateMessage = res.message
                setTimeout(() => {
                    this.updateMessage = ""
                    this.$router.push("/saved-reports")
                }, 5000)
            })
        },
        loadedReport() {
            this.report.baselineReadings.forEach((item) => {
                this.baseline.push(item)
            })
        },
        formatDate(dateReturned) {
            if (!dateReturned) return null
            const [year, month, day] = dateReturned.split('-')
            return `${month}-${day}-${year}`
        },
        getReportImages(jobid, folder, subfolder, delimiter) {
            return new Promise((resolve, reject) => {
                this.$fire.auth.currentUser.getIdToken().then((idToken) => {
                    this.$axios.$get(`${process.env.gsutil}/list/${jobid}`, {
                        params: {folder: folder, subfolder: folder + "/" + subfolder, delimiter: delimiter, bucket: "default" }, headers: {authorization: `Bearer ${idToken}`}}).then((res) => {
                        resolve(res.images)
                    }).catch((err) => {
                        reject(err)
                    })
                })
            })
        }
    },
    mounted() {
        this.$nextTick(() => {
            setTimeout(() => {
                this.$emit("domRendered");
            }, 1000)
        })
        this.getReportImages(this.report.JobId, "moisture-images",  "", "/").then((result) => {
            this.images = result
        })
        if (this.report.hasOwnProperty("baselineReadings")) {
            this.loadedReport()
        }
    }
}
</script>
<style lang="scss" scoped>
.pdf-content {
    margin:auto;
    max-width:870px;
    width:100%;
}
.pdf-item {
    position:relative;
    width:750px;
    .text-center {
        text-align:center;
    }
    .logs-pdf {
        grid-template-rows:repeat(13, 23px);
        padding:10px;
        background-color:$color-white;
        color:$color-black;
    }
    .inventory-logs {
        max-width:800px;
        grid-template-rows:repeat(40, 23px);
        padding:10px;
        background-color:$color-white;
        color:$color-black;
    }
    &__company-logo {
        width:100px;        
        margin:0 auto;
    }
    &__inline {
        display:inline-block;
    }
    &__row {
        display:flex;
        flex-direction:row;
        justify-content: space-between;
        margin-top:20px;
    }
    &__textbox {
        height:100px;
        width:100%;
        padding-left:7px;
    }
    &__table {
        display:grid;
        padding:0 15px;
        &--rows {
            grid-template-columns:120px repeat(7, 1fr);
            grid-template-rows:40px;
            width:100%;
            &:not(:first-child) {
                .pdf-item__table--cols {
                    border-bottom:1px solid;
                    border-top:1px solid;
                }
            }
        }
        &--cols {
            border-color:black;
            border-top:2px solid;
            border-bottom:1px solid;
            label {
                padding:2px;
                display:inline-block;
                font-size:.8em;
            }
            &:not(:first-child) {
                border-right:1px solid;
            }
            &:not(:last-child) {
                border-left:1px solid;
            }
            &:first-child {
                border-left:2px solid;
                border-right:1px solid;
            }
            &:last-child {
                border-left:1px solid;
                border-right:2px solid;
            }
        }
        &--data {
            text-align:center;
            display:block;
            width:100%;
            height:100%;
        }
        &--data-heading {
            text-align:center;
        }
        &.moisture-data {
            background:$color-white;
            color:$color-black;
            
            &--rows {
                grid-template-columns:130px repeat(12, 1fr);
            }
            &--cols-wrapper {
                display:flex;
                flex-direction:row;
            }
        }
        &.row-heading {
            border-top:1px solid;
            border-bottom:1px solid;
            .pdf-item__table--cols {
                grid-column:1/4 span;
                border:0;
            }
        }
    }
}
.number-input {
    display:inline-block;
    width:36px;
    input[type=text] {
        padding:2px 4px;
    }
}
</style>