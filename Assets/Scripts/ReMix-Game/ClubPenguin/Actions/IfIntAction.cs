namespace ClubPenguin.Actions
{
	public class IfIntAction : Action
	{
		public int Value;

		protected override void CopyTo(Action _destWarper)
		{
			IfIntAction ifIntAction = _destWarper as IfIntAction;
			ifIntAction.Value = Value;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			SharedActionGraphState sharedActionGraphState = SceneRefs.ActionSequencer.GetSharedActionGraphState(SceneRefs.ActionSequencer.GetTrigger(Owner));
			if (sharedActionGraphState != null && Value == sharedActionGraphState.IntData)
			{
				Completed();
			}
			else
			{
				Completed(null, false);
			}
		}
	}
}
