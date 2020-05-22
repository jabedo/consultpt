<template>
  <div>
    <h1>
      Welcome to PT Web
      <button v-b-modal.payModal v-if="selectedContact"  @click="beforeOpenDialog()" :disabled="!canContact" class="btn btn-primary mt-2 float-right">
        <i class="fa fa-video-camera" aria-hidden="true">Chat with {{selectedContact.name}}</i>
      </button>
    </h1>
    <search-preview
        :authkey="authkey"
        :amount="amount"
        :clientId="clientId"
     />
      <pay-modal v-if="selectedContact"
        :authkey="authkey"
        :amount="amount"
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
      providers: [],
      selectedContact: null,
      authkey:'',
      amount: '',
      clientId: '',
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
    if(this.$notificationHub){
      this.$notificationHub.$on("update-user-list", this.onContactUpdated);
    }
    this.$http.post('/api/payments/token').then(res => {
      this.authkey = res.data.token;
      this.amount = res.data.amount;
      this.clientId = res.data.clientId;
    });
  },
  beforeDestroy () {
      eventBus.$off('onSelectedContact', this.onContactSelected);
       //cleanUp signalR event handlers
       if(this.$notificationHub){
          this.$notificationHub.$off('update-user-list', this.onContactUpdated)
       }
  },
  methods: {
    onContactSelected (contact) {
      this.selectedContact= contact;
    },
    beforeOpenDialog(){
      eventBus.$emit("clickToPay", { authkey: this.authkey, amount: this.amount, name: this.selectedContact.name });
    },
    onClosePayModal(){
    },
    onContactUpdated(updatedList){
        const first = updatedList.find(item => item.username == this.selectedContact.name);
        if(first){
          selectedContact.connectionid = first.connectionid;
          selectedContact.isavailable = first.isavailable;
          selectedContact.roomid = first.roomid;
          selectedContact.providerclientid = first.clientId;
      }
    },

  }
}
</script>
