using UnityEngine;

namespace ClubPenguin.LOD
{
	[CreateAssetMenu(menuName = "LOD/Rules/Distance")]
	public class LODWeightingDistanceData : LODWeightingData
	{
		[HelpBox("This weights Requests based on how far away the remote player is from a transform. A closer remote player will have a higher weighting.\n\n'Start Weighting' is what the 'Unity Distance' gets subtracted from in order to calculate a remote players weighting for this rule.\n\nExample: A remote player would have the weighting calculated to the 'Start Weighting' if they are at the same position as the target transform.", 100f)]
		[Tooltip("Find a GameObject with this tag and calculate distance from that transform")]
		public string TargetTag = "Player";

		public float StartWeighting;

		public override void InstantiateRequest(GameObject entity)
		{
			entity.AddComponent<LODWeightingDistance>().Data = this;
		}
	}
}
