/* eslint-disable */
import faker from 'faker'
import * as mutationTypes from './mutation-types'
import axios from 'axios'

const GenerateDataWorker = require("worker-loader?inline=true!../worker/generate-data.js");
const gdWorker = Worker ? new GenerateDataWorker() : null;


export default {

  // updateUserStatus({ getters, state, dispatch,  }, { user }) {
  //   alert("am here!")
  //   alert("call to update user status!! clientId: "
  //     + user.clientId + " " + "room Id " + user.roomId);
  //   const userToUpdate = getters.currentContacts[user.clientId];
  //   if (userToUpdate) {
  //  /*    userToUpdate. */
  //   }
  //  },
  retrieveContacts({ commit }) {
    commit(mutationTypes.SET_GENERATING, { generating: true });
    axios.get('api/provider/all').then(res => {
      const contacts = {};
      res.data.forEach((item, _index, _array) => {
          contacts[item.clientid]= item
       });
      commit(mutationTypes.SET_CONTACTS, { contacts });
      commit(mutationTypes.SET_GENERATING, { generating: false });
    })
  },

  fetchContacts({ commit }, { quantity = 1000 } = {}) {
    if (gdWorker) {
      gdWorker.postMessage({
        quantity,
        getData: {
          // Use path because Web Worker only supports serializable object
          name: ["name", "findName"],
          address: ["address", "streetAddress"],
          avatar: ["image", "avatar"],
          words: { path: ["random", "words"], args: [10] },
        }
      });

      commit(mutationTypes.SET_GENERATING, { generating: true });

      gdWorker.onmessage = e => {
        const contacts = e.data;
        const p = contacts[0];
        commit(mutationTypes.SET_CONTACTS, { contacts });
        commit(mutationTypes.SET_GENERATING, { generating: false });
        commit(mutationTypes.SET_PROVIDER, { p });
      };
    } else {
      const contacts = {};

      new Array(quantity).fill(null).forEach(() => {
        const id = faker.random.uuid();
        contacts[id] = {
          id,
          name: faker.name.findName(),
          address: faker.address.streetAddress(),
          avatar: faker.image.avatar(),
          words: faker.random.words(10),
          phone: faker.phone.phoneNumber,
          title: faker.name.jobTitle,
          bio: faker.name.jobDescriptor,
          state: faker.address.stateAbbr(),
          roomId: '',
          isAvailable: false,
        };
      });
      const p = contacts[0];
      commit(mutationTypes.SET_CONTACTS, { contacts });
      commit(mutationTypes.SET_PROVIDER, { p });
    }
  },

  
};
