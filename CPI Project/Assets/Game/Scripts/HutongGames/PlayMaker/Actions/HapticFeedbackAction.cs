using ClubPenguin.Locomotion;
using Disney.MobileNetwork;
using Disney.Native.iOS;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Haptic Feedback")]
	public class HapticFeedbackAction : FsmStateAction
	{
		public iOSHapticFeedback.HapticFeedbackType HapticFeedback;

		public override void OnEnter()
		{
			if (null != base.Owner.GetComponentInParent<PenguinUserControl>() && HapticFeedback != 0)
			{
				Service.Get<iOSHapticFeedback>().TriggerHapticFeedback(HapticFeedback);
			}
			Finish();
		}
	}
}
