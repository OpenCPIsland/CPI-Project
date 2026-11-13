using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	[RequireComponent(typeof(Camera))]
	[DisallowMultipleComponent]
	public class ClothingDesignerCameraViewportComposite : MonoBehaviour
	{
		private Camera sceneCamera;

		private Canvas uiCanvas;

		private RenderTexture sceneTexture;

		private RawImage sceneImage;

		private void Awake()
		{
			sceneCamera = GetComponent<Camera>();
			Camera uiCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
			Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
			for (int i = 0; i < canvases.Length; i++)
			{
				if (canvases[i].renderMode == RenderMode.ScreenSpaceCamera && canvases[i].worldCamera == uiCamera)
				{
					uiCanvas = canvases[i];
					break;
				}
			}
			if (uiCanvas == null)
			{
				return;
			}
			GameObject previewObject = new GameObject("Clothing Designer Camera Preview", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
			previewObject.transform.SetParent(uiCanvas.transform, false);
			previewObject.transform.SetAsFirstSibling();
			sceneImage = previewObject.GetComponent<RawImage>();
			sceneImage.raycastTarget = false;
			updatePreview();
		}

		private void LateUpdate()
		{
			updatePreview();
		}

		private void updatePreview()
		{
			if (sceneCamera == null || uiCanvas == null)
			{
				return;
			}
			int width = Mathf.Max(1, Screen.width);
			int height = Mathf.Max(1, Screen.height);
			if (sceneTexture == null || sceneTexture.width != width || sceneTexture.height != height)
			{
				RenderTexture previousTexture = sceneTexture;
				sceneTexture = new RenderTexture(width, height, 24, RenderTextureFormat.Default);
				sceneTexture.name = "Clothing Designer Camera Preview";
				sceneTexture.Create();
				sceneCamera.targetTexture = sceneTexture;
				sceneImage.texture = sceneTexture;
				if (previousTexture != null)
				{
					previousTexture.Release();
					Destroy(previousTexture);
				}
			}
			Rect viewport = sceneCamera.rect;
			RectTransform imageRect = sceneImage.rectTransform;
			imageRect.anchorMin = viewport.min;
			imageRect.anchorMax = viewport.max;
			imageRect.offsetMin = Vector2.zero;
			imageRect.offsetMax = Vector2.zero;
			sceneImage.uvRect = viewport;
		}

		private void OnDestroy()
		{
			if (sceneCamera != null && sceneCamera.targetTexture == sceneTexture)
			{
				sceneCamera.targetTexture = null;
			}
			if (sceneTexture != null)
			{
				sceneTexture.Release();
				Destroy(sceneTexture);
			}
		}
	}
}
