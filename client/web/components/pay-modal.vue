<template>
  <b-modal
    id="payModal"
    ref="payModal"
    :no-close-on-backdrop="true"
    @modal-close="resetform"
    hide-footer
    title="Process Payment"
  >
    <b-form>
      <div class="card bg-light">
        <div class="card-header">Pay to Provider: {{ getName() }}</div>
        <div class="card-body">
          <div class="alert alert-danger" v-if="error">{{ error }}</div>
          <div class="alert alert-success" v-if="nonce">Payment successful!</div>
          <div class="form-group">
            <label for="amount">Amount</label>
            <div class="input-group">
              <div class="input-group-prepend">
                <span class="input-group-text">$</span>
              </div>
              <input type="number" v-model="amount" id="amount" class="form-control" readonly />
            </div>
          </div>
          <hr />
          <div class="form-group">
            <label>Credit Card Number</label>
            <div id="card-number" class="form-control"></div>
          </div>
          <div class="form-group">
            <div class="row">
              <div class="col-6">
                <label>Expiration Date</label>
                <div id="expiration-date" class="form-control"></div>
              </div>
              <div class="col-6">
                <label>CVV</label>
                <div id="cvv" class="form-control"></div>
              </div>
            </div>
          </div>
          <div class="text-center">
            <button
              class="btn btn-primary btn-block"
              @click.prevent="pay"
              :disabled="preventPaying"
            >Pay with Credit Card</button>
          </div>
          <hr />
          <div class="form-group text-center">
            <div id="paypalButton"></div>
          </div>
        </div>
      </div>
    </b-form>
  </b-modal>
</template>

<script>
/* eslint-disable */
import { mapGetters } from "vuex";
import { client, hostedFields, paypalCheckout } from "braintree-web";
import paypal from "paypal-checkout";
import { eventBus } from "../eventBus";

const today = new Date();
const currentMonth =
  today.getMonth() + 1 < 10
    ? "0" + (today.getMonth() + 1)
    : today.getMonth() + 1;
const currentYear = today.getFullYear();

export default {
  mounted() {
    eventBus.$on("clickToPay", this.clickToPayHandler);
    this.triggerHidden();
  },
  data() {
    return {
      title: "",
      body: "",
      nonce: "",
      hostedFieldsInstance: false,
      error: "",
      authkey: "",
      amount: "",
      clientId: ""
    };
  },
  computed: {
    preventPaying() {
      return (
        !this.hostedFieldsInstance ||
        isNaN(this.amount) ||
        parseFloat(this.amount) <= 0 ||
        !this.amount
      );
    }
  },
  methods: {
    getName() {
      return this.name;
    },
    triggerHidden() {
      this.$emit("modal-close");
    },
    onHidden() {
      Object.assign(this.form, {
        title: "",
        body: "",
        nonce: "",
        hostedFieldsInstance: false,
        error: ""
      });
    },
    resetform() {
      this.title = "";
      this.body = "";
      this.nonce = "";
      this.hostedFieldsInstance = false;
      this.error = "";
    },
    processPayment() {
      this.$http
        .post("api/payments/process", {
          nonce: this.nonce,
          providerId: this.id,
          subscriberId: this.clientId
        })
        .then(() => {
          this.$refs.payModal.hide();
        })
        .then(() => {
          eventBus.$emit("paymentauthorized");
        })
/*         .then(() =>{
           eventBus.$emit("startChat", true);
        }) */
        .catch(err => {
          this.error = "Error processing Payment. Please try again!";
          console.log(err);
        });
    },
    clickToPayHandler(name, providerId, clientId) {
      this.resetform();
      this.id = providerId;
      this.name = name;
      this.clientId = clientId;

      this.$http
        .post("/api/payments/token")
        .then(res => {
          this.authkey = res.data.token;
          this.amount = res.data.amount;
        })
        .then(() => {
          this.initBraintree();
        });
    },
    pay() {
      if (!this.preventPaying) {
        this.error = "";
        this.nonce = "";

        let amount = parseFloat(this.amount);
        this.hostedFieldsInstance
          .tokenize()
          .then(payload => {
            console.log(payload);
            this.nonce = payload.nonce;
            this.processPayment();
          })
          .catch(err => {
            console.error(err);
            if (typeof err.message !== "undefined") {
              this.error = err.message;
            } else {
              this.error = "An error occurred while processing the payment.";
            }
          });
      }
    },
    initBraintree() {
      client
        .create({
          authorization: this.authkey
        })
        .then(clientInstance => {
          let options = {
            client: clientInstance,
            styles: {
              input: {
                "font-size": "14px",
                "font-family": "Open Sans"
              }
            },
            fields: {
              number: {
                selector: "#card-number",
                placeholder: "Enter Credit Card"
              },
              cvv: {
                selector: "#cvv",
                placeholder: "Enter CVV"
              },
              expirationDate: {
                selector: "#expiration-date",
                placeholder: currentMonth + " / " + currentYear
              }
            }
          };

          let promises = [];
          promises.push(hostedFields.create(options));
          promises.push(paypalCheckout.create({ client: clientInstance }));

          return Promise.all(promises);
        })
        .then(instances => {
          this.hostedFieldsInstance = instances[0];
          const paypalInstance = instances[1];

          return paypal.Button.render(
            {
              env: "sandbox",
              style: {
                label: "paypal",
                size: "responsive",
                shape: "rect"
              },
              payment: () => {
                return paypalInstance.createPayment({
                  flow: "checkout",
                  intent: "sale",
                  amount: this.amount ? this.amount : 50,
                  displayName: "Pt Web Payment",
                  currency: "USD"
                });
              },
              onAuthorize: (data, options) => {
                return paypalInstance.tokenizePayment(options).then(payload => {
                  console.log(payload);
                  this.error = "";
                  this.nonce = payload.nonce;
                  this.processPayment();
                });
              },
              onCancel: data => {
                console.log(data);
                console.log("Payment Cancelled");
                this.$emit("paymentcancelled", res.data);
                this.$refs.payModal.hide();
              },
              onError: err => {
                console.error(err);
                this.error =
                  "An error occurred while processing the paypal payment.";
              }
            },
            "#paypalButton"
          );
        })
        .catch(err => {
          console.error(err);
          this.error = "An error occurred while creating the payment form.";
        });
    }
  }
};
</script>