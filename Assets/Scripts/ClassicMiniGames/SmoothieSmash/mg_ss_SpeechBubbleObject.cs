using System;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_SpeechBubbleObject : MonoBehaviour
	{
		private mg_ss_Order m_orderLogic;

		private Transform m_speechEnd;

		private mg_ss_OrderStepObject[] m_orderSteps;

		private Animator m_animator;

		protected void Awake()
		{
			m_orderSteps = new mg_ss_OrderStepObject[8];
			mg_ss_OrderStepObject[] componentsInChildren = GetComponentsInChildren<mg_ss_OrderStepObject>();
			mg_ss_OrderStepObject[] array = componentsInChildren;
			foreach (mg_ss_OrderStepObject mg_ss_OrderStepObject in array)
			{
				string name = mg_ss_OrderStepObject.gameObject.name;
				if (name.StartsWith("mg_ss_step_"))
				{
					m_orderSteps[Convert.ToInt32(name.Substring(name.Length - 2)) - 1] = mg_ss_OrderStepObject;
				}
			}
			m_speechEnd = base.transform.Find("mg_ss_order_end");
			m_animator = GetComponent<Animator>();
		}

		public void Initialize(mg_ss_Order p_orderLogic)
		{
			m_orderLogic = p_orderLogic;
			for (int i = 0; i < m_orderLogic.Steps.Count; i++)
			{
				m_orderSteps[i].Initialize(m_orderLogic.Steps[i]);
			}
		}

		public void StepCompleted()
		{
			mg_ss_OrderStepObject[] orderSteps = m_orderSteps;
			foreach (mg_ss_OrderStepObject mg_ss_OrderStepObject in orderSteps)
			{
				mg_ss_OrderStepObject.UpdateObject();
			}
		}

		public void NewOrderReceived()
		{
			m_animator.Play(Animator.StringToHash("Base Layer.mg_ss_SpeechBubble_Exit"), 0, 0f);
		}

		private float CalculateEndPosX()
		{
			float result = 0f;
			for (int num = m_orderSteps.Length - 1; num >= 0; num--)
			{
				if (m_orderSteps[num].State != mg_ss_EOrderStepState.INVALID)
				{
					result = m_orderSteps[num].transform.localPosition.x;
					break;
				}
			}
			return result;
		}

		public void OnCustomerLeft()
		{
			mg_ss_OrderStepObject[] orderSteps = m_orderSteps;
			foreach (mg_ss_OrderStepObject mg_ss_OrderStepObject in orderSteps)
			{
				mg_ss_OrderStepObject.Reset();
			}
			Vector2 v = m_speechEnd.localPosition;
			v.x = CalculateEndPosX();
			m_speechEnd.localPosition = v;
			m_animator.Play(Animator.StringToHash("Base Layer.mg_ss_SpeechBubble_Enter"), 0, 0f);
		}
	}
}
