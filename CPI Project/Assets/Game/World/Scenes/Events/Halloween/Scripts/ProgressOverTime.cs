using ClubPenguin.Core;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Manimal.Common.Util;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Halloween
{
	public class ProgressOverTime : MonoBehaviour
	{
		public GameObject scenePrefab;

		public Image FillMeter;

		public GameObject FillMeterComplete;

		public Text ProgressText;

		public string InProgressToken;

		public string ProgressDoneToken;

		public float MinValue = 0f;

		public float MaxValue = 1.667f;

		public float FinalValue = 1.79f;

		public bool UsePersonalCfcDonations;

		public string DonationProgressId;

		[Min(1)]
		public int DonationCoinsToFill = 1500;

		public ScheduledEventDateDefinitionKey DateDefinitionKey;

		public ScheduledEventDateDefinitionKey[] AdditionalDateDefinitionKeys;

		public int UpdateIntervalMinutes;

		[Tooltip("Range to randomize updates, will be between -value and value")]
		public int UpdateRandomSeconds = 15;

		[Tooltip("Particles to be shown when meter increases. Ignored if blank")]
		public GameObject IncreaseEffect;

		public Vector3 EffectOffset = Vector3.zero;

		[Tooltip("Audio to play when meter increases. Ignored if blank")]
		public string AudioIncreaseEffect;

		private static readonly List<ProgressOverTime> DonationProgressBars = new List<ProgressOverTime>();

		private static readonly List<ProgressOverTime> DonationProgressSelectionBag = new List<ProgressOverTime>();

		private const string DonationProgressPlayerPrefsPrefix = "ol.CFCProgress.device.";

		private const string DonationProgressIdsPlayerPrefsKey = "ol.CFCProgressIds.device";

		private const string ProcessedDeviceDonationTotalPlayerPrefsKey = "ol.CFCProgressProcessedTotal.device";

		private static EventDispatcher DonationProgressDispatcher;

		private static bool DonationTotalRequestStarted;

		private static long PendingDeviceDonationTotal = -1L;

		private ScheduledEventDateDefinition DateDefinition;

		private List<ScheduledEventDateDefinition> additionalDateDefinitions;

		private INetworkServicesManager network;

		private Localizer localizer;

		private ParticleSystem partSys;

		private float donationProgress;

		private void Start()
		{
			network = Service.Get<INetworkServicesManager>();
			localizer = Service.Get<Localizer>();
			Dictionary<int, ScheduledEventDateDefinition> dateDefinitions = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>();
			if (dateDefinitions == null || !dateDefinitions.TryGetValue(DateDefinitionKey.Id, out DateDefinition))
			{
				Debug.LogError("ProgressOverTime could not find a scheduled event date definition with ID " + DateDefinitionKey.Id + ".", this);
				enabled = false;
				return;
			}
			additionalDateDefinitions = new List<ScheduledEventDateDefinition>();
			for (int i = 0; AdditionalDateDefinitionKeys != null && i < AdditionalDateDefinitionKeys.Length; i++)
			{
				ScheduledEventDateDefinitionKey key = AdditionalDateDefinitionKeys[i];
				if (!dateDefinitions.TryGetValue(key.Id, out ScheduledEventDateDefinition additionalDateDefinition))
				{
					Debug.LogError("ProgressOverTime could not find an additional scheduled event date definition with ID " + key.Id + ".", this);
					enabled = false;
					return;
				}
				additionalDateDefinitions.Add(additionalDateDefinition);
			}
			if (UsePersonalCfcDonations)
			{
				registerDonationProgressBar();
			}
			CheckWithinDateRange();
		}

		private void OnDestroy()
		{
			CancelInvoke("updateProgressRepeating");
			unregisterDonationProgressBar();
		}

		private void CheckWithinDateRange()
		{
			ContentSchedulerService contentSchedulerService = Service.Get<ContentSchedulerService>();
			bool isDuringScheduleEventDates = contentSchedulerService.IsDuringScheduleEventDates(DateDefinition);
			bool isAfterScheduleEventDates = contentSchedulerService.IsAfterScheduleEventDates(DateDefinition);
			for (int i = 0; i < additionalDateDefinitions.Count; i++)
			{
				ScheduledEventDateDefinition dateDefinition = additionalDateDefinitions[i];
				isDuringScheduleEventDates |= contentSchedulerService.IsDuringScheduleEventDates(dateDefinition);
				isAfterScheduleEventDates &= contentSchedulerService.IsAfterScheduleEventDates(dateDefinition);
			}
			if (isDuringScheduleEventDates)
			{
				if (ProgressText != null)
				{
					ProgressText.text = localizer.GetTokenTranslation(InProgressToken);
				}
				if (IncreaseEffect != null)
				{
					Vector3 localPosition = Vector3.zero;
					if (EffectOffset != Vector3.zero)
					{
						localPosition = EffectOffset;
					}
					GameObject gameObject = UnityEngine.Object.Instantiate(IncreaseEffect, base.gameObject.transform);
					gameObject.transform.localPosition = localPosition;
					if (gameObject != null)
					{
						partSys = gameObject.GetComponent<ParticleSystem>();
					}
				}
				if (UsePersonalCfcDonations)
				{
					updateProgress();
				}
				else if (UpdateIntervalMinutes > 0)
				{
					updateProgressRepeating();
				}
				else
				{
					updateProgress();
				}
			}
			else if (isAfterScheduleEventDates)
			{
				if (ProgressText != null)
				{
					ProgressText.text = localizer.GetTokenTranslation(ProgressDoneToken);
				}
				if (!UsePersonalCfcDonations && FillMeterComplete != null)
				{
					FillMeterComplete.SetActive(true);
				}
				updateProgress(UsePersonalCfcDonations ? 0f : FinalValue);
				CancelInvoke("updateProgressRepeating");
			}
		}

		private void updateProgressRepeating()
		{
			updateProgress();
			Invoke("updateProgressRepeating", (float)UpdateIntervalMinutes * 60f + UnityEngine.Random.Range((float)(-UpdateRandomSeconds), (float)UpdateRandomSeconds));
		}

		private void updateProgress(float manualPos = 0f)
		{
			if (UsePersonalCfcDonations)
			{
				updateDonationProgress(false);
				return;
			}
			if (scenePrefab != null)
			{
				Vector3 localPosition = scenePrefab.transform.localPosition;
				if (manualPos < 0f || manualPos > 0f)
				{
					localPosition.y = manualPos;
				}
				else
				{
					localPosition.y = CalculateProgress();
				}
				scenePrefab.transform.localPosition = localPosition;
				if (partSys != null)
				{
					partSys.Play();
				}
				playAudioEvent(AudioIncreaseEffect, base.gameObject);
			}
			if (FillMeter != null)
			{
				FillMeter.fillAmount = CalculateProgress();
			}
		}

		private void registerDonationProgressBar()
		{
			if (string.IsNullOrEmpty(DonationProgressId))
			{
				Debug.LogError("Donation progress is enabled but DonationProgressId is empty.", this);
				return;
			}
			if (!DonationProgressBars.Contains(this))
			{
				DonationProgressBars.Add(this);
			}
			donationProgress = Mathf.Clamp01(PlayerPrefs.GetFloat(getDonationProgressPlayerPrefsKey(DonationProgressId), 0f));
			registerDonationProgressId(DonationProgressId);
			if (DonationProgressDispatcher == null)
			{
				DonationProgressDispatcher = Service.Get<EventDispatcher>();
				DonationProgressDispatcher.AddListener<ScheduledEventServiceEvents.CFCDonationPosted>(onDonationPosted);
				DonationProgressDispatcher.AddListener<ScheduledEventServiceEvents.CFCDonationsLoaded>(onDonationsLoaded);
			}
			updateDonationProgress(false);
			if (!DonationTotalRequestStarted)
			{
				DonationTotalRequestStarted = true;
				network.ScheduledEventService.GetCFCDonations();
			}
		}

		private void unregisterDonationProgressBar()
		{
			DonationProgressBars.Remove(this);
			DonationProgressSelectionBag.Remove(this);
			if (DonationProgressBars.Count == 0 && DonationProgressDispatcher != null)
			{
				DonationProgressDispatcher.RemoveListener<ScheduledEventServiceEvents.CFCDonationPosted>(onDonationPosted);
				DonationProgressDispatcher.RemoveListener<ScheduledEventServiceEvents.CFCDonationsLoaded>(onDonationsLoaded);
				DonationProgressDispatcher = null;
				DonationTotalRequestStarted = false;
				DonationProgressSelectionBag.Clear();
			}
		}

		private static bool onDonationPosted(ScheduledEventServiceEvents.CFCDonationPosted evt)
		{
			if (evt.DonationResult == null)
			{
				Debug.LogError("CFC donation response is missing its result.");
				return false;
			}
			long deviceTotal = Math.Max(0L, evt.DonationResult.cfcTotal);
			if (!PlayerPrefs.HasKey(getPlatformKey(ProcessedDeviceDonationTotalPlayerPrefsKey)))
			{
				processHistoricalDonations(Math.Max(0L, deviceTotal - evt.DonationAmount));
			}
			if (evt.DonationAmount > 0)
			{
				applyDonationProgress(evt.DonationAmount, true);
			}
			setProcessedDeviceDonationTotal(Math.Max(deviceTotal, getProcessedDeviceDonationTotal()));
			return false;
		}

		private static bool onDonationsLoaded(ScheduledEventServiceEvents.CFCDonationsLoaded evt)
		{
			if (evt.CFCDonations == null)
			{
				Debug.LogError("Loaded CFC donations response is missing its data.");
				return false;
			}
			PendingDeviceDonationTotal = Math.Max(0L, evt.CFCDonations.cfcTotal);
			if (DonationProgressBars.Count > 0)
			{
				ProgressOverTime progressBar = DonationProgressBars[0];
				progressBar.CancelInvoke("processPendingDeviceDonationTotal");
				progressBar.Invoke("processPendingDeviceDonationTotal", 0f);
			}
			return false;
		}

		private void processPendingDeviceDonationTotal()
		{
			if (PendingDeviceDonationTotal < 0L)
			{
				return;
			}
			long deviceTotal = PendingDeviceDonationTotal;
			PendingDeviceDonationTotal = -1L;
			bool hasProcessedTotal = PlayerPrefs.HasKey(getPlatformKey(ProcessedDeviceDonationTotalPlayerPrefsKey));
			long previousTotal = getProcessedDeviceDonationTotal();
			long donationDelta = hasProcessedTotal ? Math.Max(0L, deviceTotal - previousTotal) : deviceTotal;
			processHistoricalDonations(donationDelta);
			setProcessedDeviceDonationTotal(Math.Max(deviceTotal, previousTotal));
		}

		private static void processHistoricalDonations(long donationAmount)
		{
			int donationStep = int.MaxValue;
			foreach (ProgressOverTime bar in DonationProgressBars)
			{
				if (bar != null && bar.donationProgress < 1f)
				{
					donationStep = Math.Min(donationStep, Math.Max(1, bar.DonationCoinsToFill / 10));
				}
			}
			if (donationStep == int.MaxValue)
			{
				return;
			}
			while (donationAmount > 0)
			{
				int amount = (int)Math.Min(donationAmount, donationStep);
				if (!applyDonationProgress(amount, false))
				{
					return;
				}
				donationAmount -= amount;
			}
		}

		private static bool applyDonationProgress(int donationAmount, bool playIncreaseEffects)
		{
			if (DonationProgressSelectionBag.Count == 0)
			{
				foreach (ProgressOverTime bar in DonationProgressBars)
				{
					if (bar != null && bar.donationProgress < 1f)
					{
						DonationProgressSelectionBag.Add(bar);
					}
				}
				for (int i = DonationProgressSelectionBag.Count - 1; i > 0; i--)
				{
					int randomIndex = UnityEngine.Random.Range(0, i + 1);
					ProgressOverTime bar = DonationProgressSelectionBag[i];
					DonationProgressSelectionBag[i] = DonationProgressSelectionBag[randomIndex];
					DonationProgressSelectionBag[randomIndex] = bar;
				}
			}
			if (DonationProgressSelectionBag.Count > 0)
			{
				ProgressOverTime selectedBar = DonationProgressSelectionBag[0];
				DonationProgressSelectionBag.RemoveAt(0);
				float progressStep = Mathf.Clamp01((float)donationAmount / Mathf.Max(1, selectedBar.DonationCoinsToFill));
				selectedBar.donationProgress = Mathf.Clamp01(selectedBar.donationProgress + progressStep);
				selectedBar.updateDonationProgress(playIncreaseEffects);
				if (selectedBar.donationProgress >= 1f)
				{
					DonationProgressSelectionBag.Remove(selectedBar);
				}
				saveDonationProgress(selectedBar);
				return true;
			}
			return false;
		}

		private static long getProcessedDeviceDonationTotal()
		{
			string value = PlayerPrefs.GetString(getPlatformKey(ProcessedDeviceDonationTotalPlayerPrefsKey), "0");
			if (long.TryParse(value, out long total))
			{
				return Math.Max(0L, total);
			}
			Debug.LogError("Unable to read saved device CFC total: " + value);
			return 0L;
		}

		private static void setProcessedDeviceDonationTotal(long total)
		{
			PlayerPrefs.SetString(getPlatformKey(ProcessedDeviceDonationTotalPlayerPrefsKey), Math.Max(0L, total).ToString());
			PlayerPrefs.Save();
		}

		private static void saveDonationProgress(ProgressOverTime bar)
		{
			PlayerPrefs.SetFloat(getDonationProgressPlayerPrefsKey(bar.DonationProgressId), bar.donationProgress);
			PlayerPrefs.Save();
		}

		public static void ResetDeviceDonationProgress()
		{
			string[] donationProgressIds = PlayerPrefs.GetString(getPlatformKey(DonationProgressIdsPlayerPrefsKey), string.Empty).Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string donationProgressId in donationProgressIds)
			{
				PlayerPrefs.DeleteKey(getDonationProgressPlayerPrefsKey(donationProgressId));
			}
			PlayerPrefs.DeleteKey(getPlatformKey(DonationProgressIdsPlayerPrefsKey));
			PlayerPrefs.DeleteKey(getPlatformKey(ProcessedDeviceDonationTotalPlayerPrefsKey));
			PlayerPrefs.DeleteKey(getPlatformKey("ol.CFCProgressPendingDonations.device"));
			DonationProgressSelectionBag.Clear();
			foreach (ProgressOverTime bar in DonationProgressBars)
			{
				if (bar != null)
				{
					bar.donationProgress = 0f;
					bar.updateDonationProgress(false);
					registerDonationProgressId(bar.DonationProgressId);
				}
			}
			setProcessedDeviceDonationTotal(0L);
		}

		private static void registerDonationProgressId(string donationProgressId)
		{
			string storedIds = PlayerPrefs.GetString(getPlatformKey(DonationProgressIdsPlayerPrefsKey), string.Empty);
			List<string> donationProgressIds = new List<string>(storedIds.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
			if (!donationProgressIds.Contains(donationProgressId))
			{
				donationProgressIds.Add(donationProgressId);
				PlayerPrefs.SetString(getPlatformKey(DonationProgressIdsPlayerPrefsKey), string.Join("|", donationProgressIds.ToArray()));
				PlayerPrefs.Save();
			}
		}

		private static string getDonationProgressPlayerPrefsKey(string donationProgressId)
		{
			return getPlatformKey(DonationProgressPlayerPrefsPrefix + donationProgressId);
		}

		private static string getPlatformKey(string key)
		{
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
			if (Application.isEditor)
			{
				return "Editor_" + key;
			}
#endif
			return key;
		}

		private void updateDonationProgress(bool playIncreaseEffects)
		{
			if (scenePrefab != null)
			{
				Vector3 localPosition = scenePrefab.transform.localPosition;
				localPosition.y = Mathf.Lerp(MinValue, MaxValue, donationProgress);
				scenePrefab.transform.localPosition = localPosition;
				if (playIncreaseEffects)
				{
					if (partSys != null)
					{
						partSys.Play();
					}
					playAudioEvent(AudioIncreaseEffect, base.gameObject);
				}
			}
			if (FillMeter != null)
			{
				FillMeter.fillAmount = donationProgress;
			}
		}

		public float CalculateProgress()
		{
			DateTime dateTime = Service.Get<ContentSchedulerService>().ScheduledEventDate();
			DateTime serverTime = getServerTime();
			DateTime d = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, serverTime.Hour, serverTime.Minute, serverTime.Second);
			DateTime date = DateDefinition.Dates.StartDate.Date;
			DateTime date2 = DateDefinition.Dates.EndDate.Date;
			double num;
			if (d.Day > date2.Day)
			{
				num = FinalValue;
			}
			else
			{
				double totalMinutes = (date2 - date).TotalMinutes;
				double num2 = (UpdateIntervalMinutes <= 0) ? totalMinutes : (totalMinutes / (double)UpdateIntervalMinutes);
				double totalMinutes2 = (d - date).TotalMinutes;
				float num3 = Mathf.Abs(MaxValue - MinValue);
				double num4 = (double)num3 / num2;
				double num5 = (UpdateIntervalMinutes <= 0) ? totalMinutes2 : (totalMinutes2 / (double)UpdateIntervalMinutes);
				num = num5 * num4;
			}
			return (float)num;
		}

		private DateTime getServerTime()
		{
			if (network != null)
			{
				long gameTimeMilliseconds = network.GameTimeMilliseconds;
				if (gameTimeMilliseconds > 0)
				{
					return gameTimeMilliseconds.MsToDateTime();
				}
			}
			return DateTime.MinValue;
		}

		private void playAudioEvent(string audioEventName, GameObject anchorObj = null)
		{
			if (!string.IsNullOrEmpty(audioEventName))
			{
				if (anchorObj != null)
				{
					EventManager.Instance.PostEvent(audioEventName, EventAction.PlaySound, anchorObj);
				}
				else
				{
					EventManager.Instance.PostEvent(audioEventName, EventAction.PlaySound);
				}
			}
		}
	}
}
