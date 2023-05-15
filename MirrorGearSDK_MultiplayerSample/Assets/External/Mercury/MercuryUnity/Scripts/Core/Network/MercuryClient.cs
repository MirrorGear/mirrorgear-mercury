using MirrorGear.Mercury.Chat;
using MirrorGear.Mercury.Network;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using UnityEngine;

namespace MirrorGear.Mercury
{
	public enum DisconnectCause
	{
		None,

		//연결시 예외
		ExceptionOnConnect,
	}

	public enum ClientState
	{
		None,
		//서버 접속중
		ConnectingToServer,
		//서버 접속 완료.
		ServerConnectionComplete,

		ConnectionFailed,

		DisConnected,

		EnteredRoom,
	}
	
	public sealed partial class MercuryClient : IClient
	{
		public ClientState ClientState { get; private set; } = ClientState.None;
		public bool IsRoomConnection { get { return ClientState == ClientState.EnteredRoom; }}
		public bool IsConnected => _mercuryEngine.IsConnected;
		
		public Dictionary<ulong, Player> PlayerList { get; private set; } = new Dictionary<ulong, Player>();
		private MercuryEngine _mercuryEngine;
		private MercuryConnectingCallbackContainer _connectingCallbackContainer;
		private MercuryRoomCallbackContainer _roomCallbackContainer;
		private MercuryPacketCallbackContainer _packetCallbackContainer;

		public event Action<NetworkData> onReceiveDataRec;

		public static RoomInfo CurrentRoom { get; private set; }
		
		public MercuryClient()
		{
			//Application.logMessageReceivedThreaded += ApplicationOnlogMessageReceivedThreaded;

			try
			{
				_mercuryEngine = new MercuryEngine(this, MercuryNetwork.ServerSettings.authKey, MercuryNetwork.ServerSettings.webServiceKey, clientVersion: 0);
			}
			catch (ArgumentException e)
			{
				Debug.LogError($"ServerSettings 값을 확인해주세요. {e.Message}");
				return;
			}

			if (_connectingCallbackContainer == null)
				_connectingCallbackContainer = new MercuryConnectingCallbackContainer(this);

			if(_roomCallbackContainer == null)
				_roomCallbackContainer = new MercuryRoomCallbackContainer(this);

			if(_packetCallbackContainer == null)
				_packetCallbackContainer = new MercuryPacketCallbackContainer(this);

			//FindMercuryObject();
		}

		public void SetBehaviour(MonoBehaviour behaviour)
		{
			//_mercuryEngine.SetBehaviour(behaviour);
		}

		public MercuryChatEngine SetChatClient(IChatClient chatClient)
        {
			return _mercuryEngine.SetChatClient(chatClient);
        }
		
		//private void FindMercuryObject()
		//{
		//	var list = Object.FindObjectsOfType<GameObject>();
		//	foreach(var go in list)
		//	{
		//		var mercuryObject = go.GetComponent<MercuryObject>();
		//		if(mercuryObject == null)
		//			continue;

		//		_mercuryObjects.Add(mercuryObject.objectId, mercuryObject);
		//	}
		//}

#region  CallBackContainer

		private ObjectIDGenerator objIDGenerator = new ObjectIDGenerator();

		public void AddCallback(object target)
		{
			var connectingCallback = target as IMercuryConnectingCallback;

			if(connectingCallback != null)
				_connectingCallbackContainer.AddCallBack(target, connectingCallback);

			var roomCallback = target as IMercuryRoomCallback;

			if(roomCallback != null)
				_roomCallbackContainer.AddCallBack(target, roomCallback);

			var packeErrorCallback = target as IPacketCallback;
			if(packeErrorCallback != null)
				_packetCallbackContainer.AddCallback(target, packeErrorCallback);
		}

		public void RemoveCallback(object target)
		{
			_connectingCallbackContainer.RemoveCallBack(target);
			_roomCallbackContainer.RemoveCallBack(target);
		}


#endregion

#region BroadCast
		
		public void OnChatHistory(ChatHistoryData[] data)
		{
			if (data == null) return;
			for (int i = 0; i < data.Length; i++)
			{
				Debug.Log(data[i].ToString());
			}
		}

		public void OnObjTransform(TransformData data)
		{
			if (data == null) return;
			if (PlayerList.ContainsKey(data.accountId) == false)
			{
				var player = new Player
				{
					accountId = data.accountId,
					isObserver = false,
					spawnPosition = new Vector3(data.posX, data.posY, data.posZ),
					eulerAngles = new Vector3(data.rotX, data.rotY, data.rotZ),
				};
				PlayerList.Add(data.accountId, player);
				_mercuryEngine.RaiseEvent(MercuryOperationType.ObjShape, MercuryNetwork.LocalPlayer.accountId, data.accountId);
			}
			else
			{
				onReceiveDataRec?.Invoke(data);
			}
		}

		public void OnDynamic(DynamicData data)
		{
			if(data.code == DynamicCode.None)
			{
				_packetCallbackContainer.OnReceiveDynamicData(data);
			}
			else 
			{
				onReceiveDataRec?.Invoke(data);
			}
		}

		public void OnRpc(RPCData data)
		{
			Debug.Log($"[RPC] {data}");
		}

		public void OnShareObjStatusList(MercuryObjectSyncDataList data)
		{
			if (data == null) return;
			if (data.type != NetworkDataType.SyncObject) return;
			_connectingCallbackContainer.OnObjectInitData(data);
		}

		public void OnShareObjChange(MercuryObjectSyncData data)
		{
			if (data == null) return;

			onReceiveDataRec?.Invoke(data);
		}

		public void OnReceiveData(object data)
		{
			Debug.LogWarning($"[Obsolete] Method Called: {data}");
			switch (data)
			{
				case ChatHistoryData[] chatHistory:
				{
					Debug.LogWarning("[Obsolete] Method Called");
					break;
				}

				case TransformData transData:
				{
					Debug.LogWarning("[Obsolete] Method Called");
					break;
				}

				case DynamicData dynamicData when dynamicData.code == DynamicCode.None:
				{
					Debug.LogWarning("[Obsolete] Method Called");
					break;
				}

				case MercuryObjectSyncDataList syncDataList:
				{
					Debug.LogWarning("[Obsolete] Method Called");
					break;
				}

				case NetworkData networkData:
				{
					if (networkData == null) return;

					onReceiveDataRec?.Invoke(networkData);
					break;
				}
			}
		}

		public void OnBroadCastHandler(ref PacketInfo info)
		{
			//switch(info.packetId)
			//{
			//	case PacketID.GC_CHAT:
			//		{


			//			break;
			//		}
			//}
		}

#endregion

#region  Connect & DisConnect

		public void OnNetworkConnectionFail()
		{
			_connectingCallbackContainer.OnNetworkConnectionFail();
		}

		public void Connect()
		{
			if(ClientState == ClientState.ConnectingToServer)
				return;

			ChangeClientState(ClientState.ConnectingToServer);
			try
			{
				_mercuryEngine.RaiseEvent(MercuryOperationType.Connect, MercuryNetwork.ServerSettings.serverType);
			}
			catch (Exception e)
			{
				Debug.LogWarning($"{e.Message}");
				throw e;
			}
		}

		public void Connect(string address, int port)
		{
			if(ClientState == ClientState.ConnectingToServer)
				return;

			ChangeClientState(ClientState.ConnectingToServer);

			_mercuryEngine.RaiseEvent(MercuryOperationType.Connect, address, port);
		}

		public void DisConnect()
		{
			_mercuryEngine.RaiseEvent(MercuryOperationType.DisConnect, null);
		}

		public async Task ReConnection(ulong accountId, RoomInfo roomInfo, string nick)
		{
			_mercuryEngine.RaiseEvent(MercuryOperationType.ReConnect, accountId, roomInfo, nick);
		}

		public void AppClose()
		{
			_mercuryEngine.RaiseEvent(MercuryOperationType.AppClose);
		}

#region  Connection CallBack

		public void Login(ulong accountId)
		{
			_mercuryEngine.RaiseEvent(MercuryOperationType.Login, accountId);
		}

#endregion

#endregion

#region Room

		public void GetRoomList(params object[] args)
		{
			_mercuryEngine.RaiseEvent(MercuryOperationType.GetRoomList, args);
		}

		public void CheckRoom(ulong accountId, string roomName, int maxUserCount)
		{
			_mercuryEngine.RaiseEvent(MercuryOperationType.CheckRoom, accountId, roomName, maxUserCount);
		}

		public void InitRoom(ulong accountId, RoomInfo roomInfo)
		{
			var objectDatas = new List<MercuryObjectInitData>();
			foreach (var pair in MercuryNetwork.MecuryObjects)
			{
				var syncType = SyncObjecType.None;
				var property = "";
				var syncObj = pair.Value.GetComponent<MercurySyncObject>();
				if(syncObj != null)
				{
					syncType = syncObj.type;
					property = syncObj.Property;
				}

				objectDatas.Add(new MercuryObjectInitData
				{
					objectId = pair.Value.objectId,
					objType = (ushort)syncType,
					property = property.StringToByte(80), // MAX_PROP_OBJ
				});
			}

			_mercuryEngine.RaiseEvent(MercuryOperationType.InitRoom, accountId, roomInfo, objectDatas);
		}

		public void JoinRoom(ulong accountId, RoomInfo roomInfo, string nick, bool isObserver, string properity, bool isDirect)
		{
			_mercuryEngine.RaiseEvent(MercuryOperationType.JoinRoom, accountId, roomInfo, nick, isObserver, properity, isDirect);
		}

		public void LeaveRoom(params object[] args)
		{
			_mercuryEngine.RaiseEvent(MercuryOperationType.LeaveRoom, args);
		}

		public void JoinOrCreateRoom(Player player, RoomOption roomOption, string prefabName, string userProperty)
		{
			foreach(var pair in MercuryNetwork.MecuryObjects)
			{
				var syncType = SyncObjecType.None;
				var property = "";
				var syncObj = pair.Value.GetComponent<MercurySyncObject>();
				if(syncObj != null)
				{
					syncType = syncObj.type;
					property = syncObj.Property;
				}

				roomOption.AddMercuryObject(pair.Value.objectId, syncType, property);
			}
			roomOption.isObserver = player.isObserver;
			_mercuryEngine.RaiseEvent(MercuryOperationType.JoinOrCreateRoom, roomOption, player.accountId, player.nickname, prefabName, userProperty);
		}

#endregion
		
#region  Send

		public void SendData<T>(MercurySyncType type, ulong accountId, T data) where T : NetworkData
		{
			_mercuryEngine.SendData(type, accountId, data);
		}

		public void SendData<T>(MercurySyncType type, Player player, T data) where T : NetworkData
		{
			_mercuryEngine.SendData(type, player.accountId, data);
		}

#endregion

#region State
		private void ChangeClientState(ClientState state)
		{
			if(ClientState == state)
				return;

			ClientState = state;

			MercuryDebug.Log(ClientState.ToString().ToColorString(Color.cyan, true));
		}
#endregion

#region Player

		public void GetPlayerInfo(ulong accountId)
		{
			_mercuryEngine.RaiseEvent(MercuryOperationType.ObjShape, MercuryNetwork.LocalPlayer.accountId, accountId);
		}

		private void AddPlayer(Player player, bool isLocal = false)
		{
			if(PlayerList.ContainsKey(player.accountId) == false)
				PlayerList.Add(player.accountId, player);
			else
				PlayerList[player.accountId].DeepCopy(player);

			// 오브젝트 생성
			_roomCallbackContainer.OnInitPlayer(PlayerList[player.accountId]);
		}

#endregion

#region Debug

		private void ApplicationOnlogMessageReceivedThreaded(string condition, string stacktrace, LogType type)
		{
			switch(type)
			{
				case LogType.Error:
				case LogType.Exception:
					MercuryDebug.Log(condition);
					break;
			}
		}

		

#endregion

#region IClient

		public void OnConnected(bool isConnected)
		{
			ChangeClientState(isConnected ? ClientState.ServerConnectionComplete : ClientState.ConnectionFailed);

			if(isConnected)
			{
				_connectingCallbackContainer.OnConnected();
				MercuryNetwork.ServerSettings.sendPerSecond = _mercuryEngine.SendPerSecond;
			}
			else
			{
				//연결 실패
			}
		}

		public void OnDisConnected()
		{
			_connectingCallbackContainer.OnDisconnected();
			ChangeClientState(ClientState.DisConnected);
		}

		public void OnReConnectStart()
		{
			_connectingCallbackContainer.OnReConnectStart();
		}

		public void OnReConnectEnd()
		{
			_connectingCallbackContainer.OnReConnectEnd();
		}

		public void OnNetworkError(Error error)
		{
			_connectingCallbackContainer.OnNetworkError(error);
		}

		public void OnLoginCompleted(ulong accountId)
		{
			MercuryNetwork.LocalPlayer.accountId = accountId;
			_connectingCallbackContainer.OnLoginCompleted();
		}

		public void OnRoomList(bool isValid, RoomListData roomList)
		{
			if (roomList == null || roomList.roomList.Count == 0)
			{
				MercuryDebug.LogError("Room List is Null !!");
				return;
			}

			_roomCallbackContainer.OnRoomList(roomList.roomList);
		}

		public void OnRoomData(bool isValid, RoomDataType type, object roomData)
		{
			if(isValid == false)
				return;

			switch(type)
			{
				case RoomDataType.RoomList:
					{
						Debug.LogWarning("[Obsolete] Method Call");
					}
					break;
				case RoomDataType.CheckRoom:
					{
						_roomCallbackContainer.OnCheckedRoom(roomData as RoomInfo);
					}
					break;
				case RoomDataType.InitRoom:
					{
						_roomCallbackContainer.OnInitializedRoom(roomData as RoomInfo);
					}
					break;
				case RoomDataType.JoinedRoom:
					{
						var roomInfo = roomData as RoomInfo;

						MercuryNetwork.LocalPlayer.isObserver = roomInfo.isObserver;
						MercuryNetwork.LocalPlayer.objectId = roomInfo.userObjId;

						ChangeClientState(ClientState.EnteredRoom);

						AddPlayer(MercuryNetwork.LocalPlayer, true);
						_roomCallbackContainer.OnJoinedRoom(roomInfo);

						CurrentRoom = roomData as RoomInfo;
					}
					break;
				case RoomDataType.LeaveRoom:
					{
						_roomCallbackContainer.OnLocalPlayerLeaveRoom(roomData as LeaveRoomData);
						PlayerList.Clear();

						foreach(var pair in MercuryNetwork.MecuryObjects)
						{
							var syncObj = pair.Value.GetComponent<MercurySyncObject>();
							if(syncObj != null)
								syncObj.ResetObject();
						}
					}
					break;
			}
		}

		public void OnShapePlayer(ShapeData data)
		{
			var player = new Player
			{
				accountId = data.accountId,
				objectId = data.objectId,
				nickname = data.nick,
				prefabName = data.prefabName,
				property = data.property,
			};

			AddPlayer(player);
		}

		public void OnJoinPlayer(JoinRoomData data)
		{
			_roomCallbackContainer.OnJoinedRoom(data.roomInfo);
		}

		public void OnLeavePlayer(LeaveRoomData data)
		{
			if(PlayerList.ContainsKey(data.accountId))
			{
				// 오브젝트 삭제
				_roomCallbackContainer.OnOtherPlayerLeaveRoom(data, PlayerList[data.accountId]);

				PlayerList.Remove(data.accountId);
			}
		}

		public void OnPacketSendStarted()
		{
			_packetCallbackContainer.OnPacketSendStarted();
		}

		public void OnPacketResponsed()
		{
			_packetCallbackContainer.OnPacketResponsed();
		}

		public void OnError(Error code)
		{
			_packetCallbackContainer.OnPacketError(code);
		}

		public void OnWebMessage(WebResult result, string data)
		{
			_packetCallbackContainer.OnWebMessage(result, data);
		}

		public void OnRecieveDynamicData(DynamicData data)
        {
			_packetCallbackContainer.OnReceiveDynamicData(data);
        }

		public void OnHeartBeat(long millisec)
		{
			_connectingCallbackContainer.OnHeartBeat(millisec);
		}

		public void OnDebugMessage(DebugLevelType levelType, string message)
		{
			//if(MercuryNetwork.ServerSettings.networkLogType == EnabledNetworkLogType.None)
			//	return;

			//switch(levelType)
			//{
			//	case DebugLevelType.Normal:
			//		{
			//			MercuryDebug.Log(message);
			//		}
			//		break;
			//	case DebugLevelType.Warning:
			//		{
			//			MercuryDebug.LogWarning(message);
			//		}
			//		break;
			//	case DebugLevelType.Error:
			//		{
			//			MercuryDebug.LogError(message);
			//		}
			//		break;
			//}
		}
#endregion
	}
}


