using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace app.Pages
{
   public class ChatPageModel : PageModel
   {
      private readonly ILogger<ChatPageModel> _logger;

      [BindProperty(SupportsGet = true)]
      public string RoomId { get; set; }
      public ChatPageModel(ILogger<ChatPageModel> logger)
      {
         _logger = logger;
      }

      public void OnGet(string roomId)
      {
         RoomId = roomId;
      }
     

   }
}
