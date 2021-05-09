using MsgProcessor;
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
        private Thread handlerProc;
        private Socket chatSocket;
        private bool isClosing = false;

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
            string toSend = $"[{DateTime.Now.ToShortTimeString()}] {UserName}: {Message_textBox.Text}\n";
            Processor.Send(toSend, chatSocket);
            Message_textBox.Clear();
        }

        private void CreateClientSocket()
        {
            IPAddress ipAddress = IPAddress.Parse(IP);
            int port = int.Parse(Port);

            IPEndPoint ipPoint = new IPEndPoint(ipAddress, port);
            chatSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                chatSocket.Connect(ipPoint);
            }
            catch(SocketException ex)
            {
                MessageBox.Show($"It is impossible to connect to the server. {ex.Message}", "Connection error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (ObjectDisposedException ex)
            {
                MessageBox.Show($"It is impossible to connect to the server. {ex.Message}", "Connection error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            handlerProc = new Thread(ReceivingFromServer);
            handlerProc.Start();

            Processor.Send(UserName, chatSocket);
        }

        private void ReceivingFromServer(object obj)
        {
            string data;
            while ((data = Processor.Receive(chatSocket)) != null)
            {
                Dispatcher.Invoke(() =>
                {
                    Chat_textBlock.Text += data;
                    Chat_textBlock.Focus();
                    Chat_textBlock.CaretIndex = Chat_textBlock.Text.Length;
                }
                );
            }
            if (!isClosing)
            {
                MessageBox.Show("Server was disconnected.", "Server error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isClosing = true;
            chatSocket.Shutdown(SocketShutdown.Send);
            handlerProc.Join();
        }

        private void Reconnect_btn_Click(object sender, RoutedEventArgs e)
        {
            CreateClientSocket();
        }

        private void Message_textBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Send_btn_Click(sender, e);
            }
        }
    }
}
