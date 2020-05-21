using Newtonsoft.Json;
using System;

namespace app.Models
{
    public class JsonUser
   {
      [JsonProperty("name")]
      public string Name { get; set; }
      [JsonProperty("connectionid")]
      public string ConnectionId { get; set; }
      //[JsonProperty("isprovider")]
      //public bool IsProvider { get; set; }
      [JsonProperty("isavailable")]
      public bool IsAvailable { get; set; }
      [JsonProperty("username")]
      public String UserName { get; set; }
        public string RoomId { get; internal set; }
    }

}
