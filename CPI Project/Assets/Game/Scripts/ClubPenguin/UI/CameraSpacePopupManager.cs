using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ClubPenguin.UI
{
	public class CameraSpacePopupManager : BasePopupManager
	{
		private const float WAIT_TO_CLOSE_INTERVAL = 2f;

		public Camera PopupCamera;

		public bool ToggleCamera = true;

		private Transform popupManagerTransform;

		private float defaultPlaneDistance;

		private int defaultOrderInLayer;

		private Canvas canvas;

		private Camera stackedBaseCamera;

		private UniversalAdditionalCameraData stackedBaseCameraData;

		private void Awake()
		{
			ClubPenguin.Core.SceneRefs.Set(this);
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<PopupEvents.ShowCameraSpacePopup>(onShowPopup);
			popupManagerTransform = base.transform;
			canvas = GetComponent<Canvas>();
			defaultPlaneDistance = canvas.planeDistance;
			defaultOrderInLayer = canvas.sortingOrder;
			disableCamera();
		}

		private bool onShowPopup(PopupEvents.ShowCameraSpacePopup evt)
		{
			showPopup(evt.Popup, evt.DestroyPopupOnBackPressed);
			enableCamera();
			if (!string.IsNullOrEmpty(evt.NewCameraTag))
			{
				moveToCamera(evt.NewCameraTag, evt.PlaneDistance, evt.OrderInLayer);
			}
			else
			{
				resetCamera();
			}
			return false;
		}

		private void enableCamera()
		{
			if (PopupCamera != null && ToggleCamera)
			{
				if (!addPopupCameraToStack())
				{
					return;
				}
				if (!PopupCamera.enabled)
				{
					PopupCamera.useOcclusionCulling = Camera.main.useOcclusionCulling;
					PopupCamera.enabled = true;
					CoroutineRunner.Start(waitForPopupToClose(), this, "");
				}
			}
		}

		private bool addPopupCameraToStack()
		{
			Camera baseCamera = Camera.main;
			if (baseCamera == null)
			{
				Debug.LogError("CameraSpacePopupManager could not find the base camera for its popup camera.", this);
				return false;
			}

			UniversalAdditionalCameraData baseCameraData = baseCamera.GetComponent<UniversalAdditionalCameraData>();
			if (baseCameraData == null)
			{
				baseCameraData = baseCamera.gameObject.AddComponent<UniversalAdditionalCameraData>();
			}
			if (baseCameraData.renderType != CameraRenderType.Base)
			{
				Debug.LogError("CameraSpacePopupManager requires the MainCamera to be a URP base camera.", baseCamera);
				return false;
			}

			if (stackedBaseCamera != baseCamera)
			{
				removePopupCameraFromStack();
				stackedBaseCamera = baseCamera;
				stackedBaseCameraData = baseCameraData;
			}

			UniversalAdditionalCameraData popupCameraData = PopupCamera.GetComponent<UniversalAdditionalCameraData>();
			if (popupCameraData == null)
			{
				popupCameraData = PopupCamera.gameObject.AddComponent<UniversalAdditionalCameraData>();
			}
			popupCameraData.renderType = CameraRenderType.Overlay;
			if (!baseCameraData.cameraStack.Contains(PopupCamera))
			{
				baseCameraData.cameraStack.Add(PopupCamera);
			}
			return true;
		}

		private void removePopupCameraFromStack()
		{
			if (stackedBaseCameraData != null && PopupCamera != null)
			{
				stackedBaseCameraData.cameraStack.Remove(PopupCamera);
			}
			stackedBaseCamera = null;
			stackedBaseCameraData = null;
		}

		private void disableCamera()
		{
			if (PopupCamera != null && ToggleCamera)
			{
				PopupCamera.enabled = false;
			}
		}

		private IEnumerator waitForPopupToClose()
		{
			yield return new WaitForSeconds(2f);
			while (popupManagerTransform.childCount > 0)
			{
				yield return new WaitForSeconds(2f);
			}
			disableCamera();
		}

		private void moveToCamera(string cameraTag, float planeDistance, int orderInLayer)
		{
			GameObject gameObject = GameObject.FindWithTag(cameraTag);
			if (gameObject != null && gameObject.GetComponent<Camera>() != null)
			{
				canvas.worldCamera = gameObject.GetComponent<Camera>();
				canvas.planeDistance = planeDistance;
				canvas.sortingOrder = orderInLayer;
			}
		}

		private void resetCamera()
		{
			canvas.worldCamera = PopupCamera;
			canvas.planeDistance = defaultPlaneDistance;
			canvas.sortingOrder = defaultOrderInLayer;
		}

		private void OnDestroy()
		{
			removePopupCameraFromStack();
			ClubPenguin.Core.SceneRefs.Remove(this);
		}
	}
}
