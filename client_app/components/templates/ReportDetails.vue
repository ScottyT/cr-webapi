<!-- THIS IS USED FOR DISPATCH REPORT -->
<template lang="html">
    <section class="pdf-content" slot="pdf-content">
      
        <h1 v-if="message">{{message}}</h1>
        <h2 class="text-center">{{reportName}}</h2>
        <v-btn v-if="notPdf" @click="startEditing" dark>{{ isEditing ? "Editing" : "Edit"}}</v-btn>
        <v-btn v-show="isEditing" @click="updateReport" dark>Update</v-btn>
        <div class="report-details__section">
          <div class="report-details__data">
            <h3>Team Lead ID #:</h3>
            <span>{{repData.id}}</span>
          </div>
          <div class="report-details__data">
            <h3>Job ID:</h3>
            <span>{{repData.JobId}}</span>
          </div>
          <div class="report-details__data">
            <h3>Date of Call:</h3>
            <span>{{repData.dateFormatted}}</span>
          </div>
          <div class="report-details__data">
            <h3>Time of call:</h3>
            <span>{{repData.timeFormatted}}</span>
          </div>
        </div>
        <div class="report-details__section report-details__section--contact-group">
          <div class="report-details__section--name-group">
            <div class="report-details__data-label">Caller Name:</div>
            <span v-if="!isEditing" class="report-details__data-field" v-capital>{{repData && repData.callerName ? repData.callerName.first + ' ' + repData.callerName.last : null}}</span>
            <div v-if="isEditing" class="form__input-group--name-group">
              <input type="text" class="form__input" v-model="updatedReport.callerName.first" placeholder="First" />
              <input type="text" class="form__input" v-model="updatedReport.callerName.last" placeholder="Last" />
            </div>
            <div class="report-details__data-label">Arrival Contact Name:</div>
            <span v-if="!isEditing" class="report-details__data-field" v-capital>{{repData && repData.ArrivalContactName ? 
              repData.ArrivalContactName.first + ' ' + repData.ArrivalContactName.last : null}}</span>
            <div class="form__input-group--name-group" v-if="isEditing">
              <input type="text" class="form__input" v-model="updatedReport.ArrivalContactName.first" placeholder="First" />
              <input type="text" class="form__input" v-model="updatedReport.ArrivalContactName.last" placeholder="Last" />
            </div>
          </div>
          <div class="report-details__data-label">Address:</div>
          <input class="form__input" v-if="isEditing" v-model="updatedReport.location.address" />
          <span v-if="!isEditing"
            class="report-details__data-field">{{repData && repData.location ? repData.location.address : null}}</span>

          <div class="report-details__data-label">City, State, Zip:</div>
          <input class="form__input" v-if="isEditing" v-model="updatedReport.location.cityStateZip" />
          <span v-if="!isEditing"
            class="report-details__data-field">{{repData && repData.location ? repData.location.cityStateZip : null}}</span>

          <div class="report-details__data-label">Phone Number:</div>
          <input class="form__input" v-if="isEditing" v-model="updatedReport.phoneNumber" @input="acceptNumber" />
          <span v-if="!isEditing" class="report-details__data-field">{{repData.phoneNumber}}</span>

          <div class="report-details__data-label">Email Address:</div>
          <input class="form__input" v-if="isEditing" v-model="updatedReport.emailAddress" />
          <span v-if="!isEditing" class="report-details__data-field">{{repData.emailAddress}}</span>
          <div class="report-details__checklist">
            <div class="report-details__data-label">Property Checklist</div>
            <ul>
              <li v-for="item in repData.propertyChkList" :key="item.id">{{item}}</li>
            </ul>
          </div>
        </div>

        <div class="pdf-item report-details__section">
          <div class="report-details__data" v-for="(item, i) in repData.intrusion" :key="`intrusion-${i}`">
            <label class="form__label">{{item.label}}</label>
            <span>{{item.value}}</span>
          </div>
        </div>
        <div class="pdf-item report-details__section">
          <div class="report-details__data">
            <h3>Appointment time set for:</h3>
            <span>{{repData.appointmentTime}}</span>

          </div>
          <div class="report-details__data">
            <h3>Date of Appointment:</h3>
            <span>{{repData.appointmentDate}}</span>
          </div>
          <div class="report-details__data">
            <h3>15-Minute ETA Verification Call Time:</h3>
            <span v-if="!isEditing">{{repData.callTimeUpdate}}</span>
            <span v-if="isEditing">
              <v-dialog ref="calltimeupdate" v-model="callTimeModal" :return-value.sync="callTime" persistent
                width="400px">
                <template v-slot:activator="{ on, attrs }">
                  <input id="updateCallTime" v-model="updatedReport.callTimeUpdate" class="form__input" readonly v-bind="attrs"
                    v-on="on" />
                  <span class="button" @click="callTime = ''">clear</span>
                </template>
                <v-time-picker v-if="callTimeModal" v-model="callTime" full-width format="ampm">
                  <v-spacer></v-spacer>
                  <v-btn text color="#000" @click="callTimeModal = false">Cancel</v-btn>
                  <v-btn text color="#000" @click="$refs.calltimeupdate.save(callTime)">OK</v-btn>
                </v-time-picker>
              </v-dialog>
            </span>
          </div>
          <div class="report-details__data">
            <h3>Text ETA Verification Time:</h3>
            <span v-if="!isEditing">{{repData.textTimeUpdate}}</span>
            <span v-if="isEditing">
              <v-dialog ref="texttimeupdate" v-model="textTimeModal" :return-value.sync="textEtaTime" persistent width="400px">
                <template v-slot:activator="{ on, attrs }">
                  <input id="90minupdate" v-model="updatedReport.textTimeUpdate" class="form__input" readonly v-bind="attrs" v-on="on" />
                  <span class="button" @click="textEtaTime = ''">clear</span>
                </template>
                <v-time-picker v-if="textTimeModal" v-model="textEtaTime" full-width format="ampm">
                  <v-spacer></v-spacer>
                  <v-btn text color="#000" @click="textTimeModal = false">Cancel</v-btn>
                  <v-btn text color="#000" @click="$refs.texttimeupdate.save(textEtaTime)">OK</v-btn>
                </v-time-picker>
              </v-dialog>
            </span>
          </div>
        </div>
        <div class="report-details__section">
          <div class="report-details__data form__input-group--long">
            <h3>Summary of Call:</h3>
            <textarea class="form__input form__input--textarea" v-if="isEditing" v-model="updatedReport.summary"></textarea>
            <span v-if="!isEditing">{{repData.summary}}</span>
          </div>
          <div class="report-details__data">
            <h3>Created at</h3>
            <div>{{newCreatedAt}}</div>
          </div>
          <div class="report-details__data" v-if="repData.updatedAt">
            <h3>Updated at</h3>
            <div>{{new Date(repData.updatedAt)}}</div>
          </div>
        </div>
        <UiStorageImages class="report-details__section--pictures" v-for="(group, j) in imageFolders" :key="`group-${j}`" :imageArr="images" :jobid="report.JobId" :subPath="group.name" 
          path="dispatch">
          <template v-slot:header>
            <h3>{{group.name}}</h3>
          </template>
        </UiStorageImages>
    
    </section>
</template>
<script>
import {mapGetters, mapActions} from 'vuex';
import genericFuncs from '@/composable/utilityFunctions'
  export default {
    name: 'ReportDetails',
    props: ['report', 'notPdf', 'reportName'],
    data: (vm) => ({
      message: '',
      updatedReport: {
        JobId: "",
        location: {
          address: '',
          cityStateZip: '',
          city: ''
        },
        phoneNumber: '',
        emailAddress: '',
        summary: '',
        callTimeUpdate: vm.formatTime(new Date().toTimeString().substr(0, 5)),
        textTimeUpdate: vm.formatTime(new Date().toTimeString().substr(0, 5)),
        callerName: {
          first: '',
          last: ''
        },
        ArrivalContactName: {
          first: '',
          last: ''
        }
      },
      callTimeModal: false,
      textTimeModal: false,
      callTime: null,
      textEtaTime: null,
      isEditing: false,
      images: [],
      imageFolders: []
    }),
    watch: {
      callTime(val) {
        this.updatedReport.callTimeUpdate = this.formatTime(val)
      },
      textEtaTime(val) {
        this.updatedReport.textTimeUpdate = this.formatTime(val)
      }
    },
    computed: {
      newCreatedAt() {
        const timestamp = new Date(this.report.createdAt)
        return timestamp
      },
      ...mapGetters({
        repData: 'reports/getReport'
      })
    },
    methods: {
      ...mapActions({
        fetchReport: 'reports/fetchReport',
      }),
      onAccept(e){ 
        console.log(e)
      },
      startEditing() {
        this.isEditing = !this.isEditing;
        const address = this.report && this.report.location ? this.report.location.address : null
        const citystatezip = this.report && this.report.location ? this.report.location.cityStateZip : null
        this.updatedReport.callerName = {
          first: this.report.callerName.first,
          last: this.report.callerName.last
        }
        this.updatedReport.ArrivalContactName = {
          first: this.report.ArrivalContactName.first,
          last: this.report.ArrivalContactName.last
        }
        this.updatedReport.JobId = this.report.JobId
        this.updatedReport.location.address = address
        this.updatedReport.location.cityStateZip = citystatezip
        this.updatedReport.emailAddress = this.report.emailAddress
        this.updatedReport.phoneNumber = this.report.phoneNumber
        this.updatedReport.callTimeUpdate = this.report.callTimeUpdate === "N/A" ? 
          this.formatTime(new Date().toTimeString().substr(0, 5)) : this.report.callTimeUpdate
        this.updatedReport.textTimeUpdate = this.report.textTimeUpdate === "N/A" ? 
          this.formatTime(new Date().toTimeString().substr(0, 5)) : this.report.textTimeUpdate
        this.updatedReport.summary = this.report.summary
      },
      updateReport() {
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
                  bucket: "default"
              }
          }).then((res) => {
              resolve(res)
          })
        })
      }
    },
    mounted() {
      this.$nextTick(() => {
        this.getFolders(this.report.JobId, "dispatch", "", "/").then((result) => {
          this.imageFolders = result.folders
        })
      })
    }
  }
</script>
<style lang="scss" scoped>
.form__label {
  font-weight:bold;
}
</style>