using ClubPenguin.Classic.MiniGames;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_InputManager
	{
		private int m_fingerIndex;

		private Camera m_camera;

		private mg_if_GameLogic m_logic;

		private DelegateInputManagerSubscribeEvent m_onTouchStart;

		private DelegateInputManagerSubscribeEvent m_onTouchUp;

		private DelegateInputManagerSubscribeEvent m_onTouchDown;

		private MouseInputObserver mouseInputObserver;

		public mg_if_InputManager(Camera p_camera, mg_if_GameLogic p_logic, MouseInputObserver mouseInputObserver)
		{
			m_logic = p_logic;
			m_camera = p_camera;
			InputManager.AddCamera(m_camera);
			m_fingerIndex = -1;
			InputManager.LongTapTime = 0.05f;
			m_onTouchStart = OnTouchStart;
			m_onTouchUp = OnTouchUp;
			m_onTouchDown = OnTouchDown;
			InputManager.Subscribe(TouchEvent.ON_TOUCH_START, m_onTouchStart);
			InputManager.Subscribe(TouchEvent.ON_TOUCH_UP, m_onTouchUp);
			InputManager.Subscribe(TouchEvent.ON_TOUCH_DOWN, m_onTouchDown);
			this.mouseInputObserver = mouseInputObserver;
			mouseInputObserver.MouseMovedEvent += OnMouseMoved;
		}

		private void OnMouseMoved(Vector3 mousePosition, Vector2 mouseDelta)
		{
			if (!MinigameManager.IsPaused)
			{
				m_logic.OnMouseMoved(mousePosition);
			}
		}

		public void TidyUp()
		{
			InputManager.RemoveCamera(m_camera);
			InputManager.Unsubscribe(TouchEvent.ON_TOUCH_START, m_onTouchStart);
			InputManager.Unsubscribe(TouchEvent.ON_TOUCH_UP, m_onTouchUp);
			InputManager.Unsubscribe(TouchEvent.ON_TOUCH_DOWN, m_onTouchDown);
			mouseInputObserver.MouseMovedEvent -= OnMouseMoved;
		}

		private void OnTouchStart(Gesture p_gesture)
		{
			if (m_fingerIndex < 0)
			{
				m_fingerIndex = p_gesture.fingerIndex;
			}
		}

		private void OnTouchUp(Gesture p_gesture)
		{
			if (p_gesture.fingerIndex == m_fingerIndex)
			{
				float num = Mathf.Abs(p_gesture.startPosition.y - p_gesture.position.y);
				if (num <= 10f)
				{
					m_logic.OnSimpleTap(p_gesture.position);
				}
				m_fingerIndex = -1;
			}
		}

		private void OnTouchDown(Gesture p_gesture)
		{
			if (m_fingerIndex == p_gesture.fingerIndex)
			{
				m_logic.OnTouchDown(p_gesture.position);
			}
		}
	}
}
