using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RemoteReceiver
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            Server server = new Server();
        }       
    }

    public class Server
    {
        private TcpListener tcplistener;
        private Thread listenThread;
        private static int portNum = 6789;

        public Server()
        {
            this.tcplistener = new TcpListener(IPAddress.Any, portNum);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }

        private void ListenForClients()
        {
            tcplistener.Start();

            while (true)
            {
                TcpClient client = tcplistener.AcceptTcpClient();

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object client)
        {
            System.Media.SoundPlayer myPlayer = new System.Media.SoundPlayer();
            myPlayer.SoundLocation = "..\\Sounds\\Notify.wav";
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try { bytesRead = clientStream.Read(message, 0, 4096); }
                catch { break; }

                if (bytesRead == 0) break;

                ASCIIEncoding encoder = new ASCIIEncoding();
                System.Diagnostics.Debug.WriteLine(encoder.GetString(message, 0, bytesRead));
                //Send this to tooltipper
                Process.Start("tooltipper.exe",encoder.GetString(message,0,bytesRead));
                myPlayer.Play();
            }

            tcpClient.Close();
        }
    }
}
