using Microsoft.Owin.Hosting;
using System;

namespace SignalRServerSelfhost
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:8080";
            WebApp.Start<Startup>(url);
            {
                Console.WriteLine($"Server started at {url} on {DateTime.UtcNow:T}");
                Console.ReadLine();
            }
        }
    }
}
