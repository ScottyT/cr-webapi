import { computed, ref, reactive, readonly } from "@nuxtjs/composition-api"
import axios from 'axios';
// If image token gets changed you will have to change the image url in the database
const state = ref({
    certifications: [],
    certList: [],
    loading: false,
    submitting: false,
    submitted: false,
    message: ""
})
const setCerts = function(certs) {
    state.value.certifications = certs
}
const fetchCerts = function(user) {
    if (typeof user !== "object") {
        console.error("user needs to be an object")
        return
    }
    if (!user.hasOwnProperty("certifications")) return;
    state.value.message = ""
    setCerts(user.certifications)
}
const addCert = async function(certinfo, userinfo) {
    state.value.submitting = true
    state.value.submitted = false
    if (state.value.certifications.some(el => el.idNumber === certinfo.idNumber)) {
        state.value.message = "A certification already exists with this ID number."
        state.value.submitting = false
        state.value.submitted = true
    } else {
        axios.post(`${process.env.serverUrl}/api/certifications/new`, certinfo).then((res) => {
            fetchCerts(userinfo)
            state.value.certifications.push(res.data.data)
            state.value.message = res.data.message
            state.value.submitting = false
            state.value.submitted = true
            setTimeout(() => {
                state.value.message = ""
            })
            Promise.resolve(res.data)
        }).catch((err) => {
            state.value.message = err
        })
    }
}

export default {
    state: readonly(state.value),
    fetchCerts,
    addCert,
    //deleteCert
}