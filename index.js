var express = require('express');
var app = require('express')();
var http = require('http').createServer(app);
var io = require('socket.io')(http);

var customServers = [];
var clientId = null;

// path control
app.use(express.static('public'));
app.use(express.json());

app.post('/send', (req, res) => {
    const data = req.body;
    console.log(data);
    console.log(socketDict);
    if(data.password == "4321") {
        let id = clilentId;
        if(id != null) {
            io.to(id).emit('push message', JSON.stringify({"room" : data.room, "msg" : data.message}));
            return res.sendStatus(200);
        } 
    }
    return res.sendStatus(400);
});

app.get('/', (req, res) => {
  res.sendFile(__dirname + '/public/main.html');
});

//for game communication
io.on('connection', (socket) => {
    //user_come
    console.log(socket.id + 'user connected');

    socket.on('register client', () => {
        console.log('register client ' + socket.id);
        clientId = socket.id;
    });

    socket.on('request message', (data) => {
        console.log(data);
        chatInfo = JSON.parse(data);
        res = {
            "sender" : chatInfo.sender,
            "room" : chatInfo.room,
            "msg" : chatInfo.msg,
            "isGroupChat" : chatInfo.isGroupChat
        };
        console.log(JSON.stringify(res));
        socket.broadcast.emit('receive message', res);
        //socket.emit('push message', JSON.stringify(res));
    });
	
    //for test
    socket.on('msg', (msg) => {
        console.log(msg);
        socket.emit('msg', msg);
    });

    //custom server register
    socket.on('register', (data) => {
        console.log(JSON.stringify(data) + ' ' + socket.id);
        try {
            if(data.password === "4321") {
                if(customServers.indexOf(socket.id) === -1) {
                    customServers.push(socket.id);
                }
            }
        }
        catch(err) {
            console.error(err);
        }
    });

    //custom server send message
    socket.on('send message', (data) => {
        console.log(data);
        try {
            if(customServers.indexOf(socket.id) !== -1) {
                if(clientId != null) {
                    sendData = {
                        "room" : data.room,
                        "msg" : data.msg
                    };
                    io.to(clientId).emit('push message', JSON.stringify(sendData));
                    console.log('push success ' + clientId);
                }
            } else {
                console.log('Permission Denied ' + socket.id);
            }
        }
        catch(err) {
            console.error(err);
        }
    });

    //user out
    socket.on('disconnect', () => {
        console.log(socket.id + 'user disconnected');
    });
});

http.listen(9200, () => {
  console.log('Server start on 9200');
});
