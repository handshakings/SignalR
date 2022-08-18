using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SignalRServer
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            //await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
            ConnectedUsers.ConnectedUserIds.Add(Context.ConnectionId);
            await Clients.All.SendAsync("ReceiveBroadcast", ConnectedUsers.ConnectedUserIds);
        }


        public async Task SendMessage(string to, string message) => 
            await Clients.Client(to).SendAsync("ReceiveMessage", Context.ConnectionId, message);

        public async Task SendFileName(string to, string filePath, int fileSize) =>
            await Clients.Client(to).SendAsync("ReceiveFileName", Context.ConnectionId, filePath, fileSize);
        public async Task SendFile(string to, IAsyncEnumerable<byte[]> byteList)
        {
            await foreach (var byteListItem in byteList)
            {
                await Clients.Client(to).SendAsync("ReceiveFile", Context.ConnectionId, byteListItem);
            }  
        }
        public async Task SendFile1(IAsyncEnumerable<string> lines)
        {
            try
            {
                await foreach (var line in lines)
                {
                    Console.WriteLine(line);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            //await Clients.Client(to).SendAsync("ReceiveFile", Context.ConnectionId, byteFile, byteSent);
        }


        public async Task SwitchSend(string to, bool flag)
        {
            if (flag)
            {
                await Clients.Client(to).SendAsync("SwitchReceive",Context.ConnectionId,true);
            }
            else
            {
                await Clients.Client(to).SendAsync("SwitchReceive", Context.ConnectionId, false);
            }
        }




        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            ConnectedUsers.ConnectedUserIds.Remove(Context.ConnectionId);
            await Clients.All.SendAsync("ReceiveBroadcast", ConnectedUsers.ConnectedUserIds);
        }


    }
}
