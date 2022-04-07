<template>
    <div :class="`form-wrapper ${$vuetify.breakpoint.width < 991 ? 'flex-column' : ''}`">
        <LayoutPsychrometricChart class="chart" @addData="newChartData" :vapor="vaporPressure" :existingChart="chartdata" :dayOfJob="date" :xaxes="bulbTemp"
            :yaxes="humidityRatio" />
        <ValidationObserver ref="form" v-slot="{ errors }">
            <v-dialog width="400px" v-model="errorDialog">
                <div class="modal__error">
                    <div v-for="(error, i) in errors" :key="`error-${i}`">
                        <h3 class="form__input--error">{{ error[0] }}</h3>
                    </div>
                </div>
            </v-dialog>
            <form class="form" @submit.prevent="onSubmit">
                <div class="d-flex">
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
                    <ValidationProvider v-slot="{ errors }" name="Date" rules="required" class="form__input-group form__input-group--normal">
                        <label class="form__label">Date</label>
                        <input type="text" v-model="date" v-mask="'##/##/####'" class="form__input" />
                        <span class="form__input--error">{{ errors[0] }}</span>
                    </ValidationProvider>
                   
                </div>
                <div class="chart-inputs">
                    <div class="chart-inputs__col">
                        <div class="form__input-group">
                            <label class="form__label">Dry Bulb Temperature - &#176;F</label>
                            <UiRangeSlider slideRef="bulbTemp" htmlId="bulbTemp" minNum="20" maxNum="120" @sendInputVal="bulbTemp = $event" :parentInput="bulbTemp" />
                        </div>
                        <div class="chart-inputs__input">
                            <input type="number" min="20" max="120" v-model="bulbTemp" /> &deg;F
                        </div>
                    </div>
                    <div class="chart-inputs__col">
                        <div class="form__input-group">
                            <label class="form__label">Humidity Ratio - Grains of Moisture Per Pound of Dry Air</label>
                            <UiRangeSlider slideRef="humidityRatio" htmlId="humidityRatio" minNum="0" maxNum="210" @sendInputVal="humidityRatio = $event" :parentInput="humidityRatio" />
                        </div>
                        <div class="chart-inputs__input">
                            <input type="number" min="0" max="210" v-model="humidityRatio" /> humidity ratio
                        </div>
                    </div>
                    <div class="chart-inputs__col">
                        <ValidationProvider v-slot="{errors}" class="form__input-group form__input-group--short" name="Dew Point" rules="between:-20,89">
                            <label class="form__label">Dew Point - &deg;F</label>
                            <input type="number" min="-20" max="89" v-model="dewPoint" class="form__input" />
                            <span class="form__input--error">{{ errors[0] }}</span>
                        </ValidationProvider>
                    </div>
                    <div class="chart-inputs__col">
                        <ValidationProvider v-slot="{errors}" class="form__input-group form__input-group--short" name="Vapor Pressure" rules="between:.1,1.3">
                            <label class="form__label">Vapor Pressure - Inches of Mercury</label>
                            <input type="number" min=".1" max="1.3"  v-model="vaporPressure" class="form__input" />
                            <span class="form__input--error">{{ errors[0] }}</span>
                        </ValidationProvider>
                    </div>
                </div>
                <button type="submit" class="button button--normal">{{ submitting ? 'Submitting' : 'Submit' }}</button>
            </form>
        </ValidationObserver>
    </div>
</template>
<script>
import { defineComponent, onMounted, computed, ref, useStore, ssrPromise, watch } from '@nuxtjs/composition-api'
import axios from 'axios';
import useReports from '@/composable/reports';
export default defineComponent({
    setup(props, { refs, root, emit }) {
        var jobId = root.$route.params.id
        const store = useStore()
        const bulbTemp = ref('20')
        const humidityRatio = ref('0')
        const dewPoint = ref('')
        const vaporPressure = ref('0')
        const submittedMessage = ref('')
        const submitted = ref(false)
        const submitting = ref(false)
        const errorDialog = ref(false)
        const selectedJobId = ref('')
        const chartdata = ref([])
        const jobProgress = ref({})
        const date = ref('')
        const user = computed(() => store.getters['users/getUser'])
        const { getReportPromise, loading } = useReports()
        const getExistingChart = async (jobid) => {
            chartdata.value = []
            await getReportPromise(`psychrometric-chart/${jobid}`).then((result) => {
                for (var i = 0; i < result.jobProgress.length; i++) {
                    var data = {
                        x: result.jobProgress[i].info.dryBulbTemp,
                        y: result.jobProgress[i].info.humidityRatio
                    }
                    var dataset = { pointRadius: 5, data: [data], label: result.jobProgress[i].date, backgroundColor: result.jobProgress[i].color }
                    chartdata.value.push(dataset)
                }
            })
        }
        function newChartData(...params) {
            const { data, date, color } = params[0]
            var formSubmissionData = {
                info: {
                    dryBulbTemp: data.x,
                    humidityRatio: data.y
                },
                date: date,
                color: color
            }
            jobProgress.value = formSubmissionData
        }
        function onSubmit() {
            const post = {
                JobId: selectedJobId.value,
                teamMember: user.value,
                progress: jobProgress.value,
                dewPoint: dewPoint.value,
                vaporPressure: vaporPressure.value,
                formType: 'chart-report',
                ReportType: 'psychrometric-chart'
            };
            refs.form.validate().then(success => {
                if (!success) {
                    submitting.value = false
                    submitted.value = false
                    errorDialog.value = true
                    return;
                }
                submitting.value = true
                axios.post(`${process.env.serverUrl}/api/psychrometric-chart/new`, post).then((res) => {
                    if (res.errors) {
                        errorDialog.value = true
                        submitting.value = false
                        refs.form.setErrors({
                            JobId: res.errors.filter(obj => obj.param === 'JobId').map(v => v.msg)
                        });
                        return;
                    }
                    submittedMessage.value = res.message
                    submitting.value = false
                    submitted.value = true
                    setTimeout(() => {
                        window.location = "/"
                    }, 3000)
                })
            })
        }

        watch(selectedJobId, (val) => {
            getExistingChart(val)
        })
        
        return {
            selectedJobId,
            errorDialog,
            submitting,
            submitted,
            user,
            bulbTemp,
            humidityRatio,
            dewPoint,
            vaporPressure,
            chartdata,
            date,
            loading: computed(() => loading.value),
            jobProgress,
            newChartData,
            onSubmit
        }
    },
})
</script>
<style lang="scss">
.chart-inputs {
    display:flex;
    justify-content: space-between;
    flex-wrap:wrap;
    @include respond(tabletLargeMax) {
        flex-direction:column;
    }

    &__input {
        input[type=number] {
            padding:3px;
            text-align:right;
            box-shadow:0px 0px 3px 2px rgba(0, 0, 0, .25);
        }
    }

    &__col {
        display:flex;
    }
}
</style>