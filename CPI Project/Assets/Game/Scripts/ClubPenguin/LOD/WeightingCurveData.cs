using System;
using UnityEngine;

namespace ClubPenguin.LOD
{
	[Serializable]
	public struct WeightingCurveData
	{
		public float StartWeighting;

		public AnimationCurve Curve;

		[Tooltip("The length of time multiplier for the curve")]
		public float Length;
	}
}
