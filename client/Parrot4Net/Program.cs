using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Meebey.SmartIrc4net;

namespace Parrot4Net
{
    class Program
    {
        static void Main(string[] args)
        {
            string server = "192.168.1.3";
            int port = 6667;
            string channel = "#bots";
            Zombie z = new Zombie();
            z.ConnectAndListen(server, port, channel);
        }
    }
}
