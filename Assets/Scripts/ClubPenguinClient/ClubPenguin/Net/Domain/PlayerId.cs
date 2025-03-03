namespace ClubPenguin.Net.Domain
{
	public class PlayerId
	{
		public enum PlayerIdType
		{
			SWID = 1,
			SESSION_ID,
			NAME
		}

		public string id;

		public PlayerIdType type;
	}
}
