using DisneyMobile.CoreUnitySystems;
using UnityEngine;

public class InputTest : MonoBehaviour
{
	private Vector3 m_originalPosition;

	private Vector3 m_deltaPosition;

	public bool subscribeTouchStart = false;

	public bool subscribeTouchDown = false;

	private void Start()
	{
		m_originalPosition = base.gameObject.transform.position;
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		if (subscribeTouchStart)
		{
			InputManager.Subscribe(TouchEvent.ON_TOUCH_START, OnTouchStarted);
		}
		if (subscribeTouchDown)
		{
			InputManager.Subscribe(TouchEvent.ON_TOUCH_DOWN, OnTouchDown);
		}
		InputManager.Subscribe(TouchEvent.ON_DRAG_START, OnDragStart);
		InputManager.Subscribe(TouchEvent.ON_DRAG, OnDrag);
		InputManager.Subscribe(TouchEvent.ON_DRAG_END, OnDragEnd);
	}

	private void OnDisable()
	{
		UnsubscribeEvents();
	}

	private void OnDestroy()
	{
		UnsubscribeEvents();
	}

	private void UnsubscribeEvents()
	{
		if (subscribeTouchStart)
		{
			InputManager.Unsubscribe(TouchEvent.ON_TOUCH_START, OnTouchStarted);
		}
		if (subscribeTouchDown)
		{
			InputManager.Unsubscribe(TouchEvent.ON_TOUCH_DOWN, OnTouchDown);
		}
		InputManager.Unsubscribe(TouchEvent.ON_DRAG_START, OnDragStart);
		InputManager.Unsubscribe(TouchEvent.ON_DRAG, OnDrag);
		InputManager.Unsubscribe(TouchEvent.ON_DRAG_END, OnDragEnd);
	}

	private void OnTouchStarted(Gesture gesture)
	{
		if (!(gesture.pickObject == base.gameObject))
		{
		}
	}

	private void OnTouchDown(Gesture gesture)
	{
		if (!(gesture.pickObject == base.gameObject))
		{
		}
	}

	private void OnDragStart(Gesture gesture)
	{
		if (gesture.pickObject == base.gameObject)
		{
			Debug.Log("DRAG BEGAN");
			Vector3 touchToWordlPoint = gesture.GetTouchToWordlPoint(74f);
			m_deltaPosition = touchToWordlPoint - base.transform.position;
		}
	}

	private void OnDrag(Gesture gesture)
	{
		if (gesture.pickObject == base.gameObject)
		{
			Vector3 touchToWordlPoint = gesture.GetTouchToWordlPoint(74f);
			base.transform.position = touchToWordlPoint - m_deltaPosition;
			Debug.Log(base.transform.position);
		}
	}

	private void OnDragEnd(Gesture gesture)
	{
		if (gesture.pickObject == base.gameObject)
		{
			Debug.Log("DRAG ENDED");
			base.gameObject.transform.position = m_originalPosition;
		}
	}
}
