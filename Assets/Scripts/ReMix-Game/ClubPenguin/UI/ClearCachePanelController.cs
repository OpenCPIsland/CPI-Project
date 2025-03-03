using ClubPenguin.Kelowna.Common.ImageCache;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ClearCachePanelController : MonoBehaviour
	{
		public Button ClearCacheButton;

		[Header("Default state")]
		public GameObject ButtonImage;

		public GameObject ButtonText;

		[Header("Cache Clearing")]
		public GameObject Preloader;

		[Header("Done")]
		public GameObject DoneImage;

		public GameObject DoneText;

		public void OnClearButtonClicked()
		{
			ClearCacheButton.interactable = false;
			ButtonImage.SetActive(false);
			ButtonText.SetActive(false);
			Preloader.SetActive(true);
			DoneImage.SetActive(false);
			DoneText.SetActive(false);
			clearImageCache();
			clearContentCache();
			CoroutineRunner.Start(waitForAnimationPreloader(), this, "waitForAnimationPreloader");
		}

		private IEnumerator waitForAnimationPreloader()
		{
			yield return new WaitForSeconds(2f);
			ButtonImage.SetActive(false);
			ButtonText.SetActive(false);
			Preloader.SetActive(false);
			DoneImage.SetActive(true);
			DoneText.SetActive(true);
			CoroutineRunner.Start(waitForAnimationDone(), this, "waitForAnimationDone");
			if (MonoSingleton<NativeAccessibilityManager>.Instance.IsEnabled)
			{
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.Speak(ClearCacheButton.GetComponentInChildren<Text>().text);
			}
		}

		private IEnumerator waitForAnimationDone()
		{
			yield return new WaitForSeconds(2f);
			ButtonImage.SetActive(true);
			ButtonText.SetActive(true);
			Preloader.SetActive(false);
			DoneImage.SetActive(false);
			DoneText.SetActive(false);
			ClearCacheButton.interactable = true;
		}

		private void clearImageCache()
		{
			Service.Get<ImageCache>().ClearImageCache();
		}

		private void clearContentCache()
		{
			Caching.ClearCache();
		}
	}
}
