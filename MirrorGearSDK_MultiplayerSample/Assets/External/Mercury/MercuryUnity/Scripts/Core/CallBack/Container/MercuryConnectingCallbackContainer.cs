using System.Collections.Generic;
using MirrorGear.Mercury.Network;

namespace MirrorGear.Mercury
{
	public class MercuryConnectingCallbackContainer : BaseContainer, IMercuryConnectingCallback
	{

		private Dictionary<object, IMercuryConnectingCallback> _connectingCallbacks = new Dictionary<object, IMercuryConnectingCallback>();

		public MercuryConnectingCallbackContainer(MercuryClient client) : base(client) { }
		
		public override void Clear()
		{
			_connectingCallbacks.Clear();
		}

		public void AddCallBack(object target, IMercuryConnectingCallback callback)
		{
			if (target == null)
				return;
			
			if (_connectingCallbacks.ContainsKey(target))
				_connectingCallbacks[target] = callback;
			else
				_connectingCallbacks.Add(target, callback);
		}

		public void RemoveCallBack(object target)
		{
			if (_connectingCallbacks.ContainsKey(target))
			{
				_connectingCallbacks.Remove(target);
			}
		}

		public void OnNetworkConnectionFail()
		{
			foreach(var target in _connectingCallbacks)
			{
				target.Value.OnNetworkConnectionFail();
			}
		}

		public void OnConnected()
		{
			foreach (var target in _connectingCallbacks)
			{
				target.Value.OnConnected();
			}
		}

		public void OnDisconnected()
		{
			foreach(var target in _connectingCallbacks)
			{
				target.Value.OnDisconnected();
			}
		}
		public void OnReConnectStart()
		{
			foreach(var target in _connectingCallbacks)
			{
				target.Value.OnReConnectStart();
			}
		}

		public void OnReConnectEnd()
		{
			foreach(var target in _connectingCallbacks)
			{
				target.Value.OnReConnectEnd();
			}
		}

		public void OnLoginCompleted()
		{
			foreach(var target in _connectingCallbacks)
			{
				target.Value.OnLoginCompleted();
			}
		}
		public void OnObjectInitData(MercuryObjectSyncDataList data)
		{
			foreach(var target in _connectingCallbacks)
			{
				target.Value.OnObjectInitData(data);
			}
		}

		public void OnNetworkError(Error error)
		{
			foreach(var target in _connectingCallbacks)
			{
				target.Value.OnNetworkError(error);
			}
		}

		public void OnHeartBeat(long laytency)
		{
			foreach (var target in _connectingCallbacks)
			{
				target.Value.OnHeartBeat(laytency);
			}
		}
	}
}


