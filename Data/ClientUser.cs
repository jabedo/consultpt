using System;
namespace app.Models
{


    public class ClientUser
    {
        public String Name { get; set; }
        public string Username { get; set; }
        public string ConnectionId { get; set; }
        public bool IsAvailable { get; set; }
        public bool InCall { get; set; }
        public UserType UserType { get; set; }
        public bool IsProviderAvailable { get; internal set; }
        public string RoomId { get; internal set; }
        public string ClientId { get; internal set; }
    }

}
