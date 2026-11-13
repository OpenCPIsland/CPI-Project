using ClubPenguin.Core;
using Disney.MobileNetwork;

namespace ClubPenguin.LOD
{
	public class LODWeightingAFK : LODWeightingRule
	{
		public LODWeightingAFKData Data;

		private PresenceData presenceData;

		public override void Setup()
		{
			base.Setup();
			presenceData = Service.Get<CPDataEntityCollection>().GetComponent<PresenceData>(request.Data.PenguinHandle);
		}

		public override void OnDisable()
		{
			base.OnDisable();
			presenceData = null;
		}

		protected override float UpdateWeighting()
		{
			float result = 0f;
			if (presenceData != null && presenceData.IsAway)
			{
				result = Data.Weighting;
			}
			return result;
		}
	}
}
