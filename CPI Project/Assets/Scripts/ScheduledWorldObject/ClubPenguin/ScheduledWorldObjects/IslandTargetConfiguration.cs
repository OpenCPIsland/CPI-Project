using UnityEngine;

namespace ClubPenguin.ScheduledWorldObjects
{
	public class IslandTargetConfiguration : MonoBehaviour
	{
		[Range(1f, 1000f)]
		[Tooltip("Minimum hit count")]
		public int MinHitCount = 5;

		[Tooltip("Population multiplier. This number times the population of the target area increases the hit points for the target.")]
		[Range(1f, 5f)]
		public int PopulationHitCountMultiplier = 1;
	}
}
