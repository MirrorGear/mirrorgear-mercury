using System;
using System.Collections.Generic;
using MirrorGear.Mercury;
using MirrorGear.Mercury.Network;
using UnityEngine;

namespace MirrorGear.Mercury
{
	public class MercuryBehaviourCallBacks : MonoBehaviour, IMercuryConnectingCallback, IMercuryRoomCallback, IPacketCallback
	{
		protected virtual void OnEnable()
		{
			MercuryNetwork.AddCallBack(this);
		}

		protected virtual void OnDisable()
		{
			MercuryNetwork.RemoveCallBack(this);
		}
		public virtual void OnNetworkConnectionFail() { }
		public virtual void OnConnected() { }
		public virtual void OnDisconnected() { }
		public virtual void OnReConnectStart() { }
		public virtual void OnReConnectEnd() { }
		public virtual void OnLoginCompleted() { }
		public virtual void OnObjectInitData(MercuryObjectSyncDataList data) { }
		public virtual void OnNetworkError(Error error) { }
		public virtual void OnHeartBeat(long laytency) { }
		public virtual void OnCheckedRoom(RoomInfo roomInfo) { }
		public virtual void OnInitializedRoom(RoomInfo roomInfo) { }
		public virtual void OnJoinedRoom(RoomInfo roomInfo) { }
		public virtual void OnLocalPlayerLeaveRoom(LeaveRoomData roomData) { }
		public virtual void OnRoomList(List<RoomListInfo> roomInfos) { }
		public virtual void OnInitPlayer(Player newPlayer) { }
		public virtual void OnOtherPlayerLeaveRoom(LeaveRoomData roomData, Player leftPlayer) { }
		public virtual void OnPacketSendStarted() { }
		public virtual void OnPacketResponsed() { }
		public virtual void OnPacketError(Error code) { }
		public virtual void OnWebMessage(WebResult result, string data) { }
        public virtual void OnReceiveDynamicData(DynamicData data) { }
    }
}