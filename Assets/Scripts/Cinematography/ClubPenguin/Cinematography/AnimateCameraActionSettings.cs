using ClubPenguin.Core;
using HutongGames.PlayMaker;
using System;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	[Serializable]
	public class AnimateCameraActionSettings : AbstractAspectRatioSpecificSettings
	{
		[UIHint(UIHint.TextArea)]
		public string Position;

		[UIHint(UIHint.TextArea)]
		public string Target;

		public string TargetAnimTrigger;

		public Vector3 TargetOffset;

		public float Duration;

		public FsmAnimationCurve Curve;
	}
}
