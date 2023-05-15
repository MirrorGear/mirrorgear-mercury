using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MirrorGear.Mercury;
using UnityEngine.UI;

public class MercuryConnector : MercuryBehaviourCallBacks
{
    public InputField inpName;
    public InputField inpId;
    public InputField inpRoomName;
    public Text lbLatency;

    public Transform playerParent;
    private long latency;

    // Start is called before the first frame update
    void Awake()
    {
        MercuryNetwork.Connection();
    }

    private void Update()
    {
        lbLatency.text = $"latecy: {latency} ms";
    }

    private void OnApplicationQuit()
    {
        MercuryNetwork.Disconnect();
    }

    public void OnButton_Login()
    {
        string nick = inpName.text;
        ulong.TryParse(inpId.text, out var id);

        if (string.IsNullOrWhiteSpace(nick) || id == 0)
        {
            Debug.LogWarning("Nick or Id empty");
            return;
        }
        MercuryNetwork.Login(nick, id);
    }

    public void OnButton_Join()
    {
        var roomName = inpRoomName.text;
        if (string.IsNullOrWhiteSpace(roomName))
        {
            Debug.LogWarning("roomName empty");
            return;
        }

        var option = new RoomOption 
        { 
            name = roomName
		};
        MercuryNetwork.JoinOrCreateRoom(option, "Player/SamplePlayer01");
    }

	#region IMercuryConnectingCallback
	public override void OnConnected()
    {
        Debug.Log("OnConnected");
    }

    public override void OnDisconnected()
    {
		Debug.Log("OnDisconnected");
	}

    // 유니티 UI 변경 불가능한 메서드
    public override void OnHeartBeat(long latency)
    {
        this.latency = latency;
    }

    public override void OnLoginCompleted()
    {
        Debug.Log("OnLoginCompleted");
    }

    #endregion

    #region IMercuryRoomCallback

    public override void OnInitPlayer(Player newPlayer)
    {
        var comp = MercuryNetwork.Instantiate(newPlayer, playerParent);
        var samplePlayer = comp.GetComponent<SamplePlayer>();
        var controller = comp.GetComponent<MoveController>();
        samplePlayer.lbName.text = newPlayer.nickname;

        if(newPlayer.accountId != MercuryNetwork.LocalPlayer.accountId)
        {
            Destroy(controller);
        }
    }

    public override void OnLocalPlayerLeaveRoom(LeaveRoomData roomData)
    {
        Debug.Log($"[OnLocalPlayerLeaveRoom] {nameof(roomData)}: {roomData}");
    }

    public override void OnOtherPlayerLeaveRoom(LeaveRoomData roomData, Player leftPlayer)
    {
        Debug.Log($"[OnOtherPlayerLeaveRoom] {nameof(roomData)}: {roomData}, {nameof(leftPlayer)}: {leftPlayer}");
        var objId = leftPlayer?.objectId;
        if (objId.HasValue) MercuryNetwork.RemoveObject(objId.Value);
    }

    #endregion
}
