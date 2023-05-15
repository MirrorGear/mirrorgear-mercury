using MirrorGear.Mercury.Network;

namespace MirrorGear.Mercury
{
	public interface IPacketCallback
	{
		void OnPacketSendStarted();
		void OnPacketResponsed();
		void OnPacketError(Error code);
		void OnWebMessage(WebResult result, string data);
		void OnReceiveDynamicData(DynamicData data);
	}
}