using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	[ActionCategory("Animated Camera")]
	public class AnimateCameraAction : FsmStateAction
	{
		[UIHint(UIHint.TextArea)]
		[ActionSection("Camera Tracks")]
		[HutongGames.PlayMaker.Tooltip("[Optional] Name of a static or moving Transform that will control camera's position, optionally containing a RailDolly or Animator Controller.")]
		public string Position = null;

		[HutongGames.PlayMaker.Tooltip("[Optional] Name of an animation trigger to immediately set upon activating specified Position above.")]
		public string PositionAnimTrigger = null;

		[UIHint(UIHint.TextArea)]
		[HutongGames.PlayMaker.Tooltip("[Optional] Name of a static or moving Transform used as the camera's look-at target, optionally containing a RailDolly or Animator Controller.")]
		public string Target = null;

		[HutongGames.PlayMaker.Tooltip("[Optional] Name of an animation trigger to immediately set upon activating specified Target above.")]
		public string TargetAnimTrigger = null;

		[HutongGames.PlayMaker.Tooltip("[Optional] World offset applied to specified Target above.")]
		public Vector3 TargetOffset = Vector3.zero;

		[HasFloatSlider(0f, 10f)]
		[ActionSection("Blending (0 = Cut)")]
		[HutongGames.PlayMaker.Tooltip("Time to blend from current camera transform to the above transform(s). A Duration of 0 will Cut rather than Blend.")]
		public float Duration = 0f;

		[HutongGames.PlayMaker.Tooltip("Blend curve to use when transitioning from current camera transform to above transform(s). Required for non-zero Duration.")]
		public FsmAnimationCurve Curve;

		[ActionSection("Behaviours")]
		[HutongGames.PlayMaker.Tooltip("When ticked, this action will wait until completion (RailDolly finishes, Animation finishes, or Blending completes in the case of regular Transform(s)).")]
		public bool WaitForCompletion = false;

		[ActionSection("Platform Specific Settings")]
		public AnimateCameraActionSettings[] OverrideSettings;

		private EventDispatcher dispatcher;

		public override void Reset()
		{
			Curve = new FsmAnimationCurve();
			Curve.curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		}

		public override void OnEnter()
		{
			AnimateCameraActionSettings animateCameraActionSettings = PlatformUtils.FindAspectRatioSettings(OverrideSettings);
			if (animateCameraActionSettings != null)
			{
				applySettings(animateCameraActionSettings);
			}
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraContextEvent(Director.CameraContext.Cinematic, Curve.curve, Duration));
			Transform target = null;
			Transform position = null;
			if (!string.IsNullOrEmpty(Target))
			{
				GameObject gameObject = GameObject.Find(Target);
				if (gameObject != null)
				{
					target = gameObject.transform;
				}
				else
				{
					Disney.LaunchPadFramework.Log.LogError(this, "SetCameraPositionAndTargetAction: Unable to find Camera Target called " + Target);
				}
			}
			if (!string.IsNullOrEmpty(Position))
			{
				GameObject gameObject = GameObject.Find(Position);
				if (gameObject != null)
				{
					position = gameObject.transform;
				}
				else
				{
					Disney.LaunchPadFramework.Log.LogError(this, "SetCameraPositionAndTargetAction: Unable to find Camera Anchor called " + Position);
				}
			}
			int posAnimTrigger = 0;
			if (!string.IsNullOrEmpty(PositionAnimTrigger))
			{
				posAnimTrigger = Animator.StringToHash(PositionAnimTrigger);
			}
			int targetAnimTrigger = 0;
			if (!string.IsNullOrEmpty(TargetAnimTrigger))
			{
				targetAnimTrigger = Animator.StringToHash(TargetAnimTrigger);
			}
			if (WaitForCompletion)
			{
				dispatcher.AddListener<ScriptedCameraEvents.CameraCompleted>(onCameraCompleted);
			}
			dispatcher.DispatchEvent(new ScriptedCameraEvents.SetCameraEvent(position, target, posAnimTrigger, targetAnimTrigger, TargetOffset, Curve.curve, Duration, WaitForCompletion));
			if (!WaitForCompletion)
			{
				Finish();
			}
		}

		private bool onCameraCompleted(ScriptedCameraEvents.CameraCompleted evt)
		{
			dispatcher.RemoveListener<ScriptedCameraEvents.CameraCompleted>(onCameraCompleted);
			Finish();
			return false;
		}

		private void applySettings(AnimateCameraActionSettings settings)
		{
			Position = settings.Position;
			Target = settings.Target;
			TargetAnimTrigger = settings.TargetAnimTrigger;
			TargetOffset = settings.TargetOffset;
			Duration = settings.Duration;
			Curve = settings.Curve;
		}
	}
}
