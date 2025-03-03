namespace ClubPenguin.Net.Client
{
	public class DurableApi
	{
		private ClubPenguinClient clubPenguinClient;

		public DurableApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<EquipDurableOperation> EquipDurable(int propId)
		{
			EquipDurableOperation operation = new EquipDurableOperation(propId);
			return new APICall<EquipDurableOperation>(clubPenguinClient, operation);
		}
	}
}
