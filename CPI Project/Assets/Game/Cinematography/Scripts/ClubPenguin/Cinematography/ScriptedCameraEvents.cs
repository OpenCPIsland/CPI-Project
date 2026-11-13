using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public static class ScriptedCameraEvents
	{
		public struct SetCameraEvent
		{
			public Transform Anchor;

			public Transform Target;

			public Vector3 TargetOffset;

			public AnimationCurve BlendCurve;

			public float BlendDuration;

			public int PosAnimTrigger;

			public int TargetAnimTrigger;

			public bool WaitForCompletion;

			public SetCameraEvent(Transform position, Transform target, int posAnimTrigger, int targetAnimTrigger, Vector3 targetOffset, AnimationCurve curve, float duration, bool waitForCompletion)
			{
				Anchor = position;
				Target = target;
				TargetOffset = targetOffset;
				BlendCurve = curve;
				BlendDuration = duration;
				PosAnimTrigger = posAnimTrigger;
				TargetAnimTrigger = targetAnimTrigger;
				WaitForCompletion = waitForCompletion;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CameraCompleted
		{
		}
	}
}
