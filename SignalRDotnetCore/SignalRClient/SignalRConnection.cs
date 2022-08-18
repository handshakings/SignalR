using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRClient
{
#nullable disable
    public class SignalRConnection
    {
        public HubConnection connection { get; set; }
        public int currentClient { get; set; }  
    }
}
