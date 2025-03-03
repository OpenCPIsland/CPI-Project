using ClubPenguin.Cinematography;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(AvatarRenderTextureComponent))]
	public class AvatarRenderTextureRotator : MonoBehaviour
	{
		public RectTransform TouchArea;

		public float RotationSpeed = 15f;

		private AvatarRenderTextureComponent renderTextureComponent;

		private float previousTouchX;

		private bool isRotating;

		private void Awake()
		{
			renderTextureComponent = GetComponent<AvatarRenderTextureComponent>();
		}

		private void Start()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(CinematographyEvents.DisableElasticGlancer));
		}

		private void Update()
		{
			if (UnityEngine.Input.GetMouseButtonDown(0))
			{
				if (TouchArea == null || RectTransformUtility.RectangleContainsScreenPoint(TouchArea, UnityEngine.Input.mousePosition))
				{
					isRotating = true;
					previousTouchX = UnityEngine.Input.mousePosition.x;
				}
			}
			else if (UnityEngine.Input.GetMouseButtonUp(0))
			{
				isRotating = false;
			}
			else if (UnityEngine.Input.GetMouseButton(0) && isRotating)
			{
				float num = UnityEngine.Input.mousePosition.x - previousTouchX;
				renderTextureComponent.RotateModel((0f - num) * RotationSpeed);
				previousTouchX = UnityEngine.Input.mousePosition.x;
			}
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(CinematographyEvents.EnableElasticGlancer));
		}
	}
}
