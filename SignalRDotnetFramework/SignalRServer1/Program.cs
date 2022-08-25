using Microsoft.Owin.Hosting;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SignalRServer1
{
    internal static class Program
    {
        [STAThread]
        static async Task Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string url = "http://127.0.0.1:4444";
            WebApp.Start<StartUp>(url);
            Application.Run();
        }
    }
}
