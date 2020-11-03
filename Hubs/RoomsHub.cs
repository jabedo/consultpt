using app.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace app.Hubs
{
   public class RoomsHub : Hub<IRoomsHub>
   {
      private static readonly Dictionary<string, List<string>> _rooms = new Dictionary<string, List<string>>();
      private static readonly Dictionary<string, string>  _clientConnIds = new Dictionary<string, string>();
      private ILogger<RoomsHub> _logger;

      public RoomsHub(ILogger<RoomsHub> logger)
      {
         _logger = logger;
      }

      public override Task OnConnectedAsync()
      {
         return base.OnConnectedAsync();
      }

      public override Task OnDisconnectedAsync(Exception exception)
      {
         return base.OnDisconnectedAsync(exception);
      }
      public async Task Register(string roomId, string clientId)
      {
         _logger.LogInformation(string.Format("room/client registration {0}-{1}", roomId, clientId));
         await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
         _clientConnIds.Add(clientId, Context.ConnectionId);
         if (_rooms.ContainsKey(roomId))
         {
            string targetConnId = GetConnId(roomId, clientId);
            _rooms[roomId].Add(clientId);
           
            if (!string.IsNullOrEmpty(targetConnId))
            {
               MsgTypeResponse msgTypeResponse = new MsgTypeResponse { Type = "register", Msg = "" };
               await Clients.Client(targetConnId).OnReceiveMessage(JsonConvert.SerializeObject(msgTypeResponse, Formatting.Indented));
            }
         }
         else
         {
            _rooms.Add(roomId, new List<string> { clientId });
         }
      }

      private static string GetConnId(string roomId, string clientId)
      {
         string targetClient = _rooms[roomId].FirstOrDefault(c => c != clientId);
         _clientConnIds.TryGetValue(targetClient, out string targetConnId);
         return targetConnId;
      }

      public async Task Leave (string roomId, string clientId)
      {
         await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

         if (_rooms != null && _rooms.ContainsKey(roomId))
         {
            string targetConnId = GetConnId(roomId, clientId);
            _rooms[roomId].Remove(clientId);
            if (_clientConnIds.ContainsKey(clientId))
            {
               _clientConnIds.Remove(clientId);
            }
            var roomClient = new RoomClient { Clientid = clientId, Roomid = roomId };
            var roomClientString = JsonConvert.SerializeObject(roomClient, Formatting.Indented);
            MsgTypeResponse msgTypeResponse = new MsgTypeResponse { Type = "bye", Msg = roomClientString};
            if (!string.IsNullOrEmpty(targetConnId))
            {
               await Clients.Client(targetConnId).OnReceiveMessage(JsonConvert.SerializeObject(msgTypeResponse, Formatting.Indented));
            }
         }
      }

      public async Task Send(string roomId, string clientId, string response)
      {
         if (_rooms != null && _rooms.ContainsKey(roomId))
         {
            string targetConnId = GetConnId(roomId, clientId);
            _logger.LogInformation(string.Format("target {0}-{1}", roomId, targetConnId));
            await Clients.Client(targetConnId).OnReceiveMessage(response);
         }
      }
   }
   public interface IRoomsHub
   {
      Task OnReceiveMessage(string response);
   }
}
