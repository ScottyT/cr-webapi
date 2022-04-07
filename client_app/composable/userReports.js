import { computed, readonly, ref, useContext, useFetch, watch } from "@nuxtjs/composition-api";
import axios from 'axios';

const state = ref({
    reports: [],
    loading: false
})
const setReports = function(r) {
    state.value.reports = r
}
const fetchReports = async function(user, idToken) {
    if (user === null) return
    state.value.loading = true
    await axios.get(`${process.env.apiUrl}/api/employee/${user}/reports`, {headers: {authorization: `Bearer ${idToken}`}}).then((res) => {
        setReports(res.data)
        state.value.loading = false
    })
}
export default {
    state: readonly(state.value),
    fetchReports
}