using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Disney.Kelowna.Common
{
	public class LoadingController : MonoBehaviour
	{
		public struct TimeoutEvent
		{
			public readonly float ElapsedTime;

			public TimeoutEvent(float elapsedTime)
			{
				ElapsedTime = elapsedTime;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct LoadingScreenHiddenEvent
		{
		}

		private const string SHOW_LOG_MESSAGES_KEY = "Debug.LoadingScreen.ShowLogMessages";

		public float LoadingTimeout = 120f;

		private readonly List<UnityWebRequest> activeDownloads = new List<UnityWebRequest>();

		private readonly List<object> activeSystems = new List<object>();

		private readonly List<object> completedSystems = new List<object>();

		private List<string> logMessages = new List<string>();

		private bool done;

		private int hideDelay;

		private float startTime;

		private float elapsedTime;

		private static bool showLogMessages;

		private readonly DevCacheableType<bool> hudIndicatorEnabled = new DevCacheableType<bool>("cp.dev.HudLoadingIndicatorEnabled", false);

		private int hudIndicatorCount = 0;

		private Image hudIndicatorImage;

		private Animator hudIndicatorAnimator;

		public bool IsLoading
		{
			get
			{
				return activeSystems.Count > 0;
			}
		}

		public bool IsVisible
		{
			get
			{
				return base.gameObject.activeInHierarchy;
			}
		}

		public float LoadingTime
		{
			get
			{
				return elapsedTime;
			}
		}

		[Tweakable("Debug.LoadingScreen.ShowLogMessages")]
		public static bool ShowLogMessages
		{
			get
			{
				return showLogMessages;
			}
			set
			{
				showLogMessages = value;
				PlayerPrefs.SetInt("Debug.LoadingScreen.ShowLogMessages", value ? 1 : 0);
			}
		}

		public float? DownloadProgress
		{
			get
			{
				if (activeDownloads.Count == 0)
				{
					return null;
				}
				float num = 0f;
				float num2 = 0f;
				foreach (UnityWebRequest activeDownload in activeDownloads)
				{
					ulong downloadedBytes = activeDownload.downloadedBytes;
					float num3 = 0f;
					num3 = ((downloadedBytes != 0 && !(activeDownload.downloadProgress <= 0f)) ? ((float)downloadedBytes / activeDownload.downloadProgress) : 10000f);
					num2 += (float)downloadedBytes;
					num += num3;
				}
				if (num == 0f)
				{
					return null;
				}
				return Mathf.Clamp01(num2 / num);
			}
		}

		[Tweakable("Debug.LoadingScreen.HudIndicatorEnabled", Description = "Show a debug loading indicator on the screen when certain systems are performing intense operations.")]
		private bool HudIndicatorEnabled
		{
			get
			{
				return hudIndicatorEnabled.Value;
			}
			set
			{
				hudIndicatorEnabled.Value = value;
			}
		}

		public event System.Action OnDownloadingStarted;

		public event System.Action OnDownloadingComplete;

		public void Awake()
		{
			showLogMessages = (PlayerPrefs.GetInt("Debug.LoadingScreen.ShowLogMessages", 0) == 1);
		}

		[Conditional("NOT_RC_BUILD")]
		public void SetHudIndicator(GameObject hudIndicator)
		{
			Service.Get<ICommonGameSettings>().RegisterSetting(hudIndicatorEnabled, true);
			hudIndicatorImage = hudIndicator.GetComponent<Image>();
			hudIndicatorAnimator = hudIndicator.GetComponent<Animator>();
		}

		[Conditional("NOT_RC_BUILD")]
		public void PushHudIndicator()
		{
			if (hudIndicatorCount == 0 && hudIndicatorEnabled.Value && hudIndicatorImage != null && hudIndicatorAnimator != null)
			{
				hudIndicatorImage.enabled = true;
				hudIndicatorAnimator.enabled = true;
			}
			hudIndicatorCount++;
		}

		[Conditional("NOT_RC_BUILD")]
		public void PopHudIndicator()
		{
			hudIndicatorCount--;
			if (hudIndicatorCount < 0)
			{
				Log.LogError(this, "PopHudIndicator was called more times that PushHudIndicator");
				hudIndicatorCount = 0;
			}
			else if (hudIndicatorCount == 0 && hudIndicatorImage != null && hudIndicatorAnimator != null)
			{
				hudIndicatorImage.enabled = false;
				hudIndicatorAnimator.enabled = false;
			}
		}

		public void AddLoadingSystem(object system)
		{
			if (system == null)
			{
				throw new InvalidOperationException("Cannot add null system to LoadingController. Add request will be ignored.");
			}
			if (!IsLoading)
			{
				base.gameObject.SetActive(true);
				resetTimer();
				done = false;
				hideDelay = 1;
			}
			activeSystems.Add(system);
		}

		public bool HasLoadingSystem(object system)
		{
			return activeSystems.Contains(system);
		}

		public void RemoveLoadingSystem(object system)
		{
			if (system == null)
			{
				throw new InvalidOperationException("Cannot remove null system from LoadingController. Remove request will be ignored.");
			}
			activeSystems.Remove(system);
			completedSystems.Add(system);
			if (!IsLoading)
			{
				done = true;
				hideDelay = 5;
				UpdateElapsedTime();
			}
		}

		public void ClearAllLoadingSystems()
		{
			base.gameObject.SetActive(false);
			resetTimer();
		}

		public void RegisterDownload(UnityWebRequest request)
		{
			if (activeDownloads.Count == 0 && this.OnDownloadingStarted != null)
			{
				this.OnDownloadingStarted();
			}
			activeDownloads.Add(request);
		}

		public void UnRegisterDownload(UnityWebRequest request)
		{
			activeDownloads.Remove(request);
			if (activeDownloads.Count == 0 && this.OnDownloadingComplete != null)
			{
				this.OnDownloadingComplete();
			}
		}

		private void resetTimer()
		{
			startTime = Time.time;
			elapsedTime = 0f;
		}

		public void OnEnable()
		{
			done = false;
			hideDelay = 1;
		}

		public void OnDisable()
		{
			completedSystems.Clear();
			activeSystems.Clear();
			logMessages.Clear();
			activeDownloads.Clear();
			if (this.OnDownloadingComplete != null)
			{
				this.OnDownloadingComplete();
			}
		}

		public void Update()
		{
			if (done)
			{
				hideDelay--;
			}
			if (hideDelay <= 0)
			{
				base.gameObject.SetActive(false);
				Service.Get<EventDispatcher>().DispatchEvent(default(LoadingScreenHiddenEvent));
			}
			if (IsLoading && Time.time > startTime + LoadingTimeout)
			{
				Log.LogErrorFormatted(this, "Loading screen timed out! with blocking loads: {0}", string.Join(", ", activeSystems.ConvertAll((object a) => getLabel(a)).ToArray()));
				UpdateElapsedTime();
				Service.Get<EventDispatcher>().DispatchEvent(new TimeoutEvent(elapsedTime));
				ClearAllLoadingSystems();
			}
		}

		public void UpdateElapsedTime()
		{
			elapsedTime = Time.realtimeSinceStartup - startTime;
		}

		private string getLabel(object system)
		{
			UnityEngine.Object @object = system as UnityEngine.Object;
			return string.Format("{0} ({1})", (@object == null) ? "Unknown" : @object.name, system.GetType().Name);
		}
	}
}
