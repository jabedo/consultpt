<template>
  <div class="contactDetail__container">
    <div class="avatar__wrapper">
      <img :src="avatar" class="contact__avatar"/>
    </div>
    <div class="contact__text">
      <div class="contact__name">
         {{ name }}
      </div>
      <div>
        {{ address }}
      </div>
      <div class="contact__words">
        {{ words }}
      </div>
    </div>
      <h1>
       <button v-b-modal.payModal  v-if="readyToBeContacted" @click="onClickToPay" class="btn btn-secondary mt-2 mr-2 float-right">
          <i class="fa fa-video-camera"></i>Please pay to chat with {{ name }}
        </button>
      </h1>
      <pay-modal />
  </div>
</template>

<script>
/* eslint-disable */
import { mapGetters, mapState  } from 'vuex';
import PayModal from '@/components/pay-modal'
import { eventBus } from '../eventBus';

export default {
  components: {
    PayModal
  },
  created() {
      eventBus.$on("afterClose", this.killDialog);
      eventBus.$on("contactUpdated", this.OnContactUpdated);
  },
  props: [
   'id', 'avatar', 'words','name','address','isAvailable','roomId', 'connectionId'
  ]
  ,
 computed: {
    ...mapGetters('context', [
      'isAuthenticated',
      'clientId'
    ]),
    readyToBeContacted(){
      return this.isAvailable && this.roomId && this.connectionId;
    }
  },
  methods: {
    onClickToPay(){
      eventBus.$emit("clickToPay", this.name, this.id, this.clientId);
    },
    OnContactUpdated(user){
      /* this.user = user; */
    }
  }
};
</script>
<style lang="scss" scoped>
.contactDetail__container {
  height: 400px;
  box-sizing: border-box;
  display: flex;
  align-items: center;
  border-bottom: 1px solid rgba($color: #000000, $alpha: .1);
  padding: 10px;
  margin: 5px 15px 15px 15px;
  border-radius: 5px;
  box-shadow: 0 5px 10px rgba(0,0,0,.05);
  background: white;
  .avatar__wrapper {
    width: 55px;
    height: 55px;
    margin: 5px;
    .contact__avatar {
      width: 50px;
      height: 50px;
      border-radius: 50%;
    }
  }
  .contact__name {
    font-weight: 600;
    color: #42b983;
  }
  .contact__text {
    text-align: left;
    padding: 10px;
  }
  .contact__words {
    font-size: 11px;
  }
}
</style>
