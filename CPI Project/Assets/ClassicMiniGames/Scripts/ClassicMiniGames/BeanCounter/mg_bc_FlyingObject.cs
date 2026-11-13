using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_FlyingObject : MonoBehaviour
	{
		private int m_primaryLayer;

		private int m_secondaryLayer;

		protected mg_bc_EObjectState m_state = mg_bc_EObjectState.STATE_NONE;

		protected Animator m_animator;

		public mg_bc_EObjectState State
		{
			get
			{
				return m_state;
			}
		}

		public virtual void Awake()
		{
			m_animator = GetComponentInChildren<Animator>();
		}

		private void OnTriggerEnter2D(Collider2D _collider)
		{
			if (_collider.gameObject.name == "mg_bc_ground_plane")
			{
				OnHitGround();
			}
		}

		protected virtual void OnHitGround()
		{
			if (m_animator != null)
			{
				m_animator.SetTrigger("hit_ground");
			}
			Object.Destroy(GetComponent<Rigidbody2D>());
			Object.Destroy(GetComponent<CircleCollider2D>());
			m_state = mg_bc_EObjectState.STATE_FADING_ON_GROUND;
		}

		public void FlyingUpdate(float _delta)
		{
			if (m_state == mg_bc_EObjectState.STATE_FADING_ON_GROUND)
			{
				SpriteRenderer componentInChildren = GetComponentInChildren<SpriteRenderer>();
				Color color = componentInChildren.color;
				float num = color.a - 1f * _delta;
				componentInChildren.color = new Color(color.r, color.g, color.b, num);
				if (num <= 0f)
				{
					m_state = mg_bc_EObjectState.STATE_TO_DESTROY;
				}
			}
		}

		public void Destroy()
		{
			m_state = mg_bc_EObjectState.STATE_TO_DESTROY;
		}

		public virtual void OnCaught()
		{
		}

		public virtual void SetLayers(int _primary, int _secondary)
		{
			m_primaryLayer = _primary;
			m_secondaryLayer = _secondary;
			SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>();
			if (componentsInChildren.Length > 1)
			{
				componentsInChildren[1].sortingOrder = _secondary;
			}
			if (componentsInChildren.Length > 0)
			{
				componentsInChildren[0].sortingOrder = _primary;
			}
		}

		public void GetLayers(out int _primary, out int _secondary)
		{
			_primary = m_primaryLayer;
			_secondary = m_secondaryLayer;
		}
	}
}
