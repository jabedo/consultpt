<template>
  <b-modal id="loginModal" ref="loginModal" hide-footer title="Login" @hidden="onHidden">
    <b-form @submit.prevent="onSubmit" @reset.prevent="onCancel">
      <b-alert show variant="warning">In test, any credentials are valid!</b-alert>
      <b-form-group label="Email:" label-for="emailInput">
        <b-form-input id="emailInput"
                      type="email"
                      v-model="form.email"
                      required
                      placeholder="Enter your email address">
        </b-form-input>
      </b-form-group>
      <b-form-group label="Password:" label-for="passwordInput">
        <b-form-input id="passwordInput"
                      type="password"
                      v-model="form.password"
                      required
                      placeholder="Enter your password">
        </b-form-input>
      </b-form-group>
      <button class="btn btn-primary float-right ml-2" type="submit">Login</button>
      <button class="btn btn-secondary float-right" type="reset">Cancel</button>
    </b-form>
  </b-modal>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'
import { eventBus } from '../eventBus'
export default {
  data () {
    return {
      form: {
        email: '',
        password: ''
      },
      authMode: 'jwt',
    }
  },
  created(){
    eventBus.$on('showLogin', this.onShowHandler)
  },
  methods: {
    ...mapActions('context', [
      'login'
    ]),
    ...mapGetters("context", [
      'roomId','email'
    ]),
    onSubmit (evt) {
      this.login({ authMethod: this.authMode, credentials: this.form }).then(() => {
        this.$refs.loginModal.hide();
      /*    $notificationHub.onJoined(this.roomId, this.email, this.name); */
      eventBus.$emit("loginComplete");
      })
    },
    onCancel (evt) {
      this.$refs.loginModal.hide()
    },
    onHidden () {
      Object.assign(this.form, {
        email: '',
        password: ''
      })
    $notificationHub.onLeave(this.email);
    },
    onShowHandler() {
     this.$refs.loginModal.show();
    },
  }
}
</script>
