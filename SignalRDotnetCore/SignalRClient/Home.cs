using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SignalRClient
{
    public partial class Home : Form
    {
#nullable disable

        HubConnection hubConnection;
        string toConnection = null;
        string recFilePath = null;
        int recFileSize = 0;
        delegate void UpdateChatboxDelegate(string fromOrTo, string conId, string msg);
        private void UpdateChatbox(string sendORec, string conId, string msg)
        {
            Invoke(new Action(() =>
            {
                if(sendORec == "send")
                {
                    richTextBox1.AppendText(Environment.NewLine);
                    richTextBox1.AppendText(Environment.NewLine);
                    richTextBox1.SelectionStart = richTextBox1.TextLength;
                    richTextBox1.SelectionBackColor = Color.LightBlue;
                    richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                    richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Underline);
                    richTextBox1.SelectionIndent = 10;
                    richTextBox1.AppendText("Sent to " + conId);
                    richTextBox1.AppendText(Environment.NewLine);
                    richTextBox1.SelectionColor = Color.Green;
                    richTextBox1.SelectionIndent = 10;
                    richTextBox1.AppendText(msg);
                }
                else if(sendORec == "rec")
                {
                    richTextBox1.AppendText(Environment.NewLine);
                    richTextBox1.AppendText(Environment.NewLine);
                    richTextBox1.SelectionStart = richTextBox1.TextLength;
                    richTextBox1.SelectionBackColor = Color.LightPink;
                    richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                    richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Underline);
                    richTextBox1.SelectionIndent = 200;
                    richTextBox1.AppendText("Rec from " + conId);
                    richTextBox1.AppendText(Environment.NewLine);
                    richTextBox1.SelectionColor = Color.Red;
                    richTextBox1.SelectionIndent = 200;
                    richTextBox1.AppendText(msg);
                }   
            }));
        }

        public Home()
        {
            InitializeComponent();
        }

        private async void ConnectButton(object sender, EventArgs e)
        {
            string url = "https://localhost:5001/chathub";
            //string url = "http://127.0.0.1:4444";
            hubConnection = new HubConnectionBuilder().WithUrl(url).Build();
            
            hubConnection.On("ReceiveBroadcast", new Action<List<string>>(ReceiveBroadcastAction));
            hubConnection.On("ReceiveMessage", new Action<string,string>(ReceiveMessageAction));
            hubConnection.On("SwitchReceive", new Action<string,bool>(SwitchReceiveAction));
            hubConnection.On("ReceiveFileName", new Action<string,string,int>(ReceiveFileNameAction));
            hubConnection.On("ReceiveFile", new Action<string, byte[]>(ReceiveFileAction));


            await hubConnection.StartAsync();        
            if(hubConnection.State == HubConnectionState.Connected)
            {
                label1.Text = "Connected";
                button2.Enabled = false;
                button3.Enabled = true;
            }
        }

        private void ReceiveBroadcastAction(List<string> conId)
        {
            flowLayoutPanel1.Controls.Clear();
            foreach (var con in conId)
            {
                if (con != hubConnection.ConnectionId)
                {
                    Button button = new Button();
                    button.Text = con.ToString();
                    button.FlatStyle = FlatStyle.Popup;
                    button.BackColor = Color.Red;
                    button.Click += new EventHandler(SwitchConnection);
                    flowLayoutPanel1.Controls.Add(button);
                }
            }
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                if (c.Text == toConnection)
                {
                    c.BackColor = Color.Green;
                }
            }
            label2.Text = hubConnection.ConnectionId;
        }
        private void ReceiveMessageAction(string connectionId, string message) 
        {
            foreach(Control c in flowLayoutPanel1.Controls)
            {
                if(c.Text == connectionId)
                {
                    c.BackColor = Color.Green;
                    toConnection = connectionId;
                }
            }
            new UpdateChatboxDelegate(UpdateChatbox).DynamicInvoke("rec", connectionId, message);
        }
        private void SwitchReceiveAction(string from, bool flag)
        {
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                if (c.Text == from)
                {
                    c.BackColor = flag? Color.Green : Color.Red;
                    toConnection = flag? c.Text : null;
                }
            }
        }
        private void ReceiveFileNameAction(string from, string filePath, int fileSize)
        {
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                if (c.Text == from)
                {
                    c.BackColor = Color.Green;
                    toConnection = from;
                }
            }
            string recFileLocalPath = new Random().Next(1, 10).ToString() + "-" + Path.GetFileName(filePath);
            File.Create(recFileLocalPath).Dispose();
            recFilePath = recFileLocalPath;
            recFileSize = fileSize;
            string size = SizeSuffix(recFileSize,1);
            new UpdateChatboxDelegate(UpdateChatbox).DynamicInvoke("rec", from, recFileLocalPath+"("+size+")");
        }
        
        private void ReceiveFileAction(string from, byte[] bytes)
        {
            foreach (Control con in flowLayoutPanel1.Controls)
            {
                if (con.Text == from)
                {
                    con.BackColor = Color.Green;
                    toConnection = from;
                }
            }
            try
            {
                string recFilePathLocal = Path.GetFileName(recFilePath);
                FileStream fs = new FileStream(recFilePathLocal, FileMode.Append, FileAccess.Write);
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
                fs.Close();
                new UpdateChatboxDelegate(UpdateChatbox).DynamicInvoke("rec", from, bytes.Length.ToString());
            }
            catch (Exception ex)
            {
            }
        }

        private async void SwitchConnection(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.BackColor == Color.Green)
            {
                toConnection = null;
                await hubConnection.SendAsync("SwitchSend",btn.Text, false);
                btn.BackColor = Color.Red;
            }
            else
            {
                toConnection = btn.Text;
                await hubConnection.SendAsync("SwitchSend", btn.Text, true);
                btn.BackColor = Color.Green;
            }
        }

   

        private async void SendMessageOnKeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (hubConnection.State == HubConnectionState.Connected)
                {
                    foreach (Control c in flowLayoutPanel1.Controls)
                    {
                        if (c.BackColor == Color.Green)
                        {
                            await hubConnection.InvokeCoreAsync("SendMessage", new string[] { c.Text, textBox1.Text });
                            new UpdateChatboxDelegate(UpdateChatbox).DynamicInvoke("send", c.Text, textBox1.Text);
                        }
                    }
                }
            }
        }

        private async void SendFileButton(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            label3.Text = ofd.FileName;
            foreach (Control con in flowLayoutPanel1.Controls)
            {
                if (con.BackColor == Color.Green)
                {
                    await hubConnection.SendAsync("SendFileName", con.Text, label3.Text, new FileInfo(ofd.FileName).Length);
                    await Task.Delay(1000);

                    await hubConnection.SendAsync("SendFile", con.Text, ReadChunk(ofd.FileName));

                    new UpdateChatboxDelegate(UpdateChatbox).DynamicInvoke("send", con.Text, label3.Text);
                }
            }
        }
        private async IAsyncEnumerable<byte[]> ReadChunk(string path)
        {
            using var fsr = new FileStream(path, FileMode.Open, FileAccess.Read);
            long fileSize = new FileInfo(path).Length;
            long remainingBytes = fileSize;
            byte[] buffer = new byte[1024];
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            
            while (fs.Read(buffer, 0, buffer.Length) > 0)
            {
                yield return buffer;
            }
            fs.Close();
        }
        private async IAsyncEnumerable<string> ReadLine(string path)
        {
            using var fsr = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(fsr);
            while(reader.Peek() >= 0)
            {
                await Task.Delay(1000);
                yield return reader.ReadLine();
            }
        }

        private async void Disconnect(object sender, EventArgs e)
        {
            await hubConnection.StopAsync();
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                label1.Text = "Disconnected";
                button2.Enabled = true;
                button3.Enabled = false;
            }
        }
        public bool IsTextFile(string FilePath)
        {
            using (StreamReader reader = new StreamReader(FilePath))
            {
                int Character;
                while ((Character = reader.Read()) != -1)
                {
                    if ((Character > 0 && Character < 8) || (Character > 13 && Character < 26))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool IsBinary(string filePath, int requiredConsecutiveNul = 1)
        {
            const int charsToCheck = 8000;
            const char nulChar = '\0';

            int nulCount = 0;

            using (var streamReader = new StreamReader(filePath))
            {
                for (var i = 0; i < charsToCheck; i++)
                {
                    if (streamReader.EndOfStream)
                        return false;

                    if ((char)streamReader.Read() == nulChar)
                    {
                        nulCount++;

                        if (nulCount >= requiredConsecutiveNul)
                            return true;
                    }
                    else
                    {
                        nulCount = 0;
                    }
                }
            }

            return false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string filepath = null;
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                filepath = dlg.FileName;
            }
            string newfilepath = @"E://" + Path.GetFileName(filepath);


            long fileSize = new FileInfo(filepath).Length;
            long bytesSent = 0;
            long remainingBytes = fileSize;
            byte[] buffer = new byte[102400];
            FileStream fsr = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            
            while (remainingBytes > 0)
            {
                int sent = fsr.Read(buffer, 0, buffer.Length);
                if (sent == 0)
                    break;

                FileStream fsw = new FileStream(newfilepath, FileMode.Append, FileAccess.Write);
                fsw.Position = bytesSent;
                fsw.Write(buffer, 0, (int)new FileInfo(newfilepath).Length);
                fsw.Flush();
                fsw.Close();

                bytesSent += sent;
                remainingBytes -= sent;
            }
            fsr.Close();


        }





        readonly string[] SizeSuffixes ={ "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }

            int i = 0;
            decimal dValue = (decimal)value;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }
            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
        }

    }


    
}