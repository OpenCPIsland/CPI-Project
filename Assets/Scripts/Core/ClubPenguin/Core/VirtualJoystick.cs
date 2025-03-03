using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Core
{
	[RequireComponent(typeof(RectTransform))]
	public class VirtualJoystick : MonoBehaviour
	{
		[Range(0f, 0.5f)]
		public float DeadZoneFraction = 0.1f;

		public float RadiusFraction = 0.9f;

		private float radius;

		private RectTransform joystick;

		private RectTransform joystickPad;

		private RectTransform joystickBase;

		private float minDistance;

		private float maxDistance;

		private EventDispatcher dispatcher;

		private Camera UICamera;

		private Canvas canvas;

		private int activeFingerId;

		private bool isInteractable = true;

		public bool IsInteractable
		{
			get
			{
				return IsInteractable;
			}
			set
			{
				if (value)
				{
					EnableJoystick();
				}
				else
				{
					DisableJoystick();
				}
			}
		}

		private void Awake()
		{
			if (SceneRefs.IsSet<VirtualJoystick>())
			{
				SceneRefs.Remove(SceneRefs.Get<VirtualJoystick>());
			}
			SceneRefs.Set(this);
			joystick = GetComponent<RectTransform>();
			joystickBase = base.transform.GetChild(0).GetComponent<RectTransform>();
			joystickPad = base.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
			dispatcher = Service.Get<EventDispatcher>();
		}

		private void Start()
		{
			maxDistance = Mathf.Min(joystickBase.rect.width, joystickBase.rect.height) * 0.5f * RadiusFraction;
			minDistance = maxDistance * DeadZoneFraction;
			canvas = GetComponentInParent<Canvas>();
			if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
			{
				UICamera = GameObject.FindGameObjectWithTag(UIConstants.Tags.GUI_CAMERA).GetComponent<Camera>();
			}
			dispatcher.DispatchEvent(new VirtualJoystickEvents.JoystickAdded(this));
		}

		private void OnEnable()
		{
			activeFingerId = -1;
		}

		public void OnDestroy()
		{
			if (dispatcher != null)
			{
				dispatcher.DispatchEvent(new InputEvents.MoveEvent(Vector2.zero));
			}
			if (SceneRefs.IsSet<VirtualJoystick>() && SceneRefs.Get<VirtualJoystick>() == this)
			{
				SceneRefs.Remove(this);
			}
		}

		public bool IsInteracting()
		{
			return activeFingerId > -1;
		}

		private bool processTouch(Touch touch, out Vector3 stickPos)
		{
			bool result = touch.phase != TouchPhase.Canceled && touch.phase != TouchPhase.Ended;
			bool flag = touch.phase == TouchPhase.Began;
			stickPos = new Vector3(touch.position.x, touch.position.y, 0f);
			screenToWorld(ref stickPos);
			if (flag)
			{
				result = isPointWithinJoystickBounds(ref stickPos);
			}
			else if (activeFingerId != touch.fingerId)
			{
				result = false;
			}
			return result;
		}

		private bool processMouse(int index, out Vector3 stickPos)
		{
			bool result = Input.GetMouseButton(index);
			bool mouseButtonDown = Input.GetMouseButtonDown(index);
			stickPos = Input.mousePosition;
			screenToWorld(ref stickPos);
			if (mouseButtonDown)
			{
				result = isPointWithinJoystickBounds(ref stickPos);
			}
			else if (activeFingerId != index)
			{
				result = false;
			}
			return result;
		}

		private void screenToWorld(ref Vector3 point)
		{
			if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
			{
				point.z = canvas.planeDistance;
				point = UICamera.ScreenToWorldPoint(point);
			}
		}

		private bool isPointWithinJoystickBounds(ref Vector3 point)
		{
			Vector3 point2 = joystick.worldToLocalMatrix.MultiplyPoint(point);
			return joystick.rect.Contains(point2);
		}

		public Vector2 ProcessInput()
		{
			if (!isInteractable)
			{
				return Vector2.zero;
			}
			Vector3 stickPos = Vector3.zero;
			if (Input.touchSupported)
			{
				if (activeFingerId > -1)
				{
					int touchCount = Input.touchCount;
					for (int i = 0; i < touchCount; i++)
					{
						Touch touch = Input.GetTouch(i);
						if (touch.fingerId == activeFingerId && !processTouch(touch, out stickPos))
						{
							activeFingerId = -1;
							stickPos = Vector3.zero;
							dispatcher.DispatchEvent(default(VirtualJoystickEvents.JoystickDeactivated));
						}
					}
				}
				else
				{
					for (int i = 0; i < Input.touchCount; i++)
					{
						Touch touch2 = Input.GetTouch(i);
						if (processTouch(touch2, out stickPos))
						{
							activeFingerId = touch2.fingerId;
							dispatcher.DispatchEvent(default(VirtualJoystickEvents.JoystickActivated));
							break;
						}
					}
				}
			}
			else if (activeFingerId > -1)
			{
				if (!processMouse(activeFingerId, out stickPos))
				{
					activeFingerId = -1;
					stickPos = Vector3.zero;
					dispatcher.DispatchEvent(default(VirtualJoystickEvents.JoystickDeactivated));
				}
			}
			else if (processMouse(0, out stickPos))
			{
				activeFingerId = 0;
				dispatcher.DispatchEvent(default(VirtualJoystickEvents.JoystickActivated));
			}
			if (activeFingerId > -1)
			{
				Vector3 ls_pos = joystickBase.worldToLocalMatrix.MultiplyPoint(stickPos);
				return updateStick(ls_pos);
			}
			return Vector2.zero;
		}

		private Vector2 updateStick(Vector3 ls_pos)
		{
			float num = Mathf.Min(ls_pos.magnitude, maxDistance);
			Vector3 normalized = ls_pos.normalized;
			joystickPad.localPosition = normalized * num;
			if (num > minDistance)
			{
				Vector2 a = new Vector2(normalized.x, normalized.y);
				float d = Mathf.Clamp(num / (maxDistance - minDistance), 0f, 1f);
				a.Normalize();
				return a * d;
			}
			return Vector2.zero;
		}

		public void ResetInput()
		{
			if (activeFingerId > -1)
			{
				activeFingerId = -1;
				dispatcher.DispatchEvent(default(VirtualJoystickEvents.JoystickDeactivated));
			}
		}

		public void EnableJoystick()
		{
			isInteractable = true;
			if (joystickPad != null)
			{
				joystickPad.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			}
		}

		public void DisableJoystick()
		{
			isInteractable = false;
			joystickPad.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.2f);
			activeFingerId = -1;
		}
	}
}
