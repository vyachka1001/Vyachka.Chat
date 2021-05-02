using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        public string IP { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }

        private Socket chatSocket;
        private bool isSendBtnSet = false;
        string message = "";

        public Chat(string ip, string port, string name)
        {
            InitializeComponent();
            IP = ip;
            Port = port;
            UserName = name;
            CreateClientSocket();
        }

        private void Send_btn_Click(object sender, RoutedEventArgs e)
        {
            isSendBtnSet = true;
            Thread.Sleep(1000);
            Chat_textBlock.Text += "\n" + message;
            Message_textBox.Text = "";
        }

        private void CreateClientSocket()
        {
            IPAddress ipAddress = IPAddress.Parse(IP);
            int port = int.Parse(Port);

            IPEndPoint ipPoint = new IPEndPoint(ipAddress, port);
            chatSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            chatSocket.Connect(ipPoint);
            byte[] byteName = Encoding.UTF8.GetBytes(UserName);
            chatSocket.Send(byteName);

            Thread typing = new Thread(CommunicateWithServer);
            typing.Start();

            /*ChatSocket.Shutdown(SocketShutdown.Both);
            ChatSocket.Close();*/
        }

        private void CommunicateWithServer()
        {
            do
            {
               if (isSendBtnSet && message != "")
               {
                    byte[] outputData = Encoding.UTF8.GetBytes(message);
                    isSendBtnSet = false;
                    chatSocket.Send(outputData);

                    int bytesRead;
                    do
                    {
                        bytesRead = chatSocket.Receive(outputData);
                        message = Encoding.UTF8.GetString(outputData, 0, bytesRead);
                        message = DateTime.Now.ToShortTimeString() + " " + UserName + ": " + message;
                    } while (bytesRead > 0);
               }
            }
            while (true);
        }

        private void Message_textBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (Message_textBox.Text != "")
            {
                message = Message_textBox.Text;
            }
        }
    }
}
