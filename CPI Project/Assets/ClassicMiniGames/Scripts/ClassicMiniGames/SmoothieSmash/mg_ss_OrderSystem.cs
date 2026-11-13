using MinigameFramework;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_OrderSystem
	{
		private mg_ss_ItemGenerator_Normal m_itemGenerator;

		private mg_ss_OrderLengthData m_orderData;

		private mg_ss_SpecialOrderData m_specialOrderData;

		private int m_ordersCompleted;

		private mg_ss_Order m_currentOrder;

		private mg_ss_Order m_nextOrder;

		private mg_ss_OrderSystemObject m_orderSystemObject;

		public void Initialize(mg_ss_ItemGenerator_Normal p_itemGenerator, mg_ss_OrderSystemObject p_orderSystemObject)
		{
			m_itemGenerator = p_itemGenerator;
			m_orderSystemObject = p_orderSystemObject;
			mg_ss_Resources resources = MinigameManager.GetActive<mg_SmoothieSmash>().Resources;
			m_orderData = resources.OrderLengthData;
			m_specialOrderData = resources.SpecialOrderData;
			m_currentOrder = new mg_ss_Order();
			m_nextOrder = new mg_ss_Order();
			m_orderSystemObject.Initialize(m_currentOrder);
			GenerateNextGenericOrder();
			GenerateOrder();
			m_ordersCompleted = 0;
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			m_orderSystemObject.MinigameUpdate(p_deltaTime);
		}

		public bool IsFruitPartOfOrder(mg_ss_EItemTypes p_fruitType)
		{
			return m_currentOrder.ContainsFruit(p_fruitType);
		}

		public bool OnFruitCollision(mg_ss_Item_FruitObject p_fruit)
		{
			bool result = false;
			if (!p_fruit.ChaosItem && IsFruitPartOfOrder(p_fruit.ItemType))
			{
				result = true;
				m_currentOrder.CompleteStep(p_fruit.ItemType);
				m_orderSystemObject.StepCompleted();
				if (m_currentOrder.OrderCompleted)
				{
					OrderCompleted(m_currentOrder.IsSpecial);
				}
				m_itemGenerator.HighlightItemTypes(m_currentOrder.GetItemTypes());
			}
			return result;
		}

		public mg_ss_EItemTypes GetNextFruitToQueue()
		{
			mg_ss_EItemTypes nextFruitToQueue = m_currentOrder.GetNextFruitToQueue(this);
			if (nextFruitToQueue == mg_ss_EItemTypes.NULL)
			{
				nextFruitToQueue = m_nextOrder.GetNextFruitToQueue(this);
			}
			return nextFruitToQueue;
		}

		private void OrderCompleted(bool p_specialOrder)
		{
			m_itemGenerator.OrderCompleted(p_specialOrder);
			m_ordersCompleted++;
			if (m_orderData.NextData != null && m_orderData.NextData.MinRecipesCompleted <= m_ordersCompleted)
			{
				m_orderData = m_orderData.NextData;
			}
			GenerateOrder();
		}

		private void GenerateOrder()
		{
			m_currentOrder.CopyOrder(m_nextOrder);
			m_itemGenerator.HighlightItemTypes(m_currentOrder.GetItemTypes());
			m_orderSystemObject.NewOrderReceived();
			mg_ss_SpecialOrderData mg_ss_SpecialOrderData = CheckSpecialOrder();
			int num = 0;
			string specialCustomer = m_currentOrder.SpecialCustomer;
			while (specialCustomer != null && mg_ss_SpecialOrderData != null && mg_ss_SpecialOrderData.Tag == specialCustomer)
			{
				if (num == 10)
				{
					mg_ss_SpecialOrderData = null;
					break;
				}
				mg_ss_SpecialOrderData = CheckSpecialOrder();
				num++;
			}
			if (mg_ss_SpecialOrderData == null)
			{
				GenerateNextGenericOrder();
			}
			else
			{
				m_nextOrder.GenerateSpecialOrder(mg_ss_SpecialOrderData);
			}
		}

		private void GenerateNextGenericOrder()
		{
			m_nextOrder.GenerateGenericOrder(Random.Range(m_orderData.MinLength, m_orderData.MaxLength + 1));
		}

		private mg_ss_SpecialOrderData CheckSpecialOrder()
		{
			mg_ss_SpecialOrderData mg_ss_SpecialOrderData = null;
			if (m_orderData.SpecialAllowed)
			{
				int num = Random.Range(0, 101);
				mg_ss_SpecialOrderData = m_specialOrderData;
				while (mg_ss_SpecialOrderData != null && mg_ss_SpecialOrderData.Percentage <= num)
				{
					num -= mg_ss_SpecialOrderData.Percentage;
					mg_ss_SpecialOrderData = mg_ss_SpecialOrderData.NextOrder;
				}
			}
			return mg_ss_SpecialOrderData;
		}

		public bool IsItemSpawnedOnConveyor(mg_ss_EItemTypes p_itemType)
		{
			return m_itemGenerator.IsItemSpawnedOnConveyor(p_itemType);
		}
	}
}
