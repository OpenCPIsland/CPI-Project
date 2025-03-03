using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class ElasticGlancer : Glancer
	{
		private const float JOYSTICK_DIMENSIONS_DELAY = 1f;

		public static bool GlobalGlancersEnabled = true;

		public float MaxYaw = 15f;

		public float MaxPitch = 15f;

		public float TouchSensitivity = 2f;

		public bool InvertYaw;

		public bool InvertPitch;

		private EventDispatcher dispatcher;

		private VirtualJoystick joystick;

		private Vector2 touchStart;

		private Vector2 touchDelta;

		private int topOfJoystickInPixels;

		private float minDelta = 0.01f;

		private bool isGlancerEnabled = true;

		public void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
			if (SceneRefs.IsSet<VirtualJoystick>())
			{
				joystick = SceneRefs.Get<VirtualJoystick>();
				CoroutineRunner.Start(getTopOfJoystickInPixels(), this, "getTopOfJoystickInPixels");
			}
			else
			{
				dispatcher.AddListener<VirtualJoystickEvents.JoystickAdded>(onJoystickAdded);
			}
			dispatcher.AddListener<CinematographyEvents.DisableElasticGlancer>(onGlancerDisabled);
			dispatcher.AddListener<CinematographyEvents.EnableElasticGlancer>(onGlancerEnabled);
			isGlancerEnabled = GlobalGlancersEnabled;
		}

		public void OnDestroy()
		{
			dispatcher.RemoveListener<VirtualJoystickEvents.JoystickAdded>(onJoystickAdded);
			dispatcher.RemoveListener<CinematographyEvents.DisableElasticGlancer>(onGlancerDisabled);
			dispatcher.RemoveListener<CinematographyEvents.EnableElasticGlancer>(onGlancerEnabled);
		}

		private bool onJoystickAdded(VirtualJoystickEvents.JoystickAdded evt)
		{
			joystick = evt.Joystick;
			CoroutineRunner.Start(getTopOfJoystickInPixels(), this, "getTopOfJoystickInPixels");
			return false;
		}

		public bool onGlancerDisabled(CinematographyEvents.DisableElasticGlancer evt)
		{
			isGlancerEnabled = false;
			return false;
		}

		public bool onGlancerEnabled(CinematographyEvents.EnableElasticGlancer evt)
		{
			isGlancerEnabled = GlobalGlancersEnabled;
			return false;
		}

		public void Update()
		{
			if (!isGlancerEnabled)
			{
				return;
			}
			Vector2 position;
			if (Input.touchSupported)
			{
				if (Input.touchCount <= 0)
				{
					return;
				}
				Touch touch = Input.GetTouch(0);
				position = touch.position;
				bool flag = isValidPointerPosition(position);
				switch (touch.phase)
				{
				case TouchPhase.Began:
					if (flag)
					{
						touchStart = position;
					}
					break;
				case TouchPhase.Moved:
				case TouchPhase.Stationary:
					if (flag)
					{
						touchDelta = position - touchStart;
					}
					break;
				case TouchPhase.Ended:
				case TouchPhase.Canceled:
					touchDelta = Vector2.zero;
					Dirty = true;
					break;
				}
				return;
			}
			position = Input.mousePosition;
			if (Input.GetMouseButtonDown(0))
			{
				touchStart = position;
			}
			if (Input.GetMouseButton(0))
			{
				if (isValidPointerPosition(position))
				{
					touchDelta = position - touchStart;
					Dirty = true;
				}
			}
			else
			{
				touchDelta = Vector3.zero;
				Dirty = true;
			}
		}

		private bool isValidPointerPosition(Vector2 pos)
		{
			return pos.y > (float)topOfJoystickInPixels;
		}

		public override bool Aim(ref Setup setup)
		{
			bool result = false;
			if (joystick == null || !joystick.IsInteracting())
			{
				Vector2 vector = touchDelta * TouchSensitivity / Screen.width;
				if (vector.sqrMagnitude > minDelta)
				{
					float num = Mathf.Clamp(vector.y, -1f, 1f) * MaxPitch;
					float num2 = Mathf.Clamp(vector.x, -1f, 1f) * MaxYaw;
					if (InvertYaw)
					{
						num2 = 0f - num2;
					}
					if (InvertPitch)
					{
						num = 0f - num;
					}
					Quaternion rhs = Quaternion.AngleAxis(num, setup.Camera.right);
					Quaternion lhs = Quaternion.AngleAxis(num2, setup.Camera.up);
					setup.Glance = lhs * rhs;
					result = true;
				}
			}
			return result;
		}

		private IEnumerator getTopOfJoystickInPixels()
		{
			yield return new WaitForSeconds(1f);
			if (joystick != null)
			{
				RectTransform component = joystick.GetComponent<RectTransform>();
				Vector3[] array = new Vector3[4];
				component.GetWorldCorners(array);
				topOfJoystickInPixels = (int)array[1].y;
			}
		}
	}
}
