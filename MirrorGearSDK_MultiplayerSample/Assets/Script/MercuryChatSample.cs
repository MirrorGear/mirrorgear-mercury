using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MirrorGear.Mercury;
using MirrorGear.Mercury.Chat;

public class MercuryChatSample : MercuryChatBehaviour
{
    public void OnInp_SendMessage(string msg)
    {
        MercuryChatNetwork.SendChatMessage(msg, ChatType.Room);
    }

	#region IMercuryChatCallback
	public override void OnError(Error error)
    {
        Debug.Log("[OnError]"+error);
    }

    public override void OnReceiveChatHistory(ChatHistoryData[] dataList)
    {
    }

    public override void OnReceiveGlobalChat(ChatData data)
    {
    }

    public override void OnReceiveRoomChat(ChatData data)
    {
		Debug.Log("[Room] " + data.message);
	}

    public override void OnReceiveWorldChat(ChatData data)
    {
    }

	#endregion
}
