import { ref } from "@nuxtjs/composition-api"
import axios from 'axios';
import genericFuncs from "~/composable/utilityFunctions";
const state = () => ({
    error: null,
    all: [],
    userreports: [],
    report: {},
    creditCards:[],
    jobids:null,
    logreports: [],
    message: ""
})
const getters = {
    getReports: (state) => {
        return state.all
    },
    getReport: (state) => {
      return state.report
    },
    getUserReports: (state) => {
      return state.userreports
    },
    getCards: (state) => {
        return state.creditCards
    },
    getFolders: (state) => {
        return state.folders
    },
    getSubfolders: (state) => {
        return state.subfolders
    }
}
const mutations = {
    setLogReports: (state, payload) => {
        state.logreports = payload
    },
    setError: (state, error) => {
        state.error = error
    },      
    setReports: (state, payload) => {
        state.all = payload
    },
    setReport: (state, payload) => {
      state.report = payload
    },
    setUserReports: (state, payload) => {
      state.userreports = payload
    },
    setCreditCards: (state, payload) => {
        state.creditCards = payload
    },     
    setJobIds: (state, payload) => {
        state.jobids = payload
    },
    updateEvalTime: (state, {label, value, index}) => {
      //onst objIndex = state.report.evaluationLogs.find(i => i.index === )
      state.report.evaluationLogs[index].value = value
    }
}
const actions = {
    async fetchLogs({ commit, state }, authUser) {
      if (authUser) {
        await this.$axios.$get(`/api/logs/${authUser.email}`).then((res) => {
          let copyArr = JSON.parse(JSON.stringify(res));
          commit('setLogReports', res)
        }).catch((err) => {
          commit('setError', err)
        })
      }   
    },
    async fetchReport({ commit, state }, { authUser, path }) {
      if (authUser) {
        this.$api.$get(`/api/reports/details/${path}`).then((res) => {
          var report = genericFuncs().replaceEmptyFormFields(res)
          commit('setReport', report)
          return res
        }).catch(err => {
          commit('setError', err)
        })
      }
    },
    async updateReport({ commit, state }, authUser, data) {
      if (authUser) {
        await this.$api.$post(`/api/reports/${reportType}/${jobid}/update`, data).then((res) => {
          commit('setReport', res.data)
          state.message = res.message
        }).catch(err => {
          commit('setError', err)
        })
      }
    },
    async fetchReports({ commit, dispatch }) {
      await this.$api.get(`/api/reports`).then((res) => {
        commit('setReports', res.data)
        dispatch('mappingJobIds')
      }).catch((err) => {
        commit('setError', err)
      })
    },
    async fetchUserReports({ commit, dispatch, rootState }, authUser) {
      if (authUser) {
        this.$api.$get(`/api/reports/${authUser.email}/employee`).then((res) => {
          commit('setUserReports', res)
          dispatch('mappingJobIds')
        }).catch((err) => {
          commit('setError', err)
        })
      }
    },
    sortReports({ commit, state }, sortDirection) {
      state.reports.sort((r1, r2) => {
        let modifier = 1
        if (sortDirection === 'info-bar__sort--desc') modifier = -1;
        if (r1[this.sortBy] < r2[this.sortBy]) return -1 * modifier;
        if (r1[this.sortBy] > r2[this.sortBy]) return 1 * modifier;     
      })
    },
    mappingJobIds({commit, state}) {
      const jobids = [...new Set(state.all.map((v) => { return v.JobId }))]
      commit('setJobIds', ...jobids)
    },
    formatEvalTimes({commit, state}) {
      for (let i = 0; i < state.report.evaluationLogs.length; i++) {
        let time = state.report.evaluationLogs[i].value
        let realTime = time.substring(time.indexOf(" "), time.length)
        commit('updateEvalTime', {label: state.report.evaluationLogs[i].label, value: genericFuncs().timeConverter(realTime, '12hour'), index: i})
      }
    },
    async fetchCreditCards({commit, state}, authUser) {
      if (authUser) {
        this.$api.$get("/api/credit-card").then((res) => {
          commit('setCreditCards', res)
        }).catch(err => {
          commit('setError', err)
        })
      }
    }
}
export default {
    namespaced: true,
    state,
    getters,
    mutations,
    actions
}