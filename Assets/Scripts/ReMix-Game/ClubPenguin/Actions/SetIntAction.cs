namespace ClubPenguin.Actions
{
	public class SetIntAction : Action
	{
		public int Value;

		protected override void CopyTo(Action _destWarper)
		{
			SetIntAction setIntAction = _destWarper as SetIntAction;
			setIntAction.Value = Value;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			SharedActionGraphState sharedActionGraphState = SceneRefs.ActionSequencer.GetSharedActionGraphState(SceneRefs.ActionSequencer.GetTrigger(Owner));
			if (sharedActionGraphState != null)
			{
				sharedActionGraphState.IntData = Value;
			}
			Completed();
		}
	}
}
