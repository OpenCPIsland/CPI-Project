using CameraExtensionMethods;
using MinigameFramework;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_PlayOnAppearance : MonoBehaviour
	{
		private SpriteRenderer m_renderer;

		private float m_rightScreenEdge = 0f;

		private string m_sfxToPlay = null;

		private bool m_offRightOfScreenLastTurn = true;

		private bool m_offRightOfScreen = true;

		public string SfxToPlay
		{
			set
			{
				m_sfxToPlay = value;
			}
		}

		private void Start()
		{
			Assert.NotNull(m_sfxToPlay, "SFX should be specified immediately after component is added to gameobject");
			m_renderer = GetComponentInChildren<SpriteRenderer>();
			m_rightScreenEdge = Camera.main.RightEdgeInWorld();
			Assert.NotNull(m_renderer, "This component requires a sprite renderer");
		}

		private void Update()
		{
			if (!MinigameManager.IsPaused)
			{
				m_offRightOfScreen = (m_rightScreenEdge <= m_renderer.bounds.min.x);
				if (!m_offRightOfScreen && m_offRightOfScreenLastTurn)
				{
					MinigameManager.GetActive().PlaySFX(m_sfxToPlay);
				}
				m_offRightOfScreenLastTurn = m_offRightOfScreen;
			}
		}
	}
}
