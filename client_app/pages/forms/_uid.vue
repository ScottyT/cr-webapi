<template>
  <div class="px-3 mt-6 mb-6">
    <SlicesBlock :slice="formType" :abbreviation="abbreviation" />
  </div>
</template>
<script>
import {mapGetters, mapActions} from 'vuex'
export default {
  name: 'Page',
  data() {
    return {
      authUser: false,
      // These would be different per company
      abbreviation: "WESI"
    }
  },
  head() {
    return {
      title: this.title
    }
  },
  middleware: ['auth'],
  /* async middleware({redirect, store}) {
    if (store.state.users.user.email == null) {
      return redirect("/")
    }
  }, */
  async asyncData({ error, params }) {
    var formType = params.uid
    var title = ""
    switch (formType) {
      case "dispatch-report":
        title = "Dispatch"
        break;
      case "rapid-response":
        title = "Rapid Response"
        break;
      case "daily-containment-report":
        title = "Daily Containment Report"
        break;
      case "daily-technician-report":
        title = "Daily Technician Report"
        break;
      case "aob-contract-form":
        title = "AOB & Mitigation Contract"
        break;
      case "certificate-of-completion":
        title = "Certificate of Completion"
        break;
      default:
        title = "Report Form"
    }
    return {
      formType,
      title
    }
  }
}
</script>