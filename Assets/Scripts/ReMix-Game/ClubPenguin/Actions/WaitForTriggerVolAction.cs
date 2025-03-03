using UnityEngine;

namespace ClubPenguin.Actions
{
	public class WaitForTriggerVolAction : Action
	{
		public Collider TriggerVol;

		private bool done;

		protected override void CopyTo(Action _destWarper)
		{
			WaitForTriggerVolAction waitForTriggerVolAction = _destWarper as WaitForTriggerVolAction;
			waitForTriggerVolAction.TriggerVol = TriggerVol;
			base.CopyTo(_destWarper);
		}

		private void OnTriggerEnter(Collider trigger)
		{
			if (base.enabled && trigger == TriggerVol)
			{
				done = true;
			}
		}

		protected override void Update()
		{
			if (done)
			{
				Completed();
			}
		}
	}
}
