using Microsoft.AspNetCore.SignalR;
using System;
using System.Diagnostics;

namespace app
{
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            Console.WriteLine(string.Format("Name User Id Hub Connection Name Thingy", connection.User?.Identity?.Name));
            Debug.WriteLine(string.Format("Name User Id Hub Connection Name Thingy", connection.User?.Identity?.Name));
            return connection.User?.Identity?.Name;
        }
    }
}