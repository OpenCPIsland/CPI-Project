using MinigameFramework;
using System;
using UnityEngine;

namespace IceFishing
{
	public abstract class mg_if_GameObject : MonoBehaviour
	{
		protected mg_if_Variables m_variables;

		private SpriteRenderer[] m_renderers;

		private mg_if_EObjectState m_state;

		public mg_if_EObjectState State
		{
			get
			{
				return m_state;
			}
			private set
			{
				m_state = value;
				base.gameObject.SetActive(Convert.ToBoolean(m_state));
			}
		}

		protected virtual void Awake()
		{
			m_variables = MinigameManager.GetActive<mg_IceFishing>().Resources.Variables;
			m_renderers = GetComponentsInChildren<SpriteRenderer>();
		}

		protected virtual void Start()
		{
			State = mg_if_EObjectState.STATE_INACTIVE;
			base.gameObject.SetActive(false);
		}

		public virtual void Spawn()
		{
			State = mg_if_EObjectState.STATE_ACTIVE;
		}

		protected void Despawn()
		{
			State = mg_if_EObjectState.STATE_INACTIVE;
		}

		public virtual void MinigameUpdate(float p_deltaTime)
		{
		}

		public virtual void UpdateAlpha(float p_alpha)
		{
			SpriteRenderer[] renderers = m_renderers;
			foreach (SpriteRenderer p_renderer in renderers)
			{
				UpdateRendererAlpha(p_alpha, p_renderer);
			}
		}

		private void UpdateRendererAlpha(float p_alpha, SpriteRenderer p_renderer)
		{
			Color color = p_renderer.color;
			color.a = p_alpha;
			p_renderer.color = color;
		}
	}
}
