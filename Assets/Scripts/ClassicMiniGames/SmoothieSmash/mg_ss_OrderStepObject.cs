using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_OrderStepObject : MonoBehaviour
	{
		private static string ANIMATOR_PARAM_FRUIT = "fruit";

		private mg_ss_OrderStep m_orderStepLogic;

		private Animator m_animator;

		private Transform m_fruit;

		public mg_ss_EOrderStepState State;

		protected void Awake()
		{
			m_animator = GetComponentInChildren<Animator>();
			m_fruit = base.transform.Find("mg_ss_order_fruit");
		}

		public void Initialize(mg_ss_OrderStep p_orderStepLogic)
		{
			m_orderStepLogic = p_orderStepLogic;
		}

		public void Reset()
		{
			UpdateObject();
			m_animator.SetInteger(ANIMATOR_PARAM_FRUIT, (int)m_orderStepLogic.FruitType);
		}

		public void UpdateObject()
		{
			if (m_orderStepLogic.State != State)
			{
				switch (m_orderStepLogic.State)
				{
				case mg_ss_EOrderStepState.COMPLETE:
					OnStepCompleted();
					break;
				case mg_ss_EOrderStepState.INCOMPLETE:
					OnStepIncomplete();
					break;
				default:
					OnStepInvalid();
					break;
				}
			}
		}

		private void OnStepCompleted()
		{
			base.gameObject.SetActive(true);
			State = mg_ss_EOrderStepState.COMPLETE;
			Scale(0.5f);
			Alpha(0.4f);
		}

		private void OnStepIncomplete()
		{
			base.gameObject.SetActive(true);
			State = mg_ss_EOrderStepState.INCOMPLETE;
			Scale(1f);
			Alpha(1f);
		}

		private void OnStepInvalid()
		{
			base.gameObject.SetActive(false);
			State = mg_ss_EOrderStepState.INVALID;
		}

		private void Scale(float p_amount)
		{
			Vector2 v = m_fruit.localScale;
			v.x = p_amount;
			v.y = p_amount;
			m_fruit.localScale = v;
		}

		private void Alpha(float p_amount)
		{
			SpriteRenderer component = m_fruit.GetComponent<SpriteRenderer>();
			Color color = component.color;
			color.a = p_amount;
			component.color = color;
		}
	}
}
