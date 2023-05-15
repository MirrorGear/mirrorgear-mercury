using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirrorGear.Mercury
{
	public class MercuryRoomCallbackContainer : BaseContainer, IMercuryRoomCallback
	{
		private Dictionary<object, IMercuryRoomCallback> _roomCallbacks = new Dictionary<object, IMercuryRoomCallback>();

		public MercuryRoomCallbackContainer(MercuryClient client) : base(client) { }

		public override void Clear()
		{
			_roomCallbacks.Clear();
		}

		public void AddCallBack(object target, IMercuryRoomCallback callback)
		{
			if(target == null)
				return;

			if(_roomCallbacks.ContainsKey(target))
				_roomCallbacks[target] = callback;
			else
				_roomCallbacks.Add(target, callback);
		}

		public void RemoveCallBack(object target)
		{
			if(_roomCallbacks.ContainsKey(target))
			{
				_roomCallbacks.Remove(target);
			}
		}

		public void OnRoomList(List<RoomListInfo> roomInfos)
		{
			try
			{
				foreach(var target in _roomCallbacks)
				{
					target.Value.OnRoomList(roomInfos);
				}
			}
			catch(Exception e)
			{
				Debug.Log(e);
			}
		}

		public void OnCheckedRoom(RoomInfo roomInfo)
		{
			try
			{
				foreach(var target in _roomCallbacks)
				{
					target.Value.OnCheckedRoom(roomInfo);
				}
			}
			catch(Exception e)
			{
				Debug.Log(e);
			}
		}

		public void OnInitializedRoom(RoomInfo roomInfo)
		{
			try
			{
				foreach(var target in _roomCallbacks)
				{
					target.Value.OnInitializedRoom(roomInfo);
				}
			}
			catch(Exception e)
			{
				Debug.Log(e);
			}
		}

		public void OnJoinedRoom(RoomInfo roomInfo)
		{
			try
			{
				foreach(var target in _roomCallbacks)
				{
					target.Value.OnJoinedRoom(roomInfo);
				}
			}
			catch (Exception e)
			{
				Debug.Log(e);
			}
			
		}

		public void OnLocalPlayerLeaveRoom(LeaveRoomData roomData)
		{
			foreach (var target in _roomCallbacks)
			{
				target.Value.OnLocalPlayerLeaveRoom(roomData);
			}
		}

		public void OnInitPlayer(Player newPlayer)
		{
			foreach(var target in _roomCallbacks)
			{
				target.Value.OnInitPlayer(newPlayer);
			}
		}

		public void OnOtherPlayerLeaveRoom(LeaveRoomData roomData, Player leftPlayer)
		{
			foreach(var target in _roomCallbacks)
			{
				target.Value.OnOtherPlayerLeaveRoom(roomData, leftPlayer);
			}
		}
	}

}

