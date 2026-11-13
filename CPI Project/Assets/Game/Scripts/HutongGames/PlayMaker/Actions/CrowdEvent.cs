namespace HutongGames.PlayMaker.Actions
{
	public class CrowdEvent
	{
		public FsmEvent startAnimationEvent;

		public FsmEvent stopAnimationEvent;

		public CrowdEvent(FsmEvent start, FsmEvent stop)
		{
			startAnimationEvent = start;
			stopAnimationEvent = stop;
		}
	}
}
