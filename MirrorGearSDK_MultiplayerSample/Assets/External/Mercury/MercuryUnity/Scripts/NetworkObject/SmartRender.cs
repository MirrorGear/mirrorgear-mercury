using System.Collections;
using UnityEngine;

namespace MirrorGear.Mercury
{
	public class SmartRender : MercuryBehaviour
	{
		private const int TickTime = 1000;

		private int _nextCheckTick;

		public float renderDistance = 50;

		protected void Start()
		{
			StartCoroutine(CheckDistance());
		}

		public IEnumerator CheckDistance()
		{
			while(true)
			{
				int currentSceneStart = (int)(Time.realtimeSinceStartup * TickTime);
				if(currentSceneStart > _nextCheckTick)
				{
					_nextCheckTick = currentSceneStart + TickTime / 3;
					
					foreach (var objPair in MercuryNetwork.MecuryObjects)
					{
						if(mercuryObject == objPair.Value)
							continue;

						var distance = Vector3.Distance(mercuryObject.transform.position, objPair.Value.transform.position);
						objPair.Value.OnRangeCollision(distance < renderDistance);
					}
				}
				
				yield return new WaitForSeconds(1f);
			}
		}
	}
}