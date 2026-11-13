using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_GameLogic_Survival : mg_ss_GameLogic
	{
		private mg_ss_HealthBonusManager m_healthBonusManager;

		public int Health
		{
			get;
			private set;
		}

		public override void Initialize(mg_ss_GameScreen p_screen)
		{
			base.Scoring = new mg_ss_ScoreSurvival(this);
			Health = 100;
			base.Initialize(p_screen);
			m_healthBonusManager = (p_screen as mg_ss_GameSurvivalScreen).HealthBonusManager;
			m_healthBonusManager.Initialize(this);
		}

		protected override bool CheckForGameOver()
		{
			return Health <= 0;
		}

		protected override void OnGameOver()
		{
			base.Scoring.Score += (int)base.GameTime * 5;
			base.OnGameOver();
		}

		public override void OnFruitCollision(mg_ss_Item_FruitObject p_fruit)
		{
			m_healthBonusManager.OnFruitCollision(p_fruit);
			base.OnFruitCollision(p_fruit);
		}

		public override void OnConveyorCollision()
		{
			base.OnConveyorCollision();
			Health -= 25;
		}

		public void AddHealth(int p_health)
		{
			Health = Mathf.Min(100, Health + p_health);
		}

		public override void OnBombCollision(mg_ss_Item_BombObject p_bomb)
		{
			base.OnBombCollision(p_bomb);
			if (!base.ChaosModeActivated)
			{
				Health -= 30;
			}
		}

		public override void OnAnvilCollision(mg_ss_Item_AnvilObject p_anvil)
		{
			base.OnAnvilCollision(p_anvil);
			if (!base.ChaosModeActivated)
			{
				Health -= 25;
			}
		}
	}
}
