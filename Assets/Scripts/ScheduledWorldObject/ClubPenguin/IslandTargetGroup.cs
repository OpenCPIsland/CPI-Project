using UnityEngine;

namespace ClubPenguin
{
	public class IslandTargetGroup : MonoBehaviour
	{
		[Tooltip("Length of this phase of the target minigame")]
		[Range(15f, 120f)]
		public int SessionTimeInSeconds = 20;

		public float GroupAnimDelay = 0f;
	}
}
