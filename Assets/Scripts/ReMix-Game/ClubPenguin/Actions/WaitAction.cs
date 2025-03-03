using UnityEngine;

namespace ClubPenguin.Actions
{
	public class WaitAction : Action
	{
		public float Duration;

		private float elapsedTime;

		protected override void CopyTo(Action _destWarper)
		{
			WaitAction waitAction = _destWarper as WaitAction;
			waitAction.Duration = Duration;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= Duration)
			{
				Completed();
			}
		}
	}
}
