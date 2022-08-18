using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;

namespace SignalRServerDotnetFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://127.0.0.1:4444";
            WebApp.Start<Startup>(url);

            Console.ReadLine();
        }
    }
    class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.UseCors(CorsOptions.AllowAll);
            builder.MapSignalR();
        }

    }
}