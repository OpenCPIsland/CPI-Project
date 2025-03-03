using MinigameFramework;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_DestroyWhenPastScreen : MonoBehaviour
	{
		private SpriteRenderer m_renderer;

		private mg_jr_Resources m_resources;

		private mg_JetpackReboot m_miniGame;

		private void Start()
		{
			m_renderer = GetComponentInChildren<SpriteRenderer>();
			m_miniGame = MinigameManager.GetActive<mg_JetpackReboot>();
			m_resources = m_miniGame.Resources;
			Assert.NotNull(m_renderer, "This component requires a sprite renderer");
		}

		private void Update()
		{
			if (!m_miniGame.IsPaused)
			{
				float num = m_renderer.bounds.max.x + 0.25f;
				if (num < m_miniGame.VisibleWorldBounds.min.x)
				{
					base.gameObject.transform.parent = null;
					m_resources.ReturnPooledResource(base.gameObject);
				}
			}
		}
	}
}
