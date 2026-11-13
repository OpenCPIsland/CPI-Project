using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ItemGenerator_Normal : mg_ss_ItemGenerator
	{
		private mg_ss_OrderSystem m_orderSystem;

		public override void Initialize(mg_ss_GameLogic p_gameLogic, mg_ss_GameScreen p_screen)
		{
			base.Initialize(p_gameLogic, p_screen);
			m_orderSystem = new mg_ss_OrderSystem();
			m_orderSystem.Initialize(this, (p_screen as mg_ss_GameNormalScreen).OrderSystem);
		}

		public override void MinigameUpdate(float p_deltaTime)
		{
			base.MinigameUpdate(p_deltaTime);
			m_orderSystem.MinigameUpdate(p_deltaTime);
		}

		protected override void QueueNextItem(float p_delay)
		{
			if (m_randomTierItemsLeft > 0f)
			{
				base.QueueNextItem(p_delay);
			}
			else
			{
				QueueNextItemInOrder(p_delay);
			}
		}

		private void QueueNextItemInOrder(float p_delay)
		{
			m_randomTierItemsLeft = Random.Range(m_generationData.MinRandom, m_generationData.MaxRandom + 1);
			mg_ss_EItemTypes nextFruitToQueue = m_orderSystem.GetNextFruitToQueue();
			if (nextFruitToQueue == mg_ss_EItemTypes.NULL)
			{
				base.QueueRandomFruit(p_delay);
			}
			else
			{
				SpawnItem_Linear(nextFruitToQueue, p_delay);
			}
		}

		public override void OnFruitCollision(mg_ss_Item_FruitObject p_fruit)
		{
			bool p_correctFruit = false;
			if (m_orderSystem.OnFruitCollision(p_fruit))
			{
				p_correctFruit = true;
				m_gameLogic.ComboIncrease(p_fruit);
				MinigameManager.GetActive().PlaySFX("mg_ss_sfx_fruit_splat_" + Random.Range(1, 3).ToString("00"));
			}
			else if (!p_fruit.ChaosItem)
			{
				m_gameLogic.ComboReset();
				PlayErrorSFX();
			}
			else
			{
				MinigameManager.GetActive().PlaySFX("mg_ss_sfx_fruit_splat_" + Random.Range(1, 3).ToString("00"));
			}
			m_gameLogic.OnFruitSquashed(p_fruit, p_correctFruit);
		}

		private void PlayErrorSFX()
		{
			int num = Random.Range(1, 3);
			if (MinigameManager.GetActive().PlaySFX("mg_ss_sfx_fruit_error_" + num.ToString("00")) == null)
			{
				num = ((num != 1) ? 1 : 2);
				MinigameManager.GetActive().PlaySFX("mg_ss_sfx_fruit_error_" + num.ToString("00"));
			}
		}

		public void OrderCompleted(bool p_specialOrder)
		{
			QueueGoldenApple();
			(m_gameLogic.Scoring as mg_ss_ScoreNormal).OrdersCompleted++;
			int num = 15;
			if (p_specialOrder)
			{
				num *= 2;
			}
			m_gameLogic.Scoring.Score += num;
		}

		public override bool ItemPartOfOrder(mg_ss_EItemTypes p_itemType)
		{
			return m_orderSystem.IsFruitPartOfOrder(p_itemType);
		}

		public override void OnChaosModeEnded()
		{
			m_randomTierItemsLeft = 0f;
		}

		public void HighlightItemTypes(List<mg_ss_EItemTypes> p_itemTypes)
		{
			m_gameLogic.ItemManager.HighlightItemTypes(p_itemTypes);
		}
	}
}
