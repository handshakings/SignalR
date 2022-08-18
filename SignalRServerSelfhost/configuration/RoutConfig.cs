using System.Web.Http;

namespace SignalRServerSelfhost.configuration
{
    public class RoutConfig
    {
        public static void RegisterRoutes(HttpConfiguration route)
        {
            route.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate:"api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
        }
    }
}
