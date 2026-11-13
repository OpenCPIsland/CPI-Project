using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Animation")]
	public class AnimatorBoolAction : FsmStateAction
	{
		public bool UseOwnerGameObject = false;

		public FsmGameObject Target;

		public string Parameter;

		public bool Value;

		public bool ResetOnExit;

		private Animator anim;

		private bool valueOnEnter;

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
				valueOnEnter = anim.GetBool(Parameter);
				anim.SetBool(Parameter, Value);
			}
			Finish();
		}

		public override void OnExit()
		{
			if (anim != null && ResetOnExit)
			{
				anim.SetBool(Parameter, valueOnEnter);
			}
		}
	}
}
