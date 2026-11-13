namespace BeanCounter
{
	public class mg_bc_JellyScoreController : mg_bc_ScoreController
	{
		private readonly int[] CATCH_SCORE_NORMAL = new int[5]
		{
			4,
			10,
			15,
			20,
			25
		};

		private readonly int[] CATCH_SCORE_HARD = new int[5]
		{
			6,
			12,
			18,
			24,
			30
		};

		private readonly int[] CATCH_SCORE_EXTREME = new int[5]
		{
			10,
			20,
			30,
			40,
			50
		};

		private readonly int[] UNLOAD_SCORE_NORMAL = new int[5]
		{
			11,
			15,
			23,
			30,
			38
		};

		private readonly int[] UNLOAD_SCORE_HARD = new int[5]
		{
			9,
			18,
			27,
			36,
			45
		};

		private readonly int[] UNLOAD_SCORE_EXTREME = new int[5]
		{
			15,
			30,
			45,
			60,
			75
		};

		private readonly int[] WRONG_BAG_SCORE_NORMAL = new int[5]
		{
			-3,
			-5,
			-8,
			-10,
			-13
		};

		private readonly int[] WRONG_BAG_SCORE_HARD = new int[5]
		{
			-3,
			-6,
			-9,
			-12,
			-15
		};

		private readonly int[] WRONG_BAG_SCORE_EXTREME = new int[5]
		{
			-5,
			-10,
			-15,
			-20,
			-25
		};

		private readonly int[] THROW_BACK_SCORE_NORMAL = new int[5]
		{
			3,
			5,
			8,
			10,
			13
		};

		private readonly int[] THROW_BACK_SCORE_HARD = new int[5]
		{
			3,
			6,
			9,
			12,
			15
		};

		private readonly int[] THROW_BACK_SCORE_EXTREME = new int[5]
		{
			5,
			10,
			15,
			20,
			25
		};

		private mg_bc_EGameMode m_gameMode;

		private int m_wrongBagTally;

		public mg_bc_JellyScoreController(mg_bc_EGameMode _gameMode)
		{
			m_gameMode = _gameMode;
			m_wrongBagTally = 0;
		}

		public override void OnBagCaught()
		{
			int value = m_truck.TruckNumber.Value;
			int value2 = 0;
			switch (m_gameMode)
			{
			case mg_bc_EGameMode.JELLY_NORMAL:
				value2 = CATCH_SCORE_NORMAL[value - 1];
				break;
			case mg_bc_EGameMode.JELLY_HARD:
				value2 = CATCH_SCORE_HARD[value - 1];
				break;
			case mg_bc_EGameMode.JELLY_EXTREME:
				value2 = CATCH_SCORE_EXTREME[value - 1];
				break;
			}
			AddScore(value2);
		}

		public override void OnBagUnloaded(bool _isCorrect = true)
		{
			if (_isCorrect)
			{
				OnCorrectBagUnloaded();
			}
			else
			{
				OnIncorrectBagUnloaded();
			}
		}

		private void OnCorrectBagUnloaded()
		{
			int value = m_truck.TruckNumber.Value;
			int value2 = 0;
			switch (m_gameMode)
			{
			case mg_bc_EGameMode.JELLY_NORMAL:
				value2 = UNLOAD_SCORE_NORMAL[value - 1];
				break;
			case mg_bc_EGameMode.JELLY_HARD:
				value2 = UNLOAD_SCORE_HARD[value - 1];
				break;
			case mg_bc_EGameMode.JELLY_EXTREME:
				value2 = UNLOAD_SCORE_EXTREME[value - 1];
				break;
			}
			AddScore(value2);
		}

		private void OnIncorrectBagUnloaded()
		{
			m_wrongBagTally++;
			int value = m_truck.TruckNumber.Value;
			int num = 0;
			switch (m_gameMode)
			{
			case mg_bc_EGameMode.JELLY_NORMAL:
				num = WRONG_BAG_SCORE_NORMAL[value - 1];
				break;
			case mg_bc_EGameMode.JELLY_HARD:
				num = WRONG_BAG_SCORE_HARD[value - 1];
				break;
			case mg_bc_EGameMode.JELLY_EXTREME:
				num = WRONG_BAG_SCORE_EXTREME[value - 1];
				break;
			}
			num *= -1;
			num += m_wrongBagTally / 3 * m_wrongBagTally;
			SubtractScore(num);
		}

		public override void OnBagThrownBack(bool _isTargetBag)
		{
			if (!_isTargetBag)
			{
				int value = m_truck.TruckNumber.Value;
				int value2 = 0;
				switch (m_gameMode)
				{
				case mg_bc_EGameMode.JELLY_NORMAL:
					value2 = THROW_BACK_SCORE_NORMAL[value - 1];
					break;
				case mg_bc_EGameMode.JELLY_HARD:
					value2 = THROW_BACK_SCORE_HARD[value - 1];
					break;
				case mg_bc_EGameMode.JELLY_EXTREME:
					value2 = THROW_BACK_SCORE_EXTREME[value - 1];
					break;
				}
				AddScore(value2);
			}
		}
	}
}
