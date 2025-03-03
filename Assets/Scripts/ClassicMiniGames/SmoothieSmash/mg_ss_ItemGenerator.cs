using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ItemGenerator : MonoBehaviour
	{
		protected mg_ss_GameLogic m_gameLogic;

		protected mg_ss_ItemGeneratorData m_generationData;

		protected float m_spacingTimer;

		protected float m_randomTierItemsLeft;

		private int m_powerUpCooldown;

		private Queue<mg_ss_EItemTypes> m_queuedPowerups;

		public virtual void Initialize(mg_ss_GameLogic p_gameLogic, mg_ss_GameScreen p_screen)
		{
			m_queuedPowerups = new Queue<mg_ss_EItemTypes>();
			m_gameLogic = p_gameLogic;
			m_generationData = MinigameManager.GetActive<mg_SmoothieSmash>().Resources.ItemGeneratorData;
			m_spacingTimer = m_gameLogic.ConveyorItemSpacing;
		}

		public virtual void MinigameUpdate(float p_deltaTime)
		{
			m_spacingTimer -= p_deltaTime * m_gameLogic.ConveyorSpeed;
			if (m_spacingTimer <= 0.1f)
			{
				QueueNextItem(0f - m_spacingTimer);
				if (!m_gameLogic.ChaosModeActivated)
				{
					m_powerUpCooldown--;
					CheckToQueuePowerUp();
					SpawnNextPowerup(0f - (m_spacingTimer + m_gameLogic.ConveyorItemSpacing * 0.5f));
				}
				m_spacingTimer += m_gameLogic.ConveyorItemSpacing;
			}
		}

		private void CheckToQueuePowerUp()
		{
			if (m_generationData.PowerUpData != null && m_powerUpCooldown <= 0 && m_gameLogic.Combo >= m_generationData.PowerUpData.MinCombo)
			{
				float num = Random.Range(0, 100);
				if (num <= m_generationData.PowerUpData.SpawnPercentage)
				{
					CalculateNextPowerUp();
				}
			}
		}

		private void CalculateNextPowerUp()
		{
			mg_ss_ItemGeneratorWeightingData mg_ss_ItemGeneratorWeightingData = CalculateRandomWeighting(m_generationData.PowerUpData.PowerUps);
			if (mg_ss_ItemGeneratorWeightingData != null)
			{
				QueuePowerUp(mg_ss_ItemGeneratorWeightingData);
			}
		}

		private void QueuePowerUp(mg_ss_ItemGeneratorWeightingData p_weighting)
		{
			mg_ss_EItemTypes mg_ss_EItemTypes = mg_ss_ItemGeneratorData.CalculateItemType(p_weighting.Tag);
			if (mg_ss_EItemTypes != mg_ss_EItemTypes.NULL)
			{
				m_queuedPowerups.Enqueue(mg_ss_EItemTypes);
				m_powerUpCooldown = m_generationData.PowerUpData.SpawnCooldown;
			}
		}

		private void SpawnNextPowerup(float p_delay)
		{
			if (m_queuedPowerups.Count > 0)
			{
				m_gameLogic.ItemManager.SpawnItem(m_queuedPowerups.Dequeue(), new mg_ss_ItemMovement_Hover(), p_delay);
			}
		}

		public void CheckTierIncrease(float p_gameTime)
		{
			if (m_generationData.NextData != null && m_generationData.NextData.MinTime <= p_gameTime)
			{
				m_generationData = m_generationData.NextData;
				DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "Item Generation Tier Increase", DisneyMobile.CoreUnitySystems.Logger.TagFlags.GAME);
			}
		}

		protected virtual void QueueNextItem(float p_delay)
		{
			if (m_gameLogic.GameState == mg_ss_EGameState.ACTIVE && m_randomTierItemsLeft > 0f)
			{
				m_randomTierItemsLeft -= 1f;
				if (m_gameLogic.ChaosModeActivated)
				{
					QueueRandomFruit(p_delay);
				}
				else
				{
					QueueTierItem(p_delay);
				}
			}
			else
			{
				QueueRandomFruit(p_delay);
			}
		}

		private void QueueTierItem(float p_delay)
		{
			mg_ss_ItemGeneratorWeightingData mg_ss_ItemGeneratorWeightingData = CalculateRandomWeighting(m_generationData.Weightings);
			if (mg_ss_ItemGeneratorWeightingData != null)
			{
				QueueFromWeighting(mg_ss_ItemGeneratorWeightingData, p_delay);
			}
			else
			{
				QueueRandomFruit(p_delay);
			}
		}

		private mg_ss_ItemGeneratorWeightingData CalculateRandomWeighting(List<mg_ss_ItemGeneratorWeightingData> p_weightingList)
		{
			mg_ss_ItemGeneratorWeightingData result = null;
			int num = 0;
			foreach (mg_ss_ItemGeneratorWeightingData p_weighting in p_weightingList)
			{
				num += p_weighting.Weighting;
			}
			int num2 = Random.Range(0, num + 1);
			foreach (mg_ss_ItemGeneratorWeightingData p_weighting2 in p_weightingList)
			{
				if (num2 <= p_weighting2.Weighting)
				{
					result = p_weighting2;
					break;
				}
				num2 -= p_weighting2.Weighting;
			}
			return result;
		}

		private void QueueFromWeighting(mg_ss_ItemGeneratorWeightingData p_weightingData, float p_delay)
		{
			mg_ss_EItemTypes mg_ss_EItemTypes = mg_ss_ItemGeneratorData.CalculateItemType(p_weightingData.Tag);
			if (mg_ss_EItemTypes == mg_ss_EItemTypes.NULL)
			{
				QueueRandomFruit(p_delay);
			}
			else if (p_weightingData.BounceVelocity > 0f)
			{
				SpawnItem_Bounce(mg_ss_EItemTypes, p_delay, p_weightingData.BounceVelocity);
			}
			else
			{
				SpawnItem_Linear(mg_ss_EItemTypes, p_delay);
			}
		}

		protected virtual void QueueRandomFruit(float p_delay)
		{
			SpawnItem_Linear(CalculateRandomFruit(), p_delay);
		}

		protected void SpawnItem_Linear(mg_ss_EItemTypes p_itemType, float p_delay)
		{
			m_gameLogic.ItemManager.SpawnItem(p_itemType, new mg_ss_ItemMovement_Linear(), p_delay);
		}

		private void SpawnItem_Bounce(mg_ss_EItemTypes p_itemType, float p_delay, float p_velocity)
		{
			mg_ss_ItemMovement_Bounce p_movement = new mg_ss_ItemMovement_Bounce(p_velocity);
			m_gameLogic.ItemManager.SpawnItem(p_itemType, p_movement, p_delay);
		}

		private mg_ss_EItemTypes CalculateRandomFruit()
		{
			return (mg_ss_EItemTypes)Random.Range(0, 12);
		}

		public virtual void OnFruitCollision(mg_ss_Item_FruitObject p_fruit)
		{
		}

		public bool IsItemSpawnedOnConveyor(mg_ss_EItemTypes p_itemType)
		{
			return m_gameLogic.ItemManager.IsItemSpawnedOnConveyor(p_itemType);
		}

		public virtual bool ItemPartOfOrder(mg_ss_EItemTypes p_itemType)
		{
			return false;
		}

		protected void QueueGoldenApple()
		{
			m_queuedPowerups.Enqueue(mg_ss_EItemTypes.GOLDEN_APPLE);
		}

		public virtual void OnChaosModeEnded()
		{
		}

		public void SpawnChoasFruit(float p_delay)
		{
			mg_ss_EItemTypes mg_ss_EItemTypes = CalculateRandomFruit();
			if (mg_ss_EItemTypes != mg_ss_EItemTypes.NULL)
			{
				mg_ss_ItemMovement_Bounce p_movement = new mg_ss_ItemMovement_Bounce(mg_ss_ItemObject.CalculateRandomBounceVelocity());
				m_gameLogic.ItemManager.SpawnItem(mg_ss_EItemTypes, p_movement, p_delay, true);
			}
		}
	}
}
