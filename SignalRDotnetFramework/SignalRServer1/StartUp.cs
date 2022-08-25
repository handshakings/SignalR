using Microsoft.Owin.Cors;
using Owin;
using System.IO;

namespace SignalRServer1
{
    public class StartUp
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.UseCors(CorsOptions.AllowAll);
            builder.MapSignalR();
            if(!File.Exists("filterKeywords.txt"))
            {
                File.Create("filterKeywords.txt").Dispose();
            }
            if(!File.Exists("filterKeywords.txt"))
            {
                File.Create("fileFormats.txt").Dispose();
            }
        }
    }
}


