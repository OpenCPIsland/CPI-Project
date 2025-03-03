using ClubPenguin.BlobShadows;
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ScreenPenguinCamera : MonoBehaviour
	{
		public float ZoomPercentage = 0.83f;

		public float ZoomHeightOffset = 0.83f;

		public float ZoomMinDist = 1.5f;

		private CameraController profileCameraController;

		private Quaternion previousPenguinRotation;

		private GameObject playerPenguinGO;

		private bool isCameraSetUp;

		private bool isZoomSetup;

		private void Start()
		{
			focusCameraOnPenguin();
		}

		public void OnDestroy()
		{
			CoroutineRunner.Start(delayPenguinRotation(previousPenguinRotation), this, "delayPenguinRotation");
			resetCamera();
		}

		private void focusCameraOnPenguin()
		{
			playerPenguinGO = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (playerPenguinGO == null)
			{
				return;
			}
			CameraCullingMaskHelper.HideLayer(Camera.main, "RemotePlayer");
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Chat);
			if (gameObject != null)
			{
				Canvas[] componentsInChildren = gameObject.GetComponentsInChildren<Canvas>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = false;
				}
			}
			Service.Get<EventDispatcher>().DispatchEvent(new BlobShadowEvents.DisableBlobShadows(false));
			previousPenguinRotation = playerPenguinGO.transform.rotation;
			Vector3 vector = Camera.main.transform.position - playerPenguinGO.transform.position;
			playerPenguinGO.transform.rotation = Quaternion.LookRotation(new Vector3(vector.x, 0f, vector.z));
			CinematographyEvents.ZoomCameraEvent evt = new CinematographyEvents.ZoomCameraEvent(true, ZoomPercentage, ZoomPercentage, 0f, ZoomHeightOffset, ZoomMinDist);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
			isZoomSetup = true;
			isCameraSetUp = true;
		}

		private void resetCamera()
		{
			if (!isCameraSetUp)
			{
				return;
			}
			if (playerPenguinGO != null)
			{
				playerPenguinGO.transform.rotation = previousPenguinRotation;
			}
			if (Camera.main != null)
			{
				CameraCullingMaskHelper.ShowLayer(Camera.main, "RemotePlayer");
			}
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Chat);
			if (gameObject != null)
			{
				Canvas[] componentsInChildren = gameObject.GetComponentsInChildren<Canvas>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = true;
				}
			}
			Service.Get<EventDispatcher>().DispatchEvent(default(BlobShadowEvents.EnableBlobShadows));
			if (isZoomSetup)
			{
				CinematographyEvents.ZoomCameraEvent evt = new CinematographyEvents.ZoomCameraEvent(false);
				Service.Get<EventDispatcher>().DispatchEvent(evt);
				isZoomSetup = false;
			}
		}

		private IEnumerator delayPenguinRotation(Quaternion previousRotation)
		{
			yield return null;
			GameObject penguinGO = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (penguinGO != null)
			{
				penguinGO.transform.rotation = previousRotation;
			}
		}
	}
}
