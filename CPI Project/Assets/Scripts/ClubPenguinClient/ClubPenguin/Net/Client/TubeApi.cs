namespace ClubPenguin.Net.Client
{
	public class TubeApi
	{
		private ClubPenguinClient clubPenguinClient;

		public TubeApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<EquipTubeOperation> EquipTube(int tubeId)
		{
			EquipTubeOperation operation = new EquipTubeOperation(tubeId);
			return new APICall<EquipTubeOperation>(clubPenguinClient, operation);
		}
	}
}
