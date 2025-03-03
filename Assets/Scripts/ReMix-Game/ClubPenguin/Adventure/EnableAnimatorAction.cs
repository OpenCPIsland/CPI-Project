using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Animation")]
	public class EnableAnimatorAction : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmBool Enable;

		public FsmBool ResetOnExit;

		private Animator animator;

		public override void Reset()
		{
			gameObject = null;
			Enable = true;
			ResetOnExit = false;
		}

		public override void OnEnter()
		{
			DoEnableAnimation(base.Fsm.GetOwnerDefaultTarget(gameObject));
			Finish();
		}

		private void DoEnableAnimation(GameObject go)
		{
			if (!(go == null))
			{
				animator = go.GetComponent<Animator>();
				if (animator == null)
				{
					LogError("Missing animation component!");
				}
				else if (animator != null)
				{
					animator.enabled = Enable.Value;
				}
			}
		}

		public override void OnExit()
		{
			if (ResetOnExit.Value && animator != null)
			{
				animator.enabled = !Enable.Value;
			}
		}
	}
}
