using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[RequireComponent(typeof(Animator))]
	public class AnimationLogger : MonoBehaviour
	{
		private Animator anim;

		private bool prevIsInTransition;

		private int prevState;

		public void Awake()
		{
			anim = GetComponent<Animator>();
			prevState = anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
		}

		private string TransitionToString(AnimatorTransitionInfo info)
		{
			return string.Format("Transition: {0}\nAny State: {1}\nNormalized Time: {2}", AnimationHashes.ToString(info.fullPathHash), info.anyState, info.normalizedTime);
		}

		private string StateToString(AnimatorStateInfo info)
		{
			return string.Format("State: {0}\nLength: {1}\nLoop: {2}\nNormalized Time: {3}", AnimationHashes.ToString(info.fullPathHash), info.length, info.loop, info.normalizedTime);
		}

		private void LogMsg(string label, string str)
		{
		}

		private void LogMsg(string str)
		{
		}

		public void Update()
		{
			bool flag = anim.IsInTransition(0);
			if (prevIsInTransition != flag)
			{
				if (flag)
				{
					LogMsg("Start Transition", TransitionToString(anim.GetAnimatorTransitionInfo(0)));
				}
				else
				{
					LogMsg("End transition");
				}
				prevIsInTransition = flag;
			}
			AnimatorStateInfo currentAnimatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
			int fullPathHash = currentAnimatorStateInfo.fullPathHash;
			if (fullPathHash != prevState)
			{
				LogMsg("New state", StateToString(currentAnimatorStateInfo));
				prevState = fullPathHash;
			}
		}
	}
}
