using System;
using System.Drawing;
using System.Windows.Forms;

namespace SignalRClientDotnetFramework
{
    public partial class Home : Form
    {
        ClientHubCon clientHub;

        delegate void UpdateLabelDelegate(string con);
        private void UpdateLabel(string con)
        {
            Invoke(new Action(() => label1.Text = con));
        }

        delegate void UpdateChatboxDelegate(string fromOrTo, string conId, string msg);
        private void UpdateChatbox(string sendORec, string conId, string msg)
        {
            Invoke(new Action(() =>
            {
                if (sendORec == "send")
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
                else if (sendORec == "rec")
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
        private void Connect(object sender, EventArgs e)
        {
            string url = "http://127.0.0.1:4444";
            clientHub = new ClientHubCon(url);
            clientHub.Connect(new UpdateLabelDelegate(UpdateLabel));
        }
        private void Disconnect(object sender, EventArgs e)
        {
            
        }

        private void SendMessage(object sender, EventArgs e)
        {
            
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}
