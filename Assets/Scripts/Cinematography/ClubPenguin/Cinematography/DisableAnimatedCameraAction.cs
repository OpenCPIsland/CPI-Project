using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	[ActionCategory("Animated Camera")]
	public class DisableAnimatedCameraAction : FsmStateAction
	{
		public FsmAnimationCurve BlendCurve = null;

		public float BlendDuration = 0f;

		public bool WaitForCompletion = true;

		public float WaitDuration = 0f;

		private float startTime;

		private EventDispatcher dispatcher;

		public override void Reset()
		{
			BlendCurve = new FsmAnimationCurve();
			BlendCurve.curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		}

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraContextEvent(Director.CameraContext.Gameplay, BlendCurve.curve, BlendDuration));
			if (WaitForCompletion || WaitDuration > 0f)
			{
				startTime = Time.time;
			}
			else
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			float num = Time.time - startTime;
			if (WaitForCompletion)
			{
				if (num >= BlendDuration)
				{
					Finish();
				}
			}
			else if (num >= WaitDuration)
			{
				Finish();
			}
		}
	}
}
