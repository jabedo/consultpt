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
        [JsonProperty("isavailable")]
        public bool IsAvailable { get; set; }
        [JsonProperty("username")]
        public String UserName { get; set; }
        [JsonProperty("roomid")]
        public string RoomId { get; internal set; }
        [JsonProperty("clientid")]
        public string ClientId { get; internal set; }
    }

}
