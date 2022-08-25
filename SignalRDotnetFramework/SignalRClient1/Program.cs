using System.Windows.Forms;

namespace SignalRClient1
{
    internal static class Program
    {
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string url = "http://127.0.0.1:4444";
            ClientHub connection = new ClientHub(url);
            connection.Connect();
            Application.Run();
        }
    }
}
