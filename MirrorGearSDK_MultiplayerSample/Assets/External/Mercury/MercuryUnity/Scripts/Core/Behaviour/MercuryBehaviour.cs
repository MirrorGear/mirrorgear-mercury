using UnityEngine;

namespace MirrorGear.Mercury
{
	public class MercuryBehaviour : MonoBehaviour
	{
		private MercuryObject _mercuryObjectCache;
		public MercuryObject mercuryObject
		{
			get
			{
				if(_mercuryObjectCache == null)
					_mercuryObjectCache = GetComponent<MercuryObject>();

				return _mercuryObjectCache;
			}
		}

		public virtual NetworkData OnMercurySerialize() { return null; }
		public virtual void OnMercuryDeserialize(NetworkData data) { }
	}
}


