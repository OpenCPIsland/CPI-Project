using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_YMovement : MonoBehaviour
	{
		private float m_velocity;

		private float m_halfHeight;

		private mg_if_GameLogic m_gameLogic;

		public void Awake()
		{
			mg_IceFishing active = MinigameManager.GetActive<mg_IceFishing>();
			m_gameLogic = active.GetComponentInChildren<mg_if_GameLogic>();
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			UpdateY(base.gameObject.transform.position.y + m_velocity * p_deltaTime);
		}

		public void Initialize(float p_velocity, float p_velocityRange)
		{
			CalculateHalfHeight();
			m_velocity = Random.Range(p_velocity - p_velocityRange, p_velocity + p_velocityRange);
			if (Random.Range(0f, 1f) > 0.5f)
			{
				m_velocity *= -1f;
			}
		}

		private void CalculateHalfHeight()
		{
			Transform transform = base.transform.Find("object_size");
			if (transform != null)
			{
				m_halfHeight = transform.GetComponent<BoxCollider2D>().bounds.size.y * 0.5f;
			}
			else
			{
				m_halfHeight = base.gameObject.GetComponentInChildren<Renderer>().bounds.size.y * 0.5f;
			}
		}

		public void SetInitialPos()
		{
			UpdateY(Random.Range(m_gameLogic.BottomZone.position.y, m_gameLogic.TopZone.position.y));
		}

		private void UpdateY(float p_newY)
		{
			Vector2 v = base.transform.position;
			if (m_gameLogic.TopZone.position.y - m_halfHeight <= p_newY)
			{
				v.y = m_gameLogic.TopZone.position.y - m_halfHeight;
				m_velocity *= -1f;
			}
			else if (m_gameLogic.BottomZone.position.y + m_halfHeight >= p_newY)
			{
				v.y = m_gameLogic.BottomZone.position.y + m_halfHeight;
				m_velocity *= -1f;
			}
			else
			{
				v.y = p_newY;
			}
			base.transform.position = v;
		}
	}
}
