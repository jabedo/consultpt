<template>
  <nav class="navbar navbar-expand-md navbar-dark bg-dark shadow">
    <a class="navbar-brand" href="#/">PT Web</a>
    <button class="navbar-toggler" type="button"
    data-toggle="collapse" data-target="#main-navbar" aria-controls="main-navbar"
       aria-expanded="false"
       aria-label="Toggle navigation">
      <span class="navbar-toggler-icon"></span>
    </button>

    <div class="collapse navbar-collapse" id="main-navbar">
      <ul class="navbar-nav mr-auto">
        <li class="nav-item active">
          <a class="nav-link" href="#/">Home <span class="sr-only">(current)</span></a>
        </li>
      </ul>
      <span v-if="isAuthenticated" class="navbar-text mr-2">
      Welcome,  {{ profile.name }} <switch-button color="#90EE90" v-model="available">Availability</switch-button>
      </span>
      <form v-if="isAuthenticated" class="form-inline my-2 my-lg-0">

        <button class="btn btn-secondary my-2 my-sm-0" type="submit"
          @click.prevent="logout">Logout
        </button>

      </form>
      <form v-else class="form-inline my-2 my-lg-0">
        <button v-b-modal.prevent.loginModal class="btn btn-secondary my-2 my-sm-0" type="submit" @click.prevent>
          Login
        </button>
      </form>
       <login-modal />
    </div>
  </nav>
</template>

<script>
/* eslint-disable */
import { mapGetters, mapState, mapActions } from 'vuex'
import LoginModal from '@/components/login-modal'
import SwitchButton from '@/components/switch-button'
export default {
  data(){
    return{
      available:false //switch button variable
    }
  },
  components:{
    LoginModal,
    SwitchButton
  },
  computed: {
   ...mapState('context', [
      'profile'
    ]),
    ...mapGetters('context', [
      'isAuthenticated'
    ]),
  },
  methods: {
   ...mapActions('context', [
     'logout'
    ]),
  }
}
</script>
