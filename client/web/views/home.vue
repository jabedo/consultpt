<template>
  <div>
    <h1>
      Welcome to PT Web
      <button v-b-modal.payModal v-if="selectedContact"  @click="beforeOpenDialog()" :disabled="!canContact" class="btn btn-primary mt-2 float-right">
        <i class="fa fa-video-camera" aria-hidden="true">Chat with {{selectedContact.name}}</i>
      </button>
    </h1>
    <h1 v-if="this.callHappun" >This means updated list was called</h1>
    <search-preview
     />
      <pay-modal v-if="selectedContact"
        :name="selectedContact.name"/>
  </div>
</template>

<script>
/* eslint-disable */
import { mapGetters, mapState , mapActions } from 'vuex'
import SearchPreview from '@/components/search-preview';
import PayModal from '@/components/pay-modal'
import { eventBus } from '../eventBus';

export default {
  components: {
    SearchPreview,
    PayModal
  },
  data () {
    return {
      selectedContact: null,
      callHappun: false
    }
  },
  computed: {

    canContact() {
      return this.isAuthenticated && selectedContact.isavailable && selectedContact.roomid;
    },
    ...mapState('context', [
      'profile'
    ]),
    ...mapGetters('context', [
      'isAuthenticated'
    ]),
  },
  created () {
    eventBus.$on('onSelectedContact', this.onContactSelected);
    eventBus.$on('onClosePayModal', this.onClosePayModal);

  },
  beforeDestroy () {
      eventBus.$off('onSelectedContact', this.onContactSelected);
       //cleanUp signalR event handlers
       if(this.$notificationHub){
          this.$notificationHub.$off('update-user-status', this.onContactUpdated)
       }
  },
  methods: {
     ...mapActions({ 
         updateUserStatus: 'updateUserStatus'
       }),
    onContactSelected (contact) {
      this.selectedContact = contact;
    },
    beforeOpenDialog(){
      eventBus.$emit("clickToPay");
    },
    onClosePayModal(){
    },

/* 
        const first = updatedList.find(item => {
          if(item.clientId && item.clientId == this.selectedContact.clientId){
              return item;
          }
        });
        if(first){
          selectedContact.connectionid = first.connectionid;
          selectedContact.isavailable = first.isavailable;
          selectedContact.roomid = first.roomid;
          selectedContact.providerclientid = first.clientId;
          
      } */
    }
  }
</script>
