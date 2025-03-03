using ClubPenguin.Locomotion;
using ClubPenguin.Props;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Props")]
	public class IsHoldingPropAction : FsmStateAction
	{
		[RequiredField]
		public PropDefinition propDefinition;

		public FsmEvent OnTrueEvent;

		public FsmEvent OnFalseEvent;

		public override void OnEnter()
		{
			bool flag = false;
			PropUser component = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PropUser>();
			if (component != null && component.Prop != null)
			{
				flag = (component.Prop.PropId == propDefinition.GetNameOnServer());
				if (flag)
				{
					Animator componentInParent = base.Owner.GetComponentInParent<Animator>();
					if (componentInParent != null)
					{
						AnimatorStateInfo animatorStateInfo = LocomotionUtils.GetAnimatorStateInfo(componentInParent, 1);
						flag = (LocomotionUtils.IsHolding(animatorStateInfo) || LocomotionUtils.IsRetrieving(animatorStateInfo));
					}
					else
					{
						LogWarning("Failed to get the animator controller. IsHoldingPropAction could only determine that the owner has a prop component and did not check the animator's state.");
					}
				}
			}
			if (flag)
			{
				base.Fsm.Event(OnTrueEvent);
			}
			else
			{
				base.Fsm.Event(OnFalseEvent);
			}
			Finish();
		}
	}
}
