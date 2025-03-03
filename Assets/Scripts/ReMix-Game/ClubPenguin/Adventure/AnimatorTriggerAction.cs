using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Animation")]
	public class AnimatorTriggerAction : FsmStateAction
	{
		public bool UseOwnerGameObject = false;

		public FsmGameObject Target;

		public FsmString Parameter;

		private Animator anim;

		private void InitializeComponents()
		{
			if (anim == null)
			{
				if (UseOwnerGameObject)
				{
					anim = base.Owner.GetComponentInParent<Animator>();
				}
				else if (Target.Value != null)
				{
					anim = Target.Value.GetComponent<Animator>();
				}
			}
		}

		public override void OnEnter()
		{
			InitializeComponents();
			if (anim != null)
			{
				anim.SetTrigger(Parameter.Value);
			}
			Finish();
		}
	}
}
