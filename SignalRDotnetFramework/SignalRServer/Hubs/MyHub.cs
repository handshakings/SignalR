using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRServerDotnetFramework.Hubs
{
    public class MyHub : Hub
    {
        public void ServerTime()
        {
            Clients.All.DisplayTime($" {DateTime.UtcNow:T}");
        }


    }
}
