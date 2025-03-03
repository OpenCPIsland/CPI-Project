using UnityEngine;

namespace ClubPenguin.LOD
{
	public abstract class LODWeightingData : ScriptableObject
	{
		public abstract void InstantiateRequest(GameObject entity);
	}
}
