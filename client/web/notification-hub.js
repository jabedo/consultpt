import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";

export default {
  install(Vue) {
    // use a new Vue instance as the interface for Vue components to receive/send SignalR events
    // this way every component can listen to events or send new events using this.$notificationHub
    const notificationHub = new Vue();
    Vue.prototype.$notificationHub = notificationHub;

    // Provide methods to connect/disconnect from the SignalR hub
    let connection = null;
    let startedPromise = null;
    let manuallyClosed = false;

    Vue.prototype.startSignalR = (jwtToken) => {
      connection = new HubConnectionBuilder()
        .withUrl(`${Vue.prototype.$http.defaults.baseURL}/notification-hub`,
          {
            accessTokenFactory: () => {
              return jwtToken
            }
          } 
        )
        .configureLogging(logging => {
          // Log to the Console
          logging.AddConsole();

          // This will set ALL logging to Debug level
          logging.SetMinimumLevel(LogLevel.Debug);
        })
        .build();

      // Forward hub events through the event, so we can listen for them in the Vue components
      connection.on("UpdateUserList", (userList) => {
        notificationHub.$emit("update-user-list", userList);
      });
      connection.on("UpdateUserStatus", (user) => {
        notificationHub.$emit("update-user-status", user);
      });

      connection.on("CallAccepted", (acceptingUser) => {
        notificationHub.$emit("call-accepted", { acceptingUser });
      });
      connection.on("CallDeclined", (decliningUserConnId, reason) => {
        notificationHub.$emit("call-declined", {
          decliningUserConnId, reason
        });
      });
      connection.on("IncomingCall", (callingUser) => {
        notificationHub.$emit("incoming-call", callingUser);
      });
      connection.on("ReceiveSignal", (signallingUser, signal) => {
        notificationHub.$emit("receive-signal", { signallingUser, signal });
      });
      connection.on("CallEnded", (endingUserID, reason) => {
        notificationHub.$emit("call-ended", { endingUserID, reason });
      });

      // You need to call connection.start() to establish the connection but the client wont handle reconnecting for you!
      // Docs recommend listening onclose and handling it there.
      // This is the simplest of the strategies
 /*      function start() {
        startedPromise = connection.start().catch((err) => {
          console.error("Failed to connect with hub", err);
          return new Promise((resolve, reject) =>
            setTimeout(
              () =>
                start()
                  .then(resolve)
                  .catch(reject),
              5000
            )
          );
        });
        return startedPromise;
      }
      connection.onclose(() => {
        if (!manuallyClosed) start();
      });
 */

      async function start() {
        try {
          await connection.start();
          console.log("connected");
        } catch (err) {
          console.log(err);
          setTimeout(() => start(), 5000);
        }
      };

      connection.onclose(async () => {
        if (!manuallyClosed) await start();
      });



      // Start everything
      manuallyClosed = false;
      start();
    };
    Vue.prototype.stopSignalR = () => {
      if (!startedPromise) return;

      manuallyClosed = true;
      return startedPromise
        .then(() => connection.stop())
        .then(() => {
          startedPromise = null;
        });
    };

    // Provide methods for components to send messages back to server
    // Make sure no invocation happens until the connection is established
    notificationHub.OnRoomCreated = (roomId) => {
      if (!startedPromise) return;

      return startedPromise
        .then(() => connection.invoke("CreateRoom", {roomId }))
        .catch(console.error);
    };

    notificationHub.OnRoomJoined = (roomId) => {
      if (!startedPromise) return;

      return startedPromise
        .then(() => connection.invoke("JoinRoom", {roomId: roomId, user: null, notify: false }))
        .catch(console.error);
    };
    notificationHub.onLeave = (username) => {
      if (!startedPromise) return;

      return startedPromise
        .then(() => connection.invoke("Leave", { username }))
        .catch(console.error);
    };
    
    notificationHub.hangUp = (username, name) => {
      if (!startedPromise) return;

      return startedPromise
        .then(() => connection.invoke("HangUp"))
        .catch(console.error);
    };

    notificationHub.onAvailabilitySet = (id, roomId, isAvailable) => {
      if (!startedPromise) return;

      return startedPromise
        .then(() => connection.invoke("SetAvailability",  id, roomId, isAvailable))
        .catch(console.error);
    };

    notificationHub.callUser = (targetConnectionId) => {
      if (!startedPromise) return;

      return startedPromise
        .then(() => connection.invoke("CallUser", targetConnectionId))
        .catch(console.error);
    };

    notificationHub.answerCall = (targetConnectionId) => {
      if (!startedPromise) return;

      return startedPromise
        .then(() => connection.invoke("CallUser", targetConnectionId))
        .catch(console.error);
    };







  },
};
