const address = "http://localhost:9200";
const io = require("socket.io-client");
const ioClient = io.connect(address);
console.log('start')

const registerData = {
    "password" : "4321"
}

ioClient.on('connect', (data) => {
    ioClient.emit("register", registerData);
});

ioClient.on("receive message", (data) => {
    console.log(data);
    const res = {
        "room" : data.room,
        "msg" : data.msg
    }
    ioClient.emit("send message", res);
});