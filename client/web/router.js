/* eslint-disable */
import Vue from 'vue'
import Router from 'vue-router'
import HomePage from '@/views/home'
import ChatPage from '@/views/chat'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'Home',
      component: HomePage
    },
    {
      path: '/chat/',
      name: 'Chat',
      component:ChatPage
    },
    {
      path: '/r/:roomId',
      name: 'Chat1',
      component: ChatPage
    }
  ]
})
