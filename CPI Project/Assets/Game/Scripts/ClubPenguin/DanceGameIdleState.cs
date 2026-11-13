using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class DanceGameIdleState : StateMachineBehaviour
	{
		private Queue<DanceMove> queue = new Queue<DanceMove>();

		public void NextMove(Animator animator)
		{
			if (animator.GetInteger("DanceMove") == 0 && queue.Count > 0)
			{
				setMove(animator, queue.Dequeue());
			}
		}

		public void QueueMove(DanceMove move)
		{
			queue.Enqueue(move);
		}

		private void setMove(Animator animator, DanceMove move)
		{
			animator.gameObject.GetComponent<DanceGame>().SetCurrentMove(move);
			animator.SetInteger("DanceMove", (int)move);
		}

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			queue.Clear();
			setMove(animator, DanceMove.None);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			NextMove(animator);
		}
	}
}
