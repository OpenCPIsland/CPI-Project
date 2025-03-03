using MinigameFramework;

namespace BeanCounter
{
	public class mg_bc_ScoreController
	{
		private const int SCORE_PER_COIN = 10;

		public static mg_bc_ScoreController Instance = null;

		private int m_unCoinedScore;

		protected mg_bc_Truck m_truck;

		public mg_bc_UIValue<int> CurrentScore
		{
			get;
			private set;
		}

		public mg_bc_ScoreController()
		{
			Instance = this;
		}

		public void Initialize(mg_bc_Truck _truck)
		{
			m_unCoinedScore = 0;
			CurrentScore = new mg_bc_UIValue<int>();
			CurrentScore.SetValue(0);
			m_truck = _truck;
		}

		public virtual void OnBagCaught()
		{
		}

		public virtual void OnBagUnloaded(bool _isCorrect = true)
		{
		}

		public virtual void OnBagThrownBack(bool _isTargetBag)
		{
		}

		public void OnGameOver(bool _didWin)
		{
			if (_didWin)
			{
				MinigameManager.GetActive<mg_BeanCounter>().CoinsEarned += GetVictoryBonus();
			}
		}

		public virtual int GetVictoryBonus()
		{
			return 60;
		}

		protected void AddScore(int _value)
		{
			if (_value > 0)
			{
				CurrentScore.SetValue(CurrentScore.Value + _value);
				AdjustUncoinedScore(_value);
			}
		}

		protected void SubtractScore(int _value)
		{
			if (_value > 0)
			{
				if (CurrentScore.Value <= _value)
				{
					_value = CurrentScore.Value;
				}
				CurrentScore.SetValue(CurrentScore.Value - _value);
				AdjustUncoinedScore(-_value);
			}
		}

		private void AdjustUncoinedScore(int _value)
		{
			m_unCoinedScore += _value;
			if (m_unCoinedScore <= -10 || m_unCoinedScore >= 10)
			{
				int num = m_unCoinedScore / 10;
				m_unCoinedScore %= 10;
				MinigameManager.GetActive<mg_BeanCounter>().CoinsEarned += num;
			}
		}
	}
}
