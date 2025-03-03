using UnityEngine;

namespace ClubPenguin
{
	public class ActionRequest
	{
		private readonly Animator animator;

		private readonly int animTriggerHash;

		private readonly float duration;

		private float elapsedTime;

		public bool Active
		{
			get;
			private set;
		}

		public ActionRequest(float _duration, Animator _animator = null, int _animTriggerHash = 0)
		{
			duration = _duration;
			animator = _animator;
			animTriggerHash = _animTriggerHash;
		}

		public void Set()
		{
			if (animator != null && animTriggerHash != 0)
			{
				animator.SetTrigger(animTriggerHash);
			}
			elapsedTime = 0f;
			Active = true;
		}

		public void Update()
		{
			if (Active)
			{
				if (elapsedTime > duration)
				{
					Reset();
				}
				else
				{
					elapsedTime += Time.deltaTime;
				}
			}
		}

		public void Reset()
		{
			if (Active)
			{
				if (animator != null && animTriggerHash != 0)
				{
					animator.ResetTrigger(animTriggerHash);
				}
				Active = false;
			}
		}
	}
}
