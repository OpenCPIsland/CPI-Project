using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class WorldContainerCameraAdjuster : MonoBehaviour
	{
		private RectTransform targetTransform;

		private CanvasScalerExt canvasScaler;

		private Camera mainCamera;

		public IEnumerator Start()
		{
			yield return null;
			Initialize();
			Rect viewport = new Rect(0f, 1f - targetTransform.rect.height / canvasScaler.referenceResolution.y * canvasScaler.ScaleModifier, 1f, targetTransform.rect.height / canvasScaler.referenceResolution.y * canvasScaler.ScaleModifier);
			if (viewport != mainCamera.rect)
			{
				mainCamera.rect = viewport;
			}
		}

		public void Initialize()
		{
			targetTransform = GetComponent<RectTransform>();
			canvasScaler = GetComponentInParent<CanvasScalerExt>();
			mainCamera = Camera.main;
			if (mainCamera == null)
			{
				throw new InvalidOperationException("The scene has not been configured correctly for the CameraViewportAdjuster. No main camera was found.");
			}
		}
	}
}
