using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MirrorGear.Mercury
{
	public static partial class MercuryNetwork
	{
		private static Dictionary<uint, MercuryObject> _mercuryObjects = new Dictionary<uint, MercuryObject>();
		private static Dictionary<Type, List<MethodInfo>> _rpcMethodsCache = new Dictionary<Type, List<MethodInfo>>();

		public static Dictionary<uint, MercuryObject> MecuryObjects => _mercuryObjects;

		public static bool IsWait => _mercuryObjects.Any();

		private static void FindMercuryObject()
		{
			var list = Object.FindObjectsOfType<GameObject>();
			foreach(var go in list)
			{
				var mercuryObject = go.GetComponent<MercuryObject>();
				if(mercuryObject == null)
					continue;

				_mercuryObjects.Add(mercuryObject.objectId, mercuryObject);
			}
		}

		public static void RegisterObject(MercuryObject obj)
		{
			_mercuryObjects.Add(obj.objectId, obj);
		}

		public static void UnRegisterObject(MercuryObject obj)
		{
			_mercuryObjects.Remove(obj.objectId);
		}

		public static void MercuryObjectUpdate()
		{
			var enumerator = _mercuryObjects.GetEnumerator();
			while(enumerator.MoveNext())
			{
				var mcObj = enumerator.Current.Value;
				if(mcObj.IsMine == false)
					continue;

				mcObj.SerializeComponent();
			}
		}

		public static void SendData()
		{
			foreach(var val in _mercuryObjects)
			{
				var mcObj = val.Value;
				if(mcObj.syncDatas == null)
					continue;

				while(mcObj.syncDatas.Count > 0)
				{
					var data = mcObj.syncDatas.Dequeue();

					if(data == null)
						break;

					var syncType = MercurySyncType.None;
					switch(data.type)
					{
						case NetworkDataType.Transform:
							syncType = MercurySyncType.Transform;
							break;
						case NetworkDataType.Dynamic:
							syncType = MercurySyncType.Dynamic;
							break;
						case NetworkDataType.SyncObject:
							syncType = MercurySyncType.SyncObject;
							break;
					}

					_client.SendData(syncType, data.accountId, data);
				}
			}
		}

		public static MercuryObject GetObject(uint objId)
		{
			if(_mercuryObjects.ContainsKey(objId) == false)
				return null;

			return _mercuryObjects[objId];
		}

		public static void OnReceiveData(NetworkData data)
		{
			if(data.type == NetworkDataType.Dynamic)
			{
				var dynamicData = data as DynamicData;
				switch(dynamicData.code)
				{
					case DynamicCode.Animation:
						{
							var targetObj = _mercuryObjects.FirstOrDefault(m => m.Value.AccountId == data.accountId).Value;
							if(targetObj != null)
								targetObj.DeserializeComponent(data);
						}
						break;
					case DynamicCode.RPC:
						{
							ExecuteRPC(data);
						}
						break;
				}
			}
			else
			{
				if(_mercuryObjects.ContainsKey(data.objectId) == false)
					return;

				var targetObj = _mercuryObjects[data.objectId];
				if(targetObj != null)
					targetObj.DeserializeComponent(data);
			}
		}

		internal static void RPC(MercuryObject mo, string methodName, RPCTarget target, Player player, params object[] args)
		{
			var broadCastType = BroadCastType.Range;

			if(player != null || target == RPCTarget.Target)
				broadCastType = BroadCastType.Target;

			var data = new RPCData
			{
				accountId		= LocalPlayer.accountId,
				code			= DynamicCode.RPC,
				broadCastType	= broadCastType,
				objectId		= mo.objectId,
				methodName		= methodName,
				datas			= args.ToList(),
			};

			if(target != RPCTarget.Other)
				ExecuteRPC(data);

			data.datas ??= new List<object>();
			data.datas.Insert(0, methodName);

			SendData(MercurySyncType.Dynamic, LocalPlayer, data);
		}

		public static void ExecuteRPC(NetworkData data)
		{
			var rpcData = data as RPCData;
			if(rpcData == null)
				return;

			var mercuryObject = GetObject(data.objectId);

			if(mercuryObject.rpcMonoBehaviours == null || mercuryObject.rpcMonoBehaviours.Length == 0)
			{
				mercuryObject.RefreshRpcMonoBehaviourCache();
			}

			foreach(var monob in mercuryObject.rpcMonoBehaviours)
			{
				Type type = monob.GetType();

				List<MethodInfo> methodInfos = null;
				bool isMethodsOfType = _rpcMethodsCache.TryGetValue(type, out methodInfos);

				if(isMethodsOfType == false)
				{
					var entries = GetRPCMethods(type);
					_rpcMethodsCache[type] = entries;

					methodInfos = entries;
				}

				if(methodInfos == null)
					return;

				foreach(var mInfo in methodInfos)
				{
					if(mInfo.Name != rpcData.methodName)
						continue;

					var parameters = mInfo.GetCachedParemeters();
					object targetObj = null;

					if(rpcData.datas.Count == parameters.Length)
					{
						targetObj = mInfo.Invoke(monob, rpcData.datas.ToArray());
					}
					//else if(rpcData.datas == null && parameters.Length == 1 && parameters[0].ParameterType == typeof(MercuryAnimator))
					//{}
					else
					{
						targetObj = mInfo.Invoke(monob, null);
					}

					var ie = targetObj as IEnumerator;
					if(ie != null)
						MercuryHandler.Instance.StartCoroutine(ie);
				}
			}
		}

		private static List<MethodInfo> GetRPCMethods(Type type)
		{
			if(type == null)
				return null;

			var rpcType = typeof(MercuryRPC);

			var methods = new List<MethodInfo>();

			foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if(method.IsDefined(rpcType, false) == true)
					methods.Add(method);
			}

			return methods;
		}
	}
}