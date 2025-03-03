using ClubPenguin.Cinematography;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	[RequireComponent(typeof(Camera))]
	public class ClothingDesignerCameraController : MonoBehaviour
	{
		private const float cameraSmoothing = 5f;

		private const float cameraPositionThreashold = 0.01f;

		private const float viewportChangeThreshhold = 0.02f;

		[SerializeField]
		private List<ClothingDesignerCameraData> cameraViewData;

		[Header("Lower Number is closer. e.g. 0.6")]
		[SerializeField]
		private float cameraZoomAdjustment;

		[SerializeField]
		private float cameraSaveZoomFOV;

		private Camera mainCamera;

		private EventChannel clothingDesignerEventChannel;

		private EventChannel customizationEventChannel;

		private float originalFOV;

		private bool wasOriginalViewportSet;

		private Rect originalViewport;

		private Rect targetViewport;

		private Quaternion targetRotation;

		private float targetFOV;

		private Vector3 targetPosition;

		private bool isAnimatingCamera;

		private bool isAnimatingViewport;

		private bool checkPosition;

		private void Awake()
		{
			CameraCullingMaskHelper.HideLayer(Camera.main, "IconRender");
			SceneRefs.SetClothingDesignerCameraController(this);
			mainCamera = GetComponent<Camera>();
			isAnimatingCamera = false;
			isAnimatingViewport = false;
			checkPosition = false;
			wasOriginalViewportSet = false;
		}

		private void Start()
		{
			originalFOV = mainCamera.fieldOfView;
			setupListeners();
			ClothingDesignerCameraData cameraData = getCameraData(ClothingDesignerCameraState.Inventory);
			base.transform.position = cameraData.Position;
			base.transform.rotation = Quaternion.Euler(cameraData.Rotation);
		}

		private void setupListeners()
		{
			clothingDesignerEventChannel = new EventChannel(ClothingDesignerContext.EventBus);
			clothingDesignerEventChannel.AddListener<ClothingDesignerUIEvents.UpdateCameraState>(onUpdateCameraState);
			customizationEventChannel = new EventChannel(CustomizationContext.EventBus);
			customizationEventChannel.AddListener<CustomizerUIEvents.CameraZoomInOnGameObject>(frameObjectWithCamera);
			customizationEventChannel.AddListener<CustomizerUIEvents.StartPurchaseMoment>(onStartPurchaseMoment);
			customizationEventChannel.AddListener<CustomizerUIEvents.EndPurchaseMoment>(onEndPurchaseMoment);
		}

		private bool onUpdateCameraState(ClothingDesignerUIEvents.UpdateCameraState evt)
		{
			ClothingDesignerCameraState cameraState = evt.CameraState;
			bool animateCamera = evt.AnimateCamera;
			ClothingDesignerCameraData cameraData = getCameraData(cameraState);
			switch (cameraState)
			{
			case ClothingDesignerCameraState.Default:
				changeCameraView(targetPosition, cameraData.Rotation, originalFOV, animateCamera);
				break;
			case ClothingDesignerCameraState.Inventory:
			case ClothingDesignerCameraState.Customizer:
			case ClothingDesignerCameraState.CatalogCustomizer:
				changeCameraView(cameraData.Position, cameraData.Rotation, originalFOV, animateCamera);
				break;
			case ClothingDesignerCameraState.Save:
			case ClothingDesignerCameraState.CatalogSave:
				changeCameraView(cameraData.Position, cameraData.Rotation, cameraSaveZoomFOV, animateCamera);
				break;
			}
			return false;
		}

		private void changeCameraView(Vector3 newPosition, Vector3 newRotation, float FOV, bool isAnimated)
		{
			targetPosition = newPosition;
			targetRotation = Quaternion.Euler(newRotation);
			targetFOV = FOV;
			if (isAnimated)
			{
				isAnimatingCamera = true;
				checkPosition = true;
				return;
			}
			isAnimatingCamera = false;
			checkPosition = false;
			mainCamera.transform.position = newPosition;
			mainCamera.transform.rotation = Quaternion.Euler(newRotation);
			mainCamera.fieldOfView = FOV;
			ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.CameraPositionChangeComplete));
		}

		private bool frameObjectWithCamera(CustomizerUIEvents.CameraZoomInOnGameObject evt)
		{
			isAnimatingCamera = true;
			Quaternion rotation = evt.ObjectTransform.transform.rotation;
			evt.ObjectTransform.rotation = Quaternion.identity;
			List<MeshCollider> objectMeshColliders = evt.ObjectMeshColliders;
			Bounds bounds = objectMeshColliders[0].bounds;
			int num = 0;
			for (num = 1; num < objectMeshColliders.Count; num++)
			{
				bounds.Encapsulate(objectMeshColliders[num].bounds);
			}
			Vector3 center = bounds.center;
			Vector3 max = bounds.max;
			float x = Mathf.Abs(center.z - mainCamera.transform.position.z);
			float y = Mathf.Abs(max.x - center.x);
			float y2 = Mathf.Abs(max.y - center.y);
			float a = 2f * Mathf.Atan2(y, x) * 180f / (float)Math.PI;
			float b = 2f * Mathf.Atan2(y2, x) * 180f / (float)Math.PI;
			targetFOV = Mathf.Max(a, b) * (cameraZoomAdjustment + evt.ZoomOffset);
			evt.ObjectTransform.rotation = rotation;
			bounds = objectMeshColliders[0].bounds;
			for (num = 1; num < objectMeshColliders.Count; num++)
			{
				bounds.Encapsulate(objectMeshColliders[num].bounds);
			}
			center = bounds.center;
			Vector3 normalized = (center - mainCamera.transform.position).normalized;
			targetRotation = Quaternion.LookRotation(normalized);
			return false;
		}

		private bool onStartPurchaseMoment(CustomizerUIEvents.StartPurchaseMoment evt)
		{
			if (!wasOriginalViewportSet)
			{
				originalViewport = mainCamera.rect;
				wasOriginalViewportSet = false;
			}
			isAnimatingViewport = true;
			targetViewport = new Rect(0f, 0f, 1f, 1f);
			return false;
		}

		private bool onEndPurchaseMoment(CustomizerUIEvents.EndPurchaseMoment evt)
		{
			isAnimatingViewport = true;
			targetViewport = originalViewport;
			return false;
		}

		private void Update()
		{
			if (isAnimatingCamera)
			{
				mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, 5f * Time.deltaTime);
				mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, targetRotation, 5f * Time.deltaTime);
				mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, 5f * Time.deltaTime);
				if (checkPosition && Vector3.Distance(mainCamera.transform.position, targetPosition) < 0.01f)
				{
					checkPosition = false;
					ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.CameraPositionChangeComplete));
				}
			}
			if (isAnimatingViewport)
			{
				Vector2 vector = Vector2.Lerp(mainCamera.rect.min, targetViewport.min, 5f * Time.deltaTime);
				Vector2 vector2 = Vector2.Lerp(mainCamera.rect.max, targetViewport.max, 5f * Time.deltaTime);
				mainCamera.rect = new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
				if (Vector2.Distance(mainCamera.rect.min, targetViewport.min) < 0.02f && Vector2.Distance(mainCamera.rect.max, targetViewport.max) < 0.02f)
				{
					mainCamera.rect = targetViewport;
					isAnimatingViewport = false;
				}
			}
		}

		private ClothingDesignerCameraData getCameraData(ClothingDesignerCameraState state)
		{
			ClothingDesignerCameraData clothingDesignerCameraData = cameraViewData.Find((ClothingDesignerCameraData dat) => dat.State == state);
			if (clothingDesignerCameraData == null)
			{
				Log.LogErrorFormatted(this, "Unable to locate the camera data for state {0}. Ensure this state exists on the script as a reference.", state);
				clothingDesignerCameraData = cameraViewData[0];
			}
			return clothingDesignerCameraData;
		}

		private void OnDestroy()
		{
			if (clothingDesignerEventChannel != null)
			{
				clothingDesignerEventChannel.RemoveAllListeners();
			}
			if (customizationEventChannel != null)
			{
				clothingDesignerEventChannel.RemoveAllListeners();
			}
		}
	}
}
