/* eslint-disable */
import Vue from 'vue'
import Router from 'vue-router'
import HomePage from '@/views/home'
/* import QuestionPage from '@/views/question' */
import ChatPage from '@/components/chat-view'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'Home',
      component: HomePage
    },
    {
      path: '/chat/:id',
      name: 'Chat',
      component:ChatPage
    }

  ]
})
