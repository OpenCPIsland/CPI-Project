namespace ClubPenguin
{
	public static class ZoneTransitionEvents
	{
		public struct ZoneTransition
		{
			public enum States
			{
				Begin,
				Request,
				Done,
				Cancel
			}

			public string FromZone;

			public string ToZone;

			public States State;

			public ZoneTransition(string from, string to, States state)
			{
				FromZone = from;
				ToZone = to;
				State = state;
			}
		}
	}
}
