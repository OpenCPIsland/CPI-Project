namespace ClubPenguin.PartyGames
{
	public class PartyGamePlayer
	{
		public int TeamId;

		public long UserSessionId;

		public int RoleId;

		public PartyGamePlayer()
		{
		}

		public PartyGamePlayer(int teamId, long userSessionId, int roleId)
		{
			TeamId = teamId;
			UserSessionId = userSessionId;
			RoleId = roleId;
		}
	}
}
