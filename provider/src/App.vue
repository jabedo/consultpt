<template>
<div id="app">
  <main-navbar />
  <chat-view 
    :disabled="!this.enable"
   />
 
</div>
</template>

<script>
import { mapActions, mapState , mapGetters} from 'vuex'
import ChatView from './components/chat-view'
import MainNavbar from './components/main-navbar'
import { eventBus } from './eventBus'

export default {
  name: 'App',
    components: {
      MainNavbar,
      ChatView
  },
 computed: {
    ...mapState('context', [
      'profile'
    ]),
    ...mapGetters('context', [
      'roomId',
      'clientId'
    ]),
  },

    created () {
    this.restoreContext()
    eventBus.$on("enableChat", this.EnableChat);
  },
  data(){return {enable: false}},
  methods: {
    ...mapActions('context', [
      'restoreContext'
    ]),
    EnableChat(enable){
      this.enable = enable;
      this.$notificationHub.onAvailabilitySet(this.profile.email, this.profile.roomId, this.profile.clientId, enable).then(()=>{
         eventBus.$emit("readyToChat", enable);
      })
    }
  },
 
  
}
</script>