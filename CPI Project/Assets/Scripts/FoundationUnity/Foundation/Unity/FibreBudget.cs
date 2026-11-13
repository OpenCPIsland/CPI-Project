using System;
using UnityEngine;

namespace Foundation.Unity
{
	[CreateAssetMenu]
	public class FibreBudget : ScriptableObject
	{
		[Serializable]
		public struct TimeSlice
		{
			public string Name;

			public float DurationMs;
		}

		public TimeSlice[] TimeSlices;
	}
}
