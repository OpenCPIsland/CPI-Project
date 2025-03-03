using ClubPenguin.Cinematography;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClubPenguin.Input
{
	public class LocomotionMouseInput : LocomotionInput
	{
		[SerializeField]
		private float minDistance = 20f;

		[SerializeField]
		private float maxDistance = 80f;

		private bool movementStarted;

		private Vector2 anchoredPosition;

		private Vector2 previousMousePosition;

		private Camera cachedCameraMain;

		private Camera cameraMain
		{
			get
			{
				if (cachedCameraMain == null)
				{
					cachedCameraMain = Camera.main;
				}
				return cachedCameraMain;
			}
		}

		private Vector3 playerPosition
		{
			get
			{
				ZoneLocalPlayerManager zoneLocalPlayerManager = SceneRefs.ZoneLocalPlayerManager;
				return (zoneLocalPlayerManager != null) ? zoneLocalPlayerManager.LocalPlayerGameObject.transform.position : Vector3.zero;
			}
		}

		public override void Initialize(KeyCodeRemapper keyCodeRemapper)
		{
			ElasticGlancer.GlobalGlancersEnabled = false;
			Service.Get<EventDispatcher>().DispatchEvent(default(CinematographyEvents.DisableElasticGlancer));
			base.Initialize(keyCodeRemapper);
		}

		protected override bool process(int filter)
		{
			if (filter >= 0 && filter != 0)
			{
				return false;
			}
			bool flag = previousMousePosition != (Vector2)UnityEngine.Input.mousePosition;
			previousMousePosition = UnityEngine.Input.mousePosition;
			if (UnityEngine.Input.GetMouseButtonDown(0))
			{
				if (!EventSystem.current.IsPointerOverGameObject())
				{
					movementStarted = true;
				}
			}
			else if (!UnityEngine.Input.GetMouseButton(0))
			{
				movementStarted = false;
			}
			bool flag2 = false;
			if (movementStarted)
			{
				if (cameraMain != null)
				{
					anchoredPosition = cameraMain.WorldToScreenPoint(playerPosition);
				}
				Vector2 vector = previousMousePosition - anchoredPosition;
				float num = 0f;
				if (vector.magnitude >= minDistance)
				{
					num = Mathf.InverseLerp(minDistance, maxDistance, vector.magnitude);
					num = Mathf.Clamp01(num);
				}
				inputEvent.Direction = vector.normalized;
				inputEvent.Direction *= num;
				inputEvent.Rotation = Vector2.zero;
			}
			else
			{
				inputEvent.Direction = Vector2.zero;
				if (flag && cameraMain != null)
				{
					inputEvent.Rotation = previousMousePosition - (Vector2)cameraMain.WorldToScreenPoint(playerPosition);
					flag2 = true;
				}
				else
				{
					inputEvent.Rotation = Vector2.zero;
				}
			}
			return movementStarted || flag2;
		}
	}
}
