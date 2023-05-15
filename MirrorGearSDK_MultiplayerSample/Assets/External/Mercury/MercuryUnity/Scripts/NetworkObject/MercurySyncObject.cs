using System;
using UnityEngine;

namespace MirrorGear.Mercury
{
	public class MercurySyncObject : NetworkComponent
	{
		protected string _property;
		protected ulong _ownerId;

		public SyncObjecType type = SyncObjecType.Unique;

		public string Property => _property;

		public void ResetObject()
		{
			_ownerId = 0;
		}
	}

}
