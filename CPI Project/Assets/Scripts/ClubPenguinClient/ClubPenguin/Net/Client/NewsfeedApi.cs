namespace ClubPenguin.Net.Client
{
	public class NewsfeedApi
	{
		private ClubPenguinClient clubPenguinClient;

		public NewsfeedApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<GetLatestPostTimeOperation> GetLatestPostTime(string language)
		{
			GetLatestPostTimeOperation operation = new GetLatestPostTimeOperation(language);
			return new APICall<GetLatestPostTimeOperation>(clubPenguinClient, operation);
		}
	}
}
