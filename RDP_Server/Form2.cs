using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace RDP_Server
{
    public partial class Form2 : Form
    {
        private readonly int port;
        private TcpClient client;
        private TcpListener server;
        private NetworkStream ns;

        private readonly Thread Listening;
        private readonly Thread GetImage;
        public Form2(int Port)
        {
            port = Port;
            client = new TcpClient();
            Listening = new Thread(StartListening);
            GetImage = new Thread(ReceiveImage);
            InitializeComponent();
        }
        private void StartListening()
        {
            while (!client.Connected)
            {
                server.Start();
                client = server.AcceptTcpClient();
            }
            GetImage.Start();
        }
        private void StopListening()
        {
            server.Stop();
            client = null;
            if (Listening.IsAlive) Listening.Abort();
            if (GetImage.IsAlive) GetImage.Abort();

        }

        private void ReceiveImage()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            while (client.Connected)
            {
                ns = client.GetStream();
                pictureBox1.Image = (Image)binaryFormatter.Deserialize(ns);
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            server = new TcpListener(IPAddress.Any,port);
            Listening.Start();
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            StopListening();
        }
    }
}
