using System;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(Animator))]
	public class CreateWizardStepAnimator : MonoBehaviour
	{
		private Animator animator;

		private Action onAnimCompleteCallback;

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		public void PlayAnimation(string triggerName, Action onAnimCompleteCallback)
		{
			this.onAnimCompleteCallback = onAnimCompleteCallback;
			animator.SetTrigger(triggerName);
		}

		public void OnAnimationComplete()
		{
			if (onAnimCompleteCallback != null)
			{
				onAnimCompleteCallback();
			}
		}
	}
}
