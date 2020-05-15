/* eslint-disable */
import Vue from 'vue';
import * as mutationTypes from './mutation-types';

export default {
  [mutationTypes.SET_CONTACTS](state, { contacts }) {
    Vue.set(state.resources, 'contacts', contacts);
  },

  [mutationTypes.SET_GENERATING](state, { generating }) {
    Vue.set(state.resources, 'generating', generating);
  },

  [mutationTypes.SET_PROVIDER](state, { provider }) {
    Vue.set(state.resources, 'provider', provider);
  },

  [mutationTypes.SET_SETPROFILE](state, { profile }) {
    Vue.set(state.context, 'profile',  profile );
    if (profile) {
       Vue.set(state, "isAuthenticated", true);
    }
    else {
        Vue.set(state, "isAuthenticated", false);
    }
  },

  [mutationTypes.SET_SETTOKEN](state, { jwtToken }) {
    Vue.set(state.context, 'jwtToken', { jwtToken });
       if (jwtToken) window.localStorage.setItem('jwtToken', jwtToken)
    else window.localStorage.removeItem('jwtToken')
  },

};
