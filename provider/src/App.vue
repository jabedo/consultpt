<template>
<div id="app">
  <nav-bar />
  <chat-view />
</div>
</template>

<script>
import { mapActions } from 'vuex'
import NavBar from '@/components/nav-bar'
import ChatView from '@/components/chat-view'

export default {
  name: 'App',
    components: {
      NavBar,
      ChatView
  },
    created () {
    this.restoreContext()
  },
  methods: {
    ...mapActions('context', [
      'restoreContext'
    ])
  }
  
}
</script>