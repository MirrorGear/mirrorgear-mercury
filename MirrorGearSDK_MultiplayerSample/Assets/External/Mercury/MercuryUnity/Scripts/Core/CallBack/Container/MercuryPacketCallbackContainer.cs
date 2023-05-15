using System.Collections.Generic;
using MirrorGear.Mercury.Network;
using UnityEngine;

namespace MirrorGear.Mercury
{
	public class MercuryPacketCallbackContainer : BaseContainer, IPacketCallback
	{
		private Dictionary<object, IPacketCallback> _errorCallbacks = new Dictionary<object, IPacketCallback>();

		public MercuryPacketCallbackContainer(MercuryClient client) : base(client) { }

		public override void Clear()
		{
			_errorCallbacks.Clear();
		}

		public void AddCallback(object obj, IPacketCallback callback)
		{
			if(_errorCallbacks.ContainsKey(obj) == false)
				_errorCallbacks.Add(obj, callback);
		}

		public void RemoveCallback(object obj)
		{
			if(_errorCallbacks.ContainsKey(obj) == true)
				_errorCallbacks.Remove(obj);
		}

		public void OnPacketSendStarted()
		{
			foreach(var callback in _errorCallbacks)
			{
				callback.Value.OnPacketSendStarted();
			}
		}

		public void OnPacketResponsed()
		{
			foreach(var callback in _errorCallbacks)
			{
				callback.Value.OnPacketResponsed();
			}
		}

		public void OnPacketError(Error code)
		{
			foreach(var callback in _errorCallbacks)
			{
				callback.Value.OnPacketError(code);
			}
		}

		public void OnWebMessage(WebResult result, string data)
		{
			foreach(var callback in _errorCallbacks)
			{
				callback.Value.OnWebMessage(result, data);
			}
		}

        public void OnReceiveDynamicData(DynamicData data)
        {
			foreach (var callback in _errorCallbacks)
			{
				callback.Value.OnReceiveDynamicData(data);
			}
		}
    }
}