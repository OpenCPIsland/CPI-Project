using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Animation")]
	public class AddAnimatorAction : FsmStateAction
	{
		[RequiredField]
		public FsmGameObject TargetObject;

		public RuntimeAnimatorController animatorController;

		public override void OnEnter()
		{
			Animator animator = TargetObject.Value.GetComponent<Animator>();
			if (animator == null)
			{
				animator = TargetObject.Value.AddComponent<Animator>();
			}
			animator.runtimeAnimatorController = animatorController;
			Finish();
		}
	}
}
