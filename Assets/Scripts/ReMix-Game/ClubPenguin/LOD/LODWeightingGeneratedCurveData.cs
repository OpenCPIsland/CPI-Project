using UnityEngine;

namespace ClubPenguin.LOD
{
	[CreateAssetMenu(menuName = "LOD/Rules/Generated - Curve")]
	public class LODWeightingGeneratedCurveData : LODWeightingData
	{
		[HelpBox("This gives Requests that have an object generated more weighting that is adjusted over time.", 25f)]
		public WeightingCurveData CurveData;

		public override void InstantiateRequest(GameObject entity)
		{
			entity.AddComponent<LODWeightingGeneratedCurve>().Data = this;
		}
	}
}
