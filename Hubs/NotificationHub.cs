using app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Hubs
{
    public interface INotificationHub
    {
        Task UpdateUserList(List<JsonUser> userList);
        Task CallAccepted(JsonUser acceptingUser);
        Task CallDeclined(string decliningUserConnId, string reason);
        Task IncomingCall(JsonUser callingUser);
        Task ReceiveSignal(JsonUser signalingUser, string signal);
        Task CallEnded(string endingUserID,string reason);
        //void JoinRoom(string roomId, string clientId);
        //void RoomCreated(ClientUser clientUser);
        //Task CreateRoom(string roomId, string clientId);
    }
    [Authorize]
    public class NotificationHub: Hub<INotificationHub>
    {
        private static readonly Dictionary<String, ClientUser> Users = new Dictionary<string, ClientUser>();
        private static readonly List<UserCall> UserCalls = new List<UserCall>();
        private static readonly List<CallOffer> CallOffers = new List<CallOffer>();
        private readonly UsersDBContext _context;

        public async Task JoinRoom(string roomName, JsonUser user, bool notify)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            if (notify)
            {
                await Clients.Group(roomName).IncomingCall(user);
            }
        }

        public async Task LeaveRoom(JsonUser user)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.RoomId);
            await Clients.Group(user.RoomId).CallEnded(user.UserName,"");
        }

        public NotificationHub(UsersDBContext context)
        {
            _context = context;

            if (!Users.Any())
            {

                foreach (var provider in _context.Providers)
                {
                    Users.TryGetValue(provider.UserName, out ClientUser user);
                    if (user == null)
                    {
                        Users.Add(provider.UserName,
                            new ClientUser
                            {
                                ConnectionId = String.Empty,
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


        }

        public override Task OnConnectedAsync()
        {
       
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if(exception == null)
            {

            }
            return base.OnDisconnectedAsync(exception);
        }
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




        public async Task SetAvailability(string username, string roomId, string clientId,bool isAvailable)
        {
            var userCall = GetUserCall(Context.ConnectionId);
            if (userCall != null)
            {
                //user already in a call -- do nothing
                return;
            }

            if (isAvailable)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            }
            else
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            }

            Users.TryGetValue(username, out ClientUser user);
            if (user != null)
            {
                user.ConnectionId = Context.ConnectionId;
                user.IsAvailable = isAvailable;
                user.InCall = false;
                user.ClientId = clientId;
                user.RoomId = roomId;
                SendUserListUpdate();
            }

        }




        #region private helpers
        private  void SendUserListUpdate()
        {
            List<JsonUser> users = new List<JsonUser>();
            foreach (KeyValuePair<string, ClientUser> kvp in Users)
            {
                if(GetUserCall(kvp.Key) != null && kvp.Value.UserType == UserType.provider)
                {
                    users.Add(new JsonUser
                    {
                        Name = kvp.Value.Name,
                        UserName = kvp.Value.Username,
                        ConnectionId = kvp.Value.ConnectionId,
                        IsAvailable = kvp.Value.IsAvailable,
                        RoomId = kvp.Value.RoomId
                    });
                }
            }

            if(users != null && users.Count> 0)
            {
               Clients.All.UpdateUserList(users);
            }
        }

        private UserCall GetUserCall(string connectionId)
        {
            var matchingCall =
                UserCalls.SingleOrDefault(uc => uc.Users.SingleOrDefault(u => u.ConnectionId == connectionId) != null);
            return matchingCall;
        }

        private JsonUser GetJsonUser(ClientUser user)
        {
            return new JsonUser
            {
                Name = user.Name,
                UserName = user.Username,
                IsAvailable = user.IsAvailable,
                ConnectionId = user.ConnectionId
            };
        }
        #endregion
    }



}
