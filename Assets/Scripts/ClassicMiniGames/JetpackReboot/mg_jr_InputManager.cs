using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JetpackReboot
{
	public class mg_jr_InputManager : MonoBehaviour
	{
		private bool m_isMouseDown = false;

		private Dictionary<int, bool> m_isWaitingForUp = new Dictionary<int, bool>();

		private void Awake()
		{
			mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
			active.InputManager = this;
		}

		public void Prepare(Camera _camera)
		{
			InputManager.AddCamera(_camera);
		}

		private void OnDisable()
		{
			foreach (int key in m_isWaitingForUp.Keys)
			{
				if (m_isWaitingForUp[key])
				{
					OnTouchUp(Vector2.zero, key);
				}
			}
		}

		private void Update()
		{
			Vector3 mousePosition;
			if (Input.GetMouseButtonDown(0))
			{
				m_isMouseDown = true;
				mousePosition = Input.mousePosition;
				OnTouchDown(new Vector2(mousePosition.x, mousePosition.y));
			}
			else if (Input.GetMouseButtonUp(0))
			{
				m_isMouseDown = false;
				mousePosition = Input.mousePosition;
				OnTouchUp(new Vector2(mousePosition.x, mousePosition.y));
			}
			mousePosition = Input.mousePosition;
			OnTouchDrag(new Vector2(mousePosition.x, mousePosition.y));
			for (int i = 0; i < Input.touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				switch (touch.phase)
				{
				case TouchPhase.Began:
					OnTouchDown(touch.position, touch.fingerId);
					break;
				case TouchPhase.Moved:
					OnTouchDrag(touch.position, touch.fingerId);
					break;
				case TouchPhase.Ended:
					OnTouchUp(touch.position, touch.fingerId);
					break;
				case TouchPhase.Canceled:
					OnTouchUp(touch.position, touch.fingerId);
					break;
				default:
					DisneyMobile.CoreUnitySystems.Logger.LogWarning(touch.phase, "Unknown touch event");
					break;
				case TouchPhase.Stationary:
					break;
				}
			}
		}

		private bool IsTouchOrMouseClickOverUI(int _touchId)
		{
			return EventSystem.current.IsPointerOverGameObject(-1);
		}

		private void OnTouchDrag(Vector2 _position, int touchId = 0)
		{
			mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
			if (active != null && active.GameLogic != null && (!IsTouchOrMouseClickOverUI(touchId) || m_isWaitingForUp[touchId]))
			{
				active.GameLogic.OnTouchDrag(touchId, _position);
			}
		}

		private void OnTouchDown(Vector2 _position, int touchId = 0)
		{
			mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
			if (active != null && active.GameLogic != null)
			{
				if (!m_isWaitingForUp.ContainsKey(touchId))
				{
					m_isWaitingForUp.Add(touchId, false);
				}
				if (!IsTouchOrMouseClickOverUI(touchId))
				{
					m_isWaitingForUp[touchId] = true;
					active.GameLogic.OnTouchPress(true, touchId, _position);
				}
			}
		}

		private void OnTouchUp(Vector2 _position, int touchId = 0)
		{
			mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
			if (active != null && active.GameLogic != null && (!IsTouchOrMouseClickOverUI(touchId) || m_isWaitingForUp[touchId]))
			{
				active.GameLogic.OnTouchPress(false, touchId, _position);
				m_isWaitingForUp[touchId] = false;
			}
		}
	}
}
