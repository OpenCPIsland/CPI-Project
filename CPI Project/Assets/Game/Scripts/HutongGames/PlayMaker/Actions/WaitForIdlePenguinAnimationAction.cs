using ClubPenguin;
using ClubPenguin.Locomotion;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animation")]
	[Tooltip("Waits until the Penguin Animator Returns To Idle. You might need a wait prior to this in case the animator's state is in idle when this action executes.")]
	public class WaitForIdlePenguinAnimationAction : FsmStateAction
	{
		public FsmEvent FinishEvent;

		private Animator animator;

		public override void OnEnter()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			animator = localPlayerGameObject.GetComponent<Animator>();
		}

		private bool checkAnimatorState()
		{
			bool result = false;
			if (animator != null)
			{
				AnimatorStateInfo animatorStateInfo = LocomotionUtils.GetAnimatorStateInfo(animator);
				result = LocomotionUtils.IsIdling(animatorStateInfo);
			}
			return result;
		}

		public override void OnUpdate()
		{
			if (checkAnimatorState())
			{
				base.Fsm.Event(FinishEvent);
				Finish();
			}
		}
	}
}
