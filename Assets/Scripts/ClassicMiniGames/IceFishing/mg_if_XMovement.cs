using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_XMovement : MonoBehaviour
	{
		private float m_velocity;

		private mg_if_GameLogic m_logic;

		private mg_if_GameObject m_gameObject;

		private mg_if_EObjectsMovement m_movement;

		private float m_screenWidth;

		private float m_halfWidth;

		private float m_alphaDistance;

		public mg_if_EObjectsMovement Movement
		{
			get
			{
				return m_movement;
			}
		}

		public void Awake()
		{
			m_gameObject = GetComponent<mg_if_GameObject>();
			m_logic = MinigameManager.GetActive<mg_IceFishing>().Logic;
			Camera mainCamera = MinigameManager.GetActive().MainCamera;
			m_screenWidth = mainCamera.aspect * mainCamera.orthographicSize;
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			Vector2 v = base.transform.position;
			v.x += m_velocity * p_deltaTime;
			base.transform.position = v;
			UpdateAlpha();
		}

		public void Initialize(mg_if_EObjectsMovement p_movement, float p_velocity, float p_velocityRange)
		{
			CalculateHalfWidth();
			m_movement = p_movement;
			if (m_movement == mg_if_EObjectsMovement.MOVEMENT_AUTO)
			{
				m_movement = (mg_if_EObjectsMovement)Random.Range(1, 3);
			}
			m_velocity = Random.Range(p_velocity - p_velocityRange, p_velocity + p_velocityRange);
			Vector2 v = base.transform.localScale;
			v.x = 1f;
			if (m_movement == mg_if_EObjectsMovement.MOVEMENT_LEFT)
			{
				m_velocity *= -1f;
				v.x = -1f;
			}
			base.transform.localScale = v;
		}

		public void SwapMovement()
		{
			mg_if_EObjectsMovement movement = mg_if_EObjectsMovement.MOVEMENT_LEFT;
			if (Movement == mg_if_EObjectsMovement.MOVEMENT_LEFT)
			{
				movement = mg_if_EObjectsMovement.MOVEMENT_RIGHT;
			}
			m_movement = movement;
			m_velocity *= -1f;
		}

		private void CalculateHalfWidth()
		{
			Transform transform = base.transform.Find("object_size");
			if (transform != null)
			{
				m_halfWidth = transform.GetComponent<BoxCollider2D>().bounds.size.x * 0.5f;
			}
			else
			{
				m_halfWidth = base.gameObject.GetComponentInChildren<Renderer>().bounds.size.x * 0.5f;
			}
		}

		public void SetInitialPos()
		{
			Vector2 v = base.transform.position;
			v.x = m_screenWidth + m_halfWidth;
			m_alphaDistance = v.x - m_logic.AlphaRight.position.x;
			if (m_movement == mg_if_EObjectsMovement.MOVEMENT_RIGHT)
			{
				v.x *= -1f;
			}
			base.transform.position = v;
		}

		public bool CheckOffEdge()
		{
			bool result = false;
			if (m_movement == mg_if_EObjectsMovement.MOVEMENT_LEFT)
			{
				if (0f - m_screenWidth > base.transform.position.x + m_halfWidth)
				{
					result = true;
				}
			}
			else if (m_screenWidth < base.transform.position.x - m_halfWidth)
			{
				result = true;
			}
			return result;
		}

		private void UpdateAlpha()
		{
			float p_alpha = 1f;
			if (base.transform.position.x - m_halfWidth <= m_logic.AlphaLeft.position.x)
			{
				p_alpha = 1f - (base.transform.position.x + m_halfWidth - m_logic.AlphaLeft.position.x) / (0f - m_alphaDistance);
			}
			else if (base.transform.position.x - m_halfWidth >= m_logic.AlphaRight.position.x)
			{
				p_alpha = 1f - (base.transform.position.x - m_halfWidth - m_logic.AlphaRight.position.x) / m_alphaDistance;
			}
			m_gameObject.UpdateAlpha(p_alpha);
		}
	}
}
