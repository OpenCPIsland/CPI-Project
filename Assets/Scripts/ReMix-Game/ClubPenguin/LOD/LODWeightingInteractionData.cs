using UnityEngine;

namespace ClubPenguin.LOD
{
	[CreateAssetMenu(menuName = "LOD/Rules/Interaction")]
	public class LODWeightingInteractionData : LODWeightingData
	{
		[HelpBox("This gives Requests that interact more weighting and is adjusted over time.\n\nNOTE: This only applies to visible avatars", 45f)]
		public WeightingCurveData CurveData;

		public override void InstantiateRequest(GameObject entity)
		{
			entity.AddComponent<LODWeightingInteraction>().Data = this;
		}
	}
}
