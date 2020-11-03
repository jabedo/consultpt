using Newtonsoft.Json;

namespace app.Models
{
   public class RoomClientResponse
   {

      [JsonProperty("roomId")]
      public string RoomId { get; set; }

      [JsonProperty("clientId")]
      public string ClientId { get; set; }

      [JsonProperty("cmd")]
      public string Cmd { get; set; }

      [JsonProperty("msg")]
      public string Msg { get; set; }

      internal bool IsValid()
      {
         return Cmd == "send" && !string.IsNullOrEmpty(this.RoomId)
            && !string.IsNullOrEmpty(this.ClientId)
            && !string.IsNullOrEmpty(this.Msg);
      }

      internal string ResponseString()
      {
         string msg = JsonConvert.SerializeObject(this, Formatting.Indented);
         MsgTypeResponse typeResponse = new MsgTypeResponse
         {
            Msg = this.Msg,
            Type = ""

         };
         return JsonConvert.SerializeObject(typeResponse, Formatting.Indented); ;
      }
   }
}
