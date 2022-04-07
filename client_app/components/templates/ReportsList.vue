<template>
<!-- Used mainly in folder contents. Difference between this and the other reports component is this has animation -->
  <p v-if="reportslist.length < 0">There are no reports to show</p>
  <p v-else-if="$nuxt.isOffline">You need to be connected to the internet to view reports</p>
  <div class="reports-list" v-else>
    <div class="info-bar">
      <div class="info-bar__search-wrapper">
        <UiAutocomplete @sendReportsToParent="reportsfetched" :items="reportslist" theme="light" />
      </div>
    </div>
    <v-dialog v-model="createDirDialog" persistent max-width="500px">
      <template v-slot:activator="{ on, attrs }">
        <button class="button button--normal" v-bind="attrs" v-on="on">Create Folder</button>
      </template>
      <v-card>
        <v-card-title><span class="text-h5">Create Folder</span></v-card-title>
        <div class="form__form-group pl-5">
          <div class="form__input-group form__input-group--normal">
            <label class="form__label" for="folder">Name of folder</label>
            <input type="text" id="folder" class="form__input" v-model="jobid" />
          </div>
        </div>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn text @click="createDirDialog = false" :disabled="creating">Cancel</v-btn>
          <v-btn text @click="folderCreation" :loading="creating">Save</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <div class="reports-list__reports" :class="darkMode ? 'reports-list__reports--dark' : ''">
      <transition-group class="reports-list__reports-wrapper" name="flip-list" tag="div">
        <div class="reports-list__report flip-list-item" v-for="(report, i) in reports" :key="`report-type-${i}`">
          <nuxt-link class="reports-list__report-link" :to="`/storage/${report.JobId}`" v-if="page == 'storagePage'">
            <h3>{{report.JobId}}</h3>
          </nuxt-link>
        </div>
      </transition-group>
    </div>
  </div>
</template>
<script>
import {mapActions} from 'vuex';
import genericFuncs from '@/composable/utilityFunctions'
import axios from 'axios'
export default {
  name: "ReportsList",
  props: ['reportslist','sortoptions', 'page', "items", 'darkMode'],
  data: () => ({
    search: null,
    report: {},
    sortBy: 'JobId',
    sortDirection: 'info-bar__sort--asc',
    reports: [],
    sketches: [],
    createDirDialog: false,
    jobid: "",
    creating: false
  }),
  methods: {
    ...mapActions({
      sortReports: 'sortReports',
      fetchReports: 'fetchReports'
    }),
    reportsfetched(reports) {
      this.reports = reports.value
    },
    sortValue(s) {
      if (s.value === this.sortBy) {
        this.sortDirection = this.sortDirection === 'info-bar__sort--asc' ? 'info-bar__sort--desc' : 'info-bar__sort--asc';
      }
      this.sortBy = s.value
    },
    folderCreation() {
      const post = {folderPath: this.jobid, storageBucket: process.env.defaultStorage, delimiter: "/", root: true}
      const create = () => {
        return new Promise((resolve, reject) => {
          this.creating = true
          axios.post(`${process.env.functionsUrl}/create_folder`, post, {headers: {Authorization: `${this.$auth.strategy.token.get()}`}}).then((res) => {
            resolve(res.data)
          }).catch(err => {
            reject(err)
          })
        })
      }
      create().then((result) => {
        this.creating = false
        this.createDirDialog = false
        var newFolder = result.data.folders.find(obj => obj.name === this.jobid)
        this.reports.push({"JobId":newFolder.name})
      }).catch((err) => {
        console.log(err)
      })
    }
  },
  mounted() {
    this.reports = this.reportslist
  }
}
</script>
<style lang="scss">
.flip-list-item {
  perspective: 5000px;
  transition: all 500ms cubic-bezier(0.59, 0.12, 0.34, 0.95);
  //transform: translate3d(0, 10px, 0) scale(1);
}

.flip-list-move {
  transition: transform 500ms cubic-bezier(0.59, 0.12, 0.34, 0.95);
  //transform: translate3d(0, 10px, 0) scale(1);
}

.flip-list-enter,
.flip-list-leave-to {
  opacity: 0;
  transition: all 500ms cubic-bezier(0.59, 0.12, 0.34, 0.95);
  transform: translate3d(0, -110px, 0) scale(.01);
}

.flip-list-leave-active {
  position: absolute;
}

.shown {
  opacity: 1;
  transform: scale(1);
}

.reports-list {
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  column-gap: 40px;
  grid-area: reports;
  padding:40px 0;

  &--search-list {
    //display:grid;
  }

  &__reports{
    padding:10px;
    &--dark {
      background:$color-black;
      color:$color-white;
    }
    & >span {
      display: flex;
      flex-wrap: wrap;
    }
  }

  &__reports-wrapper {
    display: flex;
    flex-wrap: wrap;
  }

  &>span {
    &>h2 {
      display: flex;
      width: 100%;
      margin: 40px 0 10px;
    }
  }

  &__report {
    max-width: 200px;
    width: 100%;
    padding: 5px;
    background-color: inherit;

    &:not(:first-child) {
      .reports-list__report {
        margin-left: 0px;
      }
    }
  }

  &__report-link {
    padding: 20px 15px;
    display: block;
    box-shadow: -1px 2px 12px -2px rgba($color-black, .8);
    border: 1px solid #a09999;
    height:100%;

    p {
      margin-bottom:5px;
    }
  }

  &__skeleton-loading {
    display: flex;
    width: 100%;

    &>div:not(:first-child) {
      margin-left: 20px;
    }
  }
}

.info-bar {
  display: grid;
  padding-bottom: 40px;
  grid-template-rows: 46px;
  grid-template-columns: 1fr 1fr;
  column-gap: 30px;
  align-items: center;
  justify-content: space-between;
  font-weight: 500;

  &__search-wrapper {
    height: 100%;
    display: flex;
    align-items: flex-end;

    &--items {
      width: 100%;
      height: 47px;
      display: flex;
      align-items: center;
    }
  }

  &__search {
    height: 42px;
  }

  &__sort {
    max-width: 300px;
    width: 100%;
    height: 100%;
    display: flex;

    a {
      padding: 10px 14px;
      background: #a09999;
      margin: 0 10px 0 0;
    }

    &--asc:after {
      content: "\25B2";
    }

    &--desc:after {
      content: "\25BC";
    }

    &--label {
      width: 20%;
      display: flex;
      align-items: center;
    }

    &--trigger {
      padding: 8px;
      cursor: pointer;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    &--options {
      opacity: 0;
      display: flex;
      flex-direction: column;
      padding: 0 5px;
      transition: all .2s ease-in;
      cursor: none;
      visibility: hidden;
    }
  }

  &__sort-box {
    display: block;
    height: 0;
    transition: height .3s ease-in;
    background-color: $color-white;
    z-index: 2;
    position: relative;
    border: 1px solid $color-black;

    &.open {
      height: 217px;
      transition: height .3s ease-in;

      .info-bar__sort--options {
        opacity: 1;
        visibility: visible;
        cursor: pointer;
      }
    }
  }
}
</style>