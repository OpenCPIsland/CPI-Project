using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Animation")]
	public class AnimatorIntValueAction : FsmStateAction
	{
		public bool UseOwnerGameObject = false;

		public FsmString Target;

		public FsmGameObject TargetByObject;

		public FsmString Parameter;

		public FsmInt Value;

		public bool ResetOnExit;

		private Animator anim;

		private FsmInt valueOnEnter;

		public override void Awake()
		{
			InitializeComponents();
		}

		private void InitializeComponents()
		{
			if (!(anim == null))
			{
				return;
			}
			if (UseOwnerGameObject)
			{
				if (base.Owner != null)
				{
					anim = base.Owner.GetComponentInParent<Animator>();
				}
				return;
			}
			GameObject gameObject = null;
			if (!string.IsNullOrEmpty(Target.Value))
			{
				gameObject = GameObject.Find(Target.Value);
			}
			else if (TargetByObject.Value != null)
			{
				gameObject = TargetByObject.Value;
			}
			if (gameObject != null)
			{
				anim = gameObject.GetComponent<Animator>();
			}
		}

		public override void OnEnter()
		{
			InitializeComponents();
			if (anim != null)
			{
				valueOnEnter = anim.GetInteger(Parameter.Value);
				anim.SetInteger(Parameter.Value, Value.Value);
			}
			Finish();
		}

		public override void OnExit()
		{
			if (anim != null && ResetOnExit)
			{
				anim.SetInteger(Parameter.Value, valueOnEnter.Value);
			}
		}
	}
}
