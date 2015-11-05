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
            irc.OnConnected += (Object o, EventArgs e) => { Console.WriteLine("Connected"); };
            irc.OnReadLine += (Object o, ReadLineEventArgs e) => {
                string command = e.Line.Split(' ')[0];
                if (command.Equals("PING")) { irc.WriteLine("Porra Doente!"); }
                else { Console.WriteLine(e.Line.ToString()); }
            };
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
                irc.Login("solteirao", "Galactic Overlord");
                //irc.x`Login("twentysevencentimeters", "Galactic Overlord");
                string channel = "#xupaflash";
                irc.RfcJoin(channel);
                for (int i = 0; i < 3; i++)
                {
                    //irc.SendMessage(SendType.Message, channel, "Message Porra (" + i.ToString() + ")");
                    //irc.SendMessage(SendType.Action, channel, "Action Doente (" + i.ToString() + ")");
                    //irc.SendMessage(SendType.Notice, channel, "Notice xupa! (" + i.ToString() + ")");
                }
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
            while (true)
            {
                string cmd = System.Console.ReadLine();
                if (cmd.StartsWith("/list"))
                {
                    int pos = cmd.IndexOf(" ");
                    string channel = null;
                    if (pos != -1)
                    {
                        channel = cmd.Substring(pos + 1);
                    }

                    IList<ChannelInfo> channelInfos = irc.GetChannelList(channel);
                    Console.WriteLine("channel count: {0}", channelInfos.Count);
                    foreach (ChannelInfo channelInfo in channelInfos)
                    {
                        Console.WriteLine("channel: {0} user count: {1} topic: {2}",
                                            channelInfo.Channel,
                                            channelInfo.UserCount,
                                            channelInfo.Topic);
                    }
                }
                else
                {
                    irc.WriteLine(cmd);
                }
            }
                
            irc.Disconnect();
        }
    }
}
