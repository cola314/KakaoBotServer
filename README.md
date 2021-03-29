서버로 보내는 소켓
------
* 이벤트명 : "register"
```
{
   "password" : "4321"
}
```
맨처음 한번 등록해준다. 위의 json데이터를 보내야 한다.  
register를 안하거나 비밀번호 틀릴 경우 아래 프로토콜이 동작하지 않는다.

* 이벤트명 : "send message"
```
{
   "room" : "hello",
   "msg" : "ㅎㅇ"
}
```
메시지를 보내는 프로토콜

서버에서 오는 소켓
--------------
* 이벤트명 : "receive message"
```
{
   "sender" : "user1"
   "room" : "room1",
   "msg" : "ㅎㅇㅎㅇ",
   "isGroupChat" : false
}
```
카톡이 올때마다 서버로부터 날아오는 이벤트이다.

커스텀 서버 예제
-----------
```javascript
const address = "http://localhost:9200";
const io = require("socket.io-client");
const ioClient = io.connect(address);
console.log('start')

const registerData = {
    "password" : "4321"
}

ioClient.emit("register", registerData);

ioClient.on("receive message", (data) => {
    console.log(data);
    const res = {
        "room" : data.room,
        "msg" : data.msg
    }
    ioClient.emit("send message", res);
});
```