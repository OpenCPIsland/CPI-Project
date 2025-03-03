using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ExitTransitionStateHandler : MonoBehaviour
	{
		private const string EXIT_STATE = "Exit";

		private const string FSM_EVENT_TRANSITION_COMPLETE = "exit_complete";

		public string ExitAnimatorTrigger = "exit";

		private Animator animator;

		private Animator Animator
		{
			get
			{
				if (animator == null)
				{
					animator = GetComponentInChildren<Animator>();
				}
				return animator;
			}
		}

		public event Action OnExitTriggered;

		private void OnDestroy()
		{
			this.OnExitTriggered = null;
		}

		public void OnStateChanged(string state)
		{
			if (state.Equals("Exit"))
			{
				disableInteractableButtons();
				playExitAnimation();
			}
		}

		private void disableInteractableButtons()
		{
			Button[] componentsInChildren = GetComponentsInChildren<Button>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].interactable = false;
			}
		}

		private void playExitAnimation()
		{
			if (Animator != null)
			{
				CoroutineRunner.Start(waitForAnimationComplete(), this, "");
				Animator.SetTrigger(ExitAnimatorTrigger);
				if (this.OnExitTriggered != null)
				{
					this.OnExitTriggered();
				}
			}
			else
			{
				onExitAnimationComplete();
			}
		}

		private IEnumerator waitForAnimationComplete()
		{
			int animatorStateHash = Animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
			while (Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == animatorStateHash)
			{
				yield return null;
			}
			int shortNameHash = Animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
			while (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
			{
				yield return null;
			}
			onExitAnimationComplete();
		}

		private void onExitAnimationComplete()
		{
			GetComponent<StateMachine>().SendEvent("exit_complete");
			GetComponent<StateMachine>().SendEvent("disable");
		}
	}
}
