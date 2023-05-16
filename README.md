# mirrorgear-mercury
## 프로젝트 소개
MirrorGearSDK Mercury Engine 은 메타버스 네트워크 서비스를 제공합니다.

특히 동시 접속, 채팅 그리고 공유 오브젝트 관리하는 기능을 구현하는 강력한 소프트웨어 개발 도구입니다.

이 소프트웨어 개발 도구는 `unitypackage` 형태로 배포되며, 2021.3.5 버전 이상의 유니티 엔진을 지원합니다.

### 핵심 기능
이 패키지를 사용하면 Unity 엔진을 사용하여 메타버스 클라이언트 기능을 구현할 수 있습니다.

제공하는 메타버스 기능은 다음과 같습니다.

1. 20명 이상의 유저에게 동시 접속 기능 제공
2. 상태를 공유하는 공유 오브젝트를 구현하는 기능 제공
3. 구분된 세 가지 범위(Room, World, Global)에서 채팅 기능 제공

> Room: 현재 접속한 룸 범위
> 
> World: 같은 이름의 룸 범위
> 
> Global: 전체 룸 범위

클라이언트 메타버스 기능에 집중할 수 있도록 Mercury Engine 은 메타버스 서버 운영 서비스를 제공합니다.

## 시작하기 전에
### UnityPackage
이 프로젝트의 모든 기능은 유니티 패키지 형태로 제공되며 MirrorGearSDK 인 Mercury 패키지를 필요로 합니다.

### Key URL
이 프로젝트의 모든 기능은 메타버스 서버 운영 서비스를 통해 제공되며, 운영 서비스는 `AuthKey` 및 `WebServiceKey` 를 기반으로 제공됩니다.

올바른 URL을 얻으려면 '시어스랩'에 문의하시기 바랍니다.


## 사용 방법
올바르게 플러그인 설치 및 동작을 확인하기 위해서 다음과 같은 코드를 사용할 수 있습니다.
```c#
using UnityEngine;
using MirrorGear.Mercury;

public class MercuryConnectorSample : MercuryBehaviourCallBacks
{
	public override void OnConnected()
	{
		Debug.Log("OnConnected");
	}

	public override void OnDisconnected()
	{
		Debug.Log("OnDisconnected");
	}
}
```
코드 동작은 메타버스 서비스 연결/해제를 로그로 확인합니다.

SDK를 사용해주셔서 감사합니다. 더 나은 서비스를 제공하기 위해 항상 노력하겠습니다.
