import axios from 'axios'
const state = () => ({
    user: {},
    employees: [],
    avatarurl:{},
    error: "",
    signature: "",
    initial: "",
    loading: false
})
const mutations = {
    setUser: (state, payload) => {
        state.user = payload
    },
    setEmployees: (state, payload) => {
        state.employees = payload
    },
    setAvatar: (state, payload) => {
        state.avatarurl = payload
    },
    setError: (state, error) => {
        state.error = error
    },
    setLoading: (state, payload) => {
        state.loading = payload
    },
    setSignature: (state, payload) => {
        if (payload !== '') {
            state.signature = payload
        } else {
            state.signature = ""
        }
    },
    setInitial: (state, payload) => {
        if (payload !== '') {
            state.initial = payload
        } else {
            state.initial = ""
        }
    },
    setCerts: (state, payload) => {
        state.user.certifications = payload
    }
}
const actions = {
    async fetchUser({ commit, dispatch }) {
        if (!this.$auth.loggedIn) {
            return;
        }

        var email = this.$auth.user.email
        var options = {
            headers: { accept: 'application/json', authorization: this.$auth.$storage.getCookie("_token.auth0") }
            /* params: {
                id: this.$auth.user.sub
            } */
        }
        commit('setLoading', true)
        fetch(`https://localhost:7255/api/employees/${email}?id=${this.$auth.user.sub}`, options)
            .then(res => res.json())
            .then((data) => {
            
                dispatch('getSigOrInitialImage', {
                    email: email,
                    signType: 'signature.jpg'
                })
                dispatch('getSigOrInitialImage', {
                    email: email,
                    signType: 'initial.jpg'
                })
                commit('setUser', {
                    email: data.email,
                    name: data.fullName,
                    avatarurl: this.$auth.user.picture,
                    role: data.role,
                    id: data.team_id,
                    auth_id: data.auth_id
                })
                commit('setLoading', false)
            })
            
        /* this.$api.$get(`/api/employees/${email}`, options).then((res) => {
            dispatch('getSigOrInitialImage', {
                email: email,
                signType: 'signature.jpg'
            })
            dispatch('getSigOrInitialImage', {
                email: email,
                signType: 'initial.jpg'
            })
            commit('setUser', {
                email: res.email,
                name: res.fullName,
                avatarurl: this.$auth.user.picture,
                role: res.role,
                id: res.team_id,
                auth_id: res.auth_id
            })
            commit('setLoading', false)
        }).catch((err) => {
            commit('setLoading', false)[
                console.error(err)
            ]
        }) */
    },
    async onAuthStateChangedAction({ commit, dispatch }, { authUser, claims }) {
        if (!authUser) {
          claims = null
          commit('setError', 'an error with authUser')
          return
        }
        commit('setLoading', true)
        commit('setError', '')
        var options = {
            headers: { authorization: `${this.$auth.$storage.getCookie("_token.auth0")}` },
            params: { id: this.$auth.user.sub }
        }
        await axios.get(`https://codered-api.azure-api.net/api/employees/${authUser.email}`, options).then((res) => {
            this.$auth.setUser({
                email: res.data.email,
                name: res.data.name,
                avatarurl: res.data.picture,
                role: res.data.user_metadata.role,
                certifications: res.data.user_metadata.certifications,
                id: res.data.username,
                user_id: res.data.user_id
            })
            commit('users/setUser', {
                email: res.data.email,
                id: res.data.username,
                role: res.data.user_metadata.role,
                name: res.data.name,
                certifications: res.data.user_metadata.certifications
              })
            commit('setLoading', false)
        }).catch((err) => {
            [
                console.error(err)
            ]
        })
    },
    /* async signout({ commit }) {
        commit('setLoading', true)
        await this.$fire.auth.signOut().then(() => {
            commit('SET_USER', {
                email: null,
                id: null,
                role: null,
                name: null,
                avatarUrl: null
            })
            commit('setLoading', false)
        })
    },
    async signin({ commit }, { email, password }) {
        commit('setLoading', true)
        await this.$fire.auth.signInWithEmailAndPassword(email.value, password.value).then(() => {
            commit('setLoading', false)
            commit('setError', '')
        }).catch((err) => {
            commit('setError', err)
            commit('setLoading', false)
        })
    }, */
    getSigOrInitialImage({commit, state}, { signType, email }) {
        if (email === undefined) {
            email = state.user.email
        }
        this.$gcs.$get(`/list/file/${signType}`,{
            params: {folder: email, delimiter: "", bucket: "employee" }
        }).then((res) => {
            if (signType === 'initial.jpg') {
                commit('setInitial', res.imageUrl)
            }
            else if (signType === 'signature.jpg') {
                commit('setSignature', res.imageUrl)
            }
        }).catch((err) => {
            commit('setError', err)
        })
    },
    /* fetchAvatar({ commit, dispatch }) {
        var photoUrl = this.$fire.auth.currentUser.photoURL
        commit('setAvatar', photoUrl)
    } */
}
const getters = {
    getEmployees: (state) => {
        return state.employees
    },
    getSignature: (state) => {
        return state.signature
    },
    getInitial: (state) => {
        return state.initial
    },
    getUser: (state) => {
        var user = {
            email: state.user.email,
            name: state.user.name,
            avatarurl: state.user.avatarurl,
            role: state.user.role,
            id: state.user.id
        }
        return user
    },
    isLoggedIn: (state) => {
        try {
            return state.user.email !== null
        } catch {
            return false
        }
    },
    getAvatar: (state) => {
        return state.avatarurl
    }
}
export default {
    namespaced: true,
    state,
    getters,
    mutations,
    actions
}