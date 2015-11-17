using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Meebey.SmartIrc4net;

namespace Parrot4Net
{
    public class Zombie
    {
        private static IrcClient irc;
        private static bool ircenabled = true;
        private static Thread irclisten;
        private static Dictionary<string, string> OpList;

        public Zombie() {
            irc = new IrcClient();
            irc.OnConnected += IrcConnected;
            irc.OnReadLine += OnReadLine;
            irc.OnChannelMessage += OnMessage;
            irc.OnOp += OnOp;
            irc.OnDeop += OnDeOp;
            irc.OnPart += OnPart;
            irc.OnNickChange += OnNickChange;
            irc.OnTopic += OnTopic;
            irc.OnTopicChange += OnTopicChange;
            irc.OnQuit += OnQuit;
            irc.OnKick += OnKick;
            irc.OnDisconnected += IrcDisconnected;
            irc.OnNames += OnNames;
            irc.AutoRejoinOnKick = true;
            irc.AutoRejoin = true;
        }

        public void ConnectAndListen(string ircserver, int ircport, string ircchannel) {
            try
            {
                irc.Connect(ircserver, ircport);
                irc.Login("HIR_" + Utils.RandomString(), "Hirata Zombie", 0, "IRCHIR");
                irc.RfcJoin(ircchannel);
                // Spawn a thread to handle the listen.
                irclisten = new Thread(new ThreadStart(IrcListenThread));
                irclisten.Start();
            }
            catch (ConnectionException e)
            {
                Console.WriteLine("couldn't connect! Reason: " + e.Message);
                Exit();
            }
        }

        private void IrcConnected(object sender, EventArgs e)
        {
            Console.WriteLine("Connected...");
        }

        private void OnReadLine(object sender, ReadLineEventArgs e)
        {
            string command = e.Line.Split(' ')[0];
            if (command.Equals("PING"))
            {
                string server = e.Line.Split(' ')[1];
                irc.WriteLine("PONG " + server, Priority.Critical);
            }
            else if (command.Equals("422") || command.Equals("376")) // 422: motd missing // 376: end of motd
            {
                if (OpList != null) OpList.Clear();
            }
            else
            {
                Console.WriteLine(">>> " + e.Line);
            }
        }

        private void OnMessage(object sender, IrcEventArgs e)
        {
            Console.WriteLine("Got message " + e.Data.RawMessage);
        }

        private void OnOp(object sender, OpEventArgs e)
        {
            Console.WriteLine("Became operator " + e.Whom);
        }

        private void OnDeOp(object sender, DeopEventArgs e)
        {
            Console.WriteLine("Lost Operator Privilege " + e.Data.RawMessage);
        }

        private void OnPart(object sender, PartEventArgs e)
        {
            Console.WriteLine("Got Part " + e.PartMessage);
        }

        private void OnNickChange(object sender, NickChangeEventArgs e)
        {
            Console.WriteLine("Changed Nick " + e.Data.RawMessage);
        }

        private void OnTopic(object sender, TopicEventArgs e)
        {
            Console.WriteLine("On Topic " + e.Data.RawMessage);
        }

        private void OnTopicChange(object sender, TopicChangeEventArgs e)
        {
            Console.WriteLine("On Topic Change " + e.Data.RawMessage);
        }

        private void OnQuit(object sender, QuitEventArgs e)
        {
            Console.WriteLine("Quit " + e.Data.RawMessage);
        }

        private void OnKick(object sender, KickEventArgs e)
        {
            Console.WriteLine("Kick " + e.Data.RawMessage);
        }

        private void IrcDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected...");
        }

        private void OnNames(object sender, NamesEventArgs e)
        {
            Console.WriteLine("Names " + e.Data.RawMessage);
        }

        private void IrcListenThread()
        {
            while (ircenabled)
            {
                irc.Listen();
            }
        }

        public static void Exit()
        {
            // we are done, lets exit...
            Console.WriteLine("Exiting...");
            Environment.Exit(0);
        }

    }
}
