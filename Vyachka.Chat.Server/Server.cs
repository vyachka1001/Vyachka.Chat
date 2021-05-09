using MsgProcessor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Vyachka.Chat.Server
{
    public class Server
    {
        private static List<ClientData> clients = new List<ClientData>();

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
                Socket clientSocket = socket.Accept();
                if (clientSocket != null)
                {
                    string name = Processor.Receive(clientSocket);
                    ClientData data = new ClientData(clientSocket, name);
                    Processor.Send($"Hi {name}!\n", clientSocket);
                    Console.WriteLine($"{data.Name} connected");
                    clients.Add(data);

                    Thread thread = new Thread(CommunicateWithClient);
                    thread.Start(data);
                }
            }
        }

        static void CommunicateWithClient(object objData)
        {
            ClientData clientData = (ClientData) objData;

            string input;
            while ((input = Processor.Receive(clientData.ClientSocket)) != null)
            {
                Console.Write(input);
                foreach(ClientData client in clients)
                {
                    Processor.Send(input, client.ClientSocket);
                }
            }
            
            clientData.ClientSocket.Shutdown(SocketShutdown.Both);
            clientData.ClientSocket.Close();
            clients.Remove(clientData);
            Console.WriteLine($"{clientData.Name} disconnected.");
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