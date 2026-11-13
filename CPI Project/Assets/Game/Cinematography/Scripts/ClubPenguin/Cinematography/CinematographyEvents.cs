using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public static class CinematographyEvents
	{
		public struct CameraLogicChangeEvent
		{
			public CameraController Controller;

			public bool DisablePostEffects;

			public bool ForceCutTransition;
		}

		public struct CameraLogicResetEvent
		{
			public CameraController Controller;
		}

		public struct CameraSnapLockEvent
		{
			public bool SnapPosition;

			public bool SnapAim;

			public CameraSnapLockEvent(bool snapPosition, bool snapAim)
			{
				SnapPosition = snapPosition;
				SnapAim = snapAim;
			}
		}

		public struct ChangeCameraTarget
		{
			public Transform Target;

			public ChangeCameraTarget(Transform target)
			{
				Target = target;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct DirectorStartComplete
		{
		}

		public struct ChangeCameraContextEvent
		{
			public Director.CameraContext Context;

			public AnimationCurve BlendCurve;

			public float BlendDuration;

			public ChangeCameraContextEvent(Director.CameraContext context, AnimationCurve blendCurve = null, float blendDuration = 0f)
			{
				Context = context;
				BlendCurve = blendCurve;
				BlendDuration = blendDuration;
			}
		}

		public struct CameraBlendComplete
		{
			public Director.CameraContext FromCamera;

			public Director.CameraContext ToCamera;

			public CameraBlendComplete(Director.CameraContext fromCamera, Director.CameraContext toCamera)
			{
				FromCamera = fromCamera;
				ToCamera = toCamera;
			}
		}

		public struct ZoomCameraEvent
		{
			public float ZoomPercentOnMove;

			public float ZoomPercentOnIdle;

			public float ZoomOutDelay;

			public float HeightOffset;

			public float MinDist;

			public bool State;

			public ZoomCameraEvent(bool state, float percentOnMove = 0.3f, float percentOnIdle = 0.3f, float zoomOutDelay = 0f, float heightOffset = 0f, float minDist = 0f)
			{
				State = state;
				ZoomPercentOnIdle = percentOnIdle;
				ZoomPercentOnMove = percentOnMove;
				ZoomOutDelay = zoomOutDelay;
				HeightOffset = heightOffset;
				MinDist = minDist;
			}
		}

		public struct CameraShakeEvent
		{
			public bool UseCurve;

			public float ShakeSpeed;

			public float ShakeAmount;

			public float ShakeDecay;

			public float ShakeDuration;

			public AnimationCurve ShakeSpeedCurve;

			public AnimationCurve ShakeAmountCurve;

			public CameraShakeEvent(bool useCurve, float shakeSpeed, float shakeAmount, float shakeDecay, float shakeDuration, AnimationCurve shakeSpeedCurve, AnimationCurve shakeAmountCurve)
			{
				UseCurve = useCurve;
				ShakeSpeed = shakeSpeed;
				ShakeAmount = shakeAmount;
				ShakeDecay = shakeDecay;
				ShakeDuration = shakeDuration;
				ShakeSpeedCurve = shakeSpeedCurve;
				ShakeAmountCurve = shakeAmountCurve;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct DisableElasticGlancer
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct EnableElasticGlancer
		{
		}

		public struct SetGroupCullingOverride
		{
			public string[] GroupNames;

			public SetGroupCullingOverride(string[] groupNames)
			{
				GroupNames = groupNames;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ClearGroupCullingOverride
		{
		}

		public struct RenderingStateChanged
		{
			public readonly bool IsRenderingEnabled;

			public RenderingStateChanged(bool isRenderingEnabled)
			{
				IsRenderingEnabled = isRenderingEnabled;
			}
		}
	}
}
