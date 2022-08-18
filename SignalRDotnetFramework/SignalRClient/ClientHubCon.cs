using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json.Linq;
using System;

namespace SignalRClientDotnetFramework
{
    public class ClientHubCon : HubConnection
    {
        HubConnection hubConnection;
        string toConnection = null;
        public ClientHubCon(string url) : base(url)
        {
            hubConnection = new HubConnection(url);
            
        }
        public void Connect(Delegate update)
        {
            var proxy = hubConnection.CreateHubProxy("MyHub");
            try
            {
                hubConnection.Start().Wait();
                proxy.On<string>("DisplayTime", time =>
                {
                    update.DynamicInvoke($"Con at {time}");
                });
                proxy.Invoke("ServerTime");
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
