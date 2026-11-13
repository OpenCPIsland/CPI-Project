using System;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ItemMovement_PowerupExit : mg_ss_IItemMovement
	{
		private mg_ss_ItemObject m_itemObject;

		private Vector2 m_initialPosition;

		private Vector2 m_targetPosition;

		private float m_timeActive;

		public mg_ss_ItemMovement_PowerupExit(Vector2 p_targetPosition)
		{
			m_targetPosition = p_targetPosition;
		}

		public void Initialize(mg_ss_ItemObject p_itemObject)
		{
			m_itemObject = p_itemObject;
			m_initialPosition = m_itemObject.transform.localPosition;
		}

		public void UpdatePosition(Transform p_transform, float p_deltaTime, float p_conveyorSpeed)
		{
			m_timeActive += p_deltaTime;
			float num = 1f;
			float num2 = 1f - (num - m_timeActive) / num;
			Vector2 v = m_initialPosition + (m_targetPosition - m_initialPosition) * num2;
			v.y -= Mathf.Sin(num2 * (float)Math.PI) * 1.2f;
			p_transform.localPosition = v;
			if (num2 >= 1f)
			{
				m_itemObject.RemoveItem();
			}
		}

		public void OnCollided()
		{
		}
	}
}
