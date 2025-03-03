using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Cinematography
{
	[ActionCategory("Cinematography")]
	public class CameraShakeAction : FsmStateAction
	{
		public float ShakeSpeed = 20f;

		public float ShakeAmount = 0.04f;

		public float ShakeDecay = 0.97f;

		public float ShakeDuration = 1f;

		public bool UseCurve = false;

		public FsmAnimationCurve ShakeSpeedCurve;

		public FsmAnimationCurve ShakeAmountCurve;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new CinematographyEvents.CameraShakeEvent(UseCurve, ShakeSpeed, ShakeAmount, ShakeDecay, ShakeDuration, ShakeSpeedCurve.curve, ShakeAmountCurve.curve));
			Finish();
		}
	}
}
