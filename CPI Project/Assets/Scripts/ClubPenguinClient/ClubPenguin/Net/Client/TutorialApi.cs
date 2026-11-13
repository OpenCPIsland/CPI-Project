using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Client
{
	public class TutorialApi
	{
		private ClubPenguinClient clubPenguinClient;

		public TutorialApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<ClearTutorialOperation> ClearTutorial()
		{
			ClearTutorialOperation operation = new ClearTutorialOperation();
			return new APICall<ClearTutorialOperation>(clubPenguinClient, operation);
		}

		public APICall<SetTutorialOperation> SetTutorial(Tutorial tutorial)
		{
			SetTutorialOperation operation = new SetTutorialOperation(tutorial);
			return new APICall<SetTutorialOperation>(clubPenguinClient, operation);
		}
	}
}
