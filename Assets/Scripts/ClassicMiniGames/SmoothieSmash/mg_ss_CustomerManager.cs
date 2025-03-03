using MinigameFramework;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_CustomerManager : MonoBehaviour
	{
		private mg_ss_Order m_orderLogic;

		private mg_ss_OrderSystemObject m_orderSystemObject;

		private mg_ss_CustomerObject m_currentCustomer;

		private mg_ss_CustomerObject m_customerGeneric;

		private bool m_firstOrder;

		private bool m_customerTransitioning;

		protected void Awake()
		{
			m_customerGeneric = GetComponentInChildren<mg_ss_CustomerObject>();
			m_firstOrder = true;
		}

		public void Initialize(mg_ss_Order p_orderLogic, mg_ss_OrderSystemObject p_orderSystemObject)
		{
			m_orderLogic = p_orderLogic;
			m_orderSystemObject = p_orderSystemObject;
			m_currentCustomer = m_customerGeneric;
			m_customerGeneric.Initialize(this, false);
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			m_currentCustomer.MinigameUpdate(p_deltaTime);
		}

		public void NewOrderReceived()
		{
			m_customerTransitioning = true;
			if (m_firstOrder)
			{
				OnCustomerLeft();
				m_firstOrder = false;
			}
			else
			{
				MinigameManager.GetActive().PlaySFX("mg_ss_sfx_order_complete");
				m_currentCustomer.OrderCompleted();
			}
		}

		public void StepCompleted()
		{
			if (!m_customerTransitioning)
			{
				m_currentCustomer.StepCompleted();
			}
		}

		public void OnCustomerLeft()
		{
			if (m_currentCustomer.IsSpecial)
			{
				Object.Destroy(m_currentCustomer.gameObject);
				m_currentCustomer = m_customerGeneric;
			}
			if (m_orderLogic.IsSpecial)
			{
				GameObject customer = MinigameManager.GetActive<mg_SmoothieSmash>().Resources.GetCustomer(m_orderLogic.SpecialCustomer);
				MinigameSpriteHelper.AssignParentTransform(customer, base.transform);
				customer.transform.localPosition = new Vector2(0f, 0f);
				m_currentCustomer = customer.GetComponent<mg_ss_CustomerObject>();
				m_currentCustomer.Initialize(this, true);
			}
			m_currentCustomer.ZipIn();
			m_orderSystemObject.OnCustomerLeft();
			m_customerTransitioning = false;
		}
	}
}
