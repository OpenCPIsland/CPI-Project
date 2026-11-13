using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ItemMovement_Linear : mg_ss_IItemMovement
	{
		protected mg_ss_ItemObject m_itemObject;

		protected Vector2 m_initialPosition;

		public virtual void Initialize(mg_ss_ItemObject p_itemObject)
		{
			m_itemObject = p_itemObject;
			m_initialPosition = m_itemObject.transform.localPosition;
		}

		public virtual void UpdatePosition(Transform p_transform, float p_deltaTime, float p_conveyorSpeed)
		{
			Vector2 v = p_transform.localPosition;
			v.x -= p_deltaTime * p_conveyorSpeed;
			p_transform.localPosition = v;
		}

		public virtual void OnCollided()
		{
		}
	}
}
