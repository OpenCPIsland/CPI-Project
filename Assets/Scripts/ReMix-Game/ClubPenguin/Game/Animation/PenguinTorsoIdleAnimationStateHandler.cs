using ClubPenguin.Props;
using UnityEngine;

namespace ClubPenguin.Game.Animation
{
	public class PenguinTorsoIdleAnimationStateHandler : StateMachineBehaviour
	{
		private PropUser propUser;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (getPropUser(animator) != null)
			{
				getPropUser(animator).OnPenguinAnimationTorsoIdleEnter();
			}
		}

		private PropUser getPropUser(Animator animator)
		{
			if (propUser == null)
			{
				propUser = animator.GetComponent<PropUser>();
			}
			return propUser;
		}
	}
}
