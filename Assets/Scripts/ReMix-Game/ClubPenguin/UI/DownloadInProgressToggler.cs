using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DownloadInProgressToggler : MonoBehaviour
	{
		[Tooltip("Delay toggling of visibility if it was last changed within this number of seconds")]
		public float ToggleDelay = 0.5f;

		public GameObject[] GameObjectsEnabledWhenDownloading;

		public GameObject[] GameObjectsDisabledWhenDownloading;

		private LoadingController loadingController;

		private bool downloadActive;

		private bool downloadProgressShown;

		private bool stateChangeAllowed;

		public void Awake()
		{
			loadingController = Service.Get<LoadingController>();
			loadingController.OnDownloadingStarted += downloadingStarted;
			loadingController.OnDownloadingComplete += downloadingComplete;
			downloadActive = false;
			downloadProgressShown = false;
			stateChangeAllowed = true;
			if (loadingController.DownloadProgress.HasValue)
			{
				downloadingStarted();
			}
			else
			{
				downloadingComplete();
			}
		}

		public void OnDestroy()
		{
			loadingController.OnDownloadingStarted -= downloadingStarted;
			loadingController.OnDownloadingComplete -= downloadingComplete;
			CoroutineRunner.StopAllForOwner(this);
		}

		private void downloadingStarted()
		{
			downloadingActive(true);
		}

		private void downloadingComplete()
		{
			downloadingActive(false);
		}

		private void downloadingActive(bool active)
		{
			if (downloadActive != active)
			{
				downloadActive = active;
				updateDisplay();
			}
		}

		private void updateDisplay()
		{
			if (stateChangeAllowed && downloadProgressShown != downloadActive)
			{
				downloadProgressShown = downloadActive;
				int num = GameObjectsDisabledWhenDownloading.Length;
				for (int i = 0; i < num; i++)
				{
					GameObjectsDisabledWhenDownloading[i].SetActive(!downloadActive);
				}
				num = GameObjectsEnabledWhenDownloading.Length;
				for (int i = 0; i < num; i++)
				{
					GameObjectsEnabledWhenDownloading[i].SetActive(downloadActive);
				}
				stateChangeAllowed = false;
				CoroutineRunner.Start(allowStateChange(), this, "Enable download progress ");
			}
		}

		private IEnumerator allowStateChange()
		{
			yield return new WaitForSeconds(ToggleDelay);
			stateChangeAllowed = true;
			updateDisplay();
		}
	}
}
