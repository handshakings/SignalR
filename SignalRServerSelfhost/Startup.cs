
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using SignalRServerSelfhost.configuration;
using System.Web.Http;

namespace SignalRServerSelfhost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration configuration = new HttpConfiguration();
            RoutConfig.RegisterRoutes(configuration);
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
            //app.Map("signalr", map =>
            //{
            //    HubConfiguration cfg = new HubConfiguration();
            //    //map.RunSignalR(cfg);
            //});
        }
    }
}
