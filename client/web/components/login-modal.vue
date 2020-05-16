<template>
  <b-modal id="loginModal" ref="loginModal" hide-footer title="Login" @hidden="onHidden">
    <b-form @submit.prevent="onSubmit" @reset.prevent="onCancel">
      <b-alert show variant="warning">In test, any credentials are valid!</b-alert>
      <b-form-group label="Email:" label-for="emailInput">
        <b-form-input id="emailInput"
                      type="email"
                      v-model="form.email"
                      required
                      placeholder="antonia.rolfson@hodkiewicz.name">
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
/* eslint-disable */
import { mapActions } from 'vuex'

export default {
  data () {
    return {
      form: {
        email: 'antonia.rolfson@hodkiewicz.name',
        password: 'bogus123'
      },
      authMode: 'jwt',
    }
  },
  methods: {
    ...mapActions('context', [
      'login'
    ]),
    onSubmit (evt) {
      this.login({ authMethod: 'jwt', credentials: this.form }).then(() => {
        this.$refs.loginModal.hide()
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
    }
  }
}
</script>
