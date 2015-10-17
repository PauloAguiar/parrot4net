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
        public static IrcClient irc = new IrcClient();

        static void Main(string[] args)
        {
            irc = new IrcClient();
            irc.OnConnected += (Object o, EventArgs e) => { Console.WriteLine("Connecting"); };
            irc.OnReadLine += (Object o, ReadLineEventArgs e) => { irc.WriteLine("I'm alive"); };
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
            string server = "192.168.1.9";
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

            try
            {
                irc.Login("SmartIRC", "SmartIrc4net Test Bot");
                irc.RfcJoin(channel);
                new Thread(new ThreadStart(ReadCommands)).Start();
                irc.Listen();
                Console.WriteLine("Disconnected");
                Exit();
            }
            catch (ConnectionException)
            {
                Console.WriteLine("Connection error");
                Exit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occurred! Message: " + e.Message);
                Console.WriteLine("Exception: " + e.StackTrace);
                Exit();
            }

        }


        public static void Exit()
        {
            // we are done, lets exit...
            Console.WriteLine("Exiting...");
            Environment.Exit(0);
        }

        public static void ReadCommands()
        {
            for (int i = 1; i < 4; i++)
            {
                irc.WriteLine(i.ToString() + ": " + Console.ReadLine());
            }
            irc.Disconnect();
        }
    }
}
