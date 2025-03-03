using Disney.MobileNetwork;
using Disney.Native.iOS;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class HapticFeedbackAction : Action
	{
		public iOSHapticFeedback.HapticFeedbackType HapticFeedback = iOSHapticFeedback.HapticFeedbackType.None;

		protected override void CopyTo(Action _destWarper)
		{
			HapticFeedbackAction hapticFeedbackAction = _destWarper as HapticFeedbackAction;
			hapticFeedbackAction.HapticFeedback = HapticFeedback;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			GameObject target = GetTarget();
			if (HapticFeedback != 0 && target.CompareTag("Player"))
			{
				Service.Get<iOSHapticFeedback>().TriggerHapticFeedback(HapticFeedback);
			}
			Completed();
		}
	}
}
