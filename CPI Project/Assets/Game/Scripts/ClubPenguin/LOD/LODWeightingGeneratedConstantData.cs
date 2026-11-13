using UnityEngine;

namespace ClubPenguin.LOD
{
	[CreateAssetMenu(menuName = "LOD/Rules/Generated - Constant")]
	public class LODWeightingGeneratedConstantData : LODWeightingData
	{
		[HelpBox("This gives Requests that have already had an object generated more weighting over Requests that do not have an object generated.\n\nThis can be used to stop generated Requests from having their object revoked for a similar weighted Request.", 70f)]
		public float Weighting;

		public override void InstantiateRequest(GameObject entity)
		{
			entity.AddComponent<LODWeightingGeneratedConstant>().Data = this;
		}
	}
}
