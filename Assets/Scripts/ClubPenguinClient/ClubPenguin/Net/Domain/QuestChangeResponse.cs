namespace ClubPenguin.Net.Domain
{
	public class QuestChangeResponse : CPResponse
	{
		public string questId;

		public RewardJsonReader reward;

		public SignedResponse<QuestStateCollection> questStateCollection;
	}
}
