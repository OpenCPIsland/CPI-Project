using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ItemMovement_Bounce : mg_ss_ItemMovement_Linear
	{
		private float m_initialVelocity;

		private float m_currentVelocity;

		public mg_ss_ItemMovement_Bounce(float p_velocity)
		{
			m_initialVelocity = p_velocity;
			m_currentVelocity = m_initialVelocity;
		}

		public override void UpdatePosition(Transform p_transform, float p_deltaTime, float p_conveyorSpeed)
		{
			base.UpdatePosition(p_transform, p_deltaTime, p_conveyorSpeed);
			UpdatePosition_Bounce(p_transform, p_deltaTime);
		}

		private void UpdatePosition_Bounce(Transform p_transform, float p_deltaTime)
		{
			m_currentVelocity -= p_deltaTime * 12f;
			Vector2 v = p_transform.localPosition;
			v.y += m_currentVelocity * p_deltaTime;
			p_transform.localPosition = v;
			if (m_itemObject.IsItemOnConveyor())
			{
				m_itemObject.PlaceOnConveyor();
				if (m_itemObject.Collidable)
				{
					m_currentVelocity = m_initialVelocity;
					m_itemObject.PlayBounceSFX();
				}
			}
		}

		public override void OnCollided()
		{
			m_currentVelocity = 0f;
		}
	}
}
