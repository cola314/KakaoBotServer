var app = require('express')();
var server = require('http').createServer(app);
var io = require('socket.io')(server);

io.on('connection', function(socket) {
	    console.log('connect ' + socket.id);
	    socket.on('msg', data => {
		    console.log(data);
		            socket.emit('msg', data);
		        })
	    socket.on('disconnect', () => {
		            console.log('disconnect ' + socket.id);
		        })
});

server.listen(8484, function() {
	  console.log('Socket IO server listening on port 8484');
});
