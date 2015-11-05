var Server = require('ircdjs').Server


var server = new Server();

server.showLog = true;
server.config = {
    'network': 'ircn',
    'hostname': 'localhost',
    'serverDescription': 'A Node IRC daemon',
    'serverName': 'server2',
    'port': 6667,
    'linkPort': 7777,
    'whoWasLimit': 10000,
    'token': 1,
    'opers': {},
    'links': {},
    "maxNickLength": 20,
    "opers": {
        "Paulo": {}
    },
    //'serverPassword': '$2a$10$T1UJYlinVUGHqfInKSZQz./CHrYIVVqbDO3N1fRNEUvFvSEcshNdC'
};

server.start(function(err) {
    if(err)
        return console.log(err);
    console.log('Listening at ' + server.host + ':' + 6667);
    console.log(server.name);
    console.log(server.info);
});