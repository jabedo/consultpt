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
    ...mapGetters('context', [
      'roomId',
      'id',
    ]),
  },

    created () {
    eventBus.$on("enableChat", this.EnableChat);
  },
  data(){ return {enable: false}},
  methods: {
    ...mapActions('context', [
      'restoreContext'
    ]),
    EnableChat(enable){
      this.enable = enable;
      this.$notificationHub.onAvailabilitySet(this.id, this.roomId, enable).then(()=> {
            eventBus.$emit("readyToChat", enable);
      });
  },
  }
}
</script>