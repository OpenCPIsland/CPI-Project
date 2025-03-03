using UnityEngine;

namespace ClubPenguin.Game.Animation
{
	public class PenguinSnowballIdleAnimationStateHandler : StateMachineBehaviour
	{
		private AbstractPenguinSnowballThrower penguinSnowballThrow;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (getPenguinSnowballThrow(animator) != null)
			{
				getPenguinSnowballThrow(animator).OnEnterIdle();
			}
		}

		private AbstractPenguinSnowballThrower getPenguinSnowballThrow(Animator animator)
		{
			if (penguinSnowballThrow == null)
			{
				penguinSnowballThrow = animator.GetComponent<AbstractPenguinSnowballThrower>();
			}
			return penguinSnowballThrow;
		}
	}
}
