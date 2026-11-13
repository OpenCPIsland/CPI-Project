using System;
using UnityEngine;

namespace ClubPenguin.LOD
{
	[CreateAssetMenu(menuName = "LOD/Rules/Distance - Range")]
	public class LODWeightingDistanceRangeData : LODWeightingData
	{
		[Serializable]
		public struct Range
		{
			public int Distance;

			public float Weighting;
		}

		[HelpBox("This weights Requests based on how far away the remote player is from a transform in ranges.\n\nWhen calculating a Requests weighting for this rule it will categorize a remote player into a distance range.It does this by finding the first range that has a higher distance. That ranges weighting will then be used for that remote player.\n\nThe remote players distance is calculated by how far away they are in 'Unity Distance' from the target transform.", 110f)]
		[Tooltip("Find a GameObject with this tag and calculate distance from that transform")]
		public string TargetTag = "Player";

		public int DefaultWeighing = -9999;

		public Range[] Ranges;

		public override void InstantiateRequest(GameObject entity)
		{
			entity.AddComponent<LODWeightingDistanceRange>().Data = this;
		}
	}
}
