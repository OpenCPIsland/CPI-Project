using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_InputManager
	{
		private Camera m_camera;

		private mg_ss_PlayerLogic m_player;

		private List<mg_ss_InputTouch> m_touches;

		private DelegateInputManagerSubscribeEvent m_onTouchStart;

		private DelegateInputManagerSubscribeEvent m_onTouchDown;

		private DelegateInputManagerSubscribeEvent m_onTouchEnd;

		private DelegateInputManagerSubscribeEvent m_onSwipeEnd;

		public mg_ss_InputManager(Camera p_camera, mg_ss_PlayerLogic p_player)
		{
			m_touches = new List<mg_ss_InputTouch>();
			m_player = p_player;
			m_camera = p_camera;
			InputManager.AddCamera(m_camera);
			InputManager.LongTapTime = 0.05f;
			m_onTouchStart = OnTouchStart;
			m_onTouchDown = OnTouchDown;
			m_onTouchEnd = OnTouchEnd;
			m_onSwipeEnd = OnSwipeEnd;
			InputManager.Subscribe(TouchEvent.ON_TOUCH_START, m_onTouchStart);
			InputManager.Subscribe(TouchEvent.ON_TOUCH_DOWN, m_onTouchDown);
			InputManager.Subscribe(TouchEvent.ON_TOUCH_UP, m_onTouchEnd);
			InputManager.Subscribe(TouchEvent.ON_SWIPE_END, m_onSwipeEnd);
			MinigameManager.GetActive<mg_SmoothieSmash>().InputObserver.SteeringChangedEvent += OnInputObserverSteeringChanged;
		}

		private void OnInputObserverSteeringChanged(Vector2 oldSteering, Vector2 newSteering)
		{
			if (newSteering.y == -1f)
			{
				m_player.StartSmashing();
			}
		}

		public void TidyUp()
		{
			InputManager.RemoveCamera(m_camera);
			InputManager.Unsubscribe(TouchEvent.ON_TOUCH_START, m_onTouchStart);
			InputManager.Unsubscribe(TouchEvent.ON_TOUCH_DOWN, m_onTouchDown);
			InputManager.Unsubscribe(TouchEvent.ON_TOUCH_UP, m_onTouchEnd);
			InputManager.Unsubscribe(TouchEvent.ON_SWIPE_END, m_onSwipeEnd);
			mg_SmoothieSmash active = MinigameManager.GetActive<mg_SmoothieSmash>();
			if (active != null)
			{
				active.InputObserver.SteeringChangedEvent -= OnInputObserverSteeringChanged;
			}
		}

		private void OnTouchStart(Gesture p_gesture)
		{
			mg_ss_InputTouch mg_ss_InputTouch = new mg_ss_InputTouch();
			mg_ss_InputTouch.FingerIndex = p_gesture.fingerIndex;
			mg_ss_InputTouch.Position = p_gesture.position;
			mg_ss_InputTouch mg_ss_InputTouch2 = mg_ss_InputTouch;
			RemoveAllTouchIndex(mg_ss_InputTouch2.FingerIndex);
			m_touches.Add(mg_ss_InputTouch2);
		}

		private void OnTouchDown(Gesture p_gesture)
		{
			mg_ss_InputTouch mg_ss_InputTouch = FindTouch(p_gesture.fingerIndex);
			if (mg_ss_InputTouch != null)
			{
				mg_ss_InputTouch.Position = p_gesture.position;
			}
		}

		private void OnTouchEnd(Gesture p_gesture)
		{
			RemoveAllTouchIndex(p_gesture.fingerIndex);
		}

		private void RemoveAllTouchIndex(int p_fingerIndex)
		{
			for (mg_ss_InputTouch mg_ss_InputTouch = FindTouch(p_fingerIndex); mg_ss_InputTouch != null; mg_ss_InputTouch = FindTouch(p_fingerIndex))
			{
				m_touches.Remove(mg_ss_InputTouch);
			}
		}

		private void OnSwipeEnd(Gesture p_gesture)
		{
			if (p_gesture.swipe == EasyTouch.SwipeType.Down)
			{
				m_player.StartSmashing();
			}
		}

		private mg_ss_InputTouch FindTouch(int p_fingerIndex)
		{
			mg_ss_InputTouch result = null;
			foreach (mg_ss_InputTouch touch in m_touches)
			{
				if (touch.FingerIndex == p_fingerIndex)
				{
					result = touch;
					break;
				}
			}
			return result;
		}

		public mg_ss_InputTouch GetTouch()
		{
			mg_ss_InputTouch result = null;
			if (m_touches.Count > 0)
			{
				result = m_touches[0];
			}
			return result;
		}
	}
}
