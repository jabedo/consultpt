/* eslint-disable */
// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue';
import BootstrapVue from 'bootstrap-vue'
import axios from 'axios'
import router from './router'
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue/dist/bootstrap-vue.css';
import '@fortawesome/fontawesome-free/css/all.css';

import TextHighlighter from 'vue-text-highlight';
import VueVirtualScroller from 'vue-virtual-scroller';
import 'vue-virtual-scroller/dist/vue-virtual-scroller.css';

import App from './App';
import store from './store';
import NotificationHub from './notification-hub';
import RoomsHub from './rooms-hub';
Vue.config.productionTip = false;

axios.defaults.baseURL = 'http://localhost:5100' // same as the Url the server listens to
axios.defaults.withCredentials = true;

// Include the Authentication header when using JWT authentication
axios.interceptors.request.use(request => {
  if (store.state.context.jwtToken) request.headers['Authorization'] = 'Bearer ' + store.state.context.jwtToken
  return request
})

// Setup axios as the Vue default $http library
Vue.prototype.$http = axios

Vue.use(VueVirtualScroller);
Vue.component('text-highlighter', TextHighlighter);
// Install Vue extensions
Vue.use(BootstrapVue)
Vue.use(NotificationHub);
Vue.use(RoomsHub);
new Vue({
  el: '#app',
  components: { App },
  store,
  router,
  template: "<App/>"
});
