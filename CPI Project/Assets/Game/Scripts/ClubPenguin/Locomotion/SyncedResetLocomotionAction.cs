using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[ActionCategory("Locomotion")]
	public class SyncedResetLocomotionAction : FsmStateAction
	{
		public FsmString Target;

		public override void OnEnter()
		{
			GameObject gameObject = GameObject.Find(Target.Value);
			if (gameObject != null)
			{
				LocomotionHelper.SetCurrentController<RunController>(gameObject);
			}
			Finish();
		}
	}
}
