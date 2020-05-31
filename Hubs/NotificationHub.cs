using app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Hubs
{
    public interface INotificationHub
    {
        Task CallDeclined(string targetConnectionId, string v);
        Task IncomingCall(JsonUser jsonUser);
        Task CallEnded(string targetConnectionId, string v);
        Task CallAccepted(JsonUser jsonUser);
        Task UpdateUserList(List<JsonUser> users);
        Task ReceiveSignal(JsonUser jsonUser, string signal);

        Task OnRoomJoined(string roomId);
        Task OnRoomCreated(string roomId);
        Task OnSendMessage(string roomId, string clientId);
        Task UpdateUserStatus(JsonUser jsonUser);
        Task OnUpdateUsersList();
    }
    [Authorize]
    public class NotificationHub: Hub<INotificationHub>
    {
        #region members
        private static readonly Dictionary<string, ClientUser> Users = new Dictionary<string, ClientUser>();
        private static readonly List<UserCall> UserCalls = new List<UserCall>();
        private static readonly List<CallOffer> CallOffers = new List<CallOffer>();
        private readonly static ConnectionMapping<string> _connections =
                        new ConnectionMapping<string>();
        private ILogger<NotificationHub> _logger;
        #endregion

        #region ctr
        public NotificationHub(UsersDBContext context, ILogger<NotificationHub> logger)
        {
            _logger = logger;
            if (!Users.Any())
            {
                foreach (var provider in context.Providers)
                {
                    Users.Add(provider.Id.ToString(),
                                new ClientUser
                                {
                                    ConnectionId = String.Empty,
                                    Id = provider.Id.ToString(),
                                    InCall = false,
                                    IsAvailable = false,
                                    IsProviderAvailable = false,
                                    Name = string.Format("{0} {1}", provider.FirstName.TrimEnd(), provider.LastName.TrimEnd()),
                                    Username = provider.UserName,
                                    UserType = UserType.provider,
                                }
                                );
                }
            }

        }

        #endregion

        #region overrides
        public override Task OnConnectedAsync()
        {
            if (Context.User.Identity != null)
            {
                _logger.LogDebug(string.Format("Connection Succeeded by {0}: Connection ID {1}", Context.User.Identity.Name, Context.ConnectionId));

                if (!string.IsNullOrEmpty(Context.User.Identity.Name))
                {
                    _connections.Add(Context.User.Identity.Name, Context.ConnectionId);
                }

                foreach (ClientUser clientUser in Users.Values.Where(c => Context.User.Identity.Name == c.Username)) //probably just a single user
                {
                    clientUser.ConnectionId = Context.ConnectionId;
                }

                SendUserListUpdate();
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connections = _connections.GetConnections(Context.User.Identity.Name);

            if (!connections.Contains(Context.ConnectionId))
            {
                foreach (string connection in connections)
                {
                    _connections.Remove(Context.User.Identity.Name, connection);
                }

            }
            foreach (var kvp in Users)
            {
                if (kvp.Value.ConnectionId == Context.ConnectionId)
                {
                    kvp.Value.ConnectionId = string.Empty;
                }
            }
            SendUserListUpdate();
            return base.OnDisconnectedAsync(exception);
        }

        #endregion

        #region Client Methods

        public async Task CreateRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }

        public async Task RemoveFromGroup(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }

        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }
        #endregion


        public void CallUser(string targetConnectionId)
        {
           




            ClientUser callingUser;
            Users.TryGetValue(Context.ConnectionId, out callingUser);



            ClientUser targetUser;
            Users.TryGetValue(targetConnectionId, out targetUser);

            // Make sure the person we are trying to call is still here
            if (targetUser == null)
            {
                // If not, let the caller know
                Clients.Caller.CallDeclined(targetConnectionId, "The user you called has left.");
                return;
            }

            // And that they aren't already in a call
            if (GetUserCall(targetUser.ConnectionId) != null)
            {
                Clients.Caller.CallDeclined(targetConnectionId, string.Format("{0} is already in a call.", targetUser.Username));
                return;
            }


            //Male sure subscribers are not calling each other

            if (targetUser.UserType == UserType.subscriber && callingUser.UserType == UserType.subscriber)
            {
                Clients.Caller.CallDeclined(targetConnectionId, "Cannot call a fellow participant!");
                return;
            }


            // They are here, so tell them someone wants to talk
            Clients.Client(targetConnectionId).IncomingCall(GetJsonUser(callingUser));

            // Create an offer
            CallOffers.Add(new CallOffer
            {
                Caller = callingUser,
                TargetUser = targetUser
            });
        }
        public void AnswerCall(bool acceptCall, string targetConnectionId)
        {

            ClientUser callingUser;
            Users.TryGetValue(Context.ConnectionId, out callingUser);

            ClientUser targetUser;
            Users.TryGetValue(targetConnectionId, out targetUser);

            // This can only happen if the server-side came down and clients were cleared, while the user
            // still held their browser session.
            if (callingUser == null)
            {
                return;
            }

            // Make sure the original caller has not left the page yet
            if (targetUser == null)
            {
                Clients.Caller.CallEnded(targetConnectionId, "The other user in your call has left.");
                return;
            }

            // Send a decline message if the callee said no
            if (acceptCall == false)
            {
                Clients.Client(targetConnectionId).CallDeclined(callingUser.ConnectionId, string.Format("{0} did not accept your call.", callingUser.Username));
                return;
            }

            // Make sure there is still an active offer.  If there isn't, then the other use hung up before the Callee answered.
            var offerCount = CallOffers.RemoveAll(c => c.TargetUser.ConnectionId == callingUser.ConnectionId
                                                  && c.Caller.ConnectionId == targetUser.ConnectionId);
            if (offerCount < 1)
            {
                Clients.Caller.CallEnded(targetConnectionId, string.Format("{0} has already hung up.", targetUser.Username));
                return;
            }

            // And finally... make sure the user hasn't accepted another call already
            if (GetUserCall(targetUser.ConnectionId) != null)
            {
                // And that they aren't already in a call
                Clients.Caller.CallDeclined(targetConnectionId, string.Format("{0} chose to accept someone elses call instead of yours :(", targetUser.Username));
                return;
            }

            // Remove all the other offers for the call initiator, in case they have multiple calls out
            CallOffers.RemoveAll(c => c.Caller.ConnectionId == targetUser.ConnectionId);

            // Create a new call to match these folks up
            UserCalls.Add(new UserCall
            {
                Users = new List<ClientUser> { callingUser, targetUser }
            });

            // Tell the original caller that the call was accepted
            Clients.Client(targetConnectionId).CallAccepted(GetJsonUser(callingUser));


            // Update the user list, since thes two are now in a call
            SendUserListUpdate();
        }
        public void HangUp()
        {
            ClientUser callingUser;
            Users.TryGetValue(Context.ConnectionId, out callingUser);
            if (callingUser == null)
            {
                return;
            }

            var currentCall = GetUserCall(callingUser.ConnectionId);

            // Send a hang up message to each user in the call, if there is one
            if (currentCall != null)
            {
                foreach (var user in currentCall.Users.Where(u => u.ConnectionId != callingUser.ConnectionId))
                {
                    Clients.Client(user.ConnectionId).CallEnded(callingUser.ConnectionId, string.Format("{0} has hung up.", callingUser.Username));
                }

                // Remove the call from the list if there is only one (or none) person left.  This should
                // always trigger now, but will be useful when we implement conferencing.
                currentCall.Users.RemoveAll(u => u.ConnectionId == callingUser.ConnectionId);
                if (currentCall.Users.Count < 2)
                {
                    UserCalls.Remove(currentCall);
                }
            }

            // Remove all offers initiating from the caller
            CallOffers.RemoveAll(c => c.Caller.ConnectionId == callingUser.ConnectionId);

            SendUserListUpdate();
        }
        public void SendSignal(string signal, string targetConnectionId)
        {
            var callingUser = Users[Context.ConnectionId];
            var targetUser = Users[targetConnectionId];

            // Make sure both users are valid
            if (callingUser == null || targetUser == null)
            {
                 return;
            }

            // Make sure that the person sending the signal is in a call
            var userCall = GetUserCall(callingUser.ConnectionId);

            // ...and that the target is the one they are in a call with
            if (userCall != null && userCall.Users.Exists(u => u.ConnectionId == targetUser.ConnectionId))
            {
                // These folks are in a call together, let's let em talk WebRTC
                Clients.Client(targetConnectionId).ReceiveSignal(GetJsonUser(callingUser), signal);
            }

        }

        public void Leave(string username)
        {
            Users.TryGetValue(username, out ClientUser user);
            if (user != null)
            {
                user.ConnectionId = Context.ConnectionId;
                user.IsAvailable = false;
                user.InCall = false;
                user.RoomId = string.Empty;
                SendUserListUpdate();
            }
        }
        /// <summary>
        /// Used to connect provider to the hub
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="username"></param>
        /// <param name="name"></param>
        public void Join(string roomId, string username)
        {
            Users.TryGetValue(username, out ClientUser user);
            if (user != null)
            {
                user.ConnectionId = Context.ConnectionId;
                user.IsAvailable = false;
                user.InCall = false;
                user.RoomId = roomId;
                SendUserListUpdate();
            }
        }

        //public  Task LeaveRoom(string roomId)
        //{
        //    return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        //}



        ////Use for provider
        //public async Task Join(string username)
        //{
        //    ClientUser user;
        //    Users.TryGetValue(username, out user);
        //    if(user != null)
        //    {
        //        user.ConnectionId = Context.ConnectionId;
        //        user.IsAvailable = false;
        //        user.InCall = false;
        //    }
        //    else
        //    {
        //        var newUser = _context.Providers.Where(c => c.UserName == username).FirstOrDefault();
        //        if(newUser != null)
        //        {
        //            user = new ClientUser 
        //            {
        //                Username = String.IsNullOrEmpty(username) ? String.Empty : username,
        //                ConnectionId = Context.ConnectionId,
        //                IsAvailable = false,
        //                InCall = false,
        //                UserType = UserType.provider,
        //                Name = string.Format("{0} {1}", newUser.FirstName.TrimEnd(), newUser.LastName.TrimEnd()),
        //            };
        //            Users.Add(username, user);
        //        }
        //    }

        //    if(user != null)
        //    {
        //        SendUserListUpdate();
        //    }
        //}




        public async Task SetAvailability(string Id, string roomId,bool isAvailable)
        {

            var userCall = GetUserCall(Id);
            if (userCall != null)
            {
                //user already in a call -- do nothing
                return;
            }

            if (isAvailable)
            {
              await  Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            }
            else
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            }

            Users.TryGetValue(Id, out ClientUser user);
            if (user != null)
            {
                _logger.LogDebug(string.Format("Context ConnectionID: {0} User connectionId {1}", Context.ConnectionId, user.ConnectionId));
             
                user.IsAvailable = isAvailable;
                user.RoomId = roomId;
                _logger.LogDebug(string.Format("SetAvailability hit on server connectionID: {0} User {1}", Context.ConnectionId, Context.User.Identity.Name));

                SendUserStatusUpdate(new JsonUser
                {
                    Id = user.Id,
                    IsAvailable = user.IsAvailable,
                    RoomId = roomId,
                    ConnectionId = user.ConnectionId,
                    Name = user.Name,
                });
            }

        }

        private void SendUserStatusUpdate(JsonUser jsonUser)
        {
            Clients.All.UpdateUserStatus(jsonUser);
        }

        #region private helpers
        private  void SendUserListUpdate()
        {
            List<JsonUser> users = new List<JsonUser>();
            foreach (KeyValuePair<string, ClientUser> kvp in Users)
            {
                if(GetUserCall(kvp.Key) == null)
                {
                    users.Add(new JsonUser
                    {
                        Name = kvp.Value.Name,
                        ConnectionId = kvp.Value.ConnectionId,
                        IsAvailable = kvp.Value.IsAvailable,
                        RoomId = kvp.Value.RoomId,
                        Id = kvp.Value.Id,
                    });
                }
            }
            _logger.LogDebug(string.Format("Update clients list: Count {0}", users.Count));
            if (users != null && users.Count> 0)
            {
               Clients.All.UpdateUserList(users);
                _logger.LogDebug(string.Format("Update clients list: Count {0}", users.Count));
            }
        }

        private UserCall GetUserCall(string clientId)
        {
            var matchingCall =
                UserCalls.SingleOrDefault(uc => uc.Users.SingleOrDefault(u => u.Id == clientId) != null);
            return matchingCall;
            
            

        }

        private JsonUser GetJsonUser(ClientUser user)
        {
            return new JsonUser
            {
                Name = user.Name,
                Id = user.Id,
                IsAvailable = user.IsAvailable,
                ConnectionId = user.ConnectionId
            };
        }


   
        #endregion
    }



}
