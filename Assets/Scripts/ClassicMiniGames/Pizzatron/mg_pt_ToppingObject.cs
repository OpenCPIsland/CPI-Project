using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_ToppingObject : MonoBehaviour
	{
		private SpriteRenderer m_renderer;

		private Transform m_saucePosition;

		public bool OffScreen
		{
			get
			{
				return !m_renderer.isVisible;
			}
		}

		public Vector2 SaucePosition
		{
			get
			{
				return (m_saucePosition == null) ? base.transform.position : m_saucePosition.position;
			}
		}

		protected virtual void Awake()
		{
			m_renderer = GetComponentInChildren<SpriteRenderer>();
			m_saucePosition = base.transform.Find("sauce_point");
		}

		public virtual void StateUpdated(mg_pt_EToppingState p_newState)
		{
		}
	}
}
