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


        [HttpPost("leave")]
        public ActionResult HangUp(Guid roomId, Guid clientId, bool async)
        {
            //this._hubContext.Clients..Group(id.ToString()).AnswerAdded(answer);
            return Ok("nanan");
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessage(Guid roomId,  Guid clientId, bool async)
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
        [HttpPost("send")]
        public ActionResult Send()
        {
            return new JsonResult("nanan");
        }

        [HttpDelete("delete")]
        public ActionResult Delete(Guid roomId, Guid clientId, bool registered, bool async)
        {
            return new JsonResult(roomId);
        }


        [HttpPost("join")]
        public ActionResult JoinRoom(Guid roomId)
        {
            //Return SUCCESS or FULL
            this._hubContext.Clients.Group(roomId.ToString()).IncomingCall(answer);
            return new JsonResult(new { result="SUCCESS"});
        }
    }
}