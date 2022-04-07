<template>
    <span><UiBreadcrumbs page="Sketches" />
    <div class="form-wrapper">
        <h1>{{formname}}</h1>
        <h2>{{submittedMessage}}</h2>
        <ValidationObserver ref="form" v-slot="{errors}">
            <v-dialog width="400px" v-model="errorDialog">
                <div class="modal__error">
                    <div v-for="(error, i) in errors" :key="`error-${i}`">
                        <h3 class="form__input--error">{{ error[0] }}</h3>
                    </div>
                </div>
            </v-dialog>
            <form class="form" @submit.prevent="onSubmit">
                <div class="form__form-group">
                    <ValidationProvider vid="JobId" name="Job ID" v-slot="{errors}" rules="required" class="form__input-group form__input-group--normal">
                        <input type="hidden" v-model="selectedJobId" />
                        <label class="form__label">Job ID: </label>
                        <i class="form__select--icon icon--angle-down mdi" aria-label="icon"></i>
                        <select class="form__input" v-model="selectedJobId">
                            <option disabled value="">Please select a Job id</option>
                            <option v-for="(item, i) in $store.state.reports.jobids" :key="`jobid-${i}`">{{item}}</option>
                        </select>
                        <span class="form__input--error">{{ errors[0] }}</span>
                    </ValidationProvider>                   
                </div>
                <div class="form__form-group">
                    <ValidationProvider v-slot="{errors}" vid="sketch" rules="required" name="Sketch" class="form__input form__input--sketch">
                        <input type="hidden" v-model="sketchData.data" />
                        <VueSignaturePad width="100%" height="600px" id="sketchPad" ref="sketchRef" :options="{ onBegin }" />
                        <div class="form__button-wrapper">
                            <button type="button" class="button button--normal" @click="clear">Clear</button>
                            <button type="button" @click="save" :class="`button ${sketchData.isEmpty ? 'button--disabled':''}`">
                                {{sketchData.data !== undefined ? 'Saved' : 'Save'}}
                            </button>
                        </div>
                        <span class="form__input--error">{{ errors[0] }}</span>
                    </ValidationProvider>
                </div>
                <button type="submit" class="button button--normal">{{ submitting ? 'Submitting' : 'Submit' }}</button>
            </form>
        </ValidationObserver>
    </div>
    </span>
</template>
<script>
import { defineComponent, useStore, computed, ref, onMounted } from '@nuxtjs/composition-api'
export default defineComponent({
    props: ['formname'],
    setup(props, {root}) {
        const store = useStore()
        const sketchRef = ref(null)
        const user = computed(() => store.getters['users/getUser']); const getReports = computed(() => store.getters['reports/getReports']);
        const isUserAuth = computed(() => store.getters['users/isLoggedIn']);
        
        const sketchData = ref({}); const sketchFormData = ref(null); const selectedJobId = ref(""); const submittedMessage = ref("");
        const errorDialog = ref(false); const submitting = ref(false);
        function clear() {
            sketchRef.value.clearSignature();
            sketchData.value.data = null; sketchData.value.isEmpty = true
        }
        function save() {
            const { data, isEmpty } = sketchRef.value.saveSignature();
            sketchData.value = { data, isEmpty }
        }
        function onBegin() {
            const { isEmpty } = sketchRef.value.saveSignature()
            sketchData.value = { isEmpty }
        }

        return {
            sketchRef,
            clear, save, onBegin,
            sketchData,
            sketchFormData,
            selectedJobId,
            submittedMessage,
            errorDialog,
            submitting,
            user, getReports,
            loggedIn: computed(() => isUserAuth.value)
        }
    },
    methods: {
        onSubmit() {
            this.submittedMessage = ""
            const post = {
                JobId: this.selectedJobId,
                teamMember: this.user,
                sketch: this.sketchData.data,
                ReportType: this.$route.params.uid,
                formType: 'sketch-report'
            };
            this.submitting = true
            this.$refs.form.validate().then(success => {
                if (!success) {
                    this.submitting = false
                    this.errorDialog = true
                    return;
                }
                this.$api.$post(`/api/reports/${post.ReportType}/new`, post, {params: {jobid: this.selectedJobId}}).then((res) => {
                    if (res.error) {
                        this.errorDialog = true
                        this.submitting = false
                        this.$refs.form.setErrors({
                            JobId: [res.message]
                        })
                        return
                    }
                    this.submittedMessage = res
                    this.submitting = false
                    setTimeout(() => {
                        window.location = "/"
                    }, 3000)
                }).catch(err => {
                    this.submitting = false
                    console.error(err)
                })
            })
        }
    }
})
</script>
<style lang="scss">
#sketchPad {
  max-width:1200px;
  height:600px;
}
</style>