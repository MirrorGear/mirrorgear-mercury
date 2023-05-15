

namespace MirrorGear.Mercury.Chat
{
	public interface IMercuryChatCallback
	{
		void OnReceiveRoomChat(ChatData data);
		void OnReceiveWorldChat(ChatData data);
		void OnReceiveGlobalChat(ChatData data);
		void OnReceiveChatHistory(ChatHistoryData[] dataList);
		void OnError(Error error);
	}
}