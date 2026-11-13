using MinigameFramework;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ItemGenerator_Survival : mg_ss_ItemGenerator
	{
		private float m_goldenAppleTimer;

		public override void Initialize(mg_ss_GameLogic p_gameLogic, mg_ss_GameScreen p_screen)
		{
			base.Initialize(p_gameLogic, p_screen);
			m_goldenAppleTimer = 10f;
		}

		public override void MinigameUpdate(float p_deltaTime)
		{
			base.MinigameUpdate(p_deltaTime);
			if (m_gameLogic.GameState == mg_ss_EGameState.ACTIVE)
			{
				m_goldenAppleTimer -= p_deltaTime;
				if (m_goldenAppleTimer <= 0f)
				{
					QueueGoldenApple();
					m_goldenAppleTimer = 10f;
				}
			}
		}

		public override void OnFruitCollision(mg_ss_Item_FruitObject p_fruit)
		{
			if (!p_fruit.ChaosItem)
			{
				m_gameLogic.ComboIncrease(p_fruit);
			}
			MinigameManager.GetActive().PlaySFX("mg_ss_sfx_fruit_splat_" + Random.Range(1, 3).ToString("00"));
			m_gameLogic.OnFruitSquashed(p_fruit, false);
		}

		protected override void QueueNextItem(float p_delay)
		{
			if (m_randomTierItemsLeft > 0f)
			{
				base.QueueNextItem(p_delay);
				return;
			}
			m_randomTierItemsLeft = Random.Range(m_generationData.MinRandom, m_generationData.MaxRandom + 1);
			QueueRandomFruit(p_delay);
		}
	}
}
