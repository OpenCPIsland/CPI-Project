using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class InputManager : MonoBehaviour, IConfigurable
	{
		private static InputManager m_instance = null;

		private EasyTouch m_easyTouch;

		public static InputManager Instance
		{
			get
			{
				return m_instance;
			}
		}

		public static float LongTapTime
		{
			get
			{
				return m_instance.m_easyTouch.longTapTime;
			}
			set
			{
				m_instance.m_easyTouch.longTapTime = value;
			}
		}

		private void Awake()
		{
			m_instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			m_easyTouch = base.gameObject.AddComponent<EasyTouch>();
			m_easyTouch.autoSelect = true;
		}

		public void Shutdown()
		{
			m_instance = null;
			Object.Destroy(base.gameObject);
		}

		public void Configure(IDictionary<string, object> dictionary)
		{
			AutoConfigurable.AutoConfigureObject(this, dictionary);
		}

		public void Reconfigure(IDictionary<string, object> dictionary)
		{
			Configure(dictionary);
		}

		public static void Subscribe(TouchEvent touchEvent, DelegateInputManagerSubscribeEvent action)
		{
			switch (touchEvent)
			{
			case TouchEvent.ON_TOUCH_START:
				EasyTouch.On_TouchStart += action.Invoke;
				break;
			case TouchEvent.ON_TOUCH_DOWN:
				EasyTouch.On_TouchDown += action.Invoke;
				break;
			case TouchEvent.ON_TOUCH_UP:
				EasyTouch.On_TouchUp += action.Invoke;
				break;
			case TouchEvent.ON_SWIPE_START:
				EasyTouch.On_SwipeStart += action.Invoke;
				break;
			case TouchEvent.ON_SWIPE:
				EasyTouch.On_Swipe += action.Invoke;
				break;
			case TouchEvent.ON_SWIPE_END:
				EasyTouch.On_SwipeEnd += action.Invoke;
				break;
			case TouchEvent.ON_DRAG_START:
				EasyTouch.On_DragStart += action.Invoke;
				break;
			case TouchEvent.ON_DRAG:
				EasyTouch.On_Drag += action.Invoke;
				break;
			case TouchEvent.ON_DRAG_END:
				EasyTouch.On_DragEnd += action.Invoke;
				break;
			case TouchEvent.ON_CANCEL:
				EasyTouch.On_Cancel += action.Invoke;
				break;
			case TouchEvent.ON_PINCH_IN:
				EasyTouch.On_PinchIn += action.Invoke;
				break;
			case TouchEvent.ON_PINCH_OUT:
				EasyTouch.On_PinchOut += action.Invoke;
				break;
			case TouchEvent.ON_PINCH_END:
				EasyTouch.On_PinchEnd += action.Invoke;
				break;
			case TouchEvent.ON_TWIST:
				EasyTouch.On_Twist += action.Invoke;
				break;
			case TouchEvent.ON_TWIST_END:
				EasyTouch.On_TwistEnd += action.Invoke;
				break;
			case TouchEvent.ON_SIMPLE_TAP:
				EasyTouch.On_SimpleTap += action.Invoke;
				break;
			}
		}

		public static void Unsubscribe(TouchEvent touchEvent, DelegateInputManagerSubscribeEvent action)
		{
			switch (touchEvent)
			{
			case TouchEvent.ON_TOUCH_START:
				EasyTouch.On_TouchStart -= action.Invoke;
				break;
			case TouchEvent.ON_TOUCH_DOWN:
				EasyTouch.On_TouchDown -= action.Invoke;
				break;
			case TouchEvent.ON_TOUCH_UP:
				EasyTouch.On_TouchUp -= action.Invoke;
				break;
			case TouchEvent.ON_SWIPE_START:
				EasyTouch.On_SwipeStart -= action.Invoke;
				break;
			case TouchEvent.ON_SWIPE:
				EasyTouch.On_Swipe -= action.Invoke;
				break;
			case TouchEvent.ON_SWIPE_END:
				EasyTouch.On_SwipeEnd -= action.Invoke;
				break;
			case TouchEvent.ON_DRAG_START:
				EasyTouch.On_DragStart -= action.Invoke;
				break;
			case TouchEvent.ON_DRAG:
				EasyTouch.On_Drag -= action.Invoke;
				break;
			case TouchEvent.ON_DRAG_END:
				EasyTouch.On_DragEnd -= action.Invoke;
				break;
			case TouchEvent.ON_CANCEL:
				EasyTouch.On_Cancel -= action.Invoke;
				break;
			case TouchEvent.ON_PINCH_IN:
				EasyTouch.On_PinchIn -= action.Invoke;
				break;
			case TouchEvent.ON_PINCH_OUT:
				EasyTouch.On_PinchOut -= action.Invoke;
				break;
			case TouchEvent.ON_PINCH_END:
				EasyTouch.On_PinchEnd -= action.Invoke;
				break;
			case TouchEvent.ON_TWIST:
				EasyTouch.On_Twist -= action.Invoke;
				break;
			case TouchEvent.ON_TWIST_END:
				EasyTouch.On_TwistEnd -= action.Invoke;
				break;
			case TouchEvent.ON_SIMPLE_TAP:
				EasyTouch.On_SimpleTap -= action.Invoke;
				break;
			}
		}

		public static void AddCamera(Camera camera)
		{
			m_instance.m_easyTouch.touchCameras.Add(new ECamera(camera, false));
		}

		public static void SetCamera(int index, Camera camera)
		{
			m_instance.m_easyTouch.touchCameras[index].camera = camera;
		}

		public static void RemoveCamera(int index)
		{
			m_instance.m_easyTouch.touchCameras.RemoveAt(index);
		}

		public static void RemoveCamera(Camera camera)
		{
			foreach (ECamera touchCamera in m_instance.m_easyTouch.touchCameras)
			{
				if (touchCamera.camera == camera)
				{
					m_instance.m_easyTouch.touchCameras.Remove(touchCamera);
					break;
				}
			}
		}

		public static void ClearAllCameras()
		{
			m_instance.m_easyTouch.touchCameras.Clear();
		}

		public static void SetLayerMask(int mask)
		{
			m_instance.m_easyTouch.pickableLayers = mask;
		}

		public static void ClearLayerMask()
		{
			m_instance.m_easyTouch.pickableLayers = 0;
		}

		public static Vector3 GetTouchToWorldPoint(Gesture gesture, float z)
		{
			return gesture.GetTouchToWordlPoint(z);
		}
	}
}
