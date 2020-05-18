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
     <styled-button
        @click="onContactProvider"
        solid
        class="button__generate">
        Contact {{ name }}
      </styled-button>
  </div>
</template>

<script>
/* eslint-disable */
import { mapGetters  } from 'vuex';
import StyledButton from '@/components/StyledButton';

export default {
  props: [
    'avatar', 'name','address','words'/* ,'phone','state','isavailable','status' */
  ],
  methods: {
    onContactProvider(){
        alert("contact provider!");
    },
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
