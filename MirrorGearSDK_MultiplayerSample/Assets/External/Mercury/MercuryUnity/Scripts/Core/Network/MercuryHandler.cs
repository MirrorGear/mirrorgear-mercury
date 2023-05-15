using UnityEngine;

namespace MirrorGear.Mercury
{
	public class MercuryHandler : SingletonComponent<MercuryHandler>
	{
		private const int TickTime = 1000;

		private MercuryClient _client;
		private int _nextSerializeTick;
		private int _nextSendTick;

		protected internal int SerializeUpdateInterval;
		protected internal int UpdateInterval;
		
		protected override void Awake()
		{
			base.Awake();
			DontDestroyOnLoad(this);
		}

		protected override void OnDestroy()
		{
			if(_client != null)
				_client.DisConnect();
			base.OnDestroy();
		}

		public void SetMercuryClient(MercuryClient client)
		{
			_client = client;
			_client.SetBehaviour(this);
		}

		protected void LateUpdate()
		{
			int currentSceneStart = (int)(Time.realtimeSinceStartup * TickTime);
			if(_client.IsRoomConnection == true && currentSceneStart > _nextSerializeTick)
			{
				MercuryNetwork.MercuryObjectUpdate();
				_nextSerializeTick = currentSceneStart + TickTime / MercuryNetwork.ServerSettings.sendPerSecond;

				if(MercuryNetwork.IsWait == true)
					MercuryNetwork.SendData();
			}
		}

		private void OnApplicationQuit()
		{
			//Task./
		}
	}
}

