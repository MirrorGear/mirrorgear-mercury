using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MirrorGear.Mercury
{
	[AddComponentMenu("Mercury Networking/MercuryObject")] 
	public class MercuryObject : MonoBehaviour
	{
		[SerializeField]
		private uint _objectId = 0;

		[SerializeField]
		private List<NetworkComponent> _networkComponents;

		public DistanceType distanceType;

		public Queue<NetworkData> syncDatas;

		public event Action<bool> onRangeCollision;

		internal MonoBehaviour[] rpcMonoBehaviours;

		public uint objectId => _objectId;
		public ulong AccountId { get; private set; }
		public bool IsMine { get; private set; } = false;


#if UNITY_EDITOR
		
		public void FindMercuryComponent() 
		{
			if(_networkComponents == null)
				_networkComponents = GetComponentsInChildren<NetworkComponent>().ToList();
			else
			{
				var components = GetComponentsInChildren<NetworkComponent>().ToList();

				if (!_networkComponents.SequenceEqual(components))
					_networkComponents = components;
			}
		}

		public List<NetworkComponent> GetMercuryComponent()
		{
			return GetComponentsInChildren<NetworkComponent>().ToList();
		}

#endif

		protected void Awake()
		{
			if(syncDatas == null)
				syncDatas = new Queue<NetworkData>();

			//MercuryNetwork.MercuryClient.RegisterObject(this);
		}

		protected void OnDestroy()
		{
			if(MercuryNetwork.MercuryClient != null)
				MercuryNetwork.UnRegisterObject(this);
		}
		
		public void SetMine()
		{
			IsMine = true;
			AccountId = MercuryNetwork.LocalPlayer.accountId;
		}

		public void SerializeComponent()
		{
			if(_networkComponents == null || _networkComponents.Any() == false)
				return;

			foreach(var component in _networkComponents)
			{
				var observable = component as IMercuryObservable;
				if(observable != null)
				{
					var data = observable.OnMercurySerialize();
					if(data == null)
						continue;

					syncDatas.Enqueue(data);
				}
			}
		}

		public void DeserializeComponent(NetworkData data)
		{
			if(_networkComponents == null || _networkComponents.Any() == false)
				return;

			foreach (var component in _networkComponents)
			{
				var observable = component as IMercuryObservable;
				if(observable != null)
					observable.OnMercuryDeserialize(data);
			}
		}

		public void SetPlayer(Player player)
		{
			SetObjectData(player);

			if(MercuryNetwork.LocalPlayer.accountId == AccountId)
				SetMine();
		}

		public void SetObjectData(NetworkData data)
		{
			AccountId = data.accountId;
			_objectId = data.objectId;

			MercuryNetwork.RegisterObject(this);
		}

		public virtual void OnRangeCollision(bool isInRange)
		{
			onRangeCollision?.Invoke(isInRange);
		}

		public void RefreshRpcMonoBehaviourCache()
		{
			this.rpcMonoBehaviours = this.GetComponents<MonoBehaviour>();
		}

		public void RPC(string methodName, Player player, params object[] args)
		{
			MercuryNetwork.RPC(this, methodName, player, args);
		}

		public void RPC(string methodName, RPCTarget target, params object[] args)
		{
			MercuryNetwork.RPC(this, target, methodName, args);
		}

		public void RPC(string methodName, params object[] args)
		{
			MercuryNetwork.RPC(this, methodName, args);
		}

		public void OnReceiveCustomData(DynamicData data)
        {

        }
	}
}