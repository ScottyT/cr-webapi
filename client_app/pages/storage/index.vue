<template>
  <div class="storage-page">
    <v-overlay :value="isLoading" v-show="isLoading">
      <v-progress-circular
          indeterminate
          size="64"
        ></v-progress-circular>
    </v-overlay>
    <LayoutReportsList :reportslist="storage" :sortoptions="sortOptions" page="storagePage" :darkMode="true" />
  </div>
</template>
<script>
  import {
    mapGetters
  } from 'vuex'
  import axios from 'axios';
  export default {
    middlware: ['auth'],
    //layout: 'dashboard-layout',
    head() {
      return {
        title: "Storage"
      }
    },
    data: () => ({
      sortOptions: [{
          value: 'JobId',
          text: 'Report Id'
        },
        {
          value: 'teamMember',
          text: 'Employee'
        }
      ],
      reports: [],
      list: [],
      storage: [],
      isLoading: false
    }),
    methods: {
      storageItems() {
        axios.get(`${process.env.gsutil}/list`, {
          params: {folder: "", subfolder: "", delimiter: "/"}, headers: {"authorization": `${this.$auth.strategy.token.get()}`}
        }).then((res) => {
          res.data.folders.forEach((folder) => {
            this.storage.push({"JobId" : folder.name})
          })
        })
      },
    },
    mounted() {
      this.$nextTick(() => {
        this.storageItems()
      })
    }
  }
</script>
<style lang="scss">
  .storage-page {
    padding: 45px 4vw;
  }
</style>