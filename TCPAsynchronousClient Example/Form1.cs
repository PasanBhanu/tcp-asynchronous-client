using System;
using System.Windows.Forms;
using TCPAsynchronousClient;

namespace TCPAsynchronousClient_Example
{
    public partial class Form1 : Form
    {
        // For Display Data in Text Box and Info
        public delegate void AddLogDeligate(string data);
        public AddLogDeligate AddLog;
        public delegate void AddNotificationDelegate(int type, bool status);
        public AddNotificationDelegate UpdateStatusIcons;

        // Client Object
        AsynchronousClient tcp;

        public Form1()
        {
            InitializeComponent();
            AddLog = new AddLogDeligate(Log);
            UpdateStatusIcons = new AddNotificationDelegate(StatusUpdate);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            tcp = new AsynchronousClient(txtIpAddress.Text, int.Parse(txtPort.Text));
            tcp.OnConnectEvent += new AsynchronousClient.OnConnectEventHandler(OnConnect);
            tcp.OnDataRecievedEvent += new AsynchronousClient.DataReceivedEventHandler(OnRecieved);
            tcp.Connect();
        }

        private void OnConnect(bool status)
        {
            rtfLog.Invoke(AddLog, "Connection : " + status.ToString());
            lblConnected.Invoke(UpdateStatusIcons, 1, status);
        }

        private void OnRecieved(string data)
        {
            lblRead.Invoke(UpdateStatusIcons, 3, true);
            rtfLog.Invoke(AddLog, "Recieved : " + data);
        }


        private void StatusUpdate(int type, bool status)
        {
            switch (type)
            {
                case 1:
                    if (status)
                    {
                        lblConnected.Text = "Connected";
                        lblConnected.BackColor = System.Drawing.Color.Green;
                        btnConnect.Enabled = false;
                        btnDisconnect.Enabled = true;
                        btnWrite.Enabled = true;
                    }
                    else
                    {
                        lblConnected.Text = "Disconnected";
                        lblConnected.BackColor = System.Drawing.Color.Red;
                        btnConnect.Enabled = true;
                        btnDisconnect.Enabled = false;
                        btnWrite.Enabled = false;
                    }
                    break;

                case 2:
                    if (status)
                    {
                        lblWrite.Visible = true;
                    }
                    else
                    {
                        lblWrite.Visible = false;
                    }
                    break;

                case 3:
                    if (status)
                    {
                        lblRead.Visible = true;
                    }
                    else
                    {
                        lblRead.Visible = false;
                    }
                    break;

                default:
                    break;
            }
        }

        private void Log(string _data)
        {
            rtfLog.Text = rtfLog.Text + _data + "\n\n";
            rtfLog.SelectionStart = rtfLog.Text.Length;
            rtfLog.ScrollToCaret();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            tcp.Disconnect();
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            StatusUpdate(2, true);
            if (tcp.Write(txtWrite.Text))
            {
                Log("Write (Success) : " + txtWrite.Text);
            }
            else
            {
                Log("Write (Failed) : Disconnected");
            }
        }

        private void timClear_Tick(object sender, EventArgs e)
        {
            // Clear Notifications
            lblWrite.Visible = false;
            lblRead.Visible = false;
        }
    }
}
