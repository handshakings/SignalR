using System.Collections.Generic;
using System.IO;

namespace SignalRServer1
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


    }
}
