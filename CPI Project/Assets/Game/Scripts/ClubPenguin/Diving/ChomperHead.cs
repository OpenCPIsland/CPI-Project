namespace ClubPenguin.Diving
{
	public class ChomperHead : ProximityBroadcaster
	{
		private Chomper chomperController;

		public override void Awake()
		{
			base.Awake();
			chomperController = base.gameObject.GetComponentInParent<Chomper>();
		}

		public override void OnProximityEnter(ProximityListener other)
		{
			if (chomperController != null)
			{
				chomperController.IsPenguinGrabbed = true;
			}
		}
	}
}
