namespace MirrorGear.Mercury.Chat
{
	public static class MercuryChatNetwork
	{
		private static MercuryChatClient _chatClient;
		static MercuryChatNetwork()
        {
			_chatClient = new MercuryChatClient();
		}

		public static void AddChatCallBack(IMercuryChatCallback callback)
		{
			if (_chatClient == null)
				return;

			_chatClient.AddChatCallBack(callback);
		}

		public static void RemoveChatCallBack(IMercuryChatCallback callback)
		{
			if (_chatClient == null)
				return;

			_chatClient.RemoveChatCallBack(callback);
		}

		public static void SendChatMessage(string message, ChatType type)
		{
			_chatClient.Send(message, type);
		}

	}
}
