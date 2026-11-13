#if UNITY_ANDROID || UNITY_IOS
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Pizzatron
{
    public class mg_pt_InputManager : MonoBehaviour
    {
        private Camera m_camera;
        private mg_pt_ToppingBar m_toppingBar;
        public bool IsActive { get; set; } = true;

        private void Awake()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnDestroy()
        {
            EnhancedTouchSupport.Disable();
        }

        public void Initialize(Camera p_camera, mg_pt_ToppingBar p_toppingBar)
        {
            m_camera = p_camera;
            m_toppingBar = p_toppingBar;
        }

        private void Update()
        {
            if (!IsActive || m_toppingBar == null || m_camera == null)
                return;

            foreach (var touch in Touch.activeTouches)
            {
                Vector3 worldPos = m_camera.ScreenToWorldPoint(new Vector3(touch.screenPosition.x, touch.screenPosition.y, m_camera.nearClipPlane));
                int fingerIndex = touch.finger.index;

                switch (touch.phase)
                {
                    case UnityEngine.InputSystem.TouchPhase.Began:
                        m_toppingBar.OnTouchStart(worldPos, fingerIndex);
                        break;
                    case UnityEngine.InputSystem.TouchPhase.Moved:
                        m_toppingBar.OnTouchMove(worldPos, fingerIndex);
                        break;
                    case UnityEngine.InputSystem.TouchPhase.Ended:
                    case UnityEngine.InputSystem.TouchPhase.Canceled:
                        m_toppingBar.OnTouchEnd(worldPos, fingerIndex);
                        break;
                }
            }
        }
    }
}
#else
using DisneyMobile.CoreUnitySystems;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_InputManager
	{
		private Camera m_camera;

		private mg_pt_ToppingBar m_toppingBar;

		private DelegateInputManagerSubscribeEvent m_onTouchStart;

		private DelegateInputManagerSubscribeEvent m_onTouchMove;

		private DelegateInputManagerSubscribeEvent m_onTouchEnd;

		public bool IsActive
		{
			get;
			set;
		}

		public mg_pt_InputManager(Camera p_camera, mg_pt_ToppingBar p_toppingBar)
		{
			IsActive = true;
			m_toppingBar = p_toppingBar;
			m_camera = p_camera;
			InputManager.AddCamera(m_camera);
			m_onTouchStart = OnTouchStart;
			m_onTouchMove = OnTouchMove;
			m_onTouchEnd = OnTouchEnd;
			InputManager.Subscribe(TouchEvent.ON_TOUCH_START, m_onTouchStart);
			InputManager.Subscribe(TouchEvent.ON_SWIPE, m_onTouchMove);
			InputManager.Subscribe(TouchEvent.ON_TOUCH_UP, m_onTouchEnd);
		}

		public void TidyUp()
		{
			InputManager.RemoveCamera(m_camera);
			InputManager.Unsubscribe(TouchEvent.ON_TOUCH_START, m_onTouchStart);
			InputManager.Unsubscribe(TouchEvent.ON_SWIPE, m_onTouchMove);
			InputManager.Unsubscribe(TouchEvent.ON_TOUCH_UP, m_onTouchEnd);
		}

		private void OnTouchStart(Gesture p_gesture)
		{
			if (IsActive)
			{
				m_toppingBar.OnTouchStart(m_camera.ScreenToWorldPoint(p_gesture.position), p_gesture.fingerIndex);
			}
		}

		private void OnTouchMove(Gesture p_gesture)
		{
			if (IsActive)
			{
				m_toppingBar.OnTouchMove(m_camera.ScreenToWorldPoint(p_gesture.position), p_gesture.fingerIndex);
			}
		}

		private void OnTouchEnd(Gesture p_gesture)
		{
			if (IsActive)
			{
				m_toppingBar.OnTouchEnd(m_camera.ScreenToWorldPoint(p_gesture.position), p_gesture.fingerIndex);
			}
		}
	}
}
#endif
