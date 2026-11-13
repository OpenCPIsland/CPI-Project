using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[ActionCategory("Locomotion")]
	public class SyncedSetAnimatorControllerAction : FsmStateAction
	{
		public FsmString Target;

		public RuntimeAnimatorController Controller;

		public override void OnEnter()
		{
			GameObject gameObject = GameObject.Find(Target.Value);
			if (gameObject != null && Controller != null)
			{
				Animator component = gameObject.GetComponent<Animator>();
				component.runtimeAnimatorController = Controller;
			}
			Finish();
		}
	}
}
