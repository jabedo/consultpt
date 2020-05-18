<template>
  <div id="search-app">
    <div class="info__container">
     <provider-bio v-if="this.selectedContact"
        :image ="this.selectedContact.avatar"
        :bio ="this.selectedContact.words"
        :title ="this.selectedContact.title"
        :name ="this.selectedContact.name"
        >
     </provider-bio>
    </div>
    <div class="contacts_container">
      <list-contacts :searchText="search"></list-contacts>
    </div>
  </div>
</template>

<script>
import ListContacts from '@/components/ListContacts';
import ProviderBio from '@/components/Bio';

export default {
  components: {
    ListContacts,
    ProviderBio,
  },
  data() {
    return {
      search: '',
      selectedContact: null,
    };
  },
  methods: {
    onSelectedContact(e) {
      this.selectedContact = e;
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
  height: 600px;
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
