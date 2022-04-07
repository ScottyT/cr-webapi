<template>
  <div class="form-wrapper form-wrapper__rapid-form">
    <h1 class="text-center">{{company}}</h1>
    <h2 class="text-center">Rapid Response Evaluation Report</h2>
    <ValidationObserver ref="form" v-slot="{ errors }">
      <v-dialog width="600px" v-model="errorDialog">
        <div class="modal__error">
          <div v-for="(error, i) in errors" :key="`error-${i}`">
            <h3 class="form__input--error">{{ error[0] }}</h3>
          </div>
        </div>
      </v-dialog>
      <h2 class="alert alert--success">{{ successMessage }}</h2>
      <form v-if="!submitted" class="form" enctype="multipart/form-data" @submit.prevent="submitForm">
        <div class="form__form-group">
          <ValidationProvider v-slot="{ errors }" vid="JobId" name="Job ID" rules="required" class="form__input-group form__input-group--normal">
            <label for="jobId" class="form__label">Job ID</label>
            <input v-model="jobId" id="jobId" name="jobId" class="form__input" @keydown.space.prevent type="text" />
            <span class="form__input--error">{{ errors[0] }}</span>
          </ValidationProvider>
          <div class="form__input-group form__input-group--normal">
            <label for="dateOfLoss" class="form__label">Date of Loss</label>
            <v-dialog ref="dolDialog" v-model="dolModal" :return-value.sync="dateOfLoss" persistent max-width="290px">
              <template v-slot:activator="{ on, attrs }">
                <input id="dateOfLoss" v-model="dolFormatted" v-bind="attrs" class="form__input" v-on="on" readonly />
              </template>
              <v-date-picker v-if="dolModal" v-model="dateOfLoss" scrollable>
                <v-spacer></v-spacer>
                <v-btn text color="#fff" @click="dolModal = false">Cancel</v-btn>
                <v-btn text color="#fff" @click="$refs.dolDialog.save(dateOfLoss)">OK</v-btn>
              </v-date-picker>
            </v-dialog>
          </div>
          <div class="form__input-group form__input-group--normal">
            <label for="timeOfDispatch" class="form__label">Time of Dispatch</label>
            <v-dialog ref="todDialog" v-model="todModal" :return-value.sync="timeOfDispatch" persistent max-width="290px">
              <template v-slot:activator="{ on, attrs }">
                <input id="timeOfDispatch" v-model="timeOfDispatchFormatted" v-bind="attrs" class="form__input" v-on="on" readonly />
              </template>
              <v-time-picker v-model="timeOfDispatch" scrollable full-width format="ampm" light>
                <v-spacer></v-spacer>
                <v-btn text color="#fff" @click="todModal = false">Cancel</v-btn>
                <v-btn text color="#fff" @click="$refs.todDialog.save(timeOfDispatch)">OK</v-btn>
              </v-time-picker>
            </v-dialog>
          </div>
          <div class="form__input-group form__input-group--normal">
            <label for="dateOfEval" class="form__label">Date of Evaluation</label>
            <v-dialog ref="doeDialog" v-model="doeModal" :return-value.sync="dateOfEval" persistent width="400px">
              <template v-slot:activator="{ on, attrs }">
                <input id="dateOfEval" v-model="doeFormatted" v-bind="attrs" class="form__input" v-on="on" readonly />
              </template>
              <v-date-picker v-model="dateOfEval" scrollable>
                <v-spacer></v-spacer>
                <v-btn text color="#fff" @click="doeModal = false">Cancel</v-btn>
                <v-btn text color="#fff" @click="$refs.doeDialog.save(dateOfEval)">OK</v-btn>
              </v-date-picker>
            </v-dialog>
          </div>
          <ValidationProvider class="form__input-group form__input-group--very-long">
            <label for="location" class="form__label">Address</label>
            <input type="hidden" v-model="location.address" />
            <UiGeoCoder @sendAddress="setLocation($event)" :mapView="false" geocoderid="geocoder" geocoderef="geocoderProperty" />
          </ValidationProvider>
        </div>
        <div class="form__form-group form__form-group--info-box">
          <div class="form__form-group form__form-group--images-upload-section form__section">
            <ValidationProvider ref="idprovider" v-slot="{ errors }" name="Photo ID" class="upload-group upload-group--sm">
              <label class="form__label">Photo ID</label>
              <input type="hidden" v-model="idImage" />
              <UiFilesUpload singleImage path="" subDir="" :changeImageName="`id-photo-${jobId}`" :rootPath="jobId" @sendPreviews="idImage = $event" />
              <span class="form__input--error">{{ errors[0] }}</span>
            </ValidationProvider>
          </div>
          <div class="form__form-group--left-side form__section">
            <label for="contactName" class="form__label">Contact Name</label>
            <div class="form__input-group--name-group">
              <ValidationProvider rules="required" name="Contact first name" v-slot="{errors, ariaMsg}">
                <input v-model="contactName.first" placeholder="First" type="text" class="form__input capitalize" />
                <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
              </ValidationProvider>
              <ValidationProvider rules="required" name="Contact last name" v-slot="{errors, ariaMsg}">
                <input type="text" class="form__input capitalize" v-model="contactName.last" placeholder="Last" />
                <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
              </ValidationProvider>
            </div>
            <label for="PropertyOwner" class="form__label">Property Owner</label>
            <div class="form__input-group--name-group">
              <ValidationProvider rules="required" name="Property owner first name" v-slot="{errors, ariaMsg}">
                <input v-model="propertyOwner.first" placeholder="First" type="text" class="form__input capitalize" />
                <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
              </ValidationProvider>
              <ValidationProvider rules="required" name="Property owner last name" v-slot="{errors, ariaMsg}">
                <input type="text" class="form__input capitalize" v-model="propertyOwner.last" placeholder="Last" />
                <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
              </ValidationProvider>
            </div>
            <label class="form__label">City, State, Zip</label>
            <input v-model="location.cityStateZip" name="cityStateZip" type="text" class="form__input form__input--long" />
            <label class="form__label">Address</label>
            <input
              v-model="location.address"
              name="Address"
              type="text"
              class="form__input form__input--long"
            />
            <!-- <label class="form__label">City, State, Zip</label>
            <input
              v-model="location.cityStateZip"
              name="cityStateZip"
              type="text"
              class="form__input form__input--long"
              readonly
            /> -->
            <label class="form__label" for="phone">Phone Number</label>
            <input id="phone" v-model="phoneNumber" name="Phone" class="form__input form__input--short" type="phone"
              @input="acceptNumber" />
            <ValidationProvider v-slot="{ errors }" name="Email" rules="required">
              <label for="email" class="form__label">Email Address</label>
              <input v-model="emailAddress" id="email" type="text" class="form__input" name="Email" />
              <span class="form__input--error">{{ errors[0] }}</span>
            </ValidationProvider>
          </div>
          <div class="form__form-group--right-side form__section">
            <h4>Inital Response, Inspection, and Preliminary Determination</h4>
            <div v-show="jobId" ref="initResponse">
              <ul class="form__checkbox-wrapper--long">
                <div class="form__input--checkboxes" v-for="item in picturesCheck" :key="item.id">
                  <input type="checkbox" :id="item.id" v-model="selectedPictures" :value="item.text" />
                  <label :for="item.id">{{item.text}}</label>
                  <UiFilesUpload v-if="selectedPictures.includes(item.text)" path="" subDir="" singleImage :changeImageName="`${item.text}__${jobId}`" 
                    :rootPath="jobId" @sendDownloadUrl="item.image = $event" autoUpload fieldName="singleImage" />
                </div>
              </ul>
              <UiFilesUpload :singleImage="false" :isStorage="false" path="rapid-response" subDir="" :rootPath="jobId" buttonName="Add job images" delimiter="/" />
            </div>
            <h3 v-show="jobId === ''">Please put in a Job ID in order to view these fields</h3>
          </div>
          <div class="form__section">
            <h3>Source of Water Intrusion</h3>
            <ul class="form__checkbox-wrapper">
              <div class="form__input-wrapper--checkboxes" v-for="(type, i) in sourceOfIntrustion" :key="`loss-${i+1}`">
                <input type="checkbox" :id="`loss-${i+1}`" v-model="selectedTypes" :value="type.text" />
                <label :for="`loss-${i+1}`">{{type.text}}</label>
              </div>
            </ul>
          </div>
          <div class="form__form-group--block form__section">
            <div class="form__input-wrapper">
              <div class="form__input--input-group">
                <label for="dateOfIntrusion" class="form__label">Date of Intrusion</label>
                <v-dialog ref="dateIntrusionDialog" v-model="intrusionLogsDialog.dateIntrusion" persistent :return-value.sync="dateIntrusion" transition="scale-transition" max-width="320px">
                  <template v-slot:activator="{ on, attrs }">
                    <input type="text" id="dateOfIntrusion" v-model="dateIntrusionFormatted" class="form__input" v-bind="attrs" readonly v-on="on" />
                    <span class="button" @click="dateIntrusion = ''">clear</span>
                  </template>
                  <v-date-picker v-if="intrusionLogsDialog.dateIntrusion" v-model="dateIntrusion" scrollable>
                    <v-spacer></v-spacer>
                    <v-btn text color="#fff" @click="intrusionLogsDialog.dateIntrusion = false">Cancel</v-btn>
                    <v-btn text color="#fff" @click="$refs.dateIntrusionDialog.save(dateIntrusion)">OK</v-btn>
                  </v-date-picker>
                </v-dialog>
              </div>
              <div class="form__input--input-group">
                <label for="timeIntrusion" class="form__label">Time of Intrusion</label>
                <v-dialog ref="timeIntrusion" v-model="intrusionLogsDialog.timeIntrusion" persistent
                  :return-value.sync="timeIntrusion" transition="scale-transition" max-width="290px" light>
                  <template v-slot:activator="{ on, attrs }">
                    <input type="text" id="timeIntrusion" v-model="timeIntrusionFormatted" class="form__input" readonly v-bind="attrs" v-on="on" />
                    <span class="button" @click="timeIntrusion = ''">clear</span>
                  </template>
                  <v-time-picker v-if="intrusionLogsDialog.timeIntrusion" v-model="timeIntrusion" format="ampm" full-width>
                    <v-spacer></v-spacer>
                    <v-btn text color="#fff" @click="intrusionLogsDialog.timeIntrusion = false">Cancel</v-btn>
                    <v-btn text color="#fff" @click="$refs.timeIntrusion.save(timeIntrusion)">OK</v-btn>
                  </v-time-picker>
                </v-dialog>
              </div>
              <div class="form__input--input-group" v-for="intrusion in intrusionSection" :key="intrusion.label">
                <label :for="intrusion.label" class="form__label">{{intrusion.label}}</label>
                <input :type="intrusion.type" :id="intrusion.label" v-model="intrusion.value" class="form__input" />
              </div>            
            </div>
            <LazyUiSignaturePadModal v-model="cusInitial1" :sigData="initialData" sigRef="initial1Pad" inputId="initial1" name="Initial" width="200px" height="70px" 
              :dialog="false" initial sigType="customer" />
          </div>
          <div class="form__form-group--grid">
            <div class="form__section">
              <h3>Preliminary Determination</h3>
              <div class="form__checkbox-wrapper">
                <div class="form__input-wrapper--checkboxes" v-for="item in preliminaryDetermination" :key="item">
                  <input type="checkbox" :id="item" v-model="selectedPreliminary" :value="item" />
                  <label :for="item">{{item}}</label>
                </div>
              </div>
              <LazyUiSignaturePadModal v-model="cusInitial2" :sigData="initialData" sigRef="initial2Pad" inputId="initial2" name="Initial" width="200px" height="70px"
                :dialog="false" initial sigType="customer" />             
            </div>
            <div class="form__section">
              <h3>Inital Moisture Inspection</h3>
              <div class="form__checkbox-wrapper">
                <div class="form__input-wrapper--checkboxes" v-for="item in moistureInspection" :key="item.label">
                  <input type="checkbox" :id="item.label" v-model="selectedInspection" :value="item" />
                  <label :for="item.label">{{item.label}}</label>
                  <span style="width:71px;" v-if="item.hasOwnProperty('dryStandard') && selectedInspection.findIndex(el => el.label === 'Establish a Dry Standard') >= 0">
                    <imask-input :mask="dryStandardMask.mask" :unmask="true" :lazy="dryStandardMask.lazy" :placeholderChar="'_'"
                    v-model="selectedInspection.find(el => el.hasOwnProperty('dryStandard')).dryStandard" />
                  </span>
                </div>
              </div>
              <LazyUiSignaturePadModal v-model="cusInitial3" :sigData="initialData" sigRef="initial3Pad" inputId="initial3" name="Initial" width="200px" height="70px" 
                :dialog="false" initial sigType="customer" />
            </div>
          </div>
          <div class="form__form-group form__form-group--info-box form__section">
            <label class="form__label">Initial Moisture Map</label>
            <p>An initial moisture inspection should be conducted to identify the full extent of water intrusion, including the
                identification of affected assemblies, building materials, and the edge of water mitigation. Normally, this process
                begins at the source of the water intrusion. The initial inspection process should continue in all directions from the
                source of water intrusion until the restorer identifies and documents the extent of migration. The extent of the
                moisture migration should be documented using one or more appropriate methods including at a minimum a
                moisture map. (i.e., a diagram of the structure indicating the areas affected by migrating water). IICRC S500 Pg 179</p>
            <div class="map-wrapper">
              <div class="map-wrapper__map">
                <div class="map-wrapper__map--row" v-for="n in 37" :key="n">
                  <div class="map-wrapper__map--col" v-for="n in 20" :key="n"></div>
                </div>
              </div>
              <VueSignaturePad width="100%" height="703px" ref="map" class="map-wrapper__canvas" :options="{ onBegin, minWidth: 1.5, maxWidth:3.5, penColor: penColor }" />
              <div class="pt-3 pb-3">
                <button type="button" class="button--normal" @click="saveMap">{{ moistureMap.data !== '' ? 'Saved' : 'Save' }}</button>
                <button type="button" class="button--normal" @click="undoMap">Undo</button>
                <div class="map-wrapper__pen">
                  <span class="map-wrapper__pen-color" aria-label="black" @click="penColor = '#000'" style="background:#000"></span>
                  <span class="map-wrapper__pen-color" aria-label="shallow-water" @click="penColor = '#6c8ce6'" style="background:#6c8ce6"></span>
                  <span class="map-wrapper__pen-color" aria-label="normal-water" @click="penColor = '#3047f1'" style="background:#3047f1"></span>
                  <span class="map-wrapper__pen-color" aria-label="deep-water" @click="penColor = '#0a2177'" style="background:#0a2177"></span>
                </div>
              </div>
            </div>
            <LazyUiSignaturePadModal v-model="cusInitial4" :sigData="initialData" sigRef="initial4Pad" inputId="initial4" name="Initial" width="200px" height="70px" initial 
              :dialog="false" sigType="customer" />
          </div>
          <div class="form__form-group form__section">
            <h3>Pre-Restoration Evaluation</h3>
            <div class="d-flex flex-wrap">
              <span class="form__input-group form__input-group--long">
                <label for="emergencyRes" class="form__label">Emergency Response Actions Identified</label>
                <input id="emergencyRes" type="text" class="form__input" v-model="preRestoreEval.emergencyResAct" />
              </span>
              <span class="form__input-group form__input-group--long">
                <label for="buildingMat" class="form__label">Building Material Restorability</label>
                <input id="buildingMat" type="text" class="form__input" v-model="preRestoreEval.buildingMatRestore" />
              </span>
              <span class="form__input-group form__input-group--long">
                <label for="contentEval" class="form__label">Content Evaluation</label>
                <input id="contentEval" type="text" class="form__input" v-model="preRestoreEval.contentEval" />
              </span>
              <span class="form__input-group form__input-group--long">
                <label for="hvacEval" class="form__label">HVAC Evaluation</label>
                <input id="hvacEval" type="text" class="form__input" v-model="preRestoreEval.hvacEval" />
              </span>
              <span class="form__input-group form__input-group--long">
                <label for="substructure" class="form__label">Substructure & Unfinished Spaces</label>
                <input id="substructure" type="text" class="form__input" v-model="preRestoreEval.substructure" />
              </span>
            </div>
          </div>
          <div class="form__section">
            <h3>Develop Initial Project Work Plan & Communicate to Headquarters</h3>
            <div class="form__checkbox-wrapper--long form__checkbox-wrapper">
              <div class="form__input-wrapper--checkboxes" v-for="(step, i) in steps" :key="`steps-${i+1}`">
                <input type="checkbox" :id="`steps-${i+1}`" v-model="selectedSteps" :value="step.text" />
                <label :for="`steps-${i+1}`">{{step.text}}</label>
              </div>
            </div>
          </div>
          <div class="form__form-group form__section">
            <ValidationProvider v-slot="{errors}" name="Insurance" rules="required" class="form__input-group form__input-group--normal">
              <label for="insurance" class="form__label">Insurance Company</label>
              <input type="text" id="insurance" class="form__input" name="insurance" v-model="insuranceCompany" />
              <span class="form__input--error">{{ errors[0] }}</span>
            </ValidationProvider>
            <ValidationProvider v-slot="{errors}" name="claimNumber" class="form__input-group form__input-group--normal">
              <label for="claimNumber" class="form__label">Claim Number</label>
              <input type="text" id="claimNumber" class="form__input" name="claimNumber" v-model="claimNumber" />
              <span class="form__input--error">{{ errors[0] }}</span>
            </ValidationProvider>
            <ValidationProvider v-slot="{errors}" name="policyNumber" class="form__input-group form__input-group--normal">
              <label for="policyNumber" class="form__label">Policy Number</label>
              <input type="text" id="policyNumber" class="form__input" name="policyNumber" v-model="policyNumber" />
              <span class="form__input--error">{{ errors[0] }}</span>
            </ValidationProvider>
            <span class="form__input-group form__input-group--normal">
              <label for="adjusterName" class="form__label">Adjuster Name</label>
              <input id="adjusterName" type="text" class="form__input capitalize" v-model="adjusterName" />
            </span>
            <span class="form__input-group form__input-group--normal">
              <label for="adjusterPhone" class="form__label">Adjuster Phone</label>
              <input id="adjusterPhone" :value="adjusterPhone" class="form__input" v-imask="phoneMask" @accpet="adjusterPhone = $event.detail.value"
                @complete="adjusterPhone = $event.detail.value" />
              
            </span>
            <ValidationProvider v-slot="{errors}" name="Adjuster email" rules="email" class="form__input-group form__input-group--long">
              <label for="adjusterEmail" class="form__label">Adjuster Email</label>
              <input type="email" id="adjusterEmail" class="form__input" name="policyNumber" v-model="adjusterEmail" />
              <span class="form__input--error">{{ errors[0] }}</span>
            </ValidationProvider>
          </div>
          <div class="form__form-group--block form__section">
            <h3>Evaluation Logs</h3>
            <div class="form__input-wrapper">
              <div class="form__input--input-group">
                <label for="arrivalProperty" class="form__label">Team Arrival at Property</label>
                <v-dialog ref="dialogArrival" v-model="evalLogsDialog.arrivalAtProperty" light persistent
                  :return-value.sync="arrivalAtProperty" transition="scale-transition" max-width="290px">
                  <template v-slot:activator="{ on, attrs }">
                    <input type="text" id="arrivalProperty" v-model="arrivalFormatted" class="form__input" readonly
                      v-bind="attrs" v-on="on" />
                  </template>
                  <v-time-picker v-if="evalLogsDialog.arrivalAtProperty" v-model="arrivalAtProperty" format="ampm"
                    full-width>
                    <v-spacer></v-spacer>
                    <v-btn text color="#fff" @click="evalLogsDialog.arrivalAtProperty = false">Cancel</v-btn>
                    <v-btn text color="#fff" @click="$refs.dialogArrival.save(arrivalAtProperty)">OK</v-btn>
                  </v-time-picker>
                </v-dialog>
              </div>
              <div class="form__input--input-group">
                <label for="evalStart" class="form__label">Evaluation Report Start Time</label>
                <v-dialog ref="dialogEvalStart" v-model="evalLogsDialog.evalStart" light persistent
                  :return-value.sync="evalStart" transition="scale-transition" max-width="290px">
                  <template v-slot:activator="{ on, attrs }">
                    <input type="text" id="evalStart" v-model="evalStartFormatted" class="form__input" v-bind="attrs" readonly
                      v-on="on" />
                  </template>
                  <v-time-picker v-if="evalLogsDialog.evalStart" v-model="evalStart" format="ampm" full-width>
                    <v-spacer></v-spacer>
                    <v-btn text color="#fff" @click="evalLogsDialog.evalStart = false">Cancel</v-btn>
                    <v-btn text color="#fff" @click="$refs.dialogEvalStart.save(evalStart)">OK</v-btn>
                  </v-time-picker>
                </v-dialog>
              </div>
              <div class="form__input--input-group">
                <label for="evalEnd" class="form__label">Evaluation Report End Time</label>
                <v-dialog ref="dialogEvalEnd" v-model="evalLogsDialog.evalEnd" persistent light :return-value.sync="evalEnd"
                  transition="scale-transition" max-width="290px">
                  <template v-slot:activator="{ on, attrs }">
                    <input type="text" id="evalEnd" v-model="evalEndFormatted" class="form__input" v-bind="attrs"
                      v-on="on" />
                  </template>
                  <v-time-picker v-if="evalLogsDialog.evalEnd" v-model="evalEnd" format="ampm" full-width>
                    <v-spacer></v-spacer>
                    <v-btn text color="#fff" @click="evalLogsDialog.evalEnd = false">Cancel</v-btn>
                    <v-btn text color="#fff" @click="$refs.dialogEvalEnd.save(evalEnd)">OK</v-btn>
                  </v-time-picker>
                </v-dialog>
              </div>
              <div class="form__input--input-group">
                <label for="contractSigning" class="form__label">Time of Contract Signing</label>
                <v-dialog ref="dialogContractSigning" v-model="evalLogsDialog.contractSigning" light persistent
                  :return-value.sync="contractSigning" transition="scale-transition" max-width="290px">
                  <template v-slot:activator="{ on, attrs }">
                    <input type="text" id="contractSigning" v-model="contractFormatted" class="form__input"
                      v-bind="attrs" v-on="on" />
                  </template>
                  <v-time-picker v-if="evalLogsDialog.contractSigning" v-model="contractSigning" format="ampm" full-width>
                    <v-spacer></v-spacer>
                    <v-btn text color="#fff" @click="evalLogsDialog.contractSigning = false">Cancel</v-btn>
                    <v-btn text color="#fff" @click="$refs.dialogContractSigning.save(contractSigning)">OK</v-btn>
                  </v-time-picker>
                </v-dialog>
              </div>
              <!-- <div class="form__input--input-group">
                <label for="denialOfServices" class="form__label">Time of Denial of Services</label>
                <v-dialog ref="dialogDos" v-model="evalLogsDialog.denialOfServices" persistent
                  :return-value.sync="denialOfServices" transition="scale-transition" max-width="290px">
                  <template v-slot:activator="{ on, attrs }">
                    <input type="text" id="denialOfServices" v-model="dosformatted" class="form__input" v-bind="attrs"
                      v-on="on" />
                    <span class="button" @click="denialOfServices = ''">clear</span>
                  </template>
                  <v-time-picker v-if="evalLogsDialog.denialOfServices" v-model="denialOfServices" format="ampm"
                    full-width>
                    <v-spacer></v-spacer>
                    <v-btn text color="#fff" @click="evalLogsDialog.denialOfServices = false">Cancel</v-btn>
                    <v-btn text color="#fff" @click="$refs.dialogDos.save(denialOfServices)">OK</v-btn>
                  </v-time-picker>
                </v-dialog>
              </div>
              <div class="form__input--input-group">
                <label for="departureOfProperty" class="form__label">Team Departure of Property</label>
                <v-dialog ref="dialogDepartureTime" v-model="evalLogsDialog.departureTime" persistent
                  :return-value.sync="departureTime" transition="scale-transition" max-width="290px">
                  <template v-slot:activator="{ on, attrs }">
                    <input type="text" id="departureOfProperty" v-model="departureTimeFormatted" class="form__input"
                      v-bind="attrs" v-on="on" />
                    <span class="button" @click="departureTime = ''">clear</span>
                  </template>
                  <v-time-picker v-if="evalLogsDialog.departureTime" v-model="departureTime" format="ampm" full-width>
                    <v-spacer></v-spacer>
                    <v-btn text color="#fff" @click="evalLogsDialog.departureTime = false">Cancel</v-btn>
                    <v-btn text color="#fff" @click="$refs.dialogDepartureTime.save(departureTime)">OK</v-btn>
                  </v-time-picker>
                </v-dialog>
              </div> -->
            </div>
          </div>
          <div class="form__section">
            <h3>Team Lead Document Verification</h3>
            <div class="form__input-wrapper--checkboxes" v-for="item in verificationCheckboxes" :key="`item${item.id+1}`">
              <input type="checkbox" :id="`item${item.id+1}`" v-model="selectedVerification" :value="item.text" />
              <label :for="`item${item.id+1}`">{{item.text}}</label>
            </div>
          </div>
          <div class="d-flex flex-column pt-4 pb-4">
            <label class="form__label">Customer (Print)</label>
            <div class="grid grid--three-column">
              <ValidationProvider v-slot="{ errors }" name="Customer first name" rules="required">
                <input id="firstname" placeholder="First" type="text" class="form__input capitalize" v-model="customerName.first" />
                <span class="form__input--error">{{ errors[0] }}</span>
              </ValidationProvider>
              <ValidationProvider v-slot="{ errors }" name="Customer last name" rules="required">
                <input id="lastname" placeholder="Last" type="text" class="form__input capitalize" v-model="customerName.last" />
                <span class="form__input--error">{{ errors[0] }}</span>
              </ValidationProvider>
              <LazyUiSignaturePadModal width="700px" height="219px" dialog :initial="false" sigType="customer" inputId="cusSignature" :sigData="cusSignature" 
                sigRef="cusSignaturePad" name="Customer signature" />
            </div>
          </div>
          <ValidationProvider v-slot="{ errors }" name="Customer sign date" rules="required" class="form__input-group form__input-group--normal">
            <label for="signDate" class="form__label">Customer Sign Date</label>
            <input type="hidden" v-model="cusSignDate" />
            <imask-input id="signDate" @complete="cusSignDate = $event" :lazy="false" :blocks="dateMask.blocks" :mask="dateMask.mask" :format="dateMask.format" 
                :parse="dateMask.parse" :pattern="dateMask.pattern" class="form__input" />
            <span class="form__input--error">{{ errors[0] }}</span>
          </ValidationProvider>
          <div class="form__form-group">
            <ValidationProvider v-slot="{ errors }" name="Customer sign time" rules="required" class="form__input-group form__input-group--normal">
              <label class="form__label">Sign Time</label>
              <input type="hidden" v-model="cusSignTime" />
              <imask-input v-model="cusSignTime" :lazy="false" :mask="timeMask.mask" :blocks="timeMask.blocks" class="form__input" />
              <span class="form__input--error">{{ errors[0] }}</span>
            </ValidationProvider>
            <ValidationProvider v-slot="{ errors }" name="Customer sign date" rules="required" class="form__input-group form__input-group--normal">
              <label for="signDate" class="form__label">Sign Date</label>
              <input type="hidden" v-model="teamMemberSignDate" />
              <imask-input id="teamsignDate" @complete="teamMemberSignDate = $event" :lazy="false" :blocks="dateMask.blocks" :mask="dateMask.mask" :format="dateMask.format" 
                            :parse="dateMask.parse" :pattern="dateMask.pattern" class="form__input" />
              <span class="form__input--error">{{ errors[0] }}</span>
            </ValidationProvider>
            <LazyUiSignaturePadModal v-model="empSig" width="700px" height="219px" dialog :initial="false" sigType="employee" inputId="teamMemberSig" :sigData="teamMemberSig" 
              sigRef="teamSignaturePad" name="Team member signature" />
          </div>
        </div>
        <div class="form__button-wrapper"><button type="submit" class="button form__button-wrapper--submit">{{ submitting ? 'Submitting' : 'Submit' }}</button></div>
      </form>
    </ValidationObserver>
  </div>
</template>
<script>
  import {mapGetters, mapState, mapActions} from 'vuex'
  import 'animate.css'
  import goTo from 'vuetify/es5/services/goto'
  import { timeMask, dateMask } from "@/data/masks";
  export default {
    name: 'RapidResponse',
    props: ['slice', 'company', 'abbreviation'],
    data: (vm) => ({
      errorDialog: false,
      sigDialog: false,
      uploading: false,
      successMessage: '',
      errorMessage: [],
      uploadSuccess: '',
      submitting: false,
      submitted: false,
      jobId: "",
      timeOfDispatch: new Date().toTimeString().substr(0, 5),
      timeOfDispatchFormatted: vm.formatTime(new Date().toTimeString().substr(0, 5)),
      dateOfLoss: new Date().toISOString().substring(0, 10),
      dolFormatted: vm.formatDate(new Date().toISOString().substring(0, 10)),
      dateOfEval: new Date().toISOString().substr(0, 10),
      doeFormatted: vm.formatDate(new Date().toISOString().substr(0, 10)),
      arrivalAtProperty: new Date().toTimeString().substr(0, 5),
      arrivalFormatted: vm.formatTime(new Date().toTimeString().substr(0, 5)),
      evalStart: new Date().toTimeString().substr(0, 5),
      evalStartFormatted: vm.formatTime(new Date().toTimeString().substr(0, 5)),
      evalEnd: new Date().toTimeString().substr(0, 5),
      evalEndFormatted: vm.formatTime(new Date().toTimeString().substr(0, 5)),
      contractSigning: new Date().toTimeString().substr(0, 5),
      contractFormatted: vm.formatTime(new Date().toTimeString().substr(0, 5)),
      denialOfServices: null,
      dosformatted: vm.formatTime(new Date().toTimeString().substr(0, 5)),
      departureTime: null,
      departureTimeFormatted: vm.formatTime(new Date().toTimeString().substr(0, 5)),
      evalLogsDialog: {
        arrivalAtProperty: false,
        evalStart: false,
        evalEnd: false,
        contractSigning: false,
        denialOfServices: false,
        departureTime: false
      },
      todModal: false,
      dolModal: false,
      doeModal: false,
      contactName: {
        first: '',
        last:''
      },
      propertyOwner: {
        first: '',
        last: ''
      },
      phoneNumber: '',
      emailAddress: '',
      location: {
        address: null,
        cityStateZip: null,
      },
      sourceOfIntrustion: [{
          id: 1,
          text: 'Frozen Pipes',
          checked: false
        },
        {
          id: 2,
          text: 'Sump Pump Failure',
          checked: false
        },
        {
          id: 3,
          text: 'Broken Pipe',
          checked: false
        },
        {
          id: 4,
          text: 'Roof Leak',
          checked: false
        },
        {
          id: 5,
          text: 'Sprinkler System',
          checked: false
        },
        {
          id: 6,
          text: 'Outside Spicket',
          checked: false
        },
        {
          id: 7,
          text: 'Washer Line',
          checked: false
        },
        {
          id: 8,
          text: 'Toilet Overflow',
          checked: false
        },
        {
          id: 9,
          text: 'Sink Overflow',
          checked: false
        },
        {
          id: 10,
          text: 'Tub Overflow',
          checked: false
        },
        {
          id: 11,
          text: 'Sewage Backup',
          checked: false
        },
        {
          id: 12,
          text: 'Broken Valve',
          checked: false
        },
        {
          id: 13,
          text: 'Fire',
          checked: false
        },
        {
          id: 14,
          text: 'Hail',
          checked: false
        },
        {
          id: 15,
          text: 'Mold',
          checked: false
        },
        {
          id: 16,
          text: 'Vandalism',
          checked: false
        },
        {
          id: 17,
          text: 'Water',
          checked: false
        },
        {
          id: 18,
          text: 'Other',
          checked: false
        }
      ],
      intrusionLogsDialog: {
        dateIntrusion: false,
        timeIntrusion: false
      },
      intrusionSection: [
        { id:'statusOfIntrusion', label: 'Control Status of Intrusion', value: '', type: 'text' },
        { id:'structureType', label: 'Structure Type', value: '', type: 'text' },
        { id:'use', label: 'Use', value: '', type: 'text' },
        { id:'history', label: 'History', value: '', type: 'text' },
        { id:'age', label: 'Age', value: '', type: 'text' },
        { id:'appxSqft', label: 'Approximate sqft', value: '', type: 'number' },
        { id:'numberOfRooms', label: 'Number of Rooms', value: '', type: 'number' },
        { id:'numberOfFloors', label: 'Number of Floors', value: '', type: 'text' }
      ],
      dateIntrusion: new Date().toISOString().substr(0, 10),
      dateIntrusionFormatted: vm.formatDate(new Date().toISOString().substr(0, 10)),
      timeIntrusion: null,
      timeIntrusionFormatted: vm.formatTime(new Date().toTimeString().substr(0, 5)),
      preliminaryDetermination: [
        "Category 1 Water",
        "Category 2 Water",
        "Category 3 Water",
        "High Risk Occupents",
        "Public Health Issues Exist",
        "Risk of Adverse Health Effects",
        "Occupants Express Contaminates",
        "Believed Aerosolizing",
        "Stop & Determine Contaminates"
      ],
      moistureInspection: [
        {label: "Free Water"},
        {label:"Bound Water"},
        {label:"Water Migration"},
        {label:"Affected Assemblies"},
        {label:"Establish a Dry Standard", dryStandard: ""},
        {label:"Instrument Documentation"},
        {label:"Class 2 (significant)"},
        {label:"Class 3 (more than 40%)"},
        {label:"Class 4 (Deeply Bound)"}
      ],
      preRestoreEval:{
        emergencyResAct: '',
        buildingMatRestore: '',
        contentEval: '',
        hvacEval: '',
        substructure:''
      },
      selectedPreliminary: [],
      selectedInspection:[],
      selectedTypes: [],
      steps: [{
          id: 19,
          text: 'Is a TMP required?',
          checked: false
        },
        {
          id: 20,
          text: 'Is a containment tech required?',
          checked: false
        },
        {
          id: 21,
          text: 'Is a water tech required?',
          checked: false
        },
        {
          id: 23,
          text: 'Is a dumpster required?',
          checked: false
        },
        {
          id: 24,
          text: 'Is a moving truck required?',
          checked: false
        },
        {
          id: 25,
          text: 'Is a power supply required?',
          checked: false
        },
        {
          id: 26,
          text: 'Is a Remediation Tech required?',
          checked: false
        },
        {
          id: 27,
          text: 'Is a content tech required?',
          checked: false
        }
      ],
      selectedSteps: [],
      picturesCheck:[
        { id: 23, text: 'Arrival Photo of Entrance', checked: false, image:"" },
        { id: 24, text: 'Address Photo of Property', checked: false, image:"" },
        { id: 25, text: 'Site Specific Safety', checked: false, image:""}
      ],
      selectedPictures:[],
      insuranceCompany: '',
      claimNumber: '',
      policyNumber: '',
      adjusterName: '',
      adjusterPhone: '',
      adjusterEmail: '',
      verificationCheckboxes: [{
          id: 226,
          text: 'The Above property is insured properly and fully'
        },
        {
          id: 27,
          text: 'The individual, representative, or contact individual above has the authority to engage Mitigation Services'
        },
        {
          id: 29,
          text: 'Copy or Photo of Customer Photo ID'
        }
      ],
      uploadedFiles: [],
      filesUploading: [],
      selectedVerification: [],
      cusSignature: {
        data: '',
        isEmpty: true
      },
      customerName: {
        first: '',
        last: ''
      },
      uploadMessage: '',
      idImage:[],
      frontCardImage:[],
      backCardImage:[],
      cardImages: [],
      currentUploadStep: 1,
      cardZip:"",
      teamMemberSig: { data: '', isEmpty: true },
      cusSignTime: "",
      cusSignDate: "",
      dateMask: dateMask,
      timeMask: timeMask,
      phoneMask: {
        mask: '(000) 000-0000'
      },
      initialData: { data: '', isEmpty: true },
      cusInitial1: null,
      cusInitial2: null,
      cusInitial3: null,
      cusInitial4: null,
      empSig: '',
      moistureMap: {
        data: "",
        isEmpty: true
      },
      penColor: "#000",
      dryStandardMask: {
        mask: "00{%}",
        lazy: false
      },
      hasJobid: false,
      teamMemberSignDate: ''
    }),
    watch: {
      timeOfDispatch(val) {
        this.timeOfDispatchFormatted = this.formatTime(val)
      },
      dateOfLoss(val) {
        this.dolFormatted = this.formatDate(val)
      },
      dateOfEval(val) {
        this.doeFormatted = this.formatDate(val)
      },
      arrivalAtProperty(val) {
        this.arrivalFormatted = this.formatTime(val)
      },
      evalStart(val) {
        this.evalStartFormatted = this.formatTime(val)
      },
      evalEnd(val) {
        this.evalEndFormatted = this.formatTime(val)
      },
      contractSigning(val) {
        this.contractFormatted = this.formatTime(val)
      },
      denialOfServices(val) {
        this.dosformatted = this.formatTime(val)
      },
      departureTime(val) {
        this.departureTimeFormatted = this.formatTime(val)
      },
      dateIntrusion(val) {
        this.dateIntrusionFormatted = this.formatDate(val)
      },
      timeIntrusion(val) {
        this.timeIntrusionFormatted = this.formatTime(val)
      },
      jobId(val) {
        if (val !== '') {
          this.hasJobid = true
        } else {
          this.hasJobid = false
        }
      },
      hasJobid(val) {
        if (val) {
          this.animateCSS('initResponse', 'backInUp')
        }
      }
    },
    computed: {
      ...mapGetters({getUser: 'users/getUser'}),
      ...mapGetters({getReports: 'reports/getReports'}),
      date() {
        const today = new Date()
        return (today.getMonth() + 1)+'-'+today.getUTCDate()+'-'+today.getFullYear();
      }
    },
    methods: {
      onAccept(e) {
        const maskRef = e.detail;
        this.cusSignTime = maskRef.value
      },
      undoMap() {
        this.$refs.map.undoSignature()
      },
      saveMap() {
        const { isEmpty, data } = this.$refs.map.saveSignature()
        this.moistureMap.data = data
        this.moistureMap.isEmpty = isEmpty
      },
      onBegin() {
        this.$nextTick(() => {
          this.$refs.map.resizeCanvas()
        })
      },
      submitForm() {
        this.successMessage = ""
        const evaluationLogs = []
        const user = this.getUser
        var rapidRep = this.getReports.filter((v) => {
          return v.ReportType === 'rapid-response'
        })
        this.submitting = true
        const reports = rapidRep.map((v) => { return v.JobId });
        let scrollTo = 0
        evaluationLogs.push({label: 'Team Arrival at Property', value: `${this.doeFormatted} ${this.arrivalAtProperty}:00`}, 
          {label: 'Evaluation Report Start Time', value: `${this.doeFormatted} ${this.evalStart}:00`}, 
          {label: 'Evaluation Report End Time', value: `${this.doeFormatted} ${this.evalEnd}:00`}, 
          {label: 'Time of Contract Signing', value: `${this.doeFormatted} ${this.contractSigning}:00`});
        this.$refs.form.validate().then(success => {
          if (!success) {
            this.errorDialog = true
            this.submitting = false;
            this.submitted = false;
            return goTo(scrollTo); 
          }
          if (!reports.includes(this.jobId)) {
            const post = {
              JobId: this.jobId,
              DateOfLoss: this.dolFormatted,
              DateOfEvaluation: this.doeFormatted,
              ContactName: this.contactName,
              PropertyOwner: this.propertyOwner,
              location: this.location,
              PhoneNumber: this.phoneNumber,
              EmailAddress: this.emailAddress,
              ReportType: 'rapid-response',
              formType: 'initialForms',
              sourceWaterIntrusion: this.selectedTypes,
              Steps: this.selectedSteps,
              InsuranceCompany: this.insuranceCompany,
              ClaimNumber: this.claimNumber,
              PolicyNumber: this.policyNumber,
              adjusterName: this.adjusterName,
              adjusterEmail: this.adjusterEmail,
              adjusterPhone: this.adjusterPhone,
              evaluationLogs: evaluationLogs,
              documentVerification: this.selectedVerification,
              cusFirstName: this.customerName.first,
              cusLastName: this.customerName.last,
              customerSig: this.cusSignature.data,
              PictureTypes: this.selectedPictures,
              id: user.id,
              initials: {
                initial1: this.cusInitial1,
                initial2: this.cusInitial2,
                initial3: this.cusInitial3,
                initial4: this.cusInitial4
              },
              moistureMap: this.moistureMap.data,
              cusSignDate: this.cusSignDate,
              teamMember: this.getUser,
              dateIntrusion: this.dateIntrusionFormatted,
              timeIntrusion: this.timeIntrusionFormatted,
              intrusionInfo: this.intrusionSection,
              selectedPreliminary: this.selectedPreliminary,
              selectedInspection: this.selectedInspection,
              preRestorationEval: this.preRestoreEval,
              teamMemberSig: Object.keys(this.empSig).length !== 0,
              teamMemberSignDate: this.teamMemberSignDate
            };
            this.$api.$post("/api/reports/rapid-response/new", post, {
                    params: {
                        jobid: post.JobId
                    }
                }).then((res) => {
                if (res.error) {
                    this.errorDialog = true
                    this.submitting = false
                    this.$refs.form.setErrors({
                        JobId: [res.message]
                    })
                    return goTo(scrollTo)
                }
                this.successMessage = res
                this.submitting = false
                this.submitted = true
                setTimeout(() => {
                    this.successMessage = ""
                    window.location = "/"
                }, 2000)
            })
          } else {
            this.submitting = false
            this.errorDialog = true
            this.$refs.form.setErrors({
              JobId: ["Duplicate Job ID can't exist"]
            })
            return goTo(0)
          }
        })
      },
      formatDate(dateReturned) {
        if (!dateReturned) return null
        const [year, month, day] = dateReturned.split('-')
        return `${month}-${day}-${year}`
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
      parseTime(time) {
        if (!time) return null
      },
      acceptNumber() {
        const x = this.phoneNumber
          .replace(/\D/g, '')
          .match(/(\d{0,3})(\d{0,3})(\d{0,4})/)
        this.phoneNumber = !x[2] ?
          x[1] :
          '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '')
      },
      goToStep(step) {
        if (step < 1) {
          return
        }
        if (step > 2) {
          this.submitFiles(this.cardImages)
          return
        }
        this.currentUploadStep = step
      },
      setLocation(...params) {
        const { address, street, cityStateZip } = params[0]
        this.location.cityStateZip = cityStateZip
        this.location.address = street
      },
      animateCSS(element, animation, prefix = 'animate__') {
        new Promise((resolve, reject) => {
            const animationName = `${prefix}${animation}`;
            var node;
            if (this.$refs[element]._isVue) {
                node = this.$refs[element].$el
            } else if (this.$refs[element].length === 1) {
                node = this.$refs[element][0]
            } else {
                node = this.$refs[element]
            }
            node.classList.add(`${prefix}animated`, animationName);

            function handleAnimationEnd(event) {
                node.classList.remove(`${prefix}animated`, animationName);
                resolve('Animation ended');
            }

            node.addEventListener('animationend', handleAnimationEnd);
        });
      }
    }
  }
</script>
<style lang="scss">
  .signature-area {
    max-width: 700px;
  }
  .map-wrapper {
    position:relative;
    width:100%;
    max-width:660px;
    margin:auto;
    
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
    &__pen {
      display:flex;
      max-width:300px;
      width:100%;
      justify-content:space-between;
      float:right;
    }
    &__pen-color {
      border-radius:50%;
      width:50px;
      height:50px;
    }
  }
</style>