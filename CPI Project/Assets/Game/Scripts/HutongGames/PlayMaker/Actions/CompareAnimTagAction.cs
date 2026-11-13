using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Locomotion")]
	public class CompareAnimTagAction : FsmStateAction
	{
		public FsmString Target;

		public FsmString Tag;

		[RequiredField]
		public FsmEvent EqualsEvent;

		[RequiredField]
		public FsmEvent NotEqualsEvent;

		public override void OnEnter()
		{
			bool flag = false;
			GameObject gameObject = GameObject.Find(Target.Value);
			if (gameObject != null)
			{
				Animator component = gameObject.GetComponent<Animator>();
				AnimatorStateInfo animatorStateInfo = component.IsInTransition(0) ? component.GetNextAnimatorStateInfo(0) : component.GetCurrentAnimatorStateInfo(0);
				if (Tag == null || string.IsNullOrEmpty(Tag.Value))
				{
					if (animatorStateInfo.tagHash == Animator.StringToHash(""))
					{
						flag = true;
					}
				}
				else if (animatorStateInfo.tagHash == Animator.StringToHash(Tag.Value))
				{
					flag = true;
				}
			}
			if (flag)
			{
				base.Fsm.Event(EqualsEvent);
			}
			else
			{
				base.Fsm.Event(NotEqualsEvent);
			}
		}
	}
}
