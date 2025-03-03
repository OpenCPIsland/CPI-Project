using UnityEngine;

namespace ClubPenguin.LOD
{
	[CreateAssetMenu(menuName = "LOD/Rules/Zone AOI")]
	public class LODWeightingZoneAOIData : LODWeightingData
	{
		[HelpBox("This weights Requests based on whether the remote player is within the server/zone AOI with a padding from a transform.", 45f)]
		[Tooltip("Find a GameObject with this tag and calculate distance from that transform")]
		public string TargetTag = "Player";

		public Vector3 Padding = new Vector3(-4f, -4f, -4f);

		public int WeighingInside = 0;

		public int WeightingOutside = -9999;

		public override void InstantiateRequest(GameObject entity)
		{
			entity.AddComponent<LODWeightingZoneAOI>().Data = this;
		}
	}
}
