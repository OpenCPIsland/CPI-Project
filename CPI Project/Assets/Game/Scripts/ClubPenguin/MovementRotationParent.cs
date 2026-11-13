namespace ClubPenguin
{
	public class MovementRotationParent : ProximityBroadcaster
	{
		private MovementRotation[] movementRotationChildren;

		public override void Awake()
		{
			base.Awake();
			movementRotationChildren = GetComponentsInChildren<MovementRotation>();

			foreach (var movementRotation in movementRotationChildren)
			{
				movementRotation.SetActive(true);
			}
		}

		public override void OnProximityEnter(ProximityListener other) { }

		public override void OnProximityExit(ProximityListener other) { }
	}
}