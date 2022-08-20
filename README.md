# KakaoBotServer
**카톡 챗봇 메시지 전송 서버**
- KakaoBotClient와 KakaoBotManager 사이에서 메시지 전송을 담당하는 서버

<div align=center><h2>📚 STACKS</h2></div>

<div align=center>
  <img src="https://img.shields.io/badge/c%23-%23512BD4.svg?style=for-the-badge&logo=c-sharp&logoColor=white">
  <img src="https://img.shields.io/badge/Visual%20Studio%202022-5C2D91.svg?style=for-the-badge&logo=visual-studio&logoColor=white">
  <br/>
  <img src="https://img.shields.io/badge/ASP.NET%20Core-5C2D91?style=for-the-badge&logo=.net&logoColor=white">
  <img src="https://img.shields.io/badge/GRPC-4285F4?style=for-the-badge&logo=google&logoColor=white"> 
  <img src="https://img.shields.io/badge/redis-%23DD0031.svg?style=for-the-badge&logo=redis&logoColor=white"> 
  <br/>
  <img src="https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white"> 
  <img src="https://img.shields.io/badge/github%20actions-%232671E5.svg?style=for-the-badge&logo=githubactions&logoColor=white">  
  <br>
</div>

## 프로젝트 구조
```
KakaoBotServer
  ├─KakaoBotConsoleClient
  │  └─Protos
  ├─KakaoBotConsoleManager
  │  └─Model
  └─KakaoBotServer
      ├─Config
      ├─GrpcServices
      ├─Model
      ├─Properties
      ├─Protos
      └─Service
```

#### KakaoBotConsoleClient
- 콘솔 애플리케이션
- KakaoBotClient 역할을 하는 Mock 클라이언트
- 콘솔 입력으로 메시지를 보내고 푸시 메시지가 콘솔 출력으로 표시됨

### KakaoBotConsoleManager
- 콘솔 애플리케이션
- KakaoBotManager 역할을 하는 Mock 서버
- KakaoBotServer에서 보낸 메시지를 그대로 KakaoBotServer로 보냄(echo)

### KakaoBotServer
- ASP.NET Core gRPC 서버
- KakaoBotClient와 KakaoBotManager 사이에서 메시지 전송을 담당하는 서버