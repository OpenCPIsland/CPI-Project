using MinigameFramework;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_VerticalPingPong : MonoBehaviour
	{
		private const float SPEED = 2f;

		private const float MIN_SPEED = 0.5f;

		private const float SLOWING_DISTANCE = 1f;

		private mg_JetpackReboot m_miniGame;

		private SpriteRenderer m_renderer;

		private bool m_isStartTop;

		private bool m_isMovingDown;

		private float m_topMovementLimit;

		private float m_bottomMovementLimit;

		private float m_middleOfMovementRange;

		private void Awake()
		{
			m_miniGame = MinigameManager.GetActive<mg_JetpackReboot>();
			Assert.NotNull(m_miniGame, "mini game not found");
			m_renderer = GetComponentsInChildren<SpriteRenderer>(true)[0];
			Assert.NotNull(m_renderer, "Renderer not found");
		}

		private void Start()
		{
			m_isStartTop = (base.transform.position.y > 0f);
			if (m_isStartTop)
			{
				m_isMovingDown = true;
				m_topMovementLimit = base.transform.position.y;
				m_bottomMovementLimit = 0f - base.transform.position.y + m_renderer.bounds.size.y;
			}
			else
			{
				m_isMovingDown = false;
				m_topMovementLimit = 0f - base.transform.position.y + m_renderer.bounds.size.y;
				m_bottomMovementLimit = base.transform.position.y;
			}
			float num = m_topMovementLimit - m_bottomMovementLimit;
			m_middleOfMovementRange = m_bottomMovementLimit + 0.5f * num;
		}

		private void Update()
		{
			if (MinigameManager.IsPaused || !m_miniGame.VisibleWorldBounds.Intersects(m_renderer.bounds) || !m_miniGame.GameLogic.IsGameInProgress)
			{
				return;
			}
			Vector3 position = base.transform.position;
			float num = (!(base.transform.position.y > m_middleOfMovementRange)) ? Mathf.Abs(m_bottomMovementLimit - base.transform.position.y) : Mathf.Abs(m_topMovementLimit - base.transform.position.y);
			float num2 = 2f;
			if (num < 1f)
			{
				float t = Mathf.Clamp01(num / 1f);
				num2 = Mathf.Lerp(0.5f, 2f, t);
			}
			if (m_isMovingDown)
			{
				position.y -= num2 * Time.deltaTime;
				if (position.y < m_bottomMovementLimit)
				{
					position.y = m_bottomMovementLimit;
					m_isMovingDown = false;
				}
			}
			else
			{
				position.y += num2 * Time.deltaTime;
				if (position.y > m_topMovementLimit)
				{
					position.y = m_topMovementLimit;
					m_isMovingDown = true;
				}
			}
			base.transform.position = position;
		}
	}
}
