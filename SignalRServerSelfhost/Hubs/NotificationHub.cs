using Microsoft.AspNet.SignalR;
using System;

namespace SignalRServerSelfhost.Hubs
{
    public class NotificationHub : Hub
    {
        public void ServerTime()
        {
            Clients.All.DisplayTime($" {DateTime.UtcNow:T}");
        }
    }
}
