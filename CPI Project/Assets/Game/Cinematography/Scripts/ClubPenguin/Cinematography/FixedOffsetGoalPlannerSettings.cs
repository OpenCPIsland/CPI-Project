using ClubPenguin.Core;
using System;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	[Serializable]
	public class FixedOffsetGoalPlannerSettings : AbstractAspectRatioSpecificSettings
	{
		public Vector3 Offset;
	}
}
