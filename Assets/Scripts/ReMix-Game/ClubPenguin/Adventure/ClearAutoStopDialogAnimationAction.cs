using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class ClearAutoStopDialogAnimationAction : FsmStateAction
	{
		public string MascotName;

		public string AnimTriggerToReset = "Dialog";

		public override void OnEnter()
		{
			GameObject mascotObject = Service.Get<MascotService>().GetMascotObject(MascotName);
			if (mascotObject != null)
			{
				MascotController component = mascotObject.GetComponent<MascotController>();
				component.AutoStopDialog = false;
				if (!string.IsNullOrEmpty(AnimTriggerToReset))
				{
					component.GetComponent<Animator>().ResetTrigger(AnimTriggerToReset);
				}
			}
			Finish();
		}
	}
}
