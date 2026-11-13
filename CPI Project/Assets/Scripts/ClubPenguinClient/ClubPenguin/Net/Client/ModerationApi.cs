namespace ClubPenguin.Net.Client
{
	public class ModerationApi
	{
		private ClubPenguinClient clubPenguinClient;

		public ModerationApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<ReportPlayerOperation> ReportPlayer(string displayName, string reason)
		{
			ReportPlayerOperation operation = new ReportPlayerOperation(displayName, reason);
			return new APICall<ReportPlayerOperation>(clubPenguinClient, operation);
		}
	}
}
