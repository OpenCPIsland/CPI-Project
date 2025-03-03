namespace SmoothieSmash
{
	public abstract class mg_ss_Score
	{
		public int Score;

		protected mg_ss_GameLogic m_logic;

		public abstract string CustomText
		{
			get;
		}

		public abstract int CustomValue
		{
			get;
		}

		public int Coins
		{
			get
			{
				int num = 0;
				mg_ss_CoinTierData mg_ss_CoinTierData = m_logic.Minigame.Resources.CoinTierData;
				int num2 = Score;
				int num3 = 0;
				while (num2 > 0)
				{
					num2 -= mg_ss_CoinTierData.Points;
					num3 += mg_ss_CoinTierData.Points;
					if (num2 >= 0)
					{
						num++;
					}
					if (mg_ss_CoinTierData.NextTier != null && num3 >= mg_ss_CoinTierData.NextTier.MinScore)
					{
						mg_ss_CoinTierData = mg_ss_CoinTierData.NextTier;
					}
				}
				return num;
			}
		}

		public mg_ss_Score(mg_ss_GameLogic p_logic)
		{
			m_logic = p_logic;
		}
	}
}
