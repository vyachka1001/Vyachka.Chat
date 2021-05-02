using System.Net.Sockets;

namespace Vyachka.Chat.Server
{
    public class ClientData
    {
        public Socket ClientSocket { get; set; }
        public string Name { get; set; }

        public ClientData(Socket socket, string name)
        {
            ClientSocket = socket;
            Name = name;
        }
    }
}
