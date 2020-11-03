using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Models
{
   public class Message
   {

      [JsonProperty("cmd")]
      public string Cmd { get; set; }

      [JsonProperty("type")]
      public string Type { get; set; }
 
   }

}