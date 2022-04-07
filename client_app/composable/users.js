import { ref, useContext } from "@nuxtjs/composition-api";
import axios from 'axios';
export default function useUsers() {
    let userObj = ref({})
    let avatar = ref({})
    const { $axios, $fire } = useContext()
    function fetchUser(userEmail) {
        return new Promise((resolve, reject) => {
            $fire.auth.currentUser.getIdToken().then((idToken) => {
                $axios.$get(`${process.env.serverUrl}/api/employee/${userEmail}`, {headers: {authorization: `Bearer ${idToken}`}}).then((res) => {
                    userObj.value = res
                    resolve(res)
                }).catch((err) => {
                    reject(err)
                })
            })
        })
    }
    return { fetchUser, userObj }
}