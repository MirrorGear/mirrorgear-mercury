
using MirrorGear.Mercury.Chat;
using UnityEngine;

namespace MirrorGear.Mercury
{
	public abstract class MercuryChatBehaviour : MonoBehaviour, IMercuryChatCallback
	{
		public virtual void OnEnable()
		{
			MercuryChatNetwork.AddChatCallBack(this);
		}

		public void OnDisable()
		{
			MercuryChatNetwork.RemoveChatCallBack(this);
		}

		public abstract void OnReceiveRoomChat(ChatData data);
		public abstract void OnReceiveWorldChat(ChatData data);
		public abstract void OnReceiveGlobalChat(ChatData data);
        public abstract void OnReceiveChatHistory(ChatHistoryData[] dataList);
		public abstract void OnError(Error error);
    }
}