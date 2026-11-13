using MinigameFramework;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_Topping
	{
		private static float DEFAULT_POS = 50f;

		private mg_pt_EToppingState m_state;

		private mg_pt_Resources m_resources;

		private mg_pt_ToppingObject m_topping;

		private string m_heldTagSFX;

		private Vector2 m_lastPosition;

		private Vector2 m_velocity;

		private float m_rotationForce;

		public mg_pt_EToppingState State
		{
			get
			{
				return m_state;
			}
			private set
			{
				m_state = value;
				if (m_topping != null)
				{
					m_topping.StateUpdated(m_state);
				}
			}
		}

		public mg_pt_EToppingType Type
		{
			get;
			private set;
		}

		public int TouchID
		{
			get;
			private set;
		}

		public bool IsSauce
		{
			get
			{
				return Type == mg_pt_EToppingType.SAUCE_01 || Type == mg_pt_EToppingType.SAUCE_02;
			}
		}

		public bool IsCheese
		{
			get
			{
				return Type == mg_pt_EToppingType.CHEESE;
			}
		}

		public Vector2 Position
		{
			get
			{
				return m_topping.transform.position;
			}
		}

		public Vector2 SaucePosition
		{
			get
			{
				return m_topping.SaucePosition;
			}
		}

		public mg_pt_Topping(mg_pt_Resources p_resources)
		{
			m_resources = p_resources;
			m_velocity = new Vector2(0f, 0f);
			m_lastPosition = new Vector2(0f, 0f);
			Reset();
		}

		public void Reset()
		{
			State = mg_pt_EToppingState.IDLE;
			m_velocity.x = 0f;
			m_velocity.y = 0f;
			m_lastPosition.x = DEFAULT_POS;
			m_lastPosition.y = DEFAULT_POS;
			m_rotationForce = 0f;
			SetTopping(mg_pt_EToppingType.INVALID, null, null);
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			if (State == mg_pt_EToppingState.DROPPED)
			{
				Vector2 v = m_topping.transform.localPosition;
				m_velocity.y -= 1f * p_deltaTime;
				v += m_velocity;
				m_topping.transform.localPosition = v;
				m_topping.transform.Rotate(0f, 0f, m_rotationForce);
				if (m_topping.OffScreen)
				{
					Reset();
				}
			}
			else
			{
				m_velocity *= 0.9f;
			}
		}

		public void SetTopping(mg_pt_EToppingType p_toppingType, GameObject p_toppingBar, string p_heldTagSFX)
		{
			Type = p_toppingType;
			m_heldTagSFX = p_heldTagSFX;
			if (m_topping != null)
			{
				Object.Destroy(m_topping.gameObject);
				m_topping = null;
			}
			if (Type != mg_pt_EToppingType.INVALID)
			{
				m_topping = m_resources.GetTopping(Type).GetComponent<mg_pt_ToppingObject>();
				MinigameSpriteHelper.AssignParent(m_topping.gameObject, p_toppingBar);
				m_topping.StateUpdated(m_state);
			}
		}

		public void OnGrabbed(Vector2 p_position, int p_touchID)
		{
			TouchID = p_touchID;
			State = mg_pt_EToppingState.GRABBED;
			UpdatePosition(p_position);
			if (!string.IsNullOrEmpty(m_heldTagSFX))
			{
				MinigameManager.GetActive().PlaySFX(m_heldTagSFX);
			}
		}

		public void UpdatePosition(Vector2 p_position)
		{
			if (!(m_topping != null))
			{
				return;
			}
			m_topping.transform.position = p_position;
			if (State != mg_pt_EToppingState.DROPPED)
			{
				if (Mathf.Approximately(m_lastPosition.x, DEFAULT_POS) && Mathf.Approximately(m_lastPosition.y, DEFAULT_POS))
				{
					m_velocity.x = 0f;
					m_velocity.y = 0f;
				}
				else
				{
					m_velocity += (Vector2)m_topping.transform.localPosition - m_lastPosition;
				}
				m_lastPosition = m_topping.transform.localPosition;
			}
		}

		public void Drop()
		{
			State = mg_pt_EToppingState.DROPPED;
			m_velocity *= 0.2f;
			m_velocity.x = Mathf.Min(m_velocity.x, 0.5f);
			m_velocity.y = Mathf.Min(m_velocity.y, 0.5f);
			m_rotationForce = 0f - m_velocity.x / 0.1f;
			if (!string.IsNullOrEmpty(m_heldTagSFX))
			{
				MinigameManager.GetActive().StopSFX(m_heldTagSFX);
			}
		}

		public void Place(GameObject p_pizzaObject)
		{
			State = mg_pt_EToppingState.PLACED;
			if (Type == mg_pt_EToppingType.CHEESE)
			{
				Reset();
			}
			else
			{
				MinigameSpriteHelper.AssignParent(m_topping.gameObject, p_pizzaObject);
			}
		}
	}
}
