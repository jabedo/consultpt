<template>
  <div>
    <b-dropdown id="dropdown-form" text="Login" ref="dropdownlogin" class="m-2">
      <b-dropdown-form>
        <b-form-group label="Email" label-for="dropdown-form-email" @submit.stop.prevent>
	         <b-form-input id="emailInput"
	                      type="email"
	                      v-model="form.email"
	                      required
	                      placeholder="Enter your email address">
	        </b-form-input>

        </b-form-group>

        <b-form-group label="Password" label-for="dropdown-form-password">
	        <b-form-input id="passwordInput"
	                      type="password"
	                      v-model="form.password"
	                      required
	                      placeholder="Enter your password">
	        </b-form-input>
        </b-form-group>

        <b-form-checkbox class="mb-3">Remember me</b-form-checkbox>
        <b-button variant="primary" size="sm" @click="onClick">Sign In</b-button>
      </b-dropdown-form>
      <b-dropdown-divider></b-dropdown-divider>
      <b-dropdown-item-button>New around here? Sign up</b-dropdown-item-button>
      <b-dropdown-item-button>Forgot Password?</b-dropdown-item-button>
    </b-dropdown>
  </div>
</template>

<script>
import { mapActions } from 'vuex'
  export default {
  data () {
    return {
      form: {
        email: '',
        password: ''
      },
      authMode: 'cookie',
      authOptions: [
        { text: 'Cookie', value: 'cookie' },
        { text: 'JWT Bearer', value: 'jwt' }
      ]
    }
  },
    methods: {
      onClick() {
           this.login({ authMethod: this.authMode, credentials: this.form }).then(() => {
          // Close the menu and (by passing true) return focus to the toggle button
        this.$refs.dropdownlogin.hide(true)
      })
   
      }
    }
  }
</script>