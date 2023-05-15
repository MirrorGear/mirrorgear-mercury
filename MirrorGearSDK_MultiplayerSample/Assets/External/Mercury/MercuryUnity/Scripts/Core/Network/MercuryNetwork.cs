using System;
using System.Collections.Generic;
using MirrorGear.Mercury.Chat;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MirrorGear.Mercury
{
	public static partial class MercuryNetwork
	{
		private static MercuryServerSettings _serverSettings;
		
		private static int serializationFrequency = 100;

		private static MercuryClient _client;


		public static event Action<MercuryObject> onCompleteInstantiate;

		public static Player LocalPlayer { get; private set; } = new Player();

		public static IEnumerable<Player> PlayerList
		{
			get
			{
				if(_client != null || _client.PlayerList != null)
					return _client.PlayerList.Values;

				return null;
			}
		}

		public static MercuryClient MercuryClient => _client;

		public static bool IsConnected => _client.IsConnected;

		public static MercuryServerSettings ServerSettings
		{
			get
			{
				if(_serverSettings == null)
					LoadServerSettings();

				return _serverSettings;
			}
		}

		public static int SerializationRate
		{
			get
			{
				return 1000 / serializationFrequency;
			}

			set
			{
				serializationFrequency = 1000 / value;
				if(MercuryHandler.Instance != null)
				{
					MercuryHandler.Instance.SerializeUpdateInterval = serializationFrequency;
				}
			}
		}

		static MercuryNetwork()
		{
#if !UNITY_EDITOR
			Initialize();
#endif
		}

		private static void LoadServerSettings()
		{
			_serverSettings = Resources.Load(nameof(MercuryServerSettings)) as MercuryServerSettings;

			if(_serverSettings == null)
			{
				Debug.LogError("ServerSetting Error");
				return;
			}
		}

		#if UNITY_EDITOR && UNITY_2019_4_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		#endif
		private static void Initialize()
		{
			#if UNITY_EDITOR
			if(!EditorApplication.isPlayingOrWillChangePlaymode)
				return;
			#endif

			FindMercuryObject();

			if (_client == null)
			{
				_client = new MercuryClient();
				_client.onReceiveDataRec += OnReceiveData;
				MercuryHandler.Instance.SetMercuryClient(_client);
			}
		}

		public static void AddCallBack(object target)
		{
			if(_client == null)
				return;

			_client.AddCallback(target);
		}

		public static void RemoveCallBack(object target)
		{
			if(_client == null)
				return;

			_client.RemoveCallback(target);
		}

		public static MercuryChatEngine SetChatClient(IChatClient chatClient)
		{
			return _client.SetChatClient(chatClient);
		}

		public static void SendData<T>(MercurySyncType type, Player player, T data) where T : NetworkData
		{
			if(_client == null)
				return;

			_client.SendData(type, player, data);
		}
		
		public static void RaiseEvent(byte code, object data, BroadCastType broadCastType)
		{

		}

		public static void RemoveObject(uint objId)
		{
			if(_mercuryObjects.ContainsKey(objId) == true)
				Object.Destroy(_mercuryObjects[objId].gameObject);
		}

		#region Connection

		public static void Connection()
		{
			if(_client == null)
				Initialize();

			//_client.Connect(ServerSettings.address, ServerSettings.port);
			_client.Connect();
		}

		public static void Disconnect()
		{
			_client.DisConnect();
		}

		public static async Task ReConnection(ulong accountId = 0, RoomInfo roomInfo = null, string nick = "")
		{
			await _client.ReConnection(accountId, roomInfo, nick);
		}

		#endregion

		public static void AppClose()
		{
			_client.AppClose();
		}

		public static void Login(string nick, ulong accountId=0)
		{
			LocalPlayer.accountId = accountId;
			LocalPlayer.nickname = nick;

			_client.Login(accountId);
		}

		internal static void RPC(MercuryObject mo, string methodName, Player player, params object[] args)
		{
			RPC(mo, methodName, RPCTarget.Target, player, args);
		}

		internal static void RPC(MercuryObject mo, RPCTarget target, string methodName, params object[] args)
		{
			RPC(mo, methodName, target, null, args);
		}

		internal static void RPC(MercuryObject mo, string methodName, params object[] args)
		{
			RPC(mo, methodName, RPCTarget.All, null, args);
		}

		#region Room

		public static void JoinOrCreateRoom(RoomOption roomOption, string prefabName, string userProperty = "")
		{
			LocalPlayer.prefabName = prefabName;
			LocalPlayer.property = userProperty;

			_client.JoinOrCreateRoom(LocalPlayer, roomOption, prefabName, userProperty);
		}

		public static void LeaveRoom(ulong roomId)
		{
			_client.LeaveRoom(LocalPlayer.accountId, roomId);
		}

		public static void GetRoomList()
		{
			_client.GetRoomList(LocalPlayer.accountId);
		}

		#region legacy room method

		public static void CheckRoom(string roomName, int maxUserCount = 0)
		{
			_client.CheckRoom(LocalPlayer.accountId, roomName, maxUserCount);
		}

		public static void InitRoom(RoomInfo roomInfo)
		{
			_client.InitRoom(LocalPlayer.accountId, roomInfo);
		}

		public static void JoinRoom(RoomInfo roomInfo, string nick, bool isObserver, string properity, bool isDirect)
		{
			_client.JoinRoom(LocalPlayer.accountId, roomInfo, nick, isObserver, properity, isDirect);
		}

		#endregion

		#endregion

		

		#region Instantiate

		public static GameObject Instantiate(Player player, Transform parent)
		{
			var component = Instantiate(player, parent, player.spawnPosition, player.eulerAngles);

			return component;
		}

		public static GameObject Instantiate(Player player, Transform parent, Vector3 position, Vector3 eulerAngles)
		{
			var component = Instantiate(player.prefabName, parent, position, eulerAngles);
			var mcObj = component.GetComponent<MercuryObject>();
			if(mcObj != null)
			{
				mcObj.SetPlayer(player);
				mcObj.name = player.nickname;

				onCompleteInstantiate?.Invoke(mcObj);
			}

			return component;
		}

		public static GameObject Instantiate(string prefabName, NetworkData data, Transform parent, Vector3 position)
		{
			var component = Instantiate(prefabName, parent, position, Vector3.zero);
			var mcObj = component.GetComponent<MercuryObject>();
			if(mcObj != null)
				mcObj.SetObjectData(data);

			return component;
		}

		public static GameObject Instantiate(string prefabName, Transform parent)
		{
			return Instantiate(prefabName, parent, Vector3.zero, Vector3.zero);
		}

		public static GameObject Instantiate(string prefabName, Transform parent, Vector3 position, Vector3 eulerAngles, bool isLocal = false)
		{
			if(string.IsNullOrWhiteSpace(prefabName))
			{
				Debug.LogError($"{nameof(prefabName)} was empty");
				return null;
			}

			var prefab = Resources.Load<GameObject>(prefabName);
			if (prefab == null)
			{
				Debug.LogError($"{nameof(prefabName)}({prefabName}) not exists");
			}
			GameObject obj;

			if(isLocal == false)
			{
				obj = Object.Instantiate(prefab, position, Quaternion.Euler(eulerAngles), parent);
			}
			else
			{
				obj = Object.Instantiate(prefab, parent);
				obj.transform.localPosition = position;
			}

			return obj;
		}

		#endregion
	}
}


