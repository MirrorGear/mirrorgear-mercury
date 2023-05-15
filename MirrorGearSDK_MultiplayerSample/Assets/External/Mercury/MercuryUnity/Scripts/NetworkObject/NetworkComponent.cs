using UnityEngine;

namespace MirrorGear.Mercury
{
	public class NetworkComponent : MonoBehaviour, IMercuryObservable 
	{
		public MercuryObject mercuryObject => GetComponentInParent<MercuryObject>();
		public virtual NetworkData OnMercurySerialize() { return null; }
		public virtual void OnMercuryDeserialize(NetworkData data) { }
	}
}	