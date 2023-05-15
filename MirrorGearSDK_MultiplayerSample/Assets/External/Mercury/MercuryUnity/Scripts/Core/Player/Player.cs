using UnityEngine;

namespace MirrorGear.Mercury
{
	public class Player : NetworkData
	{
		public string nickname; //사용자 입력 닉네임
		//public ulong accountId; //서버
		//public uint objId;
		
		public bool isObserver;

		public string prefabName;
		public string property;

		public Vector3 spawnPosition;
		public Vector3 eulerAngles;
		public Vector3 scale;

		internal void DeepCopy(Player dest)
		{
			accountId	= dest.accountId;
			objectId	= dest.objectId;
			nickname	= dest.nickname;
			isObserver	= dest.isObserver;
			prefabName	= dest.prefabName;
			property	= dest.property;
		}
	}
}


