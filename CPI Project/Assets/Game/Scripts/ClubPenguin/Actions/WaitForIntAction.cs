namespace ClubPenguin.Actions
{
	public class WaitForIntAction : Action
	{
		public int Value;

		private SharedActionGraphState sharedData;

		protected override void CopyTo(Action _destWarper)
		{
			WaitForIntAction waitForIntAction = _destWarper as WaitForIntAction;
			waitForIntAction.Value = Value;
			base.CopyTo(_destWarper);
		}

		protected override void OnEnable()
		{
			sharedData = SceneRefs.ActionSequencer.GetSharedActionGraphState(SceneRefs.ActionSequencer.GetTrigger(Owner));
			base.OnEnable();
		}

		protected override void Update()
		{
			if (sharedData != null)
			{
				if (Value == sharedData.IntData)
				{
					Completed();
				}
			}
			else
			{
				Completed();
			}
		}
	}
}
