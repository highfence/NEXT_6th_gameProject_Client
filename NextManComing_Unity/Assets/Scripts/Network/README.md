# Network

MsgPack 사용 </br>

https://github.com/neuecc/MessagePack-CSharp </br>

Server에서는 Nuget Package로 다운로드. </br>

```
// 패키지 관리자 콘솔
Install-Package MessagePack

// Analyzer 인스톨
Install-Package MessagePackAnalyzer
```

Client에서는 Unity Package로 다운로드. </br>

```
https://github.com/neuecc/MessagePack-CSharp/releases
```

### 사용법

```Cpp
using MsgPack;

// 패킷 구조체 선언
[MessagepackObject]
public class ServerConnectReq
{
  [Key(0)]
  public string Id;

  [Key(1)]
  public Int64 Token;
}


// 실제로 사용할 때...
// 지정된 구조체를 byte[]로 Serialize해준다.
var bytes = MessagepackSerializer.Serialize(req);

// Deserialize의 경우.
var res = MessagepackSerializer.Deserialize<ServerConnectRes>(bytes);

// Serialize상태의 byte[]를 Json 형식으로 바꿔줄 수도 있다.
var json = MessagepackSerializer.ToJson(bytes);
```
