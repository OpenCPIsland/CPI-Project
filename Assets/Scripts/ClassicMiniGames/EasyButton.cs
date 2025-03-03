using UnityEngine;

[ExecuteInEditMode]
public class EasyButton : MonoBehaviour
{
	public delegate void ButtonUpHandler(string buttonName);

	public delegate void ButtonPressHandler(string buttonName);

	public delegate void ButtonDownHandler(string buttonName);

	public enum ButtonAnchor
	{
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

	public enum Broadcast
	{
		SendMessage,
		SendMessageUpwards,
		BroadcastMessage
	}

	public enum ButtonState
	{
		Down,
		Press,
		Up,
		None
	}

	public enum InteractionType
	{
		Event,
		Include
	}

	private enum MessageName
	{
		On_ButtonDown,
		On_ButtonPress,
		On_ButtonUp
	}

	public bool enable = true;

	public bool isActivated = true;

	public bool showDebugArea = true;

	public bool selected = false;

	public bool isUseGuiLayout = true;

	public ButtonState buttonState = ButtonState.None;

	[SerializeField]
	private ButtonAnchor anchor = ButtonAnchor.LowerRight;

	[SerializeField]
	private Vector2 offset = Vector2.zero;

	[SerializeField]
	private Vector2 scale = Vector2.one;

	public bool isSwipeIn = false;

	public bool isSwipeOut = false;

	public InteractionType interaction = InteractionType.Event;

	public bool useBroadcast = false;

	public GameObject receiverGameObject;

	public Broadcast messageMode;

	public bool useSpecificalMethod = false;

	public string downMethodName;

	public string pressMethodName;

	public string upMethodName;

	public int guiDepth = 0;

	[SerializeField]
	private Texture2D normalTexture;

	public Color buttonNormalColor = Color.white;

	[SerializeField]
	private Texture2D activeTexture;

	public Color buttonActiveColor = Color.white;

	public bool showInspectorProperties = true;

	public bool showInspectorPosition = true;

	public bool showInspectorEvent = false;

	public bool showInspectorTexture = false;

	private Rect buttonRect;

	private int buttonFingerIndex = -1;

	private Texture2D currentTexture;

	private Color currentColor;

	private int frame = 0;

	public ButtonAnchor Anchor
	{
		get
		{
			return anchor;
		}
		set
		{
			anchor = value;
			ComputeButtonAnchor(anchor);
		}
	}

	public Vector2 Offset
	{
		get
		{
			return offset;
		}
		set
		{
			offset = value;
			ComputeButtonAnchor(anchor);
		}
	}

	public Vector2 Scale
	{
		get
		{
			return scale;
		}
		set
		{
			scale = value;
			ComputeButtonAnchor(anchor);
		}
	}

	public Texture2D NormalTexture
	{
		get
		{
			return normalTexture;
		}
		set
		{
			normalTexture = value;
			if (normalTexture != null)
			{
				ComputeButtonAnchor(anchor);
				currentTexture = normalTexture;
			}
		}
	}

	public Texture2D ActiveTexture
	{
		get
		{
			return activeTexture;
		}
		set
		{
			activeTexture = value;
		}
	}

	public static event ButtonDownHandler On_ButtonDown;

	public static event ButtonPressHandler On_ButtonPress;

	public static event ButtonUpHandler On_ButtonUp;

	private void OnEnable()
	{
		EasyTouch.On_TouchStart += On_TouchStart;
		EasyTouch.On_TouchDown += On_TouchDown;
		EasyTouch.On_TouchUp += On_TouchUp;
	}

	private void OnDisable()
	{
		EasyTouch.On_TouchStart -= On_TouchStart;
		EasyTouch.On_TouchDown -= On_TouchDown;
		EasyTouch.On_TouchUp -= On_TouchUp;
		if (Application.isPlaying && EasyTouch.instance != null)
		{
			EasyTouch.instance.reservedVirtualAreas.Remove(buttonRect);
		}
	}

	private void OnDestroy()
	{
		EasyTouch.On_TouchStart -= On_TouchStart;
		EasyTouch.On_TouchDown -= On_TouchDown;
		EasyTouch.On_TouchUp -= On_TouchUp;
		if (Application.isPlaying && EasyTouch.instance != null)
		{
			EasyTouch.instance.reservedVirtualAreas.Remove(buttonRect);
		}
	}

	private void Start()
	{
		currentTexture = normalTexture;
		currentColor = buttonNormalColor;
		buttonState = ButtonState.None;
		VirtualScreen.ComputeVirtualScreen();
		ComputeButtonAnchor(anchor);
	}

	private void OnGUI()
	{
		if (enable)
		{
			GUI.depth = guiDepth;
			base.useGUILayout = isUseGuiLayout;
			VirtualScreen.ComputeVirtualScreen();
			VirtualScreen.SetGuiScaleMatrix();
			if (!(normalTexture != null) || !(activeTexture != null))
			{
				return;
			}
			ComputeButtonAnchor(anchor);
			if (!(normalTexture != null))
			{
				return;
			}
			if (Application.isEditor && !Application.isPlaying)
			{
				currentTexture = normalTexture;
			}
			if (showDebugArea && Application.isEditor && selected && !Application.isPlaying)
			{
				GUI.Box(buttonRect, "");
			}
			if (!(currentTexture != null))
			{
				return;
			}
			if (isActivated)
			{
				GUI.color = currentColor;
				if (Application.isPlaying)
				{
					EasyTouch.instance.reservedVirtualAreas.Remove(buttonRect);
					EasyTouch.instance.reservedVirtualAreas.Add(buttonRect);
				}
			}
			else
			{
				GUI.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.2f);
				if (Application.isPlaying)
				{
					EasyTouch.instance.reservedVirtualAreas.Remove(buttonRect);
				}
			}
			GUI.DrawTexture(buttonRect, currentTexture);
			GUI.color = Color.white;
		}
		else if (Application.isPlaying)
		{
			EasyTouch.instance.reservedVirtualAreas.Remove(buttonRect);
		}
	}

	private void Update()
	{
		if (buttonState == ButtonState.Up)
		{
			buttonState = ButtonState.None;
		}
		if (EasyTouch.GetTouchCount() == 0)
		{
			buttonFingerIndex = -1;
			currentTexture = normalTexture;
			currentColor = buttonNormalColor;
			buttonState = ButtonState.None;
		}
	}

	private void OnDrawGizmos()
	{
	}

	private void ComputeButtonAnchor(ButtonAnchor anchor)
	{
		if (normalTexture != null)
		{
			Vector2 vector = new Vector2((float)normalTexture.width * scale.x, (float)normalTexture.height * scale.y);
			Vector2 vector2 = Vector2.zero;
			switch (anchor)
			{
			case ButtonAnchor.UpperLeft:
				vector2 = new Vector2(0f, 0f);
				break;
			case ButtonAnchor.UpperCenter:
				vector2 = new Vector2(VirtualScreen.width / 2f - vector.x / 2f, offset.y);
				break;
			case ButtonAnchor.UpperRight:
				vector2 = new Vector2(VirtualScreen.width - vector.x, 0f);
				break;
			case ButtonAnchor.MiddleLeft:
				vector2 = new Vector2(0f, VirtualScreen.height / 2f - vector.y / 2f);
				break;
			case ButtonAnchor.MiddleCenter:
				vector2 = new Vector2(VirtualScreen.width / 2f - vector.x / 2f, VirtualScreen.height / 2f - vector.y / 2f);
				break;
			case ButtonAnchor.MiddleRight:
				vector2 = new Vector2(VirtualScreen.width - vector.x, VirtualScreen.height / 2f - vector.y / 2f);
				break;
			case ButtonAnchor.LowerLeft:
				vector2 = new Vector2(0f, VirtualScreen.height - vector.y);
				break;
			case ButtonAnchor.LowerCenter:
				vector2 = new Vector2(VirtualScreen.width / 2f - vector.x / 2f, VirtualScreen.height - vector.y);
				break;
			case ButtonAnchor.LowerRight:
				vector2 = new Vector2(VirtualScreen.width - vector.x, VirtualScreen.height - vector.y);
				break;
			}
			buttonRect = new Rect(vector2.x + offset.x, vector2.y + offset.y, vector.x, vector.y);
		}
	}

	private void RaiseEvent(MessageName msg)
	{
		if (interaction != 0)
		{
			return;
		}
		if (!useBroadcast)
		{
			switch (msg)
			{
			case MessageName.On_ButtonDown:
				if (EasyButton.On_ButtonDown != null)
				{
					EasyButton.On_ButtonDown(base.gameObject.name);
				}
				break;
			case MessageName.On_ButtonUp:
				if (EasyButton.On_ButtonUp != null)
				{
					EasyButton.On_ButtonUp(base.gameObject.name);
				}
				break;
			case MessageName.On_ButtonPress:
				if (EasyButton.On_ButtonPress != null)
				{
					EasyButton.On_ButtonPress(base.gameObject.name);
				}
				break;
			}
			return;
		}
		string methodName = msg.ToString();
		if (msg == MessageName.On_ButtonDown && downMethodName != "" && useSpecificalMethod)
		{
			methodName = downMethodName;
		}
		if (msg == MessageName.On_ButtonPress && pressMethodName != "" && useSpecificalMethod)
		{
			methodName = pressMethodName;
		}
		if (msg == MessageName.On_ButtonUp && upMethodName != "" && useSpecificalMethod)
		{
			methodName = upMethodName;
		}
		if (receiverGameObject != null)
		{
			switch (messageMode)
			{
			case Broadcast.BroadcastMessage:
				receiverGameObject.BroadcastMessage(methodName, base.name, SendMessageOptions.DontRequireReceiver);
				break;
			case Broadcast.SendMessage:
				receiverGameObject.SendMessage(methodName, base.name, SendMessageOptions.DontRequireReceiver);
				break;
			case Broadcast.SendMessageUpwards:
				receiverGameObject.SendMessageUpwards(methodName, base.name, SendMessageOptions.DontRequireReceiver);
				break;
			}
		}
		else
		{
			Debug.LogError("Button : " + base.gameObject.name + " : you must setup receiver gameobject");
		}
	}

	private void On_TouchStart(Gesture gesture)
	{
		if (gesture.IsInRect(VirtualScreen.GetRealRect(buttonRect), true) && enable && isActivated)
		{
			buttonFingerIndex = gesture.fingerIndex;
			currentTexture = activeTexture;
			currentColor = buttonActiveColor;
			buttonState = ButtonState.Down;
			frame = 0;
			RaiseEvent(MessageName.On_ButtonDown);
		}
	}

	private void On_TouchDown(Gesture gesture)
	{
		if (gesture.fingerIndex != buttonFingerIndex && (!isSwipeIn || buttonState != ButtonState.None))
		{
			return;
		}
		if (gesture.IsInRect(VirtualScreen.GetRealRect(buttonRect), true) && enable && isActivated)
		{
			currentTexture = activeTexture;
			currentColor = buttonActiveColor;
			frame++;
			if ((buttonState == ButtonState.Down || buttonState == ButtonState.Press) && frame >= 2)
			{
				RaiseEvent(MessageName.On_ButtonPress);
				buttonState = ButtonState.Press;
			}
			if (buttonState == ButtonState.None)
			{
				buttonFingerIndex = gesture.fingerIndex;
				buttonState = ButtonState.Down;
				frame = 0;
				RaiseEvent(MessageName.On_ButtonDown);
			}
		}
		else if ((isSwipeIn || !isSwipeIn) && !isSwipeOut && buttonState == ButtonState.Press)
		{
			buttonFingerIndex = -1;
			currentTexture = normalTexture;
			currentColor = buttonNormalColor;
			buttonState = ButtonState.None;
		}
		else if (isSwipeOut && buttonState == ButtonState.Press)
		{
			RaiseEvent(MessageName.On_ButtonPress);
			buttonState = ButtonState.Press;
		}
	}

	private void On_TouchUp(Gesture gesture)
	{
		if (gesture.fingerIndex == buttonFingerIndex)
		{
			if ((gesture.IsInRect(VirtualScreen.GetRealRect(buttonRect), true) || (isSwipeOut && buttonState == ButtonState.Press)) && enable && isActivated)
			{
				RaiseEvent(MessageName.On_ButtonUp);
			}
			buttonState = ButtonState.Up;
			buttonFingerIndex = -1;
			currentTexture = normalTexture;
			currentColor = buttonNormalColor;
		}
	}
}
