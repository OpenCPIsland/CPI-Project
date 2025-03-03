using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	internal class CameraZoomClamped : MonoBehaviour
	{
		public ZoomClampedSettings[] ZoomSettings = new ZoomClampedSettings[0];

		[Range(0f, 1f)]
		public float StartingZoomPercent = 1f;

		[Header("Tween options for zoom changes on Enable")]
		public iTween.EaseType EaseType = iTween.EaseType.easeOutQuad;

		[Range(0.1f, 5f)]
		public float TweenTime = 1f;

		public PinchZoomInput PinchZoomInput;

		private Camera myCamera;

		private void Awake()
		{
			myCamera = GetComponent<Camera>();
			if (myCamera == null)
			{
				myCamera = Camera.main;
			}
		}

		private void OnEnable()
		{
			updateCameraFOV(StartingZoomPercent, true);
			PinchZoomInput.PreviousZoom = StartingZoomPercent;
			Service.Get<EventDispatcher>().AddListener<InputEvents.ZoomEvent>(onInputZoom);
		}

		private void OnValidate()
		{
		}

		private void OnDisable()
		{
			Service.Get<EventDispatcher>().RemoveListener<InputEvents.ZoomEvent>(onInputZoom);
		}

		private bool onInputZoom(InputEvents.ZoomEvent evt)
		{
			updateCameraFOV(evt.Factor);
			return false;
		}

		private void updateCameraFOV(float percentage, bool tween = false)
		{
			float num = 30f;
			float num2 = 90f;
			AspectRatioType aspectRatioType = PlatformUtils.GetAspectRatioType();
			for (int i = 0; i < ZoomSettings.Length; i++)
			{
				if (aspectRatioType == ZoomSettings[i].Type)
				{
					num = ZoomSettings[i].MinFOV;
					num2 = ZoomSettings[i].MaxFOV;
					break;
				}
			}
			float num3 = num2 - num;
			float value = num + num3 * percentage;
			value = Mathf.Clamp(value, num, num2);
			if (tween)
			{
				TweenFOV(value);
			}
			else
			{
				myCamera.fieldOfView = value;
			}
		}

		private void TweenFOV(float newFieldOfView)
		{
			if (newFieldOfView != myCamera.fieldOfView)
			{
				Hashtable args = iTween.Hash("from", myCamera.fieldOfView, "to", newFieldOfView, "time", TweenTime, "onupdate", "UpdateCameraFOV", "easetype", EaseType);
				iTween.ValueTo(base.gameObject, args);
			}
		}

		private void UpdateCameraFOV(float newValue)
		{
			myCamera.fieldOfView = newValue;
		}
	}
}
