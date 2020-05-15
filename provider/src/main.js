import Vue from 'vue'
import BootstrapVue from 'bootstrap-vue'
import axios from 'axios'
import store from './store'
import App from './App'
import NotificationHub from './notification-hub'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import '@fortawesome/fontawesome-free/css/all.css'
Vue.config.productionTip = false

axios.defaults.baseURL = 'http://localhost:5100' // same as the Url the server listens to
axios.defaults.withCredentials = true

// Include the Authentication header when using JWT authentication
axios.interceptors.request.use(request => {
  if (store.state.context.jwtToken) request.headers['Authorization'] = 'Bearer ' + store.state.context.jwtToken
  return request
})

// Setup axios as the Vue default $http library
Vue.prototype.$http = axios

// Install Vue extensions
Vue.use(BootstrapVue)
Vue.use(NotificationHub);

new Vue({
  store,
  render: h => h(App),
}).$mount('#app')


 