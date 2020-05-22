using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app.Hubs;
using app.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {

        private readonly ILogger<RoomController> _logger;
        private readonly UsersDBContext _context;
        private readonly IHubContext<NotificationHub, INotificationHub> _hubContext;
        public RoomController(ILogger<RoomController> logger, UsersDBContext context, IHubContext<NotificationHub, INotificationHub> hubContext)
        {
            _logger = logger;
            _context = context;
            _hubContext = hubContext;
        }


        [HttpPost("leave/{roomId}/{clientId}/{async}")]
        public ActionResult HangUp(string roomId, string clientId, bool async)
        {
            //this._hubContext.Clients..Group(id.ToString()).AnswerAdded(answer);
            return Ok("nanan");
        }

        [HttpPost("message/{roomId}/{clientId}/{async}")]
        public async Task<IActionResult> SendMessage(string roomId,  string clientId, bool async)
        {
            if(async)
            {
              
            }
            return new JsonResult(roomId);
        }
        /// <summary>
        /// TODO: This is a socket command!!!
        /// </summary>
        /// <returns></returns>
        [HttpPost("send/{roomId}/{clientId}/{async}")]
        public ActionResult Send(string roomId, string clientId, bool async)
        {
            return new JsonResult("nanan");
        }

        [HttpDelete("delete/{roomId}/{clientId}/{registered}/{async}")]
        public ActionResult Delete(string roomId, string clientId, bool registered, bool async)
        {
            return new JsonResult(roomId);
        }


        [HttpPost("join/{roomId}/{clientId}/{roomLinkId}")]
        public  IActionResult Join(string roomId, string clientId, string roomLinkId)
        {
            //Return SUCCESS or FULL
            //this._hubContext.Clients.Group(roomId.ToString()).IncomingCall(answer);

             //_hubContext.Clients.Group(roomId).JoinRoom(roomId);

            return new JsonResult(new 
            {
                result = "SUCCESS",
                params1 = new { 
                    client_id = clientId,
                    room_id = roomId,
                    room_link = roomLinkId,
                    is_initiator = true,
                    messages =""
                }
            });

            //return Json(new
            //{
            //    result = "SUCCESS"
            //});

        }
    }
}