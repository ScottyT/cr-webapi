<template>
  <div class="folder-contents-wrapper">
    <h1 v-uppercase>{{currentFolder}}</h1>
    <UiBreadcrumbs page="storage" />
    <LazyFolderContents :jobid="repId" :path="currentFolder" subPath="" delimiter="/" />
  </div>
</template>
<script>
export default {
  data: () => ({
    files: []
  }),
  async middleware({
    store,
    redirect,
    route
  }) {
    if (store.state.users.user.role !== "admin") {    
      return redirect("/")
    }
  },
  async asyncData({
    params, $axios
  }) {
    const repId = params.slug
    const currentFolder = params.id
    return {
      repId,
      currentFolder
    }
  },
  head() {
    return {
      title: `Report Folder - ${this.repId} Date - ${this.subfolder}`
    }
  }
}
</script>