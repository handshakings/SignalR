using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SignalRServer;
using System;

//What is <FrameworkReference Include="Microsoft.AspNetCore.App" />
//FrameworkReference means that it is not downloaded from nuget.org. Rather assmblies available
//in this reference are included at runtime as SignalR is by default included in it so we need 
//not to download signalr nuget package
//On the other hand, packageReference is downloaded from nuget.org

WebApplicationBuilder builder = WebApplication.CreateBuilder();
builder.Services.AddSignalR(option =>
{
    option.MaximumReceiveMessageSize = null; //null means no limit on maximum data/file size
    //option.KeepAliveInterval = TimeSpan.FromMinutes(1);
    option.MaximumReceiveMessageSize = 1024000;  //default is 32KB
    option.StreamBufferCapacity = 1024000;
});
WebApplication app = builder.Build();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chathub", option =>
    {
        option.ApplicationMaxBufferSize = 0; //0 means no limit
        option.TransportMaxBufferSize = 0; 
    });
});
app.Run();



//namespace SignalRServer
//{
//    public class Program
//    {
//        static void Main(string[] args)
//        {
//            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build().Run();
//        }
//    }

//    public class Startup
//    {
//        public IConfiguration configuration { get; set; }
//        public Startup(IConfiguration configuration)
//        {
//            this.configuration = configuration;
//        }
//        public void ConfigureServices(IServiceCollection services)
//        {
//            services.AddSignalR();
//        }
//        public void Configure(IApplicationBuilder app)
//        {
//            app.UseRouting();
//            app.UseEndpoints(endpoints =>
//            {
//                endpoints.MapHub<ChatHub>("/chathub");
//            });
//        }


//    }
//}
