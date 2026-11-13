using System.Collections.Generic;
using UnityEngine;

public class EasyTouch : MonoBehaviour
{
	public delegate void TouchCancelHandler(Gesture gesture);

	public delegate void Cancel2FingersHandler(Gesture gesture);

	public delegate void TouchStartHandler(Gesture gesture);

	public delegate void TouchDownHandler(Gesture gesture);

	public delegate void TouchUpHandler(Gesture gesture);

	public delegate void SimpleTapHandler(Gesture gesture);

	public delegate void DoubleTapHandler(Gesture gesture);

	public delegate void LongTapStartHandler(Gesture gesture);

	public delegate void LongTapHandler(Gesture gesture);

	public delegate void LongTapEndHandler(Gesture gesture);

	public delegate void DragStartHandler(Gesture gesture);

	public delegate void DragHandler(Gesture gesture);

	public delegate void DragEndHandler(Gesture gesture);

	public delegate void SwipeStartHandler(Gesture gesture);

	public delegate void SwipeHandler(Gesture gesture);

	public delegate void SwipeEndHandler(Gesture gesture);

	public delegate void TouchStart2FingersHandler(Gesture gesture);

	public delegate void TouchDown2FingersHandler(Gesture gesture);

	public delegate void TouchUp2FingersHandler(Gesture gesture);

	public delegate void SimpleTap2FingersHandler(Gesture gesture);

	public delegate void DoubleTap2FingersHandler(Gesture gesture);

	public delegate void LongTapStart2FingersHandler(Gesture gesture);

	public delegate void LongTap2FingersHandler(Gesture gesture);

	public delegate void LongTapEnd2FingersHandler(Gesture gesture);

	public delegate void TwistHandler(Gesture gesture);

	public delegate void TwistEndHandler(Gesture gesture);

	public delegate void PinchInHandler(Gesture gesture);

	public delegate void PinchOutHandler(Gesture gesture);

	public delegate void PinchEndHandler(Gesture gesture);

	public delegate void DragStart2FingersHandler(Gesture gesture);

	public delegate void Drag2FingersHandler(Gesture gesture);

	public delegate void DragEnd2FingersHandler(Gesture gesture);

	public delegate void SwipeStart2FingersHandler(Gesture gesture);

	public delegate void Swipe2FingersHandler(Gesture gesture);

	public delegate void SwipeEnd2FingersHandler(Gesture gesture);

	public delegate void EasyTouchIsReadyHandler();

	public enum GestureType
	{
		Tap,
		Drag,
		Swipe,
		None,
		LongTap,
		Pinch,
		Twist,
		Cancel,
		Acquisition
	}

	public enum SwipeType
	{
		None,
		Left,
		Right,
		Up,
		Down,
		Other
	}

	private enum EventName
	{
		None,
		On_Cancel,
		On_Cancel2Fingers,
		On_TouchStart,
		On_TouchDown,
		On_TouchUp,
		On_SimpleTap,
		On_DoubleTap,
		On_LongTapStart,
		On_LongTap,
		On_LongTapEnd,
		On_DragStart,
		On_Drag,
		On_DragEnd,
		On_SwipeStart,
		On_Swipe,
		On_SwipeEnd,
		On_TouchStart2Fingers,
		On_TouchDown2Fingers,
		On_TouchUp2Fingers,
		On_SimpleTap2Fingers,
		On_DoubleTap2Fingers,
		On_LongTapStart2Fingers,
		On_LongTap2Fingers,
		On_LongTapEnd2Fingers,
		On_Twist,
		On_TwistEnd,
		On_PinchIn,
		On_PinchOut,
		On_PinchEnd,
		On_DragStart2Fingers,
		On_Drag2Fingers,
		On_DragEnd2Fingers,
		On_SwipeStart2Fingers,
		On_Swipe2Fingers,
		On_SwipeEnd2Fingers,
		On_EasyTouchIsReady
	}

	public static EasyTouch instance;

	public bool enable = true;

	public bool enableRemote = false;

	public bool useBroadcastMessage = true;

	public GameObject receiverObject = null;

	public bool isExtension = false;

	public bool enable2FingersGesture = true;

	public bool enableTwist = true;

	public bool enablePinch = true;

	public List<ECamera> touchCameras = new List<ECamera>();

	public bool autoSelect = false;

	public LayerMask pickableLayers;

	public bool enable2D = false;

	public LayerMask pickableLayers2D;

	public float StationnaryTolerance = 25f;

	public float longTapTime = 1f;

	public float swipeTolerance = 0.85f;

	public float minPinchLength = 0f;

	public float minTwistAngle = 1f;

	public bool enabledNGuiMode = false;

	public LayerMask nGUILayers;

	public List<Camera> nGUICameras = new List<Camera>();

	private bool isStartHoverNGUI = false;

	public List<Rect> reservedAreas = new List<Rect>();

	public List<Rect> reservedVirtualAreas = new List<Rect>();

	public List<Rect> reservedGuiAreas = new List<Rect>();

	public bool enableReservedArea = true;

	public KeyCode twistKey = KeyCode.LeftAlt;

	public KeyCode swipeKey = KeyCode.LeftControl;

	public bool showGeneral = true;

	public bool showSelect = true;

	public bool showGesture = true;

	public bool showTwoFinger = true;

	public bool showSecondFinger = true;

	private EasyTouchInput input;

	private GestureType complexCurrentGesture = GestureType.None;

	private GestureType oldGesture = GestureType.None;

	private float startTimeAction;

	private Finger[] fingers = new Finger[10];

	private GameObject pickObject2Finger;

	private GameObject oldPickObject2Finger;

	public Texture secondFingerTexture;

	private Vector2 startPosition2Finger;

	private int twoFinger0;

	private int twoFinger1;

	private Vector2 oldStartPosition2Finger;

	private float oldFingerDistance;

	private bool twoFingerDragStart = false;

	private bool twoFingerSwipeStart = false;

	private int oldTouchCount = 0;

	public static event TouchCancelHandler On_Cancel;

	public static event Cancel2FingersHandler On_Cancel2Fingers;

	public static event TouchStartHandler On_TouchStart;

	public static event TouchDownHandler On_TouchDown;

	public static event TouchUpHandler On_TouchUp;

	public static event SimpleTapHandler On_SimpleTap;

	public static event DoubleTapHandler On_DoubleTap;

	public static event LongTapStartHandler On_LongTapStart;

	public static event LongTapHandler On_LongTap;

	public static event LongTapEndHandler On_LongTapEnd;

	public static event DragStartHandler On_DragStart;

	public static event DragHandler On_Drag;

	public static event DragEndHandler On_DragEnd;

	public static event SwipeStartHandler On_SwipeStart;

	public static event SwipeHandler On_Swipe;

	public static event SwipeEndHandler On_SwipeEnd;

	public static event TouchStart2FingersHandler On_TouchStart2Fingers;

	public static event TouchDown2FingersHandler On_TouchDown2Fingers;

	public static event TouchUp2FingersHandler On_TouchUp2Fingers;

	public static event SimpleTap2FingersHandler On_SimpleTap2Fingers;

	public static event DoubleTap2FingersHandler On_DoubleTap2Fingers;

	public static event LongTapStart2FingersHandler On_LongTapStart2Fingers;

	public static event LongTap2FingersHandler On_LongTap2Fingers;

	public static event LongTapEnd2FingersHandler On_LongTapEnd2Fingers;

	public static event TwistHandler On_Twist;

	public static event TwistEndHandler On_TwistEnd;

	public static event PinchInHandler On_PinchIn;

	public static event PinchOutHandler On_PinchOut;

	public static event PinchEndHandler On_PinchEnd;

	public static event DragStart2FingersHandler On_DragStart2Fingers;

	public static event Drag2FingersHandler On_Drag2Fingers;

	public static event DragEnd2FingersHandler On_DragEnd2Fingers;

	public static event SwipeStart2FingersHandler On_SwipeStart2Fingers;

	public static event Swipe2FingersHandler On_Swipe2Fingers;

	public static event SwipeEnd2FingersHandler On_SwipeEnd2Fingers;

	public static event EasyTouchIsReadyHandler On_EasyTouchIsReady;

	public EasyTouch()
	{
		enable = true;
		useBroadcastMessage = false;
		enable2FingersGesture = true;
		enableTwist = true;
		enablePinch = true;
		autoSelect = false;
		StationnaryTolerance = 25f;
		longTapTime = 1f;
		swipeTolerance = 0.85f;
		minPinchLength = 0f;
		minTwistAngle = 1f;
	}

	private void OnEnable()
	{
		if (Application.isPlaying && Application.isEditor)
		{
			InitEasyTouch();
		}
	}

	private void Start()
	{
		int num = touchCameras.FindIndex((ECamera c) => c.camera == Camera.main);
		if (num < 0)
		{
			touchCameras.Add(new ECamera(Camera.main, false));
		}
		InitEasyTouch();
		RaiseReadyEvent();
	}

	private void InitEasyTouch()
	{
		input = new EasyTouchInput();
		if (instance == null)
		{
			instance = this;
		}
		if (secondFingerTexture == null)
		{
			secondFingerTexture = (Resources.Load("secondFinger") as Texture);
		}
	}

	private void OnGUI()
	{
		Vector2 secondFingerPosition = input.GetSecondFingerPosition();
		if (secondFingerPosition != new Vector2(-1f, -1f))
		{
			GUI.DrawTexture(new Rect(secondFingerPosition.x - 16f, (float)Screen.height - secondFingerPosition.y - 16f, 32f, 32f), secondFingerTexture);
		}
	}

	private void OnDrawGizmos()
	{
	}

	private void Update()
	{
		if (!enable || !(instance == this))
		{
			return;
		}
		int num = input.TouchCount();
		if (oldTouchCount == 2 && num != 2 && num > 0)
		{
			CreateGesture2Finger(EventName.On_Cancel2Fingers, Vector2.zero, Vector2.zero, Vector2.zero, 0f, SwipeType.None, 0f, Vector2.zero, 0f, 0f, 0f);
		}
		UpdateTouches(false, num);
		oldPickObject2Finger = pickObject2Finger;
		if (enable2FingersGesture)
		{
			if (num == 2)
			{
				TwoFinger();
			}
			else
			{
				complexCurrentGesture = GestureType.None;
				pickObject2Finger = null;
				twoFingerSwipeStart = false;
				twoFingerDragStart = false;
			}
		}
		for (int i = 0; i < 10; i++)
		{
			if (fingers[i] != null)
			{
				OneFinger(i);
			}
		}
		oldTouchCount = num;
	}

	private void UpdateTouches(bool realTouch, int touchCount)
	{
		Finger[] array = new Finger[10];
		fingers.CopyTo(array, 0);
		if (realTouch || enableRemote)
		{
			ResetTouches();
			for (int i = 0; i < touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				for (int j = 0; j < 10; j++)
				{
					if (fingers[i] != null)
					{
						break;
					}
					if (array[j] != null && array[j].fingerIndex == touch.fingerId)
					{
						fingers[i] = array[j];
					}
				}
				if (fingers[i] == null)
				{
					fingers[i] = new Finger();
					fingers[i].fingerIndex = touch.fingerId;
					fingers[i].gesture = GestureType.None;
					fingers[i].phase = TouchPhase.Began;
				}
				else
				{
					fingers[i].phase = touch.phase;
				}
				fingers[i].position = touch.position;
				fingers[i].deltaPosition = touch.deltaPosition;
				fingers[i].tapCount = touch.tapCount;
				fingers[i].deltaTime = touch.deltaTime;
				fingers[i].touchCount = touchCount;
			}
		}
		else
		{
			for (int i = 0; i < touchCount; i++)
			{
				fingers[i] = input.GetMouseTouch(i, fingers[i]);
				fingers[i].touchCount = touchCount;
			}
		}
	}

	private void ResetTouches()
	{
		for (int i = 0; i < 10; i++)
		{
			fingers[i] = null;
		}
	}

	private void OneFinger(int fingerIndex)
	{
		float num = 0f;
		if (fingers[fingerIndex].gesture == GestureType.None)
		{
			startTimeAction = Time.realtimeSinceStartup;
			fingers[fingerIndex].gesture = GestureType.Acquisition;
			fingers[fingerIndex].startPosition = fingers[fingerIndex].position;
			if (autoSelect)
			{
				GetPickeGameObject(ref fingers[fingerIndex]);
			}
			CreateGesture(fingerIndex, EventName.On_TouchStart, fingers[fingerIndex], 0f, SwipeType.None, 0f, Vector2.zero);
		}
		num = Time.realtimeSinceStartup - startTimeAction;
		if (fingers[fingerIndex].phase == TouchPhase.Canceled)
		{
			fingers[fingerIndex].gesture = GestureType.Cancel;
		}
		if (fingers[fingerIndex].phase != TouchPhase.Ended && fingers[fingerIndex].phase != TouchPhase.Canceled)
		{
			if (fingers[fingerIndex].phase == TouchPhase.Stationary && num >= longTapTime && fingers[fingerIndex].gesture == GestureType.Acquisition)
			{
				fingers[fingerIndex].gesture = GestureType.LongTap;
				CreateGesture(fingerIndex, EventName.On_LongTapStart, fingers[fingerIndex], num, SwipeType.None, 0f, Vector2.zero);
			}
			if ((fingers[fingerIndex].gesture == GestureType.Acquisition || fingers[fingerIndex].gesture == GestureType.LongTap) && !FingerInTolerance(fingers[fingerIndex]))
			{
				if (fingers[fingerIndex].gesture == GestureType.LongTap)
				{
					fingers[fingerIndex].gesture = GestureType.Cancel;
					CreateGesture(fingerIndex, EventName.On_LongTapEnd, fingers[fingerIndex], num, SwipeType.None, 0f, Vector2.zero);
					fingers[fingerIndex].gesture = GestureType.None;
				}
				else if ((bool)fingers[fingerIndex].pickedObject)
				{
					fingers[fingerIndex].gesture = GestureType.Drag;
					CreateGesture(fingerIndex, EventName.On_DragStart, fingers[fingerIndex], num, SwipeType.None, 0f, Vector2.zero);
				}
				else
				{
					fingers[fingerIndex].gesture = GestureType.Swipe;
					CreateGesture(fingerIndex, EventName.On_SwipeStart, fingers[fingerIndex], num, SwipeType.None, 0f, Vector2.zero);
				}
			}
			EventName eventName = EventName.None;
			switch (fingers[fingerIndex].gesture)
			{
			case GestureType.LongTap:
				eventName = EventName.On_LongTap;
				break;
			case GestureType.Drag:
				eventName = EventName.On_Drag;
				break;
			case GestureType.Swipe:
				eventName = EventName.On_Swipe;
				break;
			}
			SwipeType swipe = SwipeType.None;
			if (eventName != 0)
			{
				swipe = GetSwipe(new Vector2(0f, 0f), fingers[fingerIndex].deltaPosition);
				CreateGesture(fingerIndex, eventName, fingers[fingerIndex], num, swipe, 0f, fingers[fingerIndex].deltaPosition);
			}
			CreateGesture(fingerIndex, EventName.On_TouchDown, fingers[fingerIndex], num, swipe, 0f, fingers[fingerIndex].deltaPosition);
			return;
		}
		bool flag = true;
		switch (fingers[fingerIndex].gesture)
		{
		case GestureType.Acquisition:
		{
			if (FingerInTolerance(fingers[fingerIndex]))
			{
				if (fingers[fingerIndex].tapCount < 2)
				{
					CreateGesture(fingerIndex, EventName.On_SimpleTap, fingers[fingerIndex], num, SwipeType.None, 0f, Vector2.zero);
				}
				else
				{
					CreateGesture(fingerIndex, EventName.On_DoubleTap, fingers[fingerIndex], num, SwipeType.None, 0f, Vector2.zero);
				}
				break;
			}
			SwipeType swipe = GetSwipe(new Vector2(0f, 0f), fingers[fingerIndex].deltaPosition);
			if ((bool)fingers[fingerIndex].pickedObject)
			{
				CreateGesture(fingerIndex, EventName.On_DragStart, fingers[fingerIndex], num, SwipeType.None, 0f, Vector2.zero);
				CreateGesture(fingerIndex, EventName.On_Drag, fingers[fingerIndex], num, swipe, 0f, fingers[fingerIndex].deltaPosition);
				CreateGesture(fingerIndex, EventName.On_DragEnd, fingers[fingerIndex], num, GetSwipe(fingers[fingerIndex].startPosition, fingers[fingerIndex].position), (fingers[fingerIndex].startPosition - fingers[fingerIndex].position).magnitude, fingers[fingerIndex].position - fingers[fingerIndex].startPosition);
			}
			else
			{
				CreateGesture(fingerIndex, EventName.On_SwipeStart, fingers[fingerIndex], num, SwipeType.None, 0f, Vector2.zero);
				CreateGesture(fingerIndex, EventName.On_Swipe, fingers[fingerIndex], num, swipe, 0f, fingers[fingerIndex].deltaPosition);
				CreateGesture(fingerIndex, EventName.On_SwipeEnd, fingers[fingerIndex], num, GetSwipe(fingers[fingerIndex].startPosition, fingers[fingerIndex].position), (fingers[fingerIndex].position - fingers[fingerIndex].startPosition).magnitude, fingers[fingerIndex].position - fingers[fingerIndex].startPosition);
			}
			break;
		}
		case GestureType.LongTap:
			CreateGesture(fingerIndex, EventName.On_LongTapEnd, fingers[fingerIndex], num, SwipeType.None, 0f, Vector2.zero);
			break;
		case GestureType.Drag:
			CreateGesture(fingerIndex, EventName.On_DragEnd, fingers[fingerIndex], num, GetSwipe(fingers[fingerIndex].startPosition, fingers[fingerIndex].position), (fingers[fingerIndex].startPosition - fingers[fingerIndex].position).magnitude, fingers[fingerIndex].position - fingers[fingerIndex].startPosition);
			break;
		case GestureType.Swipe:
			CreateGesture(fingerIndex, EventName.On_SwipeEnd, fingers[fingerIndex], num, GetSwipe(fingers[fingerIndex].startPosition, fingers[fingerIndex].position), (fingers[fingerIndex].position - fingers[fingerIndex].startPosition).magnitude, fingers[fingerIndex].position - fingers[fingerIndex].startPosition);
			break;
		case GestureType.Cancel:
			CreateGesture(fingerIndex, EventName.On_Cancel, fingers[fingerIndex], 0f, SwipeType.None, 0f, Vector2.zero);
			break;
		}
		if (flag)
		{
			CreateGesture(fingerIndex, EventName.On_TouchUp, fingers[fingerIndex], num, SwipeType.None, 0f, Vector2.zero);
			fingers[fingerIndex] = null;
		}
	}

	private void CreateGesture(int touchIndex, EventName message, Finger finger, float actionTime, SwipeType swipe, float swipeLength, Vector2 swipeVector)
	{
		if (message == EventName.On_TouchStart || message == EventName.On_TouchUp)
		{
			isStartHoverNGUI = IsTouchHoverNGui(touchIndex);
		}
		if (!isStartHoverNGUI)
		{
			Gesture gesture = new Gesture();
			gesture.fingerIndex = finger.fingerIndex;
			gesture.touchCount = finger.touchCount;
			gesture.startPosition = finger.startPosition;
			gesture.position = finger.position;
			gesture.deltaPosition = finger.deltaPosition;
			gesture.actionTime = actionTime;
			gesture.deltaTime = finger.deltaTime;
			gesture.swipe = swipe;
			gesture.swipeLength = swipeLength;
			gesture.swipeVector = swipeVector;
			gesture.deltaPinch = 0f;
			gesture.twistAngle = 0f;
			gesture.pickObject = finger.pickedObject;
			gesture.otherReceiver = receiverObject;
			gesture.isHoverReservedArea = IsTouchReservedArea(touchIndex);
			gesture.pickCamera = finger.pickedCamera;
			gesture.isGuiCamera = finger.isGuiCamera;
			if (useBroadcastMessage)
			{
				SendGesture(message, gesture);
			}
			if (!useBroadcastMessage || isExtension)
			{
				RaiseEvent(message, gesture);
			}
		}
	}

	private void SendGesture(EventName message, Gesture gesture)
	{
		if (useBroadcastMessage)
		{
			if (receiverObject != null && receiverObject != gesture.pickObject)
			{
				receiverObject.SendMessage(message.ToString(), gesture, SendMessageOptions.DontRequireReceiver);
			}
			if ((bool)gesture.pickObject)
			{
				gesture.pickObject.SendMessage(message.ToString(), gesture, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				SendMessage(message.ToString(), gesture, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void TwoFinger()
	{
		float actionTime = 0f;
		bool flag = false;
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		float num = 0f;
		if (complexCurrentGesture == GestureType.None)
		{
			twoFinger0 = GetTwoFinger(-1);
			twoFinger1 = GetTwoFinger(twoFinger0);
			startTimeAction = Time.realtimeSinceStartup;
			complexCurrentGesture = GestureType.Tap;
			fingers[twoFinger0].complexStartPosition = fingers[twoFinger0].position;
			fingers[twoFinger1].complexStartPosition = fingers[twoFinger1].position;
			fingers[twoFinger0].oldPosition = fingers[twoFinger0].position;
			fingers[twoFinger1].oldPosition = fingers[twoFinger1].position;
			oldFingerDistance = Mathf.Abs(Vector2.Distance(fingers[twoFinger0].position, fingers[twoFinger1].position));
			startPosition2Finger = new Vector2((fingers[twoFinger0].position.x + fingers[twoFinger1].position.x) / 2f, (fingers[twoFinger0].position.y + fingers[twoFinger1].position.y) / 2f);
			zero2 = Vector2.zero;
			if (autoSelect)
			{
				if (GetPickeGameObject(ref fingers[twoFinger0], true))
				{
					GetPickeGameObject(ref fingers[twoFinger1], true);
					if (fingers[twoFinger0].pickedObject != fingers[twoFinger1].pickedObject)
					{
						pickObject2Finger = null;
						fingers[twoFinger0].pickedObject = null;
						fingers[twoFinger1].pickedObject = null;
						fingers[twoFinger0].isGuiCamera = false;
						fingers[twoFinger1].isGuiCamera = false;
						fingers[twoFinger0].pickedCamera = null;
						fingers[twoFinger1].pickedCamera = null;
					}
					else
					{
						pickObject2Finger = fingers[twoFinger0].pickedObject;
					}
				}
				else
				{
					pickObject2Finger = null;
				}
			}
			CreateGesture2Finger(EventName.On_TouchStart2Fingers, startPosition2Finger, startPosition2Finger, zero2, actionTime, SwipeType.None, 0f, Vector2.zero, 0f, 0f, oldFingerDistance);
		}
		actionTime = Time.realtimeSinceStartup - startTimeAction;
		zero = new Vector2((fingers[twoFinger0].position.x + fingers[twoFinger1].position.x) / 2f, (fingers[twoFinger0].position.y + fingers[twoFinger1].position.y) / 2f);
		zero2 = zero - oldStartPosition2Finger;
		num = Mathf.Abs(Vector2.Distance(fingers[twoFinger0].position, fingers[twoFinger1].position));
		if (fingers[twoFinger0].phase == TouchPhase.Canceled || fingers[twoFinger1].phase == TouchPhase.Canceled)
		{
			complexCurrentGesture = GestureType.Cancel;
		}
		if (fingers[twoFinger0].phase != TouchPhase.Ended && fingers[twoFinger1].phase != TouchPhase.Ended && complexCurrentGesture != GestureType.Cancel)
		{
			if (complexCurrentGesture == GestureType.Tap && actionTime >= longTapTime && FingerInTolerance(fingers[twoFinger0]) && FingerInTolerance(fingers[twoFinger1]))
			{
				complexCurrentGesture = GestureType.LongTap;
				CreateGesture2Finger(EventName.On_LongTapStart2Fingers, startPosition2Finger, zero, zero2, actionTime, SwipeType.None, 0f, Vector2.zero, 0f, 0f, num);
			}
			if (!FingerInTolerance(fingers[twoFinger0]) || !FingerInTolerance(fingers[twoFinger1]))
			{
				flag = true;
			}
			if (flag)
			{
				float num2 = Vector2.Dot(fingers[twoFinger0].deltaPosition.normalized, fingers[twoFinger1].deltaPosition.normalized);
				if (enablePinch && num != oldFingerDistance)
				{
					if (Mathf.Abs(num - oldFingerDistance) >= minPinchLength)
					{
						complexCurrentGesture = GestureType.Pinch;
					}
					if (complexCurrentGesture == GestureType.Pinch)
					{
						if (num < oldFingerDistance)
						{
							if (oldGesture != GestureType.Pinch)
							{
								CreateStateEnd2Fingers(oldGesture, startPosition2Finger, zero, zero2, actionTime, false, num);
								startTimeAction = Time.realtimeSinceStartup;
							}
							CreateGesture2Finger(EventName.On_PinchIn, startPosition2Finger, zero, zero2, actionTime, GetSwipe(fingers[twoFinger0].complexStartPosition, fingers[twoFinger0].position), 0f, Vector2.zero, 0f, Mathf.Abs(num - oldFingerDistance), num);
							complexCurrentGesture = GestureType.Pinch;
						}
						else if (num > oldFingerDistance)
						{
							if (oldGesture != GestureType.Pinch)
							{
								CreateStateEnd2Fingers(oldGesture, startPosition2Finger, zero, zero2, actionTime, false, num);
								startTimeAction = Time.realtimeSinceStartup;
							}
							CreateGesture2Finger(EventName.On_PinchOut, startPosition2Finger, zero, zero2, actionTime, GetSwipe(fingers[twoFinger0].complexStartPosition, fingers[twoFinger0].position), 0f, Vector2.zero, 0f, Mathf.Abs(num - oldFingerDistance), num);
							complexCurrentGesture = GestureType.Pinch;
						}
					}
				}
				if (enableTwist)
				{
					if (Mathf.Abs(TwistAngle()) > minTwistAngle)
					{
						if (complexCurrentGesture != GestureType.Twist)
						{
							CreateStateEnd2Fingers(complexCurrentGesture, startPosition2Finger, zero, zero2, actionTime, false, num);
							startTimeAction = Time.realtimeSinceStartup;
						}
						complexCurrentGesture = GestureType.Twist;
					}
					if (complexCurrentGesture == GestureType.Twist)
					{
						CreateGesture2Finger(EventName.On_Twist, startPosition2Finger, zero, zero2, actionTime, SwipeType.None, 0f, Vector2.zero, TwistAngle(), 0f, num);
					}
					fingers[twoFinger0].oldPosition = fingers[twoFinger0].position;
					fingers[twoFinger1].oldPosition = fingers[twoFinger1].position;
				}
				if (num2 > 0f)
				{
					if ((bool)pickObject2Finger && !twoFingerDragStart)
					{
						if (complexCurrentGesture != 0)
						{
							CreateStateEnd2Fingers(complexCurrentGesture, startPosition2Finger, zero, zero2, actionTime, false, num);
							startTimeAction = Time.realtimeSinceStartup;
						}
						CreateGesture2Finger(EventName.On_DragStart2Fingers, startPosition2Finger, zero, zero2, actionTime, SwipeType.None, 0f, Vector2.zero, 0f, 0f, num);
						twoFingerDragStart = true;
					}
					else if (!pickObject2Finger && !twoFingerSwipeStart)
					{
						if (complexCurrentGesture != 0)
						{
							CreateStateEnd2Fingers(complexCurrentGesture, startPosition2Finger, zero, zero2, actionTime, false, num);
							startTimeAction = Time.realtimeSinceStartup;
						}
						CreateGesture2Finger(EventName.On_SwipeStart2Fingers, startPosition2Finger, zero, zero2, actionTime, SwipeType.None, 0f, Vector2.zero, 0f, 0f, num);
						twoFingerSwipeStart = true;
					}
				}
				else if (num2 < 0f)
				{
					twoFingerDragStart = false;
					twoFingerSwipeStart = false;
				}
				if (twoFingerDragStart)
				{
					CreateGesture2Finger(EventName.On_Drag2Fingers, startPosition2Finger, zero, zero2, actionTime, GetSwipe(oldStartPosition2Finger, zero), 0f, zero2, 0f, 0f, num);
				}
				if (twoFingerSwipeStart)
				{
					CreateGesture2Finger(EventName.On_Swipe2Fingers, startPosition2Finger, zero, zero2, actionTime, GetSwipe(oldStartPosition2Finger, zero), 0f, zero2, 0f, 0f, num);
				}
			}
			else if (complexCurrentGesture == GestureType.LongTap)
			{
				CreateGesture2Finger(EventName.On_LongTap2Fingers, startPosition2Finger, zero, zero2, actionTime, SwipeType.None, 0f, Vector2.zero, 0f, 0f, num);
			}
			CreateGesture2Finger(EventName.On_TouchDown2Fingers, startPosition2Finger, zero, zero2, actionTime, GetSwipe(oldStartPosition2Finger, zero), 0f, zero2, 0f, 0f, num);
			oldFingerDistance = num;
			oldStartPosition2Finger = zero;
			oldGesture = complexCurrentGesture;
		}
		else
		{
			CreateStateEnd2Fingers(complexCurrentGesture, startPosition2Finger, zero, zero2, actionTime, true, num);
			complexCurrentGesture = GestureType.None;
			pickObject2Finger = null;
			twoFingerSwipeStart = false;
			twoFingerDragStart = false;
		}
	}

	private int GetTwoFinger(int index)
	{
		int i = index + 1;
		bool flag = false;
		for (; i < 10; i++)
		{
			if (flag)
			{
				break;
			}
			if (fingers[i] != null && i >= index)
			{
				flag = true;
			}
		}
		return i - 1;
	}

	private void CreateStateEnd2Fingers(GestureType gesture, Vector2 startPosition, Vector2 position, Vector2 deltaPosition, float time, bool realEnd, float fingerDistance)
	{
		switch (gesture)
		{
		case GestureType.Tap:
			if (fingers[twoFinger0].tapCount < 2 && fingers[twoFinger1].tapCount < 2)
			{
				CreateGesture2Finger(EventName.On_SimpleTap2Fingers, startPosition, position, deltaPosition, time, SwipeType.None, 0f, Vector2.zero, 0f, 0f, fingerDistance);
			}
			else
			{
				CreateGesture2Finger(EventName.On_DoubleTap2Fingers, startPosition, position, deltaPosition, time, SwipeType.None, 0f, Vector2.zero, 0f, 0f, fingerDistance);
			}
			break;
		case GestureType.LongTap:
			CreateGesture2Finger(EventName.On_LongTapEnd2Fingers, startPosition, position, deltaPosition, time, SwipeType.None, 0f, Vector2.zero, 0f, 0f, fingerDistance);
			break;
		case GestureType.Pinch:
			CreateGesture2Finger(EventName.On_PinchEnd, startPosition, position, deltaPosition, time, SwipeType.None, 0f, Vector2.zero, 0f, 0f, fingerDistance);
			break;
		case GestureType.Twist:
			CreateGesture2Finger(EventName.On_TwistEnd, startPosition, position, deltaPosition, time, SwipeType.None, 0f, Vector2.zero, 0f, 0f, fingerDistance);
			break;
		}
		if (realEnd)
		{
			if (twoFingerDragStart)
			{
				CreateGesture2Finger(EventName.On_DragEnd2Fingers, startPosition, position, deltaPosition, time, GetSwipe(startPosition, position), (position - startPosition).magnitude, position - startPosition, 0f, 0f, fingerDistance);
			}
			if (twoFingerSwipeStart)
			{
				CreateGesture2Finger(EventName.On_SwipeEnd2Fingers, startPosition, position, deltaPosition, time, GetSwipe(startPosition, position), (position - startPosition).magnitude, position - startPosition, 0f, 0f, fingerDistance);
			}
			CreateGesture2Finger(EventName.On_TouchUp2Fingers, startPosition, position, deltaPosition, time, SwipeType.None, 0f, Vector2.zero, 0f, 0f, fingerDistance);
		}
	}

	private void CreateGesture2Finger(EventName message, Vector2 startPosition, Vector2 position, Vector2 deltaPosition, float actionTime, SwipeType swipe, float swipeLength, Vector2 swipeVector, float twist, float pinch, float twoDistance)
	{
		if (message == EventName.On_TouchStart2Fingers)
		{
			isStartHoverNGUI = (IsTouchHoverNGui(twoFinger1) & IsTouchHoverNGui(twoFinger0));
		}
		if (!isStartHoverNGUI)
		{
			Gesture gesture = new Gesture();
			gesture.touchCount = 2;
			gesture.fingerIndex = -1;
			gesture.startPosition = startPosition;
			gesture.position = position;
			gesture.deltaPosition = deltaPosition;
			gesture.actionTime = actionTime;
			if (fingers[twoFinger0] != null)
			{
				gesture.deltaTime = fingers[twoFinger0].deltaTime;
			}
			else if (fingers[twoFinger1] != null)
			{
				gesture.deltaTime = fingers[twoFinger1].deltaTime;
			}
			else
			{
				gesture.deltaTime = 0f;
			}
			gesture.swipe = swipe;
			gesture.swipeLength = swipeLength;
			gesture.swipeVector = swipeVector;
			gesture.deltaPinch = pinch;
			gesture.twistAngle = twist;
			gesture.twoFingerDistance = twoDistance;
			if (fingers[twoFinger0] != null)
			{
				gesture.pickCamera = fingers[twoFinger0].pickedCamera;
				gesture.isGuiCamera = fingers[twoFinger0].isGuiCamera;
			}
			else if (fingers[twoFinger1] != null)
			{
				gesture.pickCamera = fingers[twoFinger1].pickedCamera;
				gesture.isGuiCamera = fingers[twoFinger1].isGuiCamera;
			}
			if (message != EventName.On_Cancel2Fingers)
			{
				gesture.pickObject = pickObject2Finger;
			}
			else
			{
				gesture.pickObject = oldPickObject2Finger;
			}
			gesture.otherReceiver = receiverObject;
			if (fingers[twoFinger0] != null)
			{
				gesture.isHoverReservedArea = IsTouchReservedArea(fingers[twoFinger0].fingerIndex);
			}
			if (fingers[twoFinger1] != null)
			{
				gesture.isHoverReservedArea = (gesture.isHoverReservedArea || IsTouchReservedArea(fingers[twoFinger1].fingerIndex));
			}
			if (useBroadcastMessage)
			{
				SendGesture2Finger(message, gesture);
			}
			else
			{
				RaiseEvent(message, gesture);
			}
		}
	}

	private void SendGesture2Finger(EventName message, Gesture gesture)
	{
		if (receiverObject != null && receiverObject != gesture.pickObject)
		{
			receiverObject.SendMessage(message.ToString(), gesture, SendMessageOptions.DontRequireReceiver);
		}
		if (gesture.pickObject != null)
		{
			gesture.pickObject.SendMessage(message.ToString(), gesture, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			SendMessage(message.ToString(), gesture, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void RaiseReadyEvent()
	{
		if (useBroadcastMessage)
		{
			if (receiverObject != null)
			{
				base.gameObject.SendMessage("On_EasyTouchIsReady", SendMessageOptions.DontRequireReceiver);
			}
		}
		else if (EasyTouch.On_EasyTouchIsReady != null)
		{
			EasyTouch.On_EasyTouchIsReady();
		}
	}

	private void RaiseEvent(EventName evnt, Gesture gesture)
	{
		switch (evnt)
		{
		case EventName.On_Cancel:
			if (EasyTouch.On_Cancel != null)
			{
				EasyTouch.On_Cancel(gesture);
			}
			break;
		case EventName.On_Cancel2Fingers:
			if (EasyTouch.On_Cancel2Fingers != null)
			{
				EasyTouch.On_Cancel2Fingers(gesture);
			}
			break;
		case EventName.On_TouchStart:
			if (EasyTouch.On_TouchStart != null)
			{
				EasyTouch.On_TouchStart(gesture);
			}
			break;
		case EventName.On_TouchDown:
			if (EasyTouch.On_TouchDown != null)
			{
				EasyTouch.On_TouchDown(gesture);
			}
			break;
		case EventName.On_TouchUp:
			if (EasyTouch.On_TouchUp != null)
			{
				EasyTouch.On_TouchUp(gesture);
			}
			break;
		case EventName.On_SimpleTap:
			if (EasyTouch.On_SimpleTap != null)
			{
				EasyTouch.On_SimpleTap(gesture);
			}
			break;
		case EventName.On_DoubleTap:
			if (EasyTouch.On_DoubleTap != null)
			{
				EasyTouch.On_DoubleTap(gesture);
			}
			break;
		case EventName.On_LongTapStart:
			if (EasyTouch.On_LongTapStart != null)
			{
				EasyTouch.On_LongTapStart(gesture);
			}
			break;
		case EventName.On_LongTap:
			if (EasyTouch.On_LongTap != null)
			{
				EasyTouch.On_LongTap(gesture);
			}
			break;
		case EventName.On_LongTapEnd:
			if (EasyTouch.On_LongTapEnd != null)
			{
				EasyTouch.On_LongTapEnd(gesture);
			}
			break;
		case EventName.On_DragStart:
			if (EasyTouch.On_DragStart != null)
			{
				EasyTouch.On_DragStart(gesture);
			}
			break;
		case EventName.On_Drag:
			if (EasyTouch.On_Drag != null)
			{
				EasyTouch.On_Drag(gesture);
			}
			break;
		case EventName.On_DragEnd:
			if (EasyTouch.On_DragEnd != null)
			{
				EasyTouch.On_DragEnd(gesture);
			}
			break;
		case EventName.On_SwipeStart:
			if (EasyTouch.On_SwipeStart != null)
			{
				EasyTouch.On_SwipeStart(gesture);
			}
			break;
		case EventName.On_Swipe:
			if (EasyTouch.On_Swipe != null)
			{
				EasyTouch.On_Swipe(gesture);
			}
			break;
		case EventName.On_SwipeEnd:
			if (EasyTouch.On_SwipeEnd != null)
			{
				EasyTouch.On_SwipeEnd(gesture);
			}
			break;
		case EventName.On_TouchStart2Fingers:
			if (EasyTouch.On_TouchStart2Fingers != null)
			{
				EasyTouch.On_TouchStart2Fingers(gesture);
			}
			break;
		case EventName.On_TouchDown2Fingers:
			if (EasyTouch.On_TouchDown2Fingers != null)
			{
				EasyTouch.On_TouchDown2Fingers(gesture);
			}
			break;
		case EventName.On_TouchUp2Fingers:
			if (EasyTouch.On_TouchUp2Fingers != null)
			{
				EasyTouch.On_TouchUp2Fingers(gesture);
			}
			break;
		case EventName.On_SimpleTap2Fingers:
			if (EasyTouch.On_SimpleTap2Fingers != null)
			{
				EasyTouch.On_SimpleTap2Fingers(gesture);
			}
			break;
		case EventName.On_DoubleTap2Fingers:
			if (EasyTouch.On_DoubleTap2Fingers != null)
			{
				EasyTouch.On_DoubleTap2Fingers(gesture);
			}
			break;
		case EventName.On_LongTapStart2Fingers:
			if (EasyTouch.On_LongTapStart2Fingers != null)
			{
				EasyTouch.On_LongTapStart2Fingers(gesture);
			}
			break;
		case EventName.On_LongTap2Fingers:
			if (EasyTouch.On_LongTap2Fingers != null)
			{
				EasyTouch.On_LongTap2Fingers(gesture);
			}
			break;
		case EventName.On_LongTapEnd2Fingers:
			if (EasyTouch.On_LongTapEnd2Fingers != null)
			{
				EasyTouch.On_LongTapEnd2Fingers(gesture);
			}
			break;
		case EventName.On_Twist:
			if (EasyTouch.On_Twist != null)
			{
				EasyTouch.On_Twist(gesture);
			}
			break;
		case EventName.On_TwistEnd:
			if (EasyTouch.On_TwistEnd != null)
			{
				EasyTouch.On_TwistEnd(gesture);
			}
			break;
		case EventName.On_PinchIn:
			if (EasyTouch.On_PinchIn != null)
			{
				EasyTouch.On_PinchIn(gesture);
			}
			break;
		case EventName.On_PinchOut:
			if (EasyTouch.On_PinchOut != null)
			{
				EasyTouch.On_PinchOut(gesture);
			}
			break;
		case EventName.On_PinchEnd:
			if (EasyTouch.On_PinchEnd != null)
			{
				EasyTouch.On_PinchEnd(gesture);
			}
			break;
		case EventName.On_DragStart2Fingers:
			if (EasyTouch.On_DragStart2Fingers != null)
			{
				EasyTouch.On_DragStart2Fingers(gesture);
			}
			break;
		case EventName.On_Drag2Fingers:
			if (EasyTouch.On_Drag2Fingers != null)
			{
				EasyTouch.On_Drag2Fingers(gesture);
			}
			break;
		case EventName.On_DragEnd2Fingers:
			if (EasyTouch.On_DragEnd2Fingers != null)
			{
				EasyTouch.On_DragEnd2Fingers(gesture);
			}
			break;
		case EventName.On_SwipeStart2Fingers:
			if (EasyTouch.On_SwipeStart2Fingers != null)
			{
				EasyTouch.On_SwipeStart2Fingers(gesture);
			}
			break;
		case EventName.On_Swipe2Fingers:
			if (EasyTouch.On_Swipe2Fingers != null)
			{
				EasyTouch.On_Swipe2Fingers(gesture);
			}
			break;
		case EventName.On_SwipeEnd2Fingers:
			if (EasyTouch.On_SwipeEnd2Fingers != null)
			{
				EasyTouch.On_SwipeEnd2Fingers(gesture);
			}
			break;
		}
	}

	private bool GetPickeGameObject(ref Finger finger, bool twoFinger = false)
	{
		finger.isGuiCamera = false;
		finger.pickedCamera = null;
		finger.pickedObject = null;
		if (touchCameras.Count > 0)
		{
			for (int i = 0; i < touchCameras.Count; i++)
			{
				if (!(touchCameras[i].camera != null) || !touchCameras[i].camera.enabled)
				{
					continue;
				}
				Vector2 zero = Vector2.zero;
				zero = (twoFinger ? finger.complexStartPosition : finger.startPosition);
				Ray ray = touchCameras[i].camera.ScreenPointToRay(zero);
				if (enable2D)
				{
					LayerMask mask = pickableLayers2D;
					RaycastHit2D[] array = new RaycastHit2D[1];
					if (Physics2D.GetRayIntersectionNonAlloc(ray, array, float.PositiveInfinity, mask) > 0)
					{
						finger.pickedCamera = touchCameras[i].camera;
						finger.isGuiCamera = touchCameras[i].guiCamera;
						finger.pickedObject = array[0].collider.gameObject;
						return true;
					}
				}
				LayerMask mask2 = pickableLayers;
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, float.MaxValue, mask2))
				{
					finger.pickedCamera = touchCameras[i].camera;
					finger.isGuiCamera = touchCameras[i].guiCamera;
					finger.pickedObject = hitInfo.collider.gameObject;
					return true;
				}
			}
		}
		else
		{
			Debug.LogWarning("No camera is assigned to EasyTouch");
		}
		return false;
	}

	private SwipeType GetSwipe(Vector2 start, Vector2 end)
	{
		Vector2 normalized = (end - start).normalized;
		if (Mathf.Abs(normalized.y) > Mathf.Abs(normalized.x))
		{
			if (Vector2.Dot(normalized, Vector2.up) >= swipeTolerance)
			{
				return SwipeType.Up;
			}
			if (Vector2.Dot(normalized, -Vector2.up) >= swipeTolerance)
			{
				return SwipeType.Down;
			}
		}
		else
		{
			if (Vector2.Dot(normalized, Vector2.right) >= swipeTolerance)
			{
				return SwipeType.Right;
			}
			if (Vector2.Dot(normalized, -Vector2.right) >= swipeTolerance)
			{
				return SwipeType.Left;
			}
		}
		return SwipeType.Other;
	}

	private bool FingerInTolerance(Finger finger)
	{
		if ((finger.position - finger.startPosition).sqrMagnitude <= StationnaryTolerance * StationnaryTolerance)
		{
			return true;
		}
		return false;
	}

	private float DeltaAngle(Vector2 start, Vector2 end)
	{
		float y = start.x * end.y - start.y * end.x;
		return Mathf.Atan2(y, Vector2.Dot(start, end));
	}

	private float TwistAngle()
	{
		Vector2 end = fingers[twoFinger0].position - fingers[twoFinger1].position;
		Vector2 start = fingers[twoFinger0].oldPosition - fingers[twoFinger1].oldPosition;
		return 57.29578f * DeltaAngle(start, end);
	}

	private bool IsTouchHoverNGui(int touchIndex)
	{
		bool flag = false;
		if (enabledNGuiMode)
		{
			LayerMask mask = nGUILayers;
			int num = 0;
			while (!flag && num < nGUICameras.Count)
			{
				Ray ray = nGUICameras[num].ScreenPointToRay(fingers[touchIndex].position);
				RaycastHit hitInfo;
				flag = Physics.Raycast(ray, out hitInfo, float.MaxValue, mask);
				num++;
			}
		}
		return flag;
	}

	private bool IsTouchReservedArea(int touchIndex)
	{
		bool flag = false;
		if (enableReservedArea)
		{
			int num = 0;
			Rect rect = new Rect(0f, 0f, 0f, 0f);
			while (!flag && num < reservedAreas.Count)
			{
				flag = reservedAreas[num].Contains(fingers[touchIndex].position);
				num++;
			}
			num = 0;
			while (!flag && num < reservedGuiAreas.Count)
			{
				rect = new Rect(reservedGuiAreas[num].x, (float)Screen.height - reservedGuiAreas[num].y - reservedGuiAreas[num].height, reservedGuiAreas[num].width, reservedGuiAreas[num].height);
				flag = rect.Contains(fingers[touchIndex].position);
				num++;
			}
			num = 0;
			while (!flag && num < reservedVirtualAreas.Count)
			{
				rect = VirtualScreen.GetRealRect(reservedVirtualAreas[num]);
				flag = new Rect(rect.x, (float)Screen.height - rect.y - rect.height, rect.width, rect.height).Contains(fingers[touchIndex].position);
				num++;
			}
		}
		return flag;
	}

	private Finger GetFinger(int finderId)
	{
		int i = 0;
		Finger finger = null;
		for (; i < 10; i++)
		{
			if (finger != null)
			{
				break;
			}
			if (fingers[i] != null && fingers[i].fingerIndex == finderId)
			{
				finger = fingers[i];
			}
		}
		return finger;
	}

	public static void SetEnabled(bool enable)
	{
		instance.enable = enable;
		if (enable)
		{
			instance.ResetTouches();
		}
	}

	public static bool GetEnabled()
	{
		return instance.enable;
	}

	public static int GetTouchCount()
	{
		return instance.input.TouchCount();
	}

	public static void SetCamera(Camera cam, bool guiCam = false)
	{
		instance.touchCameras.Add(new ECamera(cam, guiCam));
	}

	public static Camera GetCamera(int index = 0)
	{
		if (index < instance.touchCameras.Count)
		{
			return instance.touchCameras[index].camera;
		}
		return null;
	}

	public static void SetEnable2FingersGesture(bool enable)
	{
		instance.enable2FingersGesture = enable;
	}

	public static bool GetEnable2FingersGesture()
	{
		return instance.enable2FingersGesture;
	}

	public static void SetEnableTwist(bool enable)
	{
		instance.enableTwist = enable;
	}

	public static bool GetEnableTwist()
	{
		return instance.enableTwist;
	}

	public static void SetEnablePinch(bool enable)
	{
		instance.enablePinch = enable;
	}

	public static bool GetEnablePinch()
	{
		return instance.enablePinch;
	}

	public static void SetEnableAutoSelect(bool enable)
	{
		instance.autoSelect = enable;
	}

	public static bool GetEnableAutoSelect()
	{
		return instance.autoSelect;
	}

	public static void SetOtherReceiverObject(GameObject receiver)
	{
		instance.receiverObject = receiver;
	}

	public static GameObject GetOtherReceiverObject()
	{
		return instance.receiverObject;
	}

	public static void SetStationnaryTolerance(float tolerance)
	{
		instance.StationnaryTolerance = tolerance;
	}

	public static float GetStationnaryTolerance()
	{
		return instance.StationnaryTolerance;
	}

	public static void SetlongTapTime(float time)
	{
		instance.longTapTime = time;
	}

	public static float GetlongTapTime()
	{
		return instance.longTapTime;
	}

	public static void SetSwipeTolerance(float tolerance)
	{
		instance.swipeTolerance = tolerance;
	}

	public static float GetSwipeTolerance()
	{
		return instance.swipeTolerance;
	}

	public static void SetMinPinchLength(float length)
	{
		instance.minPinchLength = length;
	}

	public static float GetMinPinchLength()
	{
		return instance.minPinchLength;
	}

	public static void SetMinTwistAngle(float angle)
	{
		instance.minTwistAngle = angle;
	}

	public static float GetMinTwistAngle()
	{
		return instance.minTwistAngle;
	}

	public static GameObject GetCurrentPickedObject(int fingerIndex)
	{
		Finger finger = instance.GetFinger(fingerIndex);
		if (instance.GetPickeGameObject(ref finger))
		{
			return finger.pickedObject;
		}
		return null;
	}

	public static bool IsRectUnderTouch(Rect rect, bool guiRect = false)
	{
		bool flag = false;
		for (int i = 0; i < 10; i++)
		{
			if (instance.fingers[i] != null)
			{
				if (guiRect)
				{
					rect = new Rect(rect.x, (float)Screen.height - rect.y - rect.height, rect.width, rect.height);
				}
				flag = rect.Contains(instance.fingers[i].position);
				if (flag)
				{
					break;
				}
			}
		}
		return flag;
	}

	public static Vector2 GetFingerPosition(int fingerIndex)
	{
		if (instance.fingers[fingerIndex] != null)
		{
			return instance.GetFinger(fingerIndex).position;
		}
		return Vector2.zero;
	}

	public static bool GetIsReservedArea()
	{
		if ((bool)instance)
		{
			return instance.enableReservedArea;
		}
		return false;
	}

	public static void SetIsReservedArea(bool enable)
	{
		instance.enableReservedArea = enable;
	}

	public static void AddReservedArea(Rect rec)
	{
		if ((bool)instance)
		{
			instance.reservedAreas.Add(rec);
		}
	}

	public static void AddReservedGuiArea(Rect rec)
	{
		if ((bool)instance)
		{
			instance.reservedGuiAreas.Add(rec);
		}
	}

	public static void RemoveReservedArea(Rect rec)
	{
		if ((bool)instance)
		{
			instance.reservedAreas.Remove(rec);
		}
	}

	public static void RemoveReservedGuiArea(Rect rec)
	{
		if ((bool)instance)
		{
			instance.reservedGuiAreas.Remove(rec);
		}
	}

	public static void ResetTouch(int fingerIndex)
	{
		if ((bool)instance)
		{
			instance.GetFinger(fingerIndex).gesture = GestureType.None;
		}
	}

	public static void SetPickableLayer(LayerMask mask)
	{
		if ((bool)instance)
		{
			instance.pickableLayers = mask;
		}
	}

	public static LayerMask GetPickableLayer()
	{
		return instance.pickableLayers;
	}
}
