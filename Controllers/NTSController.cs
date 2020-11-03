using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NTSController : ControllerBase
    {
      const string accountSid = "AC7aeadbd237b8faaa18e2289caa433ebe";
      const string authToken = "2dc46cfcd79b8dc739559a942bf97ab8";
      
      [HttpPost("iceservers")]
      public IActionResult IceServers()
      {
         TwilioClient.Init(accountSid, authToken);
         var tokenResource = TokenResource.Create();
         return Ok(tokenResource);
      }
      
   }
}