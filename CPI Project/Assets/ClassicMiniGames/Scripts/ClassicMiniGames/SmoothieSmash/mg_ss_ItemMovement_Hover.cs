using System;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ItemMovement_Hover : mg_ss_ItemMovement_Linear
	{
		private float m_amplitude;

		private float m_timeActive;

		public override void Initialize(mg_ss_ItemObject p_itemObject)
		{
			base.Initialize(p_itemObject);
			m_amplitude = 0.25f;
		}

		public override void UpdatePosition(Transform p_transform, float p_deltaTime, float p_conveyorSpeed)
		{
			m_timeActive += p_deltaTime;
			base.UpdatePosition(p_transform, p_deltaTime, p_conveyorSpeed);
			Vector2 v = p_transform.localPosition;
			v.y = m_initialPosition.y + m_amplitude * Mathf.Sin((float)Math.PI * m_timeActive);
			p_transform.localPosition = v;
		}
	}
}
