using MinigameFramework;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_PenguinManager : MonoBehaviour
	{
		private mg_pt_Resources m_resources;

		private bool m_penguinEntered;

		private int m_specialChance;

		private mg_pt_OrderBubbleObject m_orderBubble;

		private mg_pt_ChefObject m_chef;

		private mg_pt_CustomerObject m_customer;

		private mg_pt_CustomerObject m_defaultCustomer;

		private mg_pt_SpecialCustomerStruct[] m_specialStructs;

		private int m_totalSpecialChance;

		public void Initialize(mg_pt_Resources p_resources, mg_pt_Order p_order)
		{
			m_resources = p_resources;
			m_orderBubble = GetComponentInChildren<mg_pt_OrderBubbleObject>();
			m_orderBubble.Initialize(p_resources, p_order);
			SetupSpecialData();
			m_chef = m_resources.GetInstancedResource(mg_pt_EResourceList.GAME_SPECIFIC_MIN).GetComponent<mg_pt_ChefObject>();
			MinigameSpriteHelper.AssignParentPositionReset(m_chef.gameObject, base.transform.Find("Chef").gameObject);
			m_defaultCustomer = m_resources.GetInstancedResource(mg_pt_EResourceList.GAME_CUSTOMER_DEFAULT).GetComponentInChildren<mg_pt_CustomerObject>();
			MinigameSpriteHelper.AssignParentPositionReset(m_defaultCustomer.gameObject, base.transform.Find("Customer").gameObject);
			m_defaultCustomer.Initialize(this, false);
			m_customer = m_defaultCustomer;
		}

		private void SetupSpecialData()
		{
			m_specialStructs = new mg_pt_SpecialCustomerStruct[3];
			m_specialStructs[0] = new mg_pt_SpecialCustomerStruct
			{
				Chance = 10,
				ResourceTag = mg_pt_EResourceList.GAME_CUSTOMER_GARY
			};
			m_specialStructs[1] = new mg_pt_SpecialCustomerStruct
			{
				Chance = 5,
				ResourceTag = mg_pt_EResourceList.GAME_CUSTOMER_ROCKHOPPER
			};
			m_specialStructs[2] = new mg_pt_SpecialCustomerStruct
			{
				Chance = 5,
				ResourceTag = mg_pt_EResourceList.GAME_CUSTOMER_SENSEI
			};
			m_totalSpecialChance = 0;
			for (int i = 0; i < m_specialStructs.Length; i++)
			{
				m_totalSpecialChance += m_specialStructs[i].Chance;
			}
		}

		public void OnOrderGenerated(int p_specialChance)
		{
			m_orderBubble.NewOrder();
			m_specialChance = p_specialChance;
			if (m_penguinEntered)
			{
				m_customer.Exit();
			}
			else
			{
				OnCustomerExit();
			}
		}

		public void OnCustomerExit()
		{
			m_penguinEntered = true;
			MinigameManager.GetActive().PlaySFX("mg_pt_sfx_pizza_complete");
			if (m_customer.Special)
			{
				Object.Destroy(m_customer.gameObject);
				m_customer = m_defaultCustomer;
				m_customer.gameObject.SetActive(true);
			}
			if (m_specialChance > 0 && Random.Range(0, 100) < m_specialChance)
			{
				m_defaultCustomer.gameObject.SetActive(false);
				m_customer = RandomizeSpecialCustomer();
				m_customer.Initialize(this, true);
				MinigameSpriteHelper.AssignParentPositionReset(m_customer.gameObject, base.transform.Find("Customer").gameObject);
			}
			m_customer.Enter();
			m_chef.OnCustomerEnter();
		}

		private mg_pt_CustomerObject RandomizeSpecialCustomer()
		{
			int num = Random.Range(0, m_totalSpecialChance);
			mg_pt_SpecialCustomerStruct mg_pt_SpecialCustomerStruct = m_specialStructs[0];
			for (int i = 0; i < m_specialStructs.Length; i++)
			{
				if (num < m_specialStructs[i].Chance)
				{
					mg_pt_SpecialCustomerStruct = m_specialStructs[i];
					break;
				}
				num -= m_specialStructs[i].Chance;
			}
			return m_resources.GetInstancedResource(mg_pt_SpecialCustomerStruct.ResourceTag).GetComponentInChildren<mg_pt_CustomerObject>();
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			m_orderBubble.MinigameUpdate(p_deltaTime);
		}

		public void OnOrderChanged()
		{
			m_orderBubble.UpdateOrder();
		}

		public void OnOrderReset()
		{
			m_orderBubble.UpdateOrder();
		}
	}
}
