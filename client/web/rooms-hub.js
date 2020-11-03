import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";

export default {
    install(Vue) {
        // use a new Vue instance as the interface for Vue components to receive/send SignalR events
        // this way every component can listen to events or send new events using this.$notificationHub
        const roomsHub = new Vue();
        Vue.prototype.$roomsHub = roomsHub;

        // Provide methods to connect/disconnect from the SignalR hub
        let connection = null;
        let startedPromise = null;
        let manuallyClosed = false;

        Vue.prototype.startSignalR = (jwtToken) => {
            connection = new HubConnectionBuilder()
                .withUrl(`${Vue.prototype.$http.defaults.baseURL}/rooms-hub`,
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
            // connection.on("UpdateUserList", (userList) => {
            //     notificationHub.$emit("update-user-list", userList);
            // });
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
   /*      notificationHub.onAvailabilitySet = (id, roomId, isAvailable) => {
            if (!startedPromise) return;

            return startedPromise
                .then(() => connection.invoke("SetAvailability", id, roomId, isAvailable))
                .catch(console.error);
        }; */

    },
};
