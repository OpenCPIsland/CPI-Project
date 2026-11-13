namespace ClubPenguin.Actions
{
	public class ForceInteractionAction : Action
	{
		public bool IsInteractionBroadcasted;

		protected override void CopyTo(Action _destWarper)
		{
			ForceInteractionAction forceInteractionAction = _destWarper as ForceInteractionAction;
			forceInteractionAction.IsInteractionBroadcasted = IsInteractionBroadcasted;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			Completed();
		}
	}
}
