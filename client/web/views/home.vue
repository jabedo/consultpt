<template>
  <div>
    <h1>
      Welcome to PT Web
      <button v-b-modal.payModal v-if="canBeContacted" @click="onClickToPay" class="btn btn-primary mt-2 float-right">
        <i class="fa fa-video-camera" aria-hidden="true">Chat with {{ selectedContact.name }}</i>
      </button>
    </h1>
    <search-preview/>
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
      selectedContact: '',
    }
  },
  computed: {
    ...mapState('context', [
      'profile'
    ]),
    ...mapGetters('context', [
      'isAuthenticated'
    ]),
    canBeContacted(){
      return this.selectedContact && this.selectedContact.isAvailable && this.selectedContact.roomId && this.selectedContact.connectionId;
    }
  },
  created () {
    eventBus.$on('onSelectedContact', this.onContactSelected);
    eventBus.$on('onClosePayModal', this.onClosePayModal);
  },
  beforeDestroy () {
      eventBus.$off('onSelectedContact', this.onContactSelected);
  },
  methods: {
    onContactSelected (contact) {
      if(contact){
         this.selectedContact = contact;
      }
      else {
        this.selectedContact ='';
      }
    },
    onClickToPay(){
      eventBus.$emit("clickToPay", this.selectedContact.name, this.selectedContact.id);
    },
    }
  }
</script>
