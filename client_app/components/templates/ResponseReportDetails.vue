<template>
<section :class="notPdf ? '' : 'pdf-content'">
  <div class="report-details report-details__response-report">
    <h1 v-if="message">{{message}}</h1>
    <h1 class="text-center">{{company}}</h1>
    <h2 class="text-center">{{reportName}}</h2>
    <v-btn v-if="notPdf" @click="startEditing" dark>{{ isEditing ? "Editing" : "Edit"}}</v-btn>
    <v-btn v-show="isEditing" @click="updateReport" dark>Update</v-btn>
    <div class="report-details__section">
      
      <div class="report-details__data">
        <h3>Team Lead ID #:</h3>
        <span>{{report.id}}</span>
      </div>
      <div class="report-details__data">
        <h3>Job ID:</h3>
        <span>{{report.JobId}}</span>
      </div>
      <div class="report-details__data">
        <h3>Date of Loss:</h3>
        <span>{{report.DateOfLoss}}</span>
      </div>
      <div class="report-details__data">
        <h3>Date Of Evaluation:</h3>
        <span>{{report.DateOfEvaluation}}</span>
      </div>
    </div>
    <div class="report-details__section report-details__section--contact-group">
      <div class="report-details__section--name-group">
          <div class="report-details__data-label">Contact Name:</div>
          <span v-if="!isEditing" class="report-details__data-field" v-uppercase>
            {{repData && repData.ContactName ? repData.ContactName.first + ' ' + repData.ContactName.last : null}}
          </span>
          <div v-if="isEditing" class="form__input-group--name-group">
            <input type="text" class="form__input capitalize" v-model="updatedReport.ContactName.first" placeholder="First" />
            <input type="text" class="form__input capitalize" v-model="updatedReport.ContactName.last" placeholder="Last" />
          </div>

          <div class="report-details__data-label">Property Owner:</div>
          <span v-if="!isEditing" class="report-details__data-field" v-uppercase>
            {{repData && repData.PropertyOwner ? repData.PropertyOwner.first + ' ' + repData.PropertyOwner.last : null}}
          </span>
          <div v-if="isEditing" class="form__input-group--name-group">
            <input type="text" class="form__input capitalize" v-model="updatedReport.PropertyOwner.first" placeholder="First" />
            <input type="text" class="form__input capitalize" v-model="updatedReport.PropertyOwner.last" placeholder="Last" />
          </div>
      </div>
      
      <div class="report-details__data-label">Address:</div>
      <input class="form__input" v-if="isEditing" v-model="updatedReport.location.address" />
      <span v-if="!isEditing"
        class="report-details__data-field">{{report && report.location ? report.location.address : null}}</span>

      <div class="report-details__data-label">City, State, Zip:</div>
      <input class="form__input" v-if="isEditing" v-model="updatedReport.location.cityStateZip" />
      <span v-if="!isEditing"
        class="report-details__data-field">{{report && report.location ? report.location.cityStateZip : null}}</span>

      <div class="report-details__data-label">Phone Number:</div>
      <input class="form__input" v-if="isEditing" :value="updatedReport.phoneNumber" v-imask="phoneMask" @complete="updatedReport.phoneNumber = $event.detail.value" />
      <span v-if="!isEditing" class="report-details__data-field">{{report.phoneNumber}}</span>

      <div class="report-details__data-label">Email Address:</div>
      <input class="form__input" v-if="isEditing" v-model="updatedReport.emailAddress" />
      <span v-if="!isEditing" class="report-details__data-field">{{report.emailAddress}}</span>
    </div>    
    
    <div class="report-details__section">
      <div class="report-details__checklist">
        <h3>Inital Response, Inspection, and Preliminary Determination</h3>
        <ul v-if="arrivalImages.length > 0" class="report-details__section--pictures">
          <li v-for="item in arrivalImages" :key="item.id" class="report-details__image">
            <img :src="item.imageUrl" />
            <p>{{item.name}}</p>
          </li>
        </ul>
        <p v-else>N/A</p>
      </div>
    </div>
  </div>
  <div class="report-details report-details__response-report">
    <div class="report-details__section">
      <div class="report-details__checklist">
        <h3>Source of Water Intrusion</h3>
        <ul>
          <li v-for="item in report.sourceWaterIntrusion" :key="item.id">
            {{item}}
          </li>
        </ul>
      </div>
    </div>
    <div class="report-details__section">
      <div class="report-details__data" v-for="(item, i) in report.intrusion" :key="`intrusion-${i}`">
        <label class="form__label">{{item.label}}</label>
        <span>{{item.value}}</span>
      </div>
    </div>
    <div class="report-details__data">
      <label class="form__label">Initial:</label>
      <img class="report-details__initial" :src="report.initials.initial1" />
    </div>
    <div class="report-details__section">
      <div class="report-details__checklist">
        <h3>Preliminary Determination</h3>
        <ul>
          <li v-for="(item, i) in report.preliminaryDetermination" :key="`prelim-${i}`">
            {{item}}
          </li>
        </ul>
      </div>
    </div>
    <div class="report-details__data">
      <label class="form__label">Initial:</label>
      <img class="report-details__initial" :src="report.initials.initial2" />
    </div>
    <div class="report-details__section">
      <div class="report-details__checklist">
        <h3>Initial Moisture Inspection</h3>
        <ul>
          <li v-for="(item, i) in report.moistureInspection" :key="`moisture-${i}`">
            {{item.label}}
            <span v-if="item.hasOwnProperty('dryStandard')">{{item.dryStandard}}</span>
          </li>
        </ul>
      </div>
    </div>
  </div>
  <div class="report-details report-details__response-report">
    
    <div class="report-details__data">
      <label class="form__label">Initial:</label>
      <img class="report-details__initial" :src="report.initials.initial3" />
    </div>
    
    <div class="report-details__section">
      <label class="form__label">Initial Moisture Map</label>
      <p>An initial moisture inspection should be conducted to identify the full extent of water intrusion,
        including the
        identification of affected assemblies, building materials, and the edge of water mitigation. Normally,
        this process
        begins at the source of the water intrusion. The initial inspection process should continue in all
        directions from the
        source of water intrusion until the restorer identifies and documents the extent of migration. The extent
        of the
        moisture migration should be documented using one or more appropriate methods including at a minimum a
        moisture map. (i.e., a diagram of the structure indicating the areas affected by migrating water). IICRC
        S500 Pg 179</p>
      <div class="map-wrapper">
        <div class="map-wrapper__map">
          <div class="map-wrapper__map--row" v-for="n in 37" :key="n">
            <div class="map-wrapper__map--col" v-for="n in 20" :key="n"></div>
          </div>
        </div>
        <img id="moisture-map-image" :src="report.moistureMap" />
      </div>
    </div>
  </div>
    <div class="report-details report-details__response-report">
      <div class="report-details__data">
        <label class="form__label">Initial:</label>
        <img class="report-details__initial" :src="report.initials.initial4" />
      </div>
      <div class="report-details__section">
        <div class="report-details__data">
          <label class="form__label">Emergency Response Actions Identified:</label>
          <span>{{report.preRestorationEval ? report.preRestorationEval.emergencyResAct : null}}</span>
        </div>
        <div class="report-details__data">
          <label class="form__label">Building Material Restorability:</label>
          <span>{{report.preRestorationEval ? report.preRestorationEval.buildingMatRestore : null}}</span>
        </div>
        <div class="report-details__data">
          <label class="form__label">Content Evaluation:</label>
          <span>{{report.preRestorationEval ? report.preRestorationEval.contentEval : null}}</span>
        </div>
        <div class="report-details__data">
          <label class="form__label">HVAC Evaluation:</label>
          <span>{{report.preRestorationEval ? report.preRestorationEval.hvacEval : null}}</span>
        </div>
        <div class="report-details__data">
          <label class="form__label">Substructure & Unfinished Spaces:</label>
          <span>{{report.preRestorationEval ? report.preRestorationEval.substructure : null}}</span>
        </div>
      </div>
    
      <div class="report-details__section">
        <div class="report-details__data">
          <h3>Insurance Company:</h3>
          <input class="form__input" type="text" v-if="isEditing" v-model="updatedReport.InsuranceCompany" />
          <span v-if="!isEditing">{{report.InsuranceCompany}}</span>
        </div>
        <div class="report-details__data">
          <h3>Claim Number:</h3>
          <input type="text" class="form__input" v-if="isEditing" v-model="updatedReport.ClaimNumber" />
          <span v-if="!isEditing">{{report.ClaimNumber}}</span>
        </div>
        <div class="report-details__data">
          <h3>Policy Number:</h3>
          <input type="text" class="form__input" v-if="isEditing" v-model="updatedReport.PolicyNumber" />
          <span v-if="!isEditing">{{report.PolicyNumber}}</span>
        </div>
        <div class="report-details__data">
          <h3>Adjuster Name:</h3>
          <input type="text" class="form__input capitalize" v-if="isEditing" v-uppercase v-model="updatedReport.adjusterName" />
          <span v-if="!isEditing">{{report.adjusterName}}</span>
        </div>
        <div class="report-details__data">
          <h3>Adjuster Phone:</h3>
          <input type="text" class="form__input" v-if="isEditing" :value="updatedReport.adjusterPhone" v-imask="phoneMask" 
            @complete="updatedReport.adjusterPhone = $event.detail.value" />
          <span v-if="!isEditing">{{report.adjusterPhone}}</span>
        </div>
        <div class="report-details__data">
          <h3>Adjuster Email:</h3>
          <input type="text" class="form__input" v-if="isEditing" v-model="updatedReport.adjusterEmail" />
          <span v-if="!isEditing">{{report.adjusterEmail}}</span>
        </div>
      </div>
      <div class="report-details__section">
        <div class="report-details__checklist">
          <h3>Develop Initial Project Work Plan & Communicate to Headquarters:</h3>
          <ul>
            <li v-for="item in report.Steps" :key="item.id">{{item}}</li>
          </ul>
        </div>
      </div>
      <div class="report-details__section">
        <div class="report-details__checklist">
          <h3>Evaluation Logs</h3>
          <ul v-if="!isEditing">
            <li v-for="(evalLog, i) in report.evaluationLogs" :key="i">
              <label class="form__label">{{ evalLog && evalLog.label ? evalLog.label : null}}</label>
              <span>{{ evalLog && evalLog.value ? evalLog.value : null}}</span>
            </li>
          </ul>
          <ul v-else>
            <li v-for="(evalLog, i) in evaltimes" :key="`copy-${i}`">
              <label class="form__label">{{ evalLog && evalLog.label ? evalLog.label : null}}</label>
              <imask-input v-if="isEditing" :value="evalLog.value" :lazy="false" :mask="timeMask.mask" 
                :blocks="timeMask.blocks" class="form__input" @complete="updateEvalTime(evaltimes[i].label, $event, i)"  />
            </li>
          </ul>
        </div>
        <div class="report-details__checklist">
          <h3>Document Verification</h3>
          <ul>
            <li v-for="(docVerify, i) in report.documentVerification" :key="`doc-${i}`">
              {{docVerify}}
            </li>
          </ul>
        </div>
      </div>
      <div class="report-details__section">
        <div class="report-details__section--name-group">
          <h3>Customer:</h3>
          <span v-if="!isEditing" v-uppercase>{{report.cusFirstName}} {{report.cusLastName}}</span>
          <div v-if="isEditing" class="form__input-group--name-group">
            <input type="text" v-model="updatedReport.cusFirstName" class="form__input capitalize" placeholder="First" />
            <input type="text" v-model="updatedReport.cusLastName" class="form__input capitalize" placeholder="Last" />
          </div>
        </div>
        <div class="report-details__data">
          <h3>Customer Signiture:</h3>
          <div v-if="signiture !== ''">
          <div class="report-details__data--cusSig" :style="'background-image:url('+signiture+')'"></div>
          </div>
        </div>
        <div class="report-details__data">
          <h3>Sign Date:</h3>
          <span>{{report.cusSignDate}}</span>
        </div>
        <div class="report-details__data">
          <h3>Team Member Signiture:</h3>
          <div v-if="report.teamMemberSig">
            <div class="report-details__data--cusSig" :style="'background-image:url('+$store.state.users.signature+')'"></div>
          </div>
          <div v-else>N/A</div>
        </div>
        <div class="report-details__data">
          <h3>Sign Time:</h3>
          <span>{{report.cusSignTime}}</span>
        </div>
      </div>
    </div>
    <LazyUiStorageImages class="report-details__section--pictures" :jobid="report.JobId" :subPath="group.name" path="rapid-response" v-for="(group, j) in imageFolders" :key="`group-${j}`">
      <template v-slot:header>
        <h3>{{group.name}}</h3>
      </template>
    </LazyUiStorageImages>
  </section>
</template>
<script>
import {mapGetters, mapActions, mapState} from 'vuex';
import genericFuncs from '@/composable/utilityFunctions'
import {fetchReportImages} from '@/composable/reports'
import { timeMask } from "@/data/masks"
  export default {
    name: 'ResponseReportDetails',
    props: ['rep', 'notPdf', 'company', 'reportName'],
    data: (vm) => ({
      message: '',
      stepsArrLength: '',
      isEditing: false,
      updatedReport: {
        ContactName: {
          first: '',
          last: ''
        },
        PropertyOwner: {
          first: '',
          last: ''
        },
        JobId: "",
        location: {
          address: '',
          cityStateZip: '',
          city: ''
        },
        phoneNumber: '',
        emailAddress: '',
        ClaimNumber: '',
        adjusterName: '',
        adjusterEmail: '',
        adjusterPhone: '',
        evaluationLogs: [
          {label: "Team Arrival at Property", value:""},
          {label: "Evaluation Report Start Time", value:""},
          {label: "Evaluation Report End Time", value:""},
          {label: "Time of Contract Signing", value:""}
        ],
        InsuranceCompany: '',
        PolicyNumber: '',
        cusFirstName: '',
        cusLastName: ''
      },
      images: [],
      imageFolders: [],
      timeMask: timeMask,
      repData: {},
      phoneMask: {
        mask: '(000) 000-0000',
        lazy:false
      },
      arrivalImages: []
    }),
    computed: {
      loading() {
        if (Object.keys(this.rep).length === 0) {
          return false
        } else {
          return true
        }
      },
      signiture() {
        var sig = this.rep && this.rep.customerSig ? this.rep.customerSig : null;
        if (sig !== null) {
          return this.rep.customerSig
        }
        return ""
      },
      ...mapGetters({
        report: "reports/getReport"
      }),
      ...mapState({
        evaltimes: state => state.reports.report.evaluationLogs
      }),
      parseDate() {
        const [month, day, year] = this.rep.DateOfEvaluation.split('-')
        return `${month.padStart(2, '0')}-${day.padStart(2, '0')}-${year}`
      }
    },
    methods: {
      ...mapActions({
        fetchReport: 'reports/fetchReport'
      }),
      updateFormField(field, e) {
        this.$store.commit('reports/updateReportField', {field, value: e})
      },
      updateEvalTime(label, e, index) {
        this.$store.commit('reports/updateEvalTime', {label, value: e, index: index})
      },
      convertTime(time) {
        const realTime = time.substring(time.indexOf(" "), time.length)
        return genericFuncs().timeConverter(realTime, "12hour")
      },
      startEditing() {
        this.isEditing = !this.isEditing;
        const address = this.report && this.report.location ? this.report.location.address : null
        const citystatezip = this.report && this.report.location ? this.report.location.cityStateZip : null
        this.updatedReport.ContactName = {
          first: this.report.ContactName.first,
          last: this.report.ContactName.last
        }
        this.updatedReport.PropertyOwner = {
          first: this.report.PropertyOwner.first,
          last: this.report.PropertyOwner.last
        }
        this.updatedReport.JobId = this.report.JobId
        this.updatedReport.location.address = address
        this.updatedReport.location.cityStateZip = citystatezip
        this.updatedReport.emailAddress = this.report.emailAddress
        this.updatedReport.phoneNumber = this.report.phoneNumber
        this.updatedReport.InsuranceCompany = this.report.InsuranceCompany
        this.updatedReport.ClaimNumber = this.report.ClaimNumber
        this.updatedReport.cusFirstName = this.report.cusFirstName
        this.updatedReport.cusLastName = this.report.cusLastName
        //this.updatedProject.Location.city = this.report.Location.city
      },
      updateReport() {
        const timeLogs = [
          {label: 'Team Arrival at Property', value: `${this.parseDate} ${genericFuncs().timeConverter(this.evaltimes[0].value,"24hour")}:00`}, 
          {label: 'Evaluation Report Start Time', value: `${this.parseDate} ${genericFuncs().timeConverter(this.evaltimes[1].value,"24hour")}:00`}, 
          {label: 'Evaluation Report End Time', value: `${this.parseDate} ${genericFuncs().timeConverter(this.evaltimes[2].value,"24hour")}:00`}, 
          {label: 'Time of Contract Signing', value: `${this.parseDate} ${genericFuncs().timeConverter(this.evaltimes[3].value,"24hour")}:00`}
        ]
        this.updatedReport.evaluationLogs = timeLogs
        /* genericFuncs().updateReport(this.report.JobId, this.report.ReportType, this.updatedReport).then((result) => {
          this.message = result.data.message
          setTimeout(() => {
            this.message = ''
          }, 7000)
          this.isEditing = false
          this.fetchReport({authUser: this.$auth.user, path: `${this.report.ReportType}/${this.report.JobId}`})
        }) */
      },
      acceptNumber() {
        var x = this.updatedReport.phoneNumber.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/)
        this.updatedReport.phoneNumber = !x[2] ?
          x[1] :
          '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '')
      },
      formatTime(timeReturned) {
        if (!timeReturned) return null
        const pieces = timeReturned.split(':')
        let hours
        let minutes
        if (pieces.length === 2) {
          hours = parseInt(pieces[0], 10)
          minutes = parseInt(pieces[1], 10)
        }
        const newFormat = hours >= 12 ? 'PM' : 'AM'
        hours = hours % 12
        // To display "0" as "12"
        hours = hours || 12
        minutes = minutes < 10 ? '0' + minutes : minutes
        return `${hours}:${minutes} ${newFormat}`
      },
      getFolders(jobid, folder, subfolder, delimiter) {
        return new Promise((resolve, reject) => {
          this.$gcs.$get(`/list/${jobid}`, {
              params: {
                  folder: folder,
                  subfolder: folder + "/" + subfolder,
                  delimiter: delimiter,
                  bucket: "employee"
              }
          }).then((res) => {
              resolve(res)
          })
        })
      }
    },
    mounted() {
      if (!this.notPdf) {
        this.getFolders(this.report.JobId, "rapid-response", "", "/").then((result) => {
          this.imageFolders = result.folders
        })
      }
      fetchReportImages(this.report.JobId, "", "", "/").then((result) => {
        this.arrivalImages = result.images.filter(obj => !obj.name.includes('id-photo'))
      })
      this.repData = this.rep
    },
  }
</script>
<style lang="scss">
#moisture-map-image {
  position:absolute;
  top:0;
  left:0;
  object-fit:contain;
}
.form__label {
  font-weight: bold;
}
.map-wrapper {
    position:relative;
    width:660px;
    margin:auto;
    /* @include respond(tabletLarge) {
      max-width:768px;
    } */
    
    &__canvas {
      position:absolute;
      width:100%;
      height:100%;
      top:0;
      height:0;
      
    }
    &__map {
      border-left:1px solid $color-black;
      border-right:1px solid $color-black;
      height:100%;
      
      &--row {
        height:19px;
        border-top:1px solid $color-black;
        display:flex;
        flex-direction:row;
        
        &:not(:first-child) {
          border-top:0px solid $color-black;
          border-bottom:0;
        }
      }
      &--col {
        flex:1 0 auto;
        border:1px solid $color-black;
        height:100%;
      }
    }
  }
  .report-details {
    &__section {
      &--name-group {
        display:flex;
        grid-column:1/2 span;
        column-gap:20px;
        row-gap:20px;
        flex-wrap:wrap;
      }
    }
  }
</style>