namespace JetpackReboot
{
	public class mg_jr_PlayerStats
	{
		private mg_jr_IStatPersistenceStrategy m_persistenceStrat;

		public int BestDistance
		{
			get;
			private set;
		}

		public int MostCoins
		{
			get;
			private set;
		}

		public mg_jr_PlayerStats(int _bestDistance, int _mostCoins, mg_jr_IStatPersistenceStrategy _persistenceStrat)
		{
			BestDistance = _bestDistance;
			MostCoins = _mostCoins;
			m_persistenceStrat = _persistenceStrat;
		}

		public mg_jr_PlayerStats(mg_jr_IStatPersistenceStrategy _persistenceStrat)
			: this(0, 0, _persistenceStrat)
		{
		}

		public void SaveStats()
		{
			m_persistenceStrat.Save(this);
		}

		public bool SubmitNewDistance(int _recordAttempt)
		{
			bool flag = _recordAttempt > BestDistance;
			if (flag)
			{
				BestDistance = _recordAttempt;
			}
			return flag;
		}

		public bool SubmitNewCoinAmount(int _recordAttempt)
		{
			bool flag = _recordAttempt > MostCoins;
			if (flag)
			{
				MostCoins = _recordAttempt;
			}
			return flag;
		}
	}
}
