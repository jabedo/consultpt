using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using app.Hubs;
using app.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Twilio.TwiML.Messaging;
using Twilio.TwiML.Video;
using Twilio.TwiML.Voice;

namespace app.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class RoomController : ControllerBase
   {

      private readonly ILogger<RoomController> _logger;
      private readonly IHubContext<RoomsHub, IRoomsHub> _hubContext;
      private static readonly Dictionary<string, List<string>> _rooms = new Dictionary<string, List<string>>();
      private static readonly Dictionary<string, List<string>> _messages = new Dictionary<string, List<string>>();
      public RoomController(ILogger<RoomController> logger, IHubContext<RoomsHub, IRoomsHub> hubContext)
      {
         _logger = logger;
         _hubContext = hubContext;
      }
      private int MAX_CONNECTED_ClIENTS = 2;


      [HttpPost("leave/{roomId}/{clientId}")]
      public IActionResult Leave(string roomId, string clientId)
      {
         if (_rooms.ContainsKey(roomId))
         {
            _rooms[roomId].Remove(clientId);
         }
         else
         {
            return NotFound();
         }
         return Ok();
      }


      [HttpPost("send/{roomId}/{clientId}")]
      public async Task<IActionResult> Send(string roomId, string clientId)
      {
         using var reader = new StreamReader(Request.Body);
         string response = await reader.ReadToEndAsync();
         if (!string.IsNullOrEmpty(response))
         {
            string resp = JsonConvert.SerializeObject(response);

            _logger.LogInformation(response);
            var str = new
            {
               originating = clientId,
            };
            resp += str;

            _logger.LogInformation("Added client data: " + resp);
            var endString = JsonConvert.SerializeObject(response, Formatting.Indented) + JsonConvert.SerializeObject(str, Formatting.Indented);
            Debugger.Launch();
            await _hubContext.Clients.Group(roomId).OnReceiveMessage(JsonConvert.SerializeObject(response, Formatting.Indented));
         }
         return Ok();
      }

      [HttpDelete("delete/{roomId}/{clientId}")]
      public IActionResult Delete(string roomId, string clientId)
      {
         return Ok();
      }

      [HttpPost("join/{roomId}")]
      public IActionResult Join(string roomId)
      {
         string appBaseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
         if (Request.Headers.TryGetValue("Origin", out StringValues stringValue))
         {
            appBaseUrl = stringValue.FirstOrDefault();
            _logger.LogInformation("Request Url:: " + appBaseUrl);
         }

        
         string clientId = Util.GetRandomString();

         List<string> jsonMessages = new List<string>();
         if (_messages.ContainsKey(roomId))
         {
            jsonMessages.AddRange(_messages[roomId]);
         }
#if DEBUG
         MAX_CONNECTED_ClIENTS = 10;
#endif
         if (_rooms.TryGetValue(roomId, out List<string> connectedClients))
         {
            if (connectedClients.Count < MAX_CONNECTED_ClIENTS)
            {
               //can join
               _rooms[roomId].Add(clientId);
               return new JsonResult(new
               {
                  RESULT = "SUCCESS",
                  PARAMS = new
                  {
                     client_id = clientId,
                     room_id = roomId,
                     room_link = appBaseUrl + "/r" + "/" + roomId,
                     is_initiator = false.ToString().ToLower(),
                     messages = jsonMessages
                  }
               });
            }
            else
            {
               return new JsonResult(new
               {
                  RESULT = "FULL",
                  PARAMS = new
                  {
                     client_id = clientId,
                     room_id = roomId,
                     room_link = appBaseUrl + "/r" + "/" + roomId,
                     is_initiator = false.ToString().ToLower(),
                     messages = jsonMessages
                  }
               });
            }
         }
         else
         {
            List<string> clients = new List<string>();
            clients.Add(clientId);
            _rooms.Add(roomId, clients);

            return new JsonResult(new
            {
               RESULT = "SUCCESS",
               PARAMS = new
               {
                  client_id = clientId,
                  room_id = roomId,
                  room_link = appBaseUrl + "/r" + "/" + roomId,
                  is_initiator = true.ToString().ToLower(),
                  messages = jsonMessages
               }
            });
         }
      }

      [HttpPost("message/{roomId}/{clientId}")]
      public async Task<IActionResult> Message(string roomId, string clientId)
      {
         using (var reader = new StreamReader(Request.Body))
         {
            string response = await reader.ReadToEndAsync();

            lock (_messages)
            {
               if (_messages.ContainsKey(roomId))
               {
                  _messages[roomId].Add(response);
                  _logger.LogInformation("response added");
               }
               else
               {
                  _messages.Add(roomId, new List<string> { response });
                  _logger.LogInformation("response added");
               }
            }
            return Ok();
         }
      }

   }
}