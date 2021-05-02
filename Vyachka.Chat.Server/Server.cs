using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Vyachka.Chat.Server
{
    public class Server
    {
        private static ArrayList clients = new ArrayList();

        static void Main(string[] args)
        {
            string hostName = Dns.GetHostName();
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            
            foreach (var ip in Dns.GetHostEntry(hostName).AddressList)
            {
                if (!ip.ToString().Contains(":") && !ip.ToString().Equals("127.0.0.1"))
                {
                    ipAddress = ip;
                }
            }

            int port = int.Parse(GetData("port"));
            Console.WriteLine($"IP address: {ipAddress}");

            IPEndPoint ipPoint = new IPEndPoint(ipAddress, port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipPoint);
            socket.Listen(10);

            while (true)
            {
                Console.WriteLine("Server is waiting for connections...");
                Socket clientSocket = socket.Accept();
                if (clientSocket != null)
                {
                    clients.Add(clientSocket);
                    byte[] byteName = new byte[256];
                    int bytes = clientSocket.Receive(byteName);
                    string name = Encoding.UTF8.GetString(byteName, 0, bytes);
                    var data = new ClientData(clientSocket, name);

                    Thread thread = new Thread(CommunicateWithClient);
                    thread.Start(data);
                    thread.Join();

                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
        }

        static void CommunicateWithClient(object objData)
        {
            ClientData data = (ClientData)objData;
            Console.WriteLine($"{data.Name} connected");
            byte[] inputData = new byte[256];
            int bytesRead;
            do
            {
                bytesRead = data.ClientSocket.Receive(inputData);
                string text = Encoding.UTF8.GetString(inputData, 0, bytesRead);
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} {data.Name}: {text}");
                foreach(Socket client in clients)
                {
                    inputData = Encoding.UTF8.GetBytes(text);
                    client.Send(inputData);
                }
            }
            while (bytesRead > 0);
        }

        private static string GetData(string type)
        {
            string value = "";
            bool isValid = false;
            while (!isValid)
            {
                Console.Write($"Please, enter {type}: ");
                value = Console.ReadLine();
                switch (type)
                {
                    case "port":
                        isValid = int.TryParse(value, out _);
                        break;

                    case "IP":
                        isValid = IPAddress.TryParse(value, out _);
                        break;
                }

                if (!isValid)
                {
                    Console.WriteLine($"Error with {type} value!");
                }
            }

            return value;
        }
    }
}