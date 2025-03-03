using System.Collections.Generic;

namespace SmoothieSmash
{
	public class mg_ss_ItemGeneratorPowerUpData
	{
		public int MinCombo
		{
			get;
			private set;
		}

		public int SpawnCooldown
		{
			get;
			private set;
		}

		public float SpawnPercentage
		{
			get;
			private set;
		}

		public List<mg_ss_ItemGeneratorWeightingData> PowerUps
		{
			get;
			private set;
		}

		public mg_ss_ItemGeneratorPowerUpData()
		{
			PowerUps = new List<mg_ss_ItemGeneratorWeightingData>();
		}
	}
}
