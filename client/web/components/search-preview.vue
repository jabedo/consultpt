<template>
  <div class="container-fluid">
    <div class="row">
          <div class="col-md-8">
          <list-contacts :searchText="search"></list-contacts>
          </div>
          <div class="col-md-4">
            <provider-bio v-if="this.selectedContact"
              :id="this.selectedContact.id"
              :avatar ="this.selectedContact.avatar"
              :words ="this.selectedContact.words"
              :name ="this.selectedContact.name"
              :address ="this.selectedContact.address"
              :isAvailable="this.selectedContact.isAvailable"
              :roomId="this.selectedContact.roomId"
              :connectionId="this.selectedContact.connectionId"
              >
            </provider-bio>
          </div>
      </div>
  <!--     <chat-modal v-if="enableChat"/> -->
  </div>
</template>
<script>
import ListContacts from '@/components/list-contacts';
import ProviderBio from '@/components/provider-bio';
/* import ChatModal from '@/components/chat-modal' */
import { eventBus } from '../eventBus'

export default {
  components: {
    ListContacts,
    ProviderBio,
/*     ChatModal, */
  },

  created() {
      eventBus.$on('paymentcancelled', this.onPaymentCancelled);
      eventBus.$on('paymentauthorized', this.onPaymentAuthorized);
      eventBus.$on('onSelectedContact', this.onSelectedContact)
  },
  data() {
    return {
      search: '',
      selectedContact: null,
      enableChat: false
    };
  },
  methods: {
    onSelectedContact(e) {
      this.selectedContact = e;
    },
   onPaymentCancelled() {
       this.enableChat = false;
         eventBus.$emit("startChat", false, this.selectedContact.roomId, this.selectedContact.name, this.$);
   },
    onPaymentAuthorized() {
        this.enableChat = true;
       this.$router.push({ name: "Chat", params: { roomId: this.selectedContact.roomId , name: this.selectedContact.name, canChat: true} });

    },
  },
};
</script>

<style lang="scss">
#search-app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin: auto;
  margin-top: calc(25vh - 100px);
  width: 1000px;
  height: 400px;
  display: grid;
  grid-column-gap: 20px;
  grid-template-columns: repeat(5, 200px);
  justify-items: center;
  .info__container {
    grid-column-start: 1;
    grid-column-end: 3;
    text-align: left;
    display: flex;
    flex-direction: column;
    justify-content: center;
    .info__header {
      padding-bottom: 5px;
      border-bottom: 1px solid rgba($color: #000000, $alpha: .1);
      &.main {
        color: #42b983;
      }
    }
    .info__description {
      margin-top: 0;
    }
    .info__preformat {
      padding: 20px;
      background-color: #FBFBFB;
      border-radius: 5px;
    }
    a:link, a:visited {
      color: #42b983;
      text-decoration: none;
    }
  }
  .contacts_container {
    grid-column-start: 3;
    grid-column-end: 6;
    display: flex;
    flex-direction: column;
    justify-content: center;
  }
}
</style>
