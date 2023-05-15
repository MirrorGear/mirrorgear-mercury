namespace MirrorGear.Mercury
{
	public interface IMercuryObservable
	{
		/// <summary>
		/// Serialize
		/// </summary>
		/// <returns></returns>
		NetworkData OnMercurySerialize();
		/// <summary>
		/// Deserialize
		/// </summary>
		/// <param name="data"></param>
		void OnMercuryDeserialize(NetworkData data);
	}

	public interface IMercuryTransformObservable
	{
		void OnMercurySerialize(NetworkData data);
	}
}

