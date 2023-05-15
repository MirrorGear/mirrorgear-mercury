

namespace MirrorGear.Mercury.Chat
{
	public class MercuryChatClient : IChatClient
	{
		private MercuryChatEngine _chatEngine;
		private IMercuryChatCallback _callback;
		
		public MercuryChatClient()
		{
			//_chatEngine = mercuryEngine.SetChatClient(this);
			_chatEngine = MercuryNetwork.SetChatClient(this);
		}
		
		public void AddChatCallBack(IMercuryChatCallback callback)
		{
			_callback = callback;
		}

		public void RemoveChatCallBack(IMercuryChatCallback callback)
		{
			_callback = null;
		}

		public void Send(string message, ChatType type)
		{
			var roomId = type == ChatType.Global ? 0 : MercuryClient.CurrentRoom.roomId;
			
			_chatEngine.Send(MercuryNetwork.LocalPlayer.nickname, message, type, roomId);
		}

		#region Interface

		public void OnConnected(bool isConnected)
		{

		}

		public void OnDisConnected()
		{

		}

		public void OnChat(ChatData data)
		{
			if(_callback == null)
				return;

			switch(data.chatType)
			{
				case ChatType.Room:
					_callback.OnReceiveRoomChat(data);
					break;
				case ChatType.World:
					_callback.OnReceiveWorldChat(data);
					break;
				case ChatType.Global:
					_callback.OnReceiveGlobalChat(data);
					break;
			}
		}
		public void OnError(Error error)
		{
			_callback.OnError(error);
		}

        public void OnChatHistory(ChatHistoryData[] dataList)
        {
			_callback.OnReceiveChatHistory(dataList);
        }

        #endregion
    }
}

