using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meebey.SmartIrc4net;

namespace Parrot4Net
{
    class Program
    {
        

        static void Main(string[] args)
        {

            var irc = new IrcClient();
            irc = new IrcClient();
            irc.OnConnected += (Object o, EventArgs e) => { Console.WriteLine("Connectando"); };
            //irc.OnReadLine += OnReadLine;
            //irc.OnChannelMessage += OnMessage;
            //irc.OnOp += OnOp;
            //irc.OnDeop += OnDeOp;
            //irc.OnPart += OnPart;
            //irc.OnNickChange += OnNickChange;
            //irc.OnTopic += OnTopic;
            //irc.OnTopicChange += OnTopicChange;
            //irc.OnQuit += OnQuit;
            //irc.OnKick += OnKick;
            //irc.OnDisconnected += IrcDisconnected;
            //irc.OnNames += OnNames;
            irc.AutoRejoinOnKick = true;
            irc.AutoRejoin = true;
            string server;
            server = "192.168.1.9";
            int port = 6667;
            string channel = "#smartirc-test";
            try
            {
                irc.Connect(server, port);
            }
            catch (ConnectionException e) {
                Console.WriteLine("couldn't connect! Reason: "+e.Message);
                Exit();
            }
        }


        public static void Exit()
        {
            // we are done, lets exit...
            System.Console.WriteLine("Exiting...");
            System.Environment.Exit(0);
        }
    }
}
