using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Locomotion")]
	public class SyncedAnimatorBoolAction : FsmStateAction
	{
		public FsmString Target;

		public string Parameter;

		public bool BoolState;

		public override void OnEnter()
		{
			GameObject gameObject = GameObject.Find(Target.Value);
			if (gameObject != null)
			{
				Animator component = gameObject.GetComponent<Animator>();
				component.SetBool(Parameter, BoolState);
			}
			Finish();
		}
	}
}
