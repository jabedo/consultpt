import Vue from 'vue'
import axios from 'axios'

const store = {
  namespaced: true,

  state: {
    profile: {},
    jwtToken: null,
    contacts: [],
    isGenerating: false,
  },

  getters: {
    isAuthenticated: state => state.profile.name && state.profile.email && state.profile.jwtToken,
    jwtToken: (state) => state.jwtToken,
    clientId: (state) => state.profile.clientId,
  },

  mutations: {
    setProfile (state, profile) {
      state.profile = profile
    },
    setJwtToken (state, jwtToken) {
      state.jwtToken = jwtToken
    },

  },

  actions: {

    // Used during startup to reload the profile from the server
    restoreContext ({ commit, getters, state }) {
      return axios.get('account/context').then(res => {
        commit('setProfile', res.data)
        if (getters.isAuthenticated) return Vue.prototype.startSignalR(state.jwtToken)
      })
    },
    // Login methods. Either use cookie-based auth or jwt-based auth
    login ({ state, dispatch }, { authMethod, credentials }) {
      const loginAction = dispatch('loginToken', credentials);
      return loginAction.then(() => Vue.prototype.startSignalR(state.jwtToken))
    },
  
    loginToken ({ commit }, credentials) {
      return axios.post('account/token', credentials).then(res => {
        const profile = res.data //jwtToken, name, email, role, clientId
        const jwtToken = res.data.jwtToken
        
        commit('setProfile', profile)
        commit('setJwtToken', jwtToken)
      })
    },
  
    logout ({ commit, state }) {
      const logoutAction = state.jwtToken
        ? Promise.resolve()
        : axios.post('account/logout')

      return logoutAction.then(() => {
        commit('setProfile', {})
        commit('setJwtToken', null)
        return Vue.prototype.stopSignalR()
      })
    },
  }
}

export default store
