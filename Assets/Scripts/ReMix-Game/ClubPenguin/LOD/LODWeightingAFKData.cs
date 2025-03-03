using UnityEngine;

namespace ClubPenguin.LOD
{
	[CreateAssetMenu(menuName = "LOD/Rules/Away")]
	public class LODWeightingAFKData : LODWeightingData
	{
		[HelpBox("This will give requests that are AFK a negative weighting, giving them the lowest priority.", 25f)]
		public float Weighting = -9999999f;

		public override void InstantiateRequest(GameObject entity)
		{
			entity.AddComponent<LODWeightingAFK>().Data = this;
		}
	}
}
