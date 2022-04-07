<template>
  <div class="folder-contents-wrapper">
    <UiBreadcrumbs page="storage" />
    <div class="d-flex">
      <v-btn class="button button--normal" :loading="downloading" @click="handleDownloadZip">Download All</v-btn>
    </div>
    
    <LazyFolderContents :jobid="this.$route.params.uid" path="" subPath="" delimiter="/" />
    <v-dialog v-model="dialog" width="450">
      <div class="modal__error">
        <h3 class="form__input--error">{{errorMessage}}</h3>
      </div>
    </v-dialog>
  </div>
</template>
<script>
import { saveAs } from "file-saver";
import axios from "axios"
export default {
  layout: 'default',
  middlware: ['auth'],
  data: () => ({
    folders: [],
    subfolders: [],
    downloading: false,
    errorMessage: "",
    dialog: false
  }),
  /* async middleware({
    store,
    redirect,
    route
  }) {
    if (store.state.users.user.role !== "admin") {    
      return redirect("/")
    }
  }, */
  head() {
      return {
          title: `Report Folder - ${this.$route.params.uid}`
      }
  },
  methods: {
    handleDownloadZip() {
      this.errorMessage = ""
      const post = {
        folderPath: this.$route.params.uid
      }
      this.downloading = true
      axios.post(`${process.env.gsutil}/zip`, post, {
          responseType: 'arraybuffer',
          headers: {
              "authorization": `${this.$auth.$storage.getCookie("_token.auth0")}`
          }
      }).then((res) => {
          var data = new Blob([res.data]);
          var defaultFilename = post.folderPath
          saveAs(data, `Job_${defaultFilename}_files.zip`)
          this.downloading = false
      }).catch((err) => {
          this.dialog = true
          this.errorMessage = err
          this.downloading = false
      })
    }
  }
}
</script>