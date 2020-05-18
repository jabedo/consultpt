<template>
<div id="app">
  <main-navbar />
  <chat-view 
    :disabled="!this.disable"
   />
 
</div>
</template>

<script>
import { mapActions } from 'vuex'
import ChatView from './components/chat-view'
import MainNavbar from './components/main-navbar'
import { eventBus } from './eventBus'

export default {
  name: 'App',
    components: {
      MainNavbar,
      ChatView
  },
    created () {
    this.restoreContext()
    eventBus.$on("enableChat", this.DisableChat);
  },
  data(){return {disable: false}},
  methods: {
    ...mapActions('context', [
      'restoreContext'
    ]),
    DisableChat(disable){
      $notification-hub.$emit("setAvailability", {name: profile.name, roomId: this.roomId, })
      eventBus.$emit("readyToChat", disable);
    }
  },
 
  
}
</script>