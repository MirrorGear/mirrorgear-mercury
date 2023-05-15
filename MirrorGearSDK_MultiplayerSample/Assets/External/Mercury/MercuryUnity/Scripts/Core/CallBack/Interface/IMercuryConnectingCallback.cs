namespace MirrorGear.Mercury
{
	public interface IMercuryConnectingCallback
	{
		void OnNetworkConnectionFail();
		void OnConnected();
		void OnDisconnected();
		void OnReConnectStart();
		void OnReConnectEnd();
		void OnLoginCompleted();
		void OnObjectInitData(MercuryObjectSyncDataList data);
		void OnNetworkError(Error error);
		void OnHeartBeat(long laytency);
	}
}


