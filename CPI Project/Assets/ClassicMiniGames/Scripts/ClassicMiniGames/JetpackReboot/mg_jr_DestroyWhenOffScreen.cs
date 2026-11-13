using MinigameFramework;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_DestroyWhenOffScreen : MonoBehaviour
	{
		private SpriteRenderer[] m_renderers;

		private mg_jr_Resources m_resources;

		private mg_JetpackReboot m_miniGame;

		private void Start()
		{
			m_renderers = GetComponentsInChildren<SpriteRenderer>();
			m_miniGame = MinigameManager.GetActive<mg_JetpackReboot>();
			m_resources = m_miniGame.Resources;
			Assert.IsTrue(m_renderers.Length > 0, "This component requires at least one sprite renderer");
		}

		private void Update()
		{
			if (m_miniGame.IsPaused)
			{
				return;
			}
			bool flag = false;
			SpriteRenderer[] renderers = m_renderers;
			foreach (SpriteRenderer spriteRenderer in renderers)
			{
				flag = spriteRenderer.bounds.Intersects(m_miniGame.VisibleWorldBounds);
				if (flag)
				{
					break;
				}
			}
			if (!flag)
			{
				base.gameObject.transform.parent = null;
				m_resources.ReturnPooledResource(base.gameObject);
			}
		}
	}
}
