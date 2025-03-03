using UnityEngine;

[ExecuteInEditMode]
public class EasyJoystick : MonoBehaviour
{
	public delegate void JoystickMoveStartHandler(MovingJoystick move);

	public delegate void JoystickMoveHandler(MovingJoystick move);

	public delegate void JoystickMoveEndHandler(MovingJoystick move);

	public delegate void JoystickTouchStartHandler(MovingJoystick move);

	public delegate void JoystickTapHandler(MovingJoystick move);

	public delegate void JoystickDoubleTapHandler(MovingJoystick move);

	public delegate void JoystickTouchUpHandler(MovingJoystick move);

	public enum JoystickAnchor
	{
		None,
		UpperLeft,
		UpperCenter,
		UpperRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		LowerLeft,
		LowerCenter,
		LowerRight
	}

	public enum PropertiesInfluenced
	{
		Rotate,
		RotateLocal,
		Translate,
		TranslateLocal,
		Scale
	}

	public enum AxisInfluenced
	{
		X,
		Y,
		Z,
		XYZ
	}

	public enum DynamicArea
	{
		FullScreen,
		Left,
		Right,
		Top,
		Bottom,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	public enum InteractionType
	{
		Direct,
		Include,
		EventNotification,
		DirectAndEvent
	}

	public enum Broadcast
	{
		SendMessage,
		SendMessageUpwards,
		BroadcastMessage
	}

	private enum MessageName
	{
		On_JoystickMoveStart,
		On_JoystickTouchStart,
		On_JoystickTouchUp,
		On_JoystickMove,
		On_JoystickMoveEnd,
		On_JoystickTap,
		On_JoystickDoubleTap
	}

	private Vector2 joystickAxis;

	private Vector2 joystickTouch;

	private Vector2 joystickValue;

	public bool enable = true;

	public bool visible = true;

	public bool isActivated = true;

	public bool showDebugRadius = false;

	public bool selected = false;

	public bool useFixedUpdate = false;

	public bool isUseGuiLayout = true;

	[SerializeField]
	private bool dynamicJoystick = false;

	public DynamicArea area = DynamicArea.FullScreen;

	[SerializeField]
	private JoystickAnchor joyAnchor = JoystickAnchor.LowerLeft;

	[SerializeField]
	private Vector2 joystickPositionOffset = Vector2.zero;

	[SerializeField]
	private float zoneRadius = 100f;

	[SerializeField]
	private float touchSize = 30f;

	public float deadZone = 20f;

	[SerializeField]
	private bool restrictArea = false;

	public bool resetFingerExit = false;

	[SerializeField]
	private InteractionType interaction = InteractionType.Direct;

	public bool useBroadcast = false;

	public Broadcast messageMode;

	public GameObject receiverGameObject;

	public Vector2 speed;

	public bool enableXaxis = true;

	[SerializeField]
	private Transform xAxisTransform;

	public CharacterController xAxisCharacterController;

	public float xAxisGravity = 0f;

	[SerializeField]
	private PropertiesInfluenced xTI;

	public AxisInfluenced xAI;

	public bool inverseXAxis = false;

	public bool enableXClamp = false;

	public float clampXMax;

	public float clampXMin;

	public bool enableXAutoStab = false;

	[SerializeField]
	private float thresholdX = 0.01f;

	[SerializeField]
	private float stabSpeedX = 20f;

	public bool enableYaxis = true;

	[SerializeField]
	private Transform yAxisTransform;

	public CharacterController yAxisCharacterController;

	public float yAxisGravity = 0f;

	[SerializeField]
	private PropertiesInfluenced yTI;

	public AxisInfluenced yAI;

	public bool inverseYAxis = false;

	public bool enableYClamp = false;

	public float clampYMax;

	public float clampYMin;

	public bool enableYAutoStab = false;

	[SerializeField]
	private float thresholdY = 0.01f;

	[SerializeField]
	private float stabSpeedY = 20f;

	public bool enableSmoothing = false;

	[SerializeField]
	public Vector2 smoothing = new Vector2(2f, 2f);

	public bool enableInertia = false;

	[SerializeField]
	public Vector2 inertia = new Vector2(100f, 100f);

	public int guiDepth = 0;

	public bool showZone = true;

	public bool showTouch = true;

	public bool showDeadZone = true;

	public Texture areaTexture;

	public Color areaColor = Color.white;

	public Texture touchTexture;

	public Color touchColor = Color.white;

	public Texture deadTexture;

	public bool showProperties = true;

	public bool showInteraction = false;

	public bool showAppearance = false;

	public bool showPosition = true;

	private Vector2 joystickCenter;

	private Rect areaRect;

	private Rect deadRect;

	private Vector2 anchorPosition = Vector2.zero;

	private bool virtualJoystick = true;

	private int joystickIndex = -1;

	private float touchSizeCoef = 0f;

	private bool sendEnd = true;

	private float startXLocalAngle = 0f;

	private float startYLocalAngle = 0f;

	public Vector2 JoystickAxis
	{
		get
		{
			return joystickAxis;
		}
	}

	public Vector2 JoystickTouch
	{
		get
		{
			return new Vector2(joystickTouch.x / zoneRadius, joystickTouch.y / zoneRadius);
		}
		set
		{
			float x = Mathf.Clamp(value.x, -1f, 1f) * zoneRadius;
			float y = Mathf.Clamp(value.y, -1f, 1f) * zoneRadius;
			joystickTouch = new Vector2(x, y);
		}
	}

	public Vector2 JoystickValue
	{
		get
		{
			return joystickValue;
		}
	}

	public bool DynamicJoystick
	{
		get
		{
			return dynamicJoystick;
		}
		set
		{
			if (!Application.isPlaying)
			{
				joystickIndex = -1;
				dynamicJoystick = value;
				if (dynamicJoystick)
				{
					virtualJoystick = false;
					return;
				}
				virtualJoystick = true;
				joystickCenter = joystickPositionOffset;
			}
		}
	}

	public JoystickAnchor JoyAnchor
	{
		get
		{
			return joyAnchor;
		}
		set
		{
			joyAnchor = value;
			ComputeJoystickAnchor(joyAnchor);
		}
	}

	public Vector2 JoystickPositionOffset
	{
		get
		{
			return joystickPositionOffset;
		}
		set
		{
			joystickPositionOffset = value;
			joystickCenter = joystickPositionOffset;
			ComputeJoystickAnchor(joyAnchor);
		}
	}

	public float ZoneRadius
	{
		get
		{
			return zoneRadius;
		}
		set
		{
			zoneRadius = value;
			ComputeJoystickAnchor(joyAnchor);
		}
	}

	public float TouchSize
	{
		get
		{
			return touchSize;
		}
		set
		{
			touchSize = value;
			if (touchSize > zoneRadius / 2f && restrictArea)
			{
				touchSize = zoneRadius / 2f;
			}
			ComputeJoystickAnchor(joyAnchor);
		}
	}

	public bool RestrictArea
	{
		get
		{
			return restrictArea;
		}
		set
		{
			restrictArea = value;
			if (restrictArea)
			{
				touchSizeCoef = touchSize;
			}
			else
			{
				touchSizeCoef = 0f;
			}
			ComputeJoystickAnchor(joyAnchor);
		}
	}

	public InteractionType Interaction
	{
		get
		{
			return interaction;
		}
		set
		{
			interaction = value;
			if (interaction == InteractionType.Direct || interaction == InteractionType.Include)
			{
				useBroadcast = false;
			}
		}
	}

	public Transform XAxisTransform
	{
		get
		{
			return xAxisTransform;
		}
		set
		{
			xAxisTransform = value;
			if (xAxisTransform != null)
			{
				xAxisCharacterController = xAxisTransform.GetComponent<CharacterController>();
				return;
			}
			xAxisCharacterController = null;
			xAxisGravity = 0f;
		}
	}

	public PropertiesInfluenced XTI
	{
		get
		{
			return xTI;
		}
		set
		{
			xTI = value;
			if (xTI != PropertiesInfluenced.RotateLocal)
			{
				enableXAutoStab = false;
				enableXClamp = false;
			}
		}
	}

	public float ThresholdX
	{
		get
		{
			return thresholdX;
		}
		set
		{
			if (value <= 0f)
			{
				thresholdX = value * -1f;
			}
			else
			{
				thresholdX = value;
			}
		}
	}

	public float StabSpeedX
	{
		get
		{
			return stabSpeedX;
		}
		set
		{
			if (value <= 0f)
			{
				stabSpeedX = value * -1f;
			}
			else
			{
				stabSpeedX = value;
			}
		}
	}

	public Transform YAxisTransform
	{
		get
		{
			return yAxisTransform;
		}
		set
		{
			yAxisTransform = value;
			if (yAxisTransform != null)
			{
				yAxisCharacterController = yAxisTransform.GetComponent<CharacterController>();
				return;
			}
			yAxisCharacterController = null;
			yAxisGravity = 0f;
		}
	}

	public PropertiesInfluenced YTI
	{
		get
		{
			return yTI;
		}
		set
		{
			yTI = value;
			if (yTI != PropertiesInfluenced.RotateLocal)
			{
				enableYAutoStab = false;
				enableYClamp = false;
			}
		}
	}

	public float ThresholdY
	{
		get
		{
			return thresholdY;
		}
		set
		{
			if (value <= 0f)
			{
				thresholdY = value * -1f;
			}
			else
			{
				thresholdY = value;
			}
		}
	}

	public float StabSpeedY
	{
		get
		{
			return stabSpeedY;
		}
		set
		{
			if (value <= 0f)
			{
				stabSpeedY = value * -1f;
			}
			else
			{
				stabSpeedY = value;
			}
		}
	}

	public Vector2 Smoothing
	{
		get
		{
			return smoothing;
		}
		set
		{
			smoothing = value;
			if (smoothing.x < 0f)
			{
				smoothing.x = 0f;
			}
			if (smoothing.y < 0f)
			{
				smoothing.y = 0f;
			}
		}
	}

	public Vector2 Inertia
	{
		get
		{
			return inertia;
		}
		set
		{
			inertia = value;
			if (inertia.x <= 0f)
			{
				inertia.x = 1f;
			}
			if (inertia.y <= 0f)
			{
				inertia.y = 1f;
			}
		}
	}

	public static event JoystickMoveStartHandler On_JoystickMoveStart;

	public static event JoystickMoveHandler On_JoystickMove;

	public static event JoystickMoveEndHandler On_JoystickMoveEnd;

	public static event JoystickTouchStartHandler On_JoystickTouchStart;

	public static event JoystickTapHandler On_JoystickTap;

	public static event JoystickDoubleTapHandler On_JoystickDoubleTap;

	public static event JoystickTouchUpHandler On_JoystickTouchUp;

	private void OnLevelWasLoaded()
	{
		joystickIndex = -1;
	}

	private void OnEnable()
	{
		EasyTouch.On_TouchStart += On_TouchStart;
		EasyTouch.On_TouchUp += On_TouchUp;
		EasyTouch.On_TouchDown += On_TouchDown;
		EasyTouch.On_SimpleTap += On_SimpleTap;
		EasyTouch.On_DoubleTap += On_DoubleTap;
	}

	private void OnDisable()
	{
		EasyTouch.On_TouchStart -= On_TouchStart;
		EasyTouch.On_TouchUp -= On_TouchUp;
		EasyTouch.On_TouchDown -= On_TouchDown;
		EasyTouch.On_SimpleTap -= On_SimpleTap;
		EasyTouch.On_DoubleTap -= On_DoubleTap;
		if (Application.isPlaying && EasyTouch.instance != null)
		{
			EasyTouch.instance.reservedVirtualAreas.Remove(areaRect);
		}
	}

	private void OnDestroy()
	{
		EasyTouch.On_TouchStart -= On_TouchStart;
		EasyTouch.On_TouchUp -= On_TouchUp;
		EasyTouch.On_TouchDown -= On_TouchDown;
		EasyTouch.On_SimpleTap -= On_SimpleTap;
		EasyTouch.On_DoubleTap -= On_DoubleTap;
		if (Application.isPlaying && EasyTouch.instance != null)
		{
			EasyTouch.instance.reservedVirtualAreas.Remove(areaRect);
		}
	}

	private void Start()
	{
		if (!dynamicJoystick)
		{
			joystickCenter = joystickPositionOffset;
			ComputeJoystickAnchor(joyAnchor);
			virtualJoystick = true;
		}
		else
		{
			virtualJoystick = false;
		}
		VirtualScreen.ComputeVirtualScreen();
		startXLocalAngle = GetStartAutoStabAngle(xAxisTransform, xAI);
		startYLocalAngle = GetStartAutoStabAngle(yAxisTransform, yAI);
	}

	private void Update()
	{
		if (!useFixedUpdate && enable)
		{
			UpdateJoystick();
		}
	}

	private void FixedUpdate()
	{
		if (useFixedUpdate && enable)
		{
			UpdateJoystick();
		}
	}

	private void UpdateJoystick()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (EasyTouch.GetTouchCount() == 0)
		{
			joystickIndex = -1;
			if (dynamicJoystick)
			{
				virtualJoystick = false;
			}
		}
		if (!isActivated)
		{
			return;
		}
		if (joystickIndex == -1 || (joystickAxis == Vector2.zero && joystickIndex > -1))
		{
			if (enableXAutoStab)
			{
				DoAutoStabilisation(xAxisTransform, xAI, thresholdX, stabSpeedX, startXLocalAngle);
			}
			if (enableYAutoStab)
			{
				DoAutoStabilisation(yAxisTransform, yAI, thresholdY, stabSpeedY, startYLocalAngle);
			}
		}
		if (!dynamicJoystick)
		{
			joystickCenter = joystickPositionOffset;
		}
		if (joystickIndex == -1)
		{
			if (!enableSmoothing)
			{
				joystickTouch = Vector2.zero;
			}
			else if ((double)joystickTouch.sqrMagnitude > 0.0001)
			{
				joystickTouch = new Vector2(joystickTouch.x - joystickTouch.x * smoothing.x * Time.deltaTime, joystickTouch.y - joystickTouch.y * smoothing.y * Time.deltaTime);
			}
			else
			{
				joystickTouch = Vector2.zero;
			}
		}
		Vector2 lhs = new Vector2(joystickAxis.x, joystickAxis.y);
		float num = ComputeDeadZone();
		joystickAxis = new Vector2(joystickTouch.x * num, joystickTouch.y * num);
		if (inverseXAxis)
		{
			joystickAxis.x *= -1f;
		}
		if (inverseYAxis)
		{
			joystickAxis.y *= -1f;
		}
		Vector2 a = new Vector2(speed.x * joystickAxis.x, speed.y * joystickAxis.y);
		if (enableInertia)
		{
			Vector2 vector = a - joystickValue;
			vector.x /= inertia.x;
			vector.y /= inertia.y;
			joystickValue += vector;
		}
		else
		{
			joystickValue = a;
		}
		if (lhs == Vector2.zero && joystickAxis != Vector2.zero && interaction != 0 && interaction != InteractionType.Include)
		{
			CreateEvent(MessageName.On_JoystickMoveStart);
		}
		UpdateGravity();
		if (joystickAxis != Vector2.zero)
		{
			sendEnd = false;
			switch (interaction)
			{
			case InteractionType.Include:
				break;
			case InteractionType.Direct:
				UpdateDirect();
				break;
			case InteractionType.EventNotification:
				CreateEvent(MessageName.On_JoystickMove);
				break;
			case InteractionType.DirectAndEvent:
				UpdateDirect();
				CreateEvent(MessageName.On_JoystickMove);
				break;
			}
		}
		else if (!sendEnd)
		{
			CreateEvent(MessageName.On_JoystickMoveEnd);
			sendEnd = true;
		}
	}

	private void OnGUI()
	{
		if (enable && visible)
		{
			GUI.depth = guiDepth;
			base.useGUILayout = isUseGuiLayout;
			if (dynamicJoystick && Application.isEditor && !Application.isPlaying)
			{
				switch (area)
				{
				case DynamicArea.Bottom:
					ComputeJoystickAnchor(JoystickAnchor.LowerCenter);
					break;
				case DynamicArea.BottomLeft:
					ComputeJoystickAnchor(JoystickAnchor.LowerLeft);
					break;
				case DynamicArea.BottomRight:
					ComputeJoystickAnchor(JoystickAnchor.LowerRight);
					break;
				case DynamicArea.FullScreen:
					ComputeJoystickAnchor(JoystickAnchor.MiddleCenter);
					break;
				case DynamicArea.Left:
					ComputeJoystickAnchor(JoystickAnchor.MiddleLeft);
					break;
				case DynamicArea.Right:
					ComputeJoystickAnchor(JoystickAnchor.MiddleRight);
					break;
				case DynamicArea.Top:
					ComputeJoystickAnchor(JoystickAnchor.UpperCenter);
					break;
				case DynamicArea.TopLeft:
					ComputeJoystickAnchor(JoystickAnchor.UpperLeft);
					break;
				case DynamicArea.TopRight:
					ComputeJoystickAnchor(JoystickAnchor.UpperRight);
					break;
				}
			}
			if (Application.isEditor && !Application.isPlaying)
			{
				VirtualScreen.ComputeVirtualScreen();
				ComputeJoystickAnchor(joyAnchor);
			}
			VirtualScreen.SetGuiScaleMatrix();
			if ((showZone && areaTexture != null && !dynamicJoystick) || (showZone && dynamicJoystick && virtualJoystick && areaTexture != null) || (dynamicJoystick && Application.isEditor && !Application.isPlaying))
			{
				if (isActivated)
				{
					GUI.color = areaColor;
					if (Application.isPlaying && !dynamicJoystick)
					{
						EasyTouch.instance.reservedVirtualAreas.Remove(areaRect);
						EasyTouch.instance.reservedVirtualAreas.Add(areaRect);
					}
				}
				else
				{
					GUI.color = new Color(areaColor.r, areaColor.g, areaColor.b, 0.2f);
					if (Application.isPlaying && !dynamicJoystick)
					{
						EasyTouch.instance.reservedVirtualAreas.Remove(areaRect);
					}
				}
				if (showDebugRadius && Application.isEditor && selected && !Application.isPlaying)
				{
					GUI.Box(areaRect, "");
				}
				GUI.DrawTexture(areaRect, areaTexture, ScaleMode.StretchToFill, true);
			}
			if ((showTouch && touchTexture != null && !dynamicJoystick) || (showTouch && dynamicJoystick && virtualJoystick && touchTexture != null) || (dynamicJoystick && Application.isEditor && !Application.isPlaying))
			{
				if (isActivated)
				{
					GUI.color = touchColor;
				}
				else
				{
					GUI.color = new Color(touchColor.r, touchColor.g, touchColor.b, 0.2f);
				}
				GUI.DrawTexture(new Rect(anchorPosition.x + joystickCenter.x + (joystickTouch.x - touchSize), anchorPosition.y + joystickCenter.y - (joystickTouch.y + touchSize), touchSize * 2f, touchSize * 2f), touchTexture, ScaleMode.ScaleToFit, true);
			}
			if ((showDeadZone && deadTexture != null && !dynamicJoystick) || (showDeadZone && dynamicJoystick && virtualJoystick && deadTexture != null) || (dynamicJoystick && Application.isEditor && !Application.isPlaying))
			{
				GUI.DrawTexture(deadRect, deadTexture, ScaleMode.ScaleToFit, true);
			}
			GUI.color = Color.white;
		}
		else if (Application.isPlaying)
		{
			EasyTouch.instance.reservedVirtualAreas.Remove(areaRect);
		}
	}

	private void OnDrawGizmos()
	{
	}

	private void CreateEvent(MessageName message)
	{
		MovingJoystick movingJoystick = new MovingJoystick();
		movingJoystick.joystickName = base.gameObject.name;
		movingJoystick.joystickAxis = joystickAxis;
		movingJoystick.joystickValue = joystickValue;
		movingJoystick.joystick = this;
		if (!useBroadcast)
		{
			switch (message)
			{
			case MessageName.On_JoystickMoveStart:
				if (EasyJoystick.On_JoystickMoveStart != null)
				{
					EasyJoystick.On_JoystickMoveStart(movingJoystick);
				}
				break;
			case MessageName.On_JoystickMove:
				if (EasyJoystick.On_JoystickMove != null)
				{
					EasyJoystick.On_JoystickMove(movingJoystick);
				}
				break;
			case MessageName.On_JoystickMoveEnd:
				if (EasyJoystick.On_JoystickMoveEnd != null)
				{
					EasyJoystick.On_JoystickMoveEnd(movingJoystick);
				}
				break;
			case MessageName.On_JoystickTouchStart:
				if (EasyJoystick.On_JoystickTouchStart != null)
				{
					EasyJoystick.On_JoystickTouchStart(movingJoystick);
				}
				break;
			case MessageName.On_JoystickTap:
				if (EasyJoystick.On_JoystickTap != null)
				{
					EasyJoystick.On_JoystickTap(movingJoystick);
				}
				break;
			case MessageName.On_JoystickDoubleTap:
				if (EasyJoystick.On_JoystickDoubleTap != null)
				{
					EasyJoystick.On_JoystickDoubleTap(movingJoystick);
				}
				break;
			case MessageName.On_JoystickTouchUp:
				if (EasyJoystick.On_JoystickTouchUp != null)
				{
					EasyJoystick.On_JoystickTouchUp(movingJoystick);
				}
				break;
			}
		}
		else
		{
			if (!useBroadcast)
			{
				return;
			}
			if (receiverGameObject != null)
			{
				switch (messageMode)
				{
				case Broadcast.BroadcastMessage:
					receiverGameObject.BroadcastMessage(message.ToString(), movingJoystick, SendMessageOptions.DontRequireReceiver);
					break;
				case Broadcast.SendMessage:
					receiverGameObject.SendMessage(message.ToString(), movingJoystick, SendMessageOptions.DontRequireReceiver);
					break;
				case Broadcast.SendMessageUpwards:
					receiverGameObject.SendMessageUpwards(message.ToString(), movingJoystick, SendMessageOptions.DontRequireReceiver);
					break;
				}
			}
			else
			{
				Debug.LogError("Joystick : " + base.gameObject.name + " : you must setup receiver gameobject");
			}
		}
	}

	private void UpdateDirect()
	{
		if (xAxisTransform != null)
		{
			Vector3 influencedAxis = GetInfluencedAxis(xAI);
			DoActionDirect(xAxisTransform, xTI, influencedAxis, joystickValue.x, xAxisCharacterController);
			if (enableXClamp && xTI == PropertiesInfluenced.RotateLocal)
			{
				DoAngleLimitation(xAxisTransform, xAI, clampXMin, clampXMax, startXLocalAngle);
			}
		}
		if (YAxisTransform != null)
		{
			Vector3 influencedAxis = GetInfluencedAxis(yAI);
			DoActionDirect(yAxisTransform, yTI, influencedAxis, joystickValue.y, yAxisCharacterController);
			if (enableYClamp && yTI == PropertiesInfluenced.RotateLocal)
			{
				DoAngleLimitation(yAxisTransform, yAI, clampYMin, clampYMax, startYLocalAngle);
			}
		}
	}

	private void UpdateGravity()
	{
		if (joystickAxis == Vector2.zero)
		{
			if (xAxisCharacterController != null && xAxisGravity > 0f)
			{
				xAxisCharacterController.Move(Vector3.down * xAxisGravity * Time.deltaTime);
			}
			if (yAxisCharacterController != null && yAxisGravity > 0f)
			{
				yAxisCharacterController.Move(Vector3.down * yAxisGravity * Time.deltaTime);
			}
		}
	}

	private Vector3 GetInfluencedAxis(AxisInfluenced axisInfluenced)
	{
		Vector3 result = Vector3.zero;
		switch (axisInfluenced)
		{
		case AxisInfluenced.X:
			result = Vector3.right;
			break;
		case AxisInfluenced.Y:
			result = Vector3.up;
			break;
		case AxisInfluenced.Z:
			result = Vector3.forward;
			break;
		case AxisInfluenced.XYZ:
			result = Vector3.one;
			break;
		}
		return result;
	}

	private void DoActionDirect(Transform axisTransform, PropertiesInfluenced inlfuencedProperty, Vector3 axis, float sensibility, CharacterController charact)
	{
		Vector3 a;
		switch (inlfuencedProperty)
		{
		case PropertiesInfluenced.Rotate:
			axisTransform.Rotate(axis * sensibility * Time.deltaTime, Space.World);
			break;
		case PropertiesInfluenced.RotateLocal:
			axisTransform.Rotate(axis * sensibility * Time.deltaTime, Space.Self);
			break;
		case PropertiesInfluenced.Translate:
			if (charact == null)
			{
				axisTransform.Translate(axis * sensibility * Time.deltaTime, Space.World);
				break;
			}
			a = new Vector3(axis.x, axis.y, axis.z);
			a.y = 0f - (yAxisGravity + xAxisGravity);
			charact.Move(a * sensibility * Time.deltaTime);
			break;
		case PropertiesInfluenced.TranslateLocal:
			if (charact == null)
			{
				axisTransform.Translate(axis * sensibility * Time.deltaTime, Space.Self);
				break;
			}
			a = charact.transform.TransformDirection(axis) * sensibility;
			a.y = 0f - (yAxisGravity + xAxisGravity);
			charact.Move(a * Time.deltaTime);
			break;
		case PropertiesInfluenced.Scale:
			axisTransform.localScale += axis * sensibility * Time.deltaTime;
			break;
		}
	}

	private void DoAngleLimitation(Transform axisTransform, AxisInfluenced axisInfluenced, float clampMin, float clampMax, float startAngle)
	{
		float num = 0f;
		switch (axisInfluenced)
		{
		case AxisInfluenced.X:
			num = axisTransform.localRotation.eulerAngles.x;
			break;
		case AxisInfluenced.Y:
			num = axisTransform.localRotation.eulerAngles.y;
			break;
		case AxisInfluenced.Z:
			num = axisTransform.localRotation.eulerAngles.z;
			break;
		}
		if (num <= 360f && num >= 180f)
		{
			num -= 360f;
		}
		num = Mathf.Clamp(num, 0f - clampMax, clampMin);
		switch (axisInfluenced)
		{
		case AxisInfluenced.X:
			axisTransform.localEulerAngles = new Vector3(num, axisTransform.localEulerAngles.y, axisTransform.localEulerAngles.z);
			break;
		case AxisInfluenced.Y:
			axisTransform.localEulerAngles = new Vector3(axisTransform.localEulerAngles.x, num, axisTransform.localEulerAngles.z);
			break;
		case AxisInfluenced.Z:
			axisTransform.localEulerAngles = new Vector3(axisTransform.localEulerAngles.x, axisTransform.localEulerAngles.y, num);
			break;
		}
	}

	private void DoAutoStabilisation(Transform axisTransform, AxisInfluenced axisInfluenced, float threshold, float speed, float startAngle)
	{
		float num = 0f;
		switch (axisInfluenced)
		{
		case AxisInfluenced.X:
			num = axisTransform.localRotation.eulerAngles.x;
			break;
		case AxisInfluenced.Y:
			num = axisTransform.localRotation.eulerAngles.y;
			break;
		case AxisInfluenced.Z:
			num = axisTransform.localRotation.eulerAngles.z;
			break;
		}
		if (num <= 360f && num >= 180f)
		{
			num -= 360f;
		}
		if (num > startAngle - threshold || num < startAngle + threshold)
		{
			float num2 = 0f;
			Vector3 euler = Vector3.zero;
			if (num > startAngle - threshold)
			{
				num2 = num + speed / 100f * Mathf.Abs(num - startAngle) * Time.deltaTime * -1f;
			}
			if (num < startAngle + threshold)
			{
				num2 = num + speed / 100f * Mathf.Abs(num - startAngle) * Time.deltaTime;
			}
			switch (axisInfluenced)
			{
			case AxisInfluenced.X:
				euler = new Vector3(num2, axisTransform.localRotation.eulerAngles.y, axisTransform.localRotation.eulerAngles.z);
				break;
			case AxisInfluenced.Y:
				euler = new Vector3(axisTransform.localRotation.eulerAngles.x, num2, axisTransform.localRotation.eulerAngles.z);
				break;
			case AxisInfluenced.Z:
				euler = new Vector3(axisTransform.localRotation.eulerAngles.x, axisTransform.localRotation.eulerAngles.y, num2);
				break;
			}
			axisTransform.localRotation = Quaternion.Euler(euler);
		}
	}

	private float GetStartAutoStabAngle(Transform axisTransform, AxisInfluenced axisInfluenced)
	{
		float num = 0f;
		if (axisTransform != null)
		{
			switch (axisInfluenced)
			{
			case AxisInfluenced.X:
				num = axisTransform.localRotation.eulerAngles.x;
				break;
			case AxisInfluenced.Y:
				num = axisTransform.localRotation.eulerAngles.y;
				break;
			case AxisInfluenced.Z:
				num = axisTransform.localRotation.eulerAngles.z;
				break;
			}
			if (num <= 360f && num >= 180f)
			{
				num -= 360f;
			}
		}
		return num;
	}

	private float ComputeDeadZone()
	{
		float num = 0f;
		float num2 = Mathf.Max(joystickTouch.magnitude, 0.1f);
		if (restrictArea)
		{
			return Mathf.Max(num2 - deadZone, 0f) / (zoneRadius - touchSize - deadZone) / num2;
		}
		return Mathf.Max(num2 - deadZone, 0f) / (zoneRadius - deadZone) / num2;
	}

	private void ComputeJoystickAnchor(JoystickAnchor anchor)
	{
		float num = 0f;
		if (!restrictArea)
		{
			num = touchSize;
		}
		switch (anchor)
		{
		case JoystickAnchor.UpperLeft:
			anchorPosition = new Vector2(zoneRadius + num, zoneRadius + num);
			break;
		case JoystickAnchor.UpperCenter:
			anchorPosition = new Vector2(VirtualScreen.width / 2f, zoneRadius + num);
			break;
		case JoystickAnchor.UpperRight:
			anchorPosition = new Vector2(VirtualScreen.width - zoneRadius - num, zoneRadius + num);
			break;
		case JoystickAnchor.MiddleLeft:
			anchorPosition = new Vector2(zoneRadius + num, VirtualScreen.height / 2f);
			break;
		case JoystickAnchor.MiddleCenter:
			anchorPosition = new Vector2(VirtualScreen.width / 2f, VirtualScreen.height / 2f);
			break;
		case JoystickAnchor.MiddleRight:
			anchorPosition = new Vector2(VirtualScreen.width - zoneRadius - num, VirtualScreen.height / 2f);
			break;
		case JoystickAnchor.LowerLeft:
			anchorPosition = new Vector2(zoneRadius + num, VirtualScreen.height - zoneRadius - num);
			break;
		case JoystickAnchor.LowerCenter:
			anchorPosition = new Vector2(VirtualScreen.width / 2f, VirtualScreen.height - zoneRadius - num);
			break;
		case JoystickAnchor.LowerRight:
			anchorPosition = new Vector2(VirtualScreen.width - zoneRadius - num, VirtualScreen.height - zoneRadius - num);
			break;
		case JoystickAnchor.None:
			anchorPosition = Vector2.zero;
			break;
		}
		areaRect = new Rect(anchorPosition.x + joystickCenter.x - zoneRadius, anchorPosition.y + joystickCenter.y - zoneRadius, zoneRadius * 2f, zoneRadius * 2f);
		deadRect = new Rect(anchorPosition.x + joystickCenter.x - deadZone, anchorPosition.y + joystickCenter.y - deadZone, deadZone * 2f, deadZone * 2f);
	}

	private void On_TouchStart(Gesture gesture)
	{
		if (!visible || ((gesture.isHoverReservedArea || !dynamicJoystick) && dynamicJoystick) || !isActivated)
		{
			return;
		}
		if (!dynamicJoystick)
		{
			Vector2 b = new Vector2((anchorPosition.x + joystickCenter.x) * VirtualScreen.xRatio, (VirtualScreen.height - anchorPosition.y - joystickCenter.y) * VirtualScreen.yRatio);
			if ((gesture.position - b).sqrMagnitude < zoneRadius * VirtualScreen.xRatio * (zoneRadius * VirtualScreen.xRatio))
			{
				joystickIndex = gesture.fingerIndex;
				CreateEvent(MessageName.On_JoystickTouchStart);
			}
		}
		else
		{
			if (virtualJoystick)
			{
				return;
			}
			switch (area)
			{
			case DynamicArea.FullScreen:
				virtualJoystick = true;
				break;
			case DynamicArea.Bottom:
				if (gesture.position.y < (float)(Screen.height / 2))
				{
					virtualJoystick = true;
				}
				break;
			case DynamicArea.Top:
				if (gesture.position.y > (float)(Screen.height / 2))
				{
					virtualJoystick = true;
				}
				break;
			case DynamicArea.Right:
				if (gesture.position.x > (float)(Screen.width / 2))
				{
					virtualJoystick = true;
				}
				break;
			case DynamicArea.Left:
				if (gesture.position.x < (float)(Screen.width / 2))
				{
					virtualJoystick = true;
				}
				break;
			case DynamicArea.TopRight:
				if (gesture.position.y > (float)(Screen.height / 2) && gesture.position.x > (float)(Screen.width / 2))
				{
					virtualJoystick = true;
				}
				break;
			case DynamicArea.TopLeft:
				if (gesture.position.y > (float)(Screen.height / 2) && gesture.position.x < (float)(Screen.width / 2))
				{
					virtualJoystick = true;
				}
				break;
			case DynamicArea.BottomRight:
				if (gesture.position.y < (float)(Screen.height / 2) && gesture.position.x > (float)(Screen.width / 2))
				{
					virtualJoystick = true;
				}
				break;
			case DynamicArea.BottomLeft:
				if (gesture.position.y < (float)(Screen.height / 2) && gesture.position.x < (float)(Screen.width / 2))
				{
					virtualJoystick = true;
				}
				break;
			}
			if (virtualJoystick)
			{
				joystickCenter = new Vector2(gesture.position.x / VirtualScreen.xRatio, VirtualScreen.height - gesture.position.y / VirtualScreen.yRatio);
				JoyAnchor = JoystickAnchor.None;
				joystickIndex = gesture.fingerIndex;
			}
		}
	}

	private void On_SimpleTap(Gesture gesture)
	{
		if (visible && ((!gesture.isHoverReservedArea && dynamicJoystick) || !dynamicJoystick) && isActivated && gesture.fingerIndex == joystickIndex)
		{
			CreateEvent(MessageName.On_JoystickTap);
		}
	}

	private void On_DoubleTap(Gesture gesture)
	{
		if (visible && ((!gesture.isHoverReservedArea && dynamicJoystick) || !dynamicJoystick) && isActivated && gesture.fingerIndex == joystickIndex)
		{
			CreateEvent(MessageName.On_JoystickDoubleTap);
		}
	}

	private void On_TouchDown(Gesture gesture)
	{
		if (!visible || ((gesture.isHoverReservedArea || !dynamicJoystick) && dynamicJoystick) || !isActivated)
		{
			return;
		}
		Vector2 b = new Vector2((anchorPosition.x + joystickCenter.x) * VirtualScreen.xRatio, (VirtualScreen.height - (anchorPosition.y + joystickCenter.y)) * VirtualScreen.yRatio);
		if (gesture.fingerIndex != joystickIndex)
		{
			return;
		}
		if (((gesture.position - b).sqrMagnitude < zoneRadius * VirtualScreen.xRatio * (zoneRadius * VirtualScreen.xRatio) && resetFingerExit) || !resetFingerExit)
		{
			joystickTouch = new Vector2(gesture.position.x, gesture.position.y) - b;
			joystickTouch = new Vector2(joystickTouch.x / VirtualScreen.xRatio, joystickTouch.y / VirtualScreen.yRatio);
			if (!enableXaxis)
			{
				joystickTouch.x = 0f;
			}
			if (!enableYaxis)
			{
				joystickTouch.y = 0f;
			}
			if ((joystickTouch / (zoneRadius - touchSizeCoef)).sqrMagnitude > 1f)
			{
				joystickTouch.Normalize();
				joystickTouch *= zoneRadius - touchSizeCoef;
			}
		}
		else
		{
			On_TouchUp(gesture);
		}
	}

	private void On_TouchUp(Gesture gesture)
	{
		if (visible && gesture.fingerIndex == joystickIndex)
		{
			joystickIndex = -1;
			if (dynamicJoystick)
			{
				virtualJoystick = false;
			}
			CreateEvent(MessageName.On_JoystickTouchUp);
		}
	}

	public void On_Manual(Vector2 movement)
	{
		if (!isActivated)
		{
			return;
		}
		if (movement != Vector2.zero)
		{
			if (!virtualJoystick)
			{
				virtualJoystick = true;
				CreateEvent(MessageName.On_JoystickTouchStart);
			}
			joystickIndex = 0;
			joystickTouch.x = movement.x * (areaRect.width / 2f);
			joystickTouch.y = movement.y * (areaRect.height / 2f);
		}
		else if (virtualJoystick)
		{
			virtualJoystick = false;
			joystickIndex = -1;
			CreateEvent(MessageName.On_JoystickTouchUp);
		}
	}
}
