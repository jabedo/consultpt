<template>
  <div class="contacts-list-search">
    <div class="inputs__container">
      <styled-button
        @click="fetchItems"
        solid
        class="button__generate">
        Refresh
      </styled-button>
      <input-field
        @keyup="searchChange"
        placeholder="Search"
        class="searchbar"
      />
    </div>
    <div class="empty__message centered" v-if="!items.length">Please Refresh</div>
    <div
      class="empty__message centered"
      v-if="items.length && searchText && !results.length">
      Empty result
    </div>
    <img
      class="loading__icon centered"
      src="../assets/loading-cubes.svg"
      v-if="generating"
    />
    <virtual-scroller
      class="scroller"
      :items="searchText ? results : items"
      item-height="135">
      <template slot-scope="props" v-if="items.length">
        <div @click="onSelectedContact(props.item)">
          <contact-detail
            :key="props.itemKey"
            :name="props.item.name"
            :address="props.item.address"
            :words="props.item.words"
            :avatar="props.item.avatar"
            :search="searchText">
          </contact-detail>
        </div>
      </template>
    </virtual-scroller>
  </div>
</template>

<script>
/* eslint-disable */
import axios from 'axios'
import { mapGetters, mapActions } from 'vuex';

import {
  actionTypes,
  getterTypes,
  mapActions as mapSearchActions,
  mapGetters as mapSearchGetters,
} from 'vuex-search';
import ContactDetail from '@/components/ContactDetail';
import InputField from '@/components/InputField';
import StyledButton from '@/components/StyledButton';
import { eventBus } from '../eventBus'

export default {
  components: {
    ContactDetail,
    InputField,
    StyledButton,
  },
  data() {
    return {
      searchText: '',
          //temp
      isAvailable: false
    };
  },
  created() {
    // if(this.$notificationHub){
    //  this.$notificationHub.on('update-user-list', this.updateUserList);
    // }
    this.retrieveContacts();
  },
  computed: {
    ...mapGetters({
      itemsMap: 'currentContacts',
      generating: 'isGenerating',
    }),
    items() {
      return Object.values(this.itemsMap);
    },
    ...mapSearchGetters('contacts', { resultIds: getterTypes.result }),
    results() {
      return this.resultIds.map(id => this.itemsMap[id]);
    },
  },
  methods: {
    ...mapActions({ fetchItems: 'fetchContacts' }),
    ...mapActions({ retrieveContacts: 'retrieveContacts' }),
    ...mapActions({updateContacts: 'updateContacts'}),

    ...mapSearchActions('contacts', { searchContacts: actionTypes.search }),
    searchChange(e) {
      this.searchText = e.target.value;
      this.searchContacts(this.searchText);
      // this.onSelectedContact(null);
    },
    onSelectedContact(e) {
      eventBus.$emit('onSelectedContact', e);
    },
    //  updateUserList(userList) {
    //    //update users list upon invoke by hub
    //   this.updateContacts(userList);
    // }
  
  },
};
</script>

<style lang="scss" scoped>

.contacts-list-search {
  width: 450px;
  height: 1000px;
  position: relative;
  .inputs__container {
    display: flex;
    justify-content: center;
    align-items: center;
    .button__generate {
      margin-right: 15px;
    }
    .searchbar {
      flex-grow: 1;
    }
  }
  .centered {
    position: absolute;
    top: 55%;
    left: 50%;
    transform: translateX(-50%);
    font-size: 18px;
    font-weight: 500;
    opacity: 0.5;
  }
  .loading__icon {
    width: 50px;
    filter: invert(100%);
    margin-top: 20px;
  }
  .scroller {
    background-color: #FBFBFB;
    border: 1px solid rgba($color: #000000, $alpha: .1);
    box-shadow: 0 5px 10px rgba(0,0,0,.05);
    border-radius: 5px;
    height: 500px;
    width: 100%;
    padding: 20px;
    box-sizing: border-box;
  }
}
</style>
