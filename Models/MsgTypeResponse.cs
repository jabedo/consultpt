using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Models
{
   public class MsgTypeResponse
   {

      [JsonProperty("type")]
      public string Type { get; set; }

      [JsonProperty("msg")]
      public string Msg { get; set; }
   }
}
