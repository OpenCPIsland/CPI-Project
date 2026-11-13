using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Locomotion")]
	public class SyncedAnimatorTriggerAction : FsmStateAction
	{
		public FsmString Target;

		public string Parameter;

		public override void OnEnter()
		{
			GameObject gameObject = GameObject.Find(Target.Value);
			if (gameObject != null)
			{
				Animator component = gameObject.GetComponent<Animator>();
				component.SetTrigger(Parameter);
			}
			Finish();
		}
	}
}
