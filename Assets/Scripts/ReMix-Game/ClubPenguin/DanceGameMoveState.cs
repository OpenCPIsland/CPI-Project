using UnityEngine;

namespace ClubPenguin
{
	public class DanceGameMoveState : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.SetInteger("DanceMove", 0);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.GetBehaviour<DanceGameIdleState>().NextMove(animator);
		}
	}
}
