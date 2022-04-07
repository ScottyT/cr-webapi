import { onMounted, ref, useAsync, useContext, useFetch, computed, useStore } from "@nuxtjs/composition-api";
import axios from 'axios';
import genericFuncs from "./utilityFunctions";
export default function useReports() {
    let reports = ref([])
    let report = ref({})
    let signature = ref("")
    let error = ref(false)
    let errorMessage = ref("")
    let images = ref([])
    let loading = ref(false)
    let groupedReports = ref({})
    let filters = ref([])
    const { $axios, $fire, $api, $gcs } = useContext()
    const store = useStore()
    const { groupByKey } = genericFuncs()

    const htmlToPdfOptions = computed(() => {
        return {
            margin:[20, 10, 20, 10],
            filename: `${report.value.ReportType}-${report.value.JobId}`,
            image: {
                type: "jpeg",
                quality: 0.95
            },
            html2canvas: {
                scale: 2,
                useCORS: true,
                logging: true,
                removeContainer: true
            },
            jsPDF: {
                unit: 'px',
                format: 'a4',
                orientation: 'portrait',
                hotfixes: ['px_scaling']
            }
        }
    })

    function filterConditions(arr, type) {
        filters.value = arr
        
        return groupByKey(arr, type)
    }
    function addHeadingToReports(value) {
        return new Promise((resolve) => {
            value.heading = changeFormName(value.ReportType, value.formType)
            resolve(value)
        })
    }
    function getReports(filterRepType = false, filterFormType = false, grouped = false) {
        loading.value = true
        
        const { fetch: fetchReports, fetchState } = useFetch(async () => {
            const promises = []
            reports.value = await $api.$get(`/api/reports`).then((res) => {
                for (var i = 0; i < res.length; i++) {
                    if (res[i].formType === null && res[i].ReportType === null) {
                        error.value = "Report type and form type are required."
                        return;
                    }
                    promises.push(addHeadingToReports(res[i]))
                }
                Promise.all(promises).then((result) => {
                    var formTypeFilter = result.filter(el => filters.value.includes(el.formType))
                    var reportTypeFilter = result.filter(el => filters.value.includes(el.ReportType))
                    
                    if (grouped) {
                        var f = filterConditions(formTypeFilter, "formType")
                        var r = filterConditions(reportTypeFilter, "ReportType")
                        
                        if (filterFormType && !filterRepType) {
                            groupedReports.value = f
                        }
                        if (filterRepType && !filterFormType) {
                            groupedReports.value = r
                        }
                        else {
                            return Object.assign(groupedReports.value,f,r)
                        }
                    } else {
                        reports.value = result
                    }
                    //reports.value = result
                    error.value = false
                    loading.value = false
                }).catch((e) => {
                    error.value = true
                    console.error(e.response.data)
                    errorMessage.value = e.response.data
                })
            })
            
            //const token = await $fire.auth.currentUser.getIdToken()
            // This is using the Fetch API 
        })
        return { fetchReports, fetchState, reports, groupedReports }
    }

    const getReportsPromise = () => {
        loading.value = true
        return new Promise((resolve, reject) => {
            $api.$get(`${process.env.serverUrl}/api/reports`).then((res) => {
                reports.value = res.data
                error.value = false
                loading.value = false
                resolve(res)
            }).catch((err) => {
                error.value = true
                errorMessage.value = err.response.data
                loading.value = false
                reject(err)
            })            
        })
    }

    const getReport = (path) => {
        const { fetch: fetchReport, fetchState } = useFetch(async () => {
            await $api.$get(`/api/reports/details/${path}`).then((res) => {
                res.heading = changeFormName(res.ReportType)
                report.value = res
                store.dispatch("users/getSigOrInitialImage", {signType: "signature.jpg", email: store.getters["reports/getReport"].teamMember.email})

            }).catch((err) => {
                error.value = true
                errorMessage.value = err.response.data
            })
        })
        return { fetchReport, fetchState }
    }

    const getReportPromise = (path) => {
        loading.value = true
        return new Promise((resolve, reject) => {
            $api.$get(`/api/reports/details/${path}`).then((res) => {
                report.value = res
                loading.value = false
                errorMessage.value = res.error
                resolve(res)
            }).catch((err) => {
                reject(err)
                loading.value = false
            })
        })
    }

    const getReportImages = (jobid, folder, subfolder, delimiter) => {
        loading.value = true
        const { fetch: fetchImages, fetchState } = useFetch(async () => {
            $gcs.$get(`/list/${jobid}`, {
                params: {folder: folder, subfolder: folder + "/" + subfolder, delimiter: delimiter, bucket: "default" }}).then((res) => {
                report.value = res
                images.value = res.images
            }).catch(err => {
                error.value = true
                errorMessage.value = err.response.data
            })
        })
        return { fetchImages, fetchState, loading }
    }

    const getCertReport = (path) => {
        loading.value = true
        const {fetch: fetchCertReport, fetchState} = useFetch(async () => {
            $api.$get(`/api/reports/${path}`).then((res) => {
                report.value = res
                loading.value = false
            }).catch(err => {
                loading.value = false
                errorMessage.value = err
            })
        })
        return { fetchCertReport, fetchState, loading }
    }

    const changeFormName = (reportType, formType) => {
        switch(reportType) {
            case "upholstery-form":
                return "Upholstery Form"
            case "quality-control":
                return "Quality Control Evaluation Reports"
            case "case-file-technician":
                return "Daily Technician Report"
            case "moisture-sketch":
                return "Moisture Mapping Location and Sketch"
            case "measurements-sketch":
                return "Measurements and Sketch"
            case "equipment-location-sketch":
                return "Equipment Location and Sketch"
            case "moisture-map":
                return "Moisture Readings"
            case "psychrometric-chart":
                return "Psychrometric Chart"
            case "quality-control":
                return "Quality Control Evaluation Report"
        }
        switch (formType) {
            case "initialForms":
                return "Dispatch and Rapid Response Reports"
            case "chart-report":
                return "Charts"
            case "case-report":
                return "Daily Containment and Tech Reports"
            case "logs-report":
                return "Inventory and Atmospheric Readings"
            case "sketch-report":
                return "Sketch Reports"
        }
        switch (true) {
            case reportType.includes("aob"):
                return "AOB & Mitigation Contract"
            case reportType.includes("coc"):
                return "Certificate of Completion"
            case reportType.includes("contracting-agreement"):
                return "Contracting Service Agreement"
            case reportType.includes("scope-of-work"):
                return "Scope of Work"
        }
    }

    async function beforeDownload({html2pdf, options, pdfContent}) {
        await html2pdf().set(options).from(pdfContent).toPdf().get('pdf').then((pdf) => {
            const totalPages = pdf.internal.getNumberOfPages()
            for (let i = 0; i<= totalPages; i++) {
                pdf.setPage(i)
                pdf.setFontSize(14)
                pdf.text('Page ' + i + ' of ' + totalPages, (pdf.internal.pageSize.getWidth() * 0.88), (pdf.internal.pageSize.getHeight() - 10))
            }
        }).save()
    }
    return { getReports, fetch, reports, report, images, error, errorMessage, getReport, getReportPromise, getReportImages, loading,
        getReportsPromise, filterConditions, groupedReports, changeFormName, beforeDownload, signature, getCertReport
    }
}

export function fetchReportImages(jobid, folder, subfolder, delimiter) {
    return new Promise((resolve, reject) => {
        axios.get(`${process.env.gsutil}/list/${jobid}`,
            { params: { folder: folder, subfolder: folder + "/" + subfolder, delimiter: delimiter }, headers: {
                authorization: `Bearer ${this.$auth.strategy.token.get().split(' ')[1]}`
            }}).then((res) => {
                resolve(res.data)
            })
    }).catch((err) => {
        error.value = err
        reject(err)
    })
}
