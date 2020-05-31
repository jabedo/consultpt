using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app.Hubs;
using app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ConsultPT.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {

        private readonly ILogger<ProviderController> _logger;
        private readonly UsersDBContext _context;
        private readonly IHubContext<NotificationHub, INotificationHub> _hubContext;
        public ProviderController(ILogger<ProviderController> logger, UsersDBContext context, IHubContext<NotificationHub, INotificationHub> hubContext)
        {
            _logger = logger;
            _context = context;
            _hubContext = hubContext;
        }


        [HttpGet("all")]
        public IActionResult GetProviders()
        {
            var providers = _context.Providers.Select(c => new
            {
                Bio = c.Bio,
                ConnectionId = string.Empty,
                InCall = false,
                LicenseNumber = c.IDLicenceNumber,
                StateRegistered = c.LicenseState,
                Title = c.Credentials,
                IsAvailable = false,
                Username = c.UserName,
                UserType = UserType.provider,
                PhoneNumber = c.PhoneNumber,
                Name = string.Format("{0} {1}", c.FirstName.TrimEnd(), c.LastName.TrimEnd()),
                Address = c.Address,
                PhotoName = c.PhotoName_URL,
                Avatar = c.PhotoName_URL,
                Id = c.Id.ToString(),
                Words = c.Bio.Substring(0, 150)
            });


            if (providers != null)
            {
                return Ok(providers);
            }

            return NotFound();
        }


        [HttpPost("join")]
        [Authorize]
        public IActionResult Join(string roomId)
        {
            return Ok("nana");
        }


        // GET: api/Users/5
        [HttpGet("{name}", Name = "Get")]
        public ActionResult<ProviderInfo> GetProvider(string name)
        {
            var  provider=  _context.Providers.Where(c => c.UserName == name).Select(c => new ProviderInfo
            {
                Bio = c.Bio,
                ConnectionId = null,
                InCall = false,
                LicenseNumber = c.IDLicenceNumber,
                StateRegistered = c.LicenseState,
                Title = c.Credentials,
                IsAvailable = false,
                Username = c.UserName,
                UserType = UserType.provider,
                PhoneNumber = c.PhoneNumber,
                Name = string.Format("{0} {1}", c.FirstName.TrimEnd(), c.LastName.TrimEnd()),
                Address = c.City,
                PhotoName = c.PhotoName_URL
            }).FirstOrDefault();

            if(provider == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(provider);
            }
        }

        // POST: api/Users
        [HttpPost]
        [Authorize]
        public ActionResult Post(ProviderInfo providerInfoItem)
        {

            var existingProvider = _context.Providers.FirstOrDefault(c => c.UserName == providerInfoItem.Username);
            if(existingProvider == null)
            {
                _context.Add(new Provider 
                { 
               
                });
                _context.SaveChangesAsync();
            }
            return Conflict("Cannot create the user because user exists");
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        [Authorize]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [Authorize]
        public void Delete(int id)
        {
        }




    }




}
