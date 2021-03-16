var express = require('express');
var app = require('express')();
var http = require('http').createServer(app);
var io = require('socket.io')(http);

var socketDict = {};

// path control
app.use(express.static('public'));
app.use(express.json());

app.post('/send', (req, res) => {
    const data = req.body;
    console.log(data);
    console.log(socketDict);
    if(data.password == "4321") {
        let id = socketDict[data.room];
        if(id != undefined) {
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
	
    socket.on('request message', (data) => {
        console.log(data);
        chatInfo = JSON.parse(data);
        res = {
            "room" : chatInfo.room,
            "msg" : chatInfo.msg
        };
        socketDict[res.room] = socket.id;
        console.log(JSON.stringify(res));
        socket.emit('push message', JSON.stringify(res));
    });
	
    socket.on('msg', (msg) => {
        console.log(msg);
        socket.emit('msg', msg);
    });

    //user out
    socket.on('disconnect', () => {
        console.log(socket.id + 'user disconnected');
    });
});

http.listen(9200, () => {
  console.log('Server start on 9200');
});
