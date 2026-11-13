using UnityEngine;

namespace ClubPenguin.ScheduledWorldObjects
{
	public class ScheduledWorldObjectConfiguration : MonoBehaviour
	{
		[Range(1f, 3600f)]
		[Tooltip("The amount of time between updated selections")]
		public int CycleTimeInSeconds = 17;

		[Tooltip("How the next selection is determined.")]
		public SelectionBehaviour Behaviour = SelectionBehaviour.NonRepeatingRandom;
	}
}
