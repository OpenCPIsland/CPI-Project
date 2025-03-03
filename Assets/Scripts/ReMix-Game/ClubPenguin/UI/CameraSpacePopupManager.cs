using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

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
			if (PopupCamera != null && !PopupCamera.enabled && ToggleCamera)
			{
				PopupCamera.useOcclusionCulling = Camera.main.useOcclusionCulling;
				PopupCamera.enabled = true;
				CoroutineRunner.Start(waitForPopupToClose(), this, "");
			}
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
			ClubPenguin.Core.SceneRefs.Remove(this);
		}
	}
}
