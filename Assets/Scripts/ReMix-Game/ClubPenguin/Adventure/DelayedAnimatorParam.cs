using Disney.Kelowna.Common;
using HutongGames.PlayMaker;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class DelayedAnimatorParam : FsmStateAction
	{
		[RequiredField]
		public FsmGameObject TargetGameObject;

		[RequiredField]
		public FsmString AnimationTrigger;

		public float DelayBeforeTrigger = 0f;

		public FsmEvent FinishedEvent;

		public override void OnEnter()
		{
			if (!TargetGameObject.IsNone)
			{
				CoroutineRunner.Start(TriggerDelay(), this, "");
			}
		}

		private IEnumerator TriggerDelay()
		{
			yield return new WaitForSeconds(DelayBeforeTrigger);
			Animator animator = TargetGameObject.Value.GetComponent<Animator>();
			if (animator != null)
			{
				animator.SetTrigger(AnimationTrigger.Value);
			}
			base.Fsm.Event(FinishedEvent);
		}
	}
}
