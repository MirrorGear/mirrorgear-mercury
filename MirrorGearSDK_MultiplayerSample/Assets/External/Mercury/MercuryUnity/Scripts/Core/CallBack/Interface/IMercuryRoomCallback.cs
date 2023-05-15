using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirrorGear.Mercury
{
	public interface IMercuryRoomCallback
	{
		void OnRoomList(List<RoomListInfo> roomInfos);
		void OnCheckedRoom(RoomInfo roomInfo);
		void OnInitializedRoom(RoomInfo roomInfo);
		void OnJoinedRoom(RoomInfo roomInfo);
		void OnInitPlayer(Player newPlayer);
		void OnLocalPlayerLeaveRoom(LeaveRoomData roomData); // 자신이 나가는 경우
		void OnOtherPlayerLeaveRoom(LeaveRoomData roomData, Player leftPlayer); // 타인이 나가는 경우
	}
}