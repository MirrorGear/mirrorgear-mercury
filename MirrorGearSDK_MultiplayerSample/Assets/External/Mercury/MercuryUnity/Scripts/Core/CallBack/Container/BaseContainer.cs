using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirrorGear.Mercury
{
	public abstract class BaseContainer
	{
		protected MercuryClient _client;

		public BaseContainer(MercuryClient client)
		{
			_client = client;
		}

		public abstract void Clear();
	}
}


