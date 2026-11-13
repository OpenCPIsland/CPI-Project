#if UNITY_ANDROID || UNITY_IOS
using ClubPenguin.Kelowna.Common.ImageCache;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public partial class ClearCachePanelController : MonoBehaviour
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
			clearLocalCaches();
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
			if (!Caching.ClearCache())
			{
				Debug.LogError("Unable to clear the Unity content cache.");
			}
		}
	}
}
#elif UNITY_WEBGL
using ClubPenguin.Kelowna.Common.ImageCache;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public partial class ClearCachePanelController : MonoBehaviour
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
		}
	}
}
#else
using ClubPenguin.Kelowna.Common.ImageCache;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
    public partial class ClearCachePanelController : MonoBehaviour
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
            setActiveSafe(ButtonImage, false);
            setActiveSafe(ButtonText, false);
            setActiveSafe(Preloader, true);
            setActiveSafe(DoneImage, false);
            setActiveSafe(DoneText, false);
            clearImageCache();
            clearContentCache();
            clearLocalCaches();
            CoroutineRunner.Start(waitForAnimationPreloader(), this, "waitForAnimationPreloader");
        }

        private IEnumerator waitForAnimationPreloader()
        {
            yield return new WaitForSeconds(2f);

            setActiveSafe(ButtonImage, false);
            setActiveSafe(ButtonText, false);
            setActiveSafe(Preloader, false);
            setActiveSafe(DoneImage, true);
            setActiveSafe(DoneText, true);

            CoroutineRunner.Start(waitForAnimationDone(), this, "waitForAnimationDone");

            if (MonoSingleton<NativeAccessibilityManager>.Instance.IsEnabled)
            {
                TMP_Text label = DoneText != null ? DoneText.GetComponentInChildren<TMP_Text>(true) : null;

                if (label != null)
                {
                    MonoSingleton<NativeAccessibilityManager>.Instance.Native.Speak(label.text);
                }
            }
        }

        private IEnumerator waitForAnimationDone()
        {
            yield return new WaitForSeconds(2f);

            setActiveSafe(ButtonImage, true);
            setActiveSafe(ButtonText, true);
            setActiveSafe(Preloader, false);
            setActiveSafe(DoneImage, false);
            setActiveSafe(DoneText, false);

            ClearCacheButton.interactable = true;
        }

        private static void setActiveSafe(GameObject target, bool active)
        {
            if (target != null)
            {
                target.SetActive(active);
            }
        }

        private static TMP_Text getTMPText(GameObject target)
        {
            if (target == null)
            {
                return null;
            }

            return target.GetComponentInChildren<TMP_Text>(true);
        }

        private void clearImageCache()
        {
            Service.Get<ImageCache>().ClearImageCache();
        }

        private void clearContentCache()
        {
            if (!Caching.ClearCache())
            {
                Debug.LogError("Unable to clear the Unity content cache.");
            }
        }
    }
}
#endif

#if !UNITY_WEBGL
namespace ClubPenguin.UI
{
	public partial class ClearCachePanelController
	{
		private const string APP_DATA_FILE_NAME = "cp.AppData.dat";
		private const string APP_DATA_PLAYER_PREFS_KEY = "cp.AppData";

		private static void clearLocalCaches()
		{
			string persistentDataPath = UnityEngine.Application.persistentDataPath;
			deleteDirectory(System.IO.Path.Combine(persistentDataPath, "KeyValueDatabase"));
			deleteFile(System.IO.Path.Combine(persistentDataPath, APP_DATA_FILE_NAME));
			clearAppDataPlayerPrefs();
			clearDirectoryContents(UnityEngine.Application.temporaryCachePath);
			runSafely("HTTP disk cache", HTTP.DiskCache.ClearPersistentCache);
		}

		private static void clearAppDataPlayerPrefs()
		{
			try
			{
				UnityEngine.PlayerPrefs.DeleteKey(APP_DATA_PLAYER_PREFS_KEY);
				UnityEngine.PlayerPrefs.Save();
			}
			catch (System.Exception exception)
			{
				UnityEngine.Debug.LogError("Unable to clear app-data PlayerPrefs: " + exception);
			}
		}

		private static void deleteDirectory(string path)
		{
			if (!System.IO.Directory.Exists(path))
			{
				return;
			}

			try
			{
				System.IO.Directory.Delete(path, true);
			}
			catch (System.Exception exception)
			{
				UnityEngine.Debug.LogError("Unable to clear cache directory " + path + ": " + exception);
			}
		}

		private static void deleteFile(string path)
		{
			if (!System.IO.File.Exists(path))
			{
				return;
			}

			try
			{
				System.IO.File.Delete(path);
			}
			catch (System.Exception exception)
			{
				UnityEngine.Debug.LogError("Unable to clear cache file " + path + ": " + exception);
			}
		}

		private static void clearDirectoryContents(string path)
		{
			if (string.IsNullOrEmpty(path) || !System.IO.Directory.Exists(path))
			{
				return;
			}

			try
			{
				foreach (string file in System.IO.Directory.GetFiles(path))
				{
					deleteFile(file);
				}
				foreach (string directory in System.IO.Directory.GetDirectories(path))
				{
					try
					{
						System.IO.Directory.Delete(directory, true);
					}
					catch (System.Exception exception)
					{
						UnityEngine.Debug.LogError("Unable to clear cache directory " + directory + ": " + exception);
					}
				}
			}
			catch (System.Exception exception)
			{
				UnityEngine.Debug.LogError("Unable to enumerate cache directory " + path + ": " + exception);
			}
		}

		private static void runSafely(string cacheName, System.Action clearCache)
		{
			try
			{
				clearCache();
			}
			catch (System.Exception exception)
			{
				UnityEngine.Debug.LogError("Unable to clear " + cacheName + ": " + exception);
			}
		}
	}
}
#endif
