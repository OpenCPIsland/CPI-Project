namespace ClubPenguin.Commerce
{
	public struct CSGInfo
	{
		public string PlanId;

		public string ExternalReference;

		public string SkuDuration;

		public string TrialDuration;

		public SkuInfo SkuData;

		public CSGInfo(string PlanId, string ExternalReference, string SkuDuration, string TrialDuration, SkuInfo SkuData)
		{
			this.PlanId = PlanId;
			this.ExternalReference = ExternalReference;
			this.SkuDuration = SkuDuration;
			this.TrialDuration = TrialDuration;
			this.SkuData = SkuData;
		}
	}
}
