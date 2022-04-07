<template>
  <div>
    <UiAutocomplete :items="reports" @sendReportsToParent="reportsFetched" :theme="pagetheme" />

    <UiDataPagination :items="reportslist" :itemsPerPage="4" :itemStyle="flex ? 'data-list--flex' : 'data-list--grid'" :themeMode="theme" :visiblePageNumbers="visiblePages">
        <template v-slot:content="slotProps">
            <nuxt-link class="pa-2" :to="`/${fieldJacket ? 'field-jacket' : 'contracts'}/${slotProps.report.ReportType}/${slotProps.report.JobId}`">
                <p>{{slotProps.report.JobId}}</p>
                <p>{{slotProps.report.teamMember !== undefined ? slotProps.report.teamMember.name : ""}}</p>
                <span v-if="slotProps.report.date !== null"><span class="form__label">Date:</span>{{slotProps.report.date}}</span>
                <p>{{slotProps.report.ReportType}}</p>
            </nuxt-link>
            <button v-show="slotProps.report.ReportType === 'case-file-technician'" class="button button--normal"
                    @click="createTimesheet(slotProps.report.JobId)">Create timesheet</button>
        </template>
        <p v-if="reportslist.length <= 0">No reports to show</p>
    </UiDataPagination>
  </div>
</template>
<script>
import { defineComponent, toRefs, ref, watch, onMounted, computed, useStore } from "@nuxtjs/composition-api";
import axios from 'axios';
import genericFuncs from "@/composable/utilityFunctions"
import { saveAs } from "file-saver";
export default defineComponent({
    props: {
        reports: {
            type: Array
        },
        theme: {
            type: String
        },
        flex: Boolean,
        fieldJacket: {
            type: Boolean,
            default: true
        }
    },
    setup(props, context) {
        const {
            reports,
            theme
        } = toRefs(props)
        const store = useStore()
        const { groupByKey } = genericFuncs()
        const allReports = computed(() => store.getters["reports/getReports"])
        const reportslist = ref([])
        const visiblePages = ref(5)
        const reportsFetched = (data) => {
            reportslist.value = data.value
        }
        async function createTimesheet(jobid) {
            var rapidResponseRep = allReports.value.find(el => el.ReportType === 'rapid-response')
            var techSheets = allReports.value.filter(el => el.ReportType === "case-file-technician" && el.JobId === jobid)
            var techByDate = groupByKey(techSheets, "date")
            var teamArrival = ""
            var endTime = ""
            var dispatchStart = ""
            var empArr = Array.from(new Set(allReports.value.map(elObj => elObj.teamMember.name)))
            var dateArr = Array.from(new Set(techSheets.map((elObj) => {
                return elObj.date
            })))
            //`${process.env.functionsUrl}/download_excel`
            
            axios.post(`${process.env.functionsUrl}/download_excel`, {
                filename: `${jobid}-timesheet.xlsx`, dateArr: dateArr,
                techByDate: techByDate
            }, { responseType: 'blob'}).then((res) => {
                var defaultFilename = jobid + "-timesheet.xlsx"
                var data = new Blob([res.data])
                saveAs(data, defaultFilename)
            })
        }
        const initData = () => {
            if (reports.value.length > 0) {
                reportslist.value = reports.value
            }
        }
        onMounted(initData)
        return {
            reportslist,
            pagetheme: theme.value,
            reportsFetched,
            initData,
            createTimesheet,
            visiblePages
        }
    }
})
</script>