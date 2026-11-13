using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.MiniGames.Fishing;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;
using Disney.Manimal.Common.Util;

namespace ClubPenguin.UI
{
	public class FishingButtonUIUpdater : MonoBehaviour
	{
		private const string MINIGAME_PROGRESS_KEY = "fishing";

		private const int MaxBaitCount = 10;

		private TrayInputButton trayInputButton;

		private GameObject baitUI;

		private GameObject clock;

		private GameObject timerTooltip;

		private TooltipInputButton tooltipInputButton;

		private Text baitText;

		private Text timerText;

		private Timer updateTimer;

		private DateTime resetDailyDateTime;

		private INetworkServicesManager networkServiceManager;

		private EventChannel eventChannel;

		private int baitCount;

		private bool casted;

		private bool isQuest;

		private void Start()
		{
			bool flag = false;
			networkServiceManager = Service.Get<INetworkServicesManager>();
			Button componentInParent = GetComponentInParent<Button>();
			if (componentInParent != null)
			{
				trayInputButton = componentInParent.GetComponent<TrayInputButton>();
				tooltipInputButton = componentInParent.gameObject.AddComponent<TooltipInputButton>();
				Transform transform = findLayoutPanel();
				if (transform != null)
				{
					baitUI = transform.Find("BaitUI").gameObject;
					baitText = baitUI.GetComponentInChildren<Text>();
					clock = transform.Find("ClockPanel").gameObject;
					clock.SetActive(false);
					timerTooltip = transform.Find("Tooltip").gameObject;
					timerText = timerTooltip.transform.Find("TimerText").GetComponent<Text>();
					timerTooltip.GetComponent<Button>().onClick.AddListener(onTooltipClicked);
					tooltipInputButton.TooltipAnimator = timerTooltip.GetComponent<Animator>();
					tooltipInputButton.TooltipEnabled = false;
					SubscribeToEvents();
					updateTimer = new Timer(1f, true, delegate
					{
						OnTimerTick();
					});
					CoroutineRunner.Start(updateTimer.Start(), this, "FishingButtonTimer");
					updateBaitUI();
					flag = true;
				}
			}
			if (!flag)
			{
				Log.LogError(this, "Failed to find all the UI components required to initialize FishingButtonUIUpdater. Aborting");
			}
		}

		private void SubscribeToEvents()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<FishingEvents.UpdateFishingBaitUI>(onUpdateFishingBaitUI);
			eventChannel.AddListener<FishingEvents.SetTimeRemaining>(setTimeRemaining);
			eventChannel.AddListener<FishingEvents.SetFishingState>(onSetFishingState);
		}

		private void onTooltipClicked()
		{
			tooltipInputButton.CloseTooltip();
		}

		private void CalculateTomorrowMidnight()
		{
			resetDailyDateTime = Service.Get<INetworkServicesManager>().ServerDateTime.AddDays(1.0).Date;
		}

		private bool onUpdateFishingBaitUI(FishingEvents.UpdateFishingBaitUI evt)
		{
			updateBaitUI();
			return false;
		}

		private void updateBaitUI()
		{
			isQuest = !string.IsNullOrEmpty(Service.Get<QuestService>().CurrentFishingPrize);
			if (isQuest)
			{
				baitUI.SetActive(false);
				clock.SetActive(false);
				tooltipInputButton.TooltipEnabled = false;
				if (!casted)
				{
					trayInputButton.SetState(TrayInputButton.ButtonState.Pulsing);
				}
				else
				{
					trayInputButton.SetState(TrayInputButton.ButtonState.Default);
				}
				return;
			}
			int fishingPlayCount = GetFishingPlayCount();
			baitCount = 10 - fishingPlayCount;
			baitText.text = baitCount.ToString();
			if (baitCount == 0)
			{
				baitUI.SetActive(false);
				CalculateTomorrowMidnight();
				clock.SetActive(true);
				tooltipInputButton.TooltipEnabled = true;
				trayInputButton.SetState(TrayInputButton.ButtonState.Highlighted);
			}
			else
			{
				baitUI.SetActive(true);
				clock.SetActive(false);
				tooltipInputButton.TooltipEnabled = false;
				setActiveTrayState();
			}
		}

		private int GetFishingPlayCount()
		{
			if (networkServiceManager != null && Service.Get<ICommonGameSettings>().OfflineMode)
			{
				string username = Service.Get<RememberMeService>().CurrentUsername;
				if (string.IsNullOrEmpty(username))
				{
					return 0;
				}
				long num = DateTime.UtcNow.Date.GetTimeInMilliseconds();
				long storedDay = 0L;
				string storedDayStr = PlayerPrefs.GetString(GetPlatformKey("OfflineMinigameProgress_Day_" + username), "0");
				long.TryParse(storedDayStr, out storedDay);
				if (storedDay != num)
				{
					return 0;
				}
				return PlayerPrefs.GetInt(GetPlatformKey("OfflineMinigameProgress_fishing_" + username), 0);
			}
			else
			{
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				MiniGamePlayCountData component;
				if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
				{
					if (!component.MinigamePlayCounts.ContainsKey(MINIGAME_PROGRESS_KEY))
					{
						component.SetMinigamePlayCount(MINIGAME_PROGRESS_KEY, 0);
					}
					return component.MinigamePlayCounts[MINIGAME_PROGRESS_KEY];
				}
			}
			return 0;
		}

		private bool setTimeRemaining(FishingEvents.SetTimeRemaining evt)
		{
			return false;
		}

		private bool onSetFishingState(FishingEvents.SetFishingState evt)
		{
			if (evt.State == FishingEvents.FishingState.Hold)
			{
				if (isQuest)
				{
					baitUI.SetActive(false);
					setActiveTrayState();
				}
				else if (baitCount == 0)
				{
					trayInputButton.SetState(TrayInputButton.ButtonState.Highlighted);
				}
				else
				{
					baitUI.SetActive(true);
					setActiveTrayState();
				}
			}
			else if (evt.State == FishingEvents.FishingState.Cast)
			{
				baitUI.SetActive(false);
				casted = true;
			}
			return false;
		}

		private void setActiveTrayState()
		{
			if (!casted)
			{
				trayInputButton.SetState(TrayInputButton.ButtonState.Pulsing);
			}
			else
			{
				trayInputButton.SetState(TrayInputButton.ButtonState.Default);
			}
		}

		private Transform findLayoutPanel()
		{
			if (base.transform != null)
			{
				Transform parent = base.transform.parent;
				while (parent != null)
				{
					if (parent.name == "LayoutPanel")
					{
						return parent;
					}
					parent = parent.parent;
				}
			}
			return null;
		}

		private void OnTimerTick()
		{
			if (networkServiceManager != null)
			{
				TimeSpan time = resetDailyDateTime - networkServiceManager.ServerDateTime;
				if (time.TotalSeconds > 0.0)
				{
					timerText.gameObject.SetActive(true);
					timerText.text = TimeFormatUtils.FormatTimeString(time);
				}
				else
				{
					clock.SetActive(false);
					tooltipInputButton.CloseTooltip();
					tooltipInputButton.TooltipEnabled = false;
				}
			}
		}

		private static string GetPlatformKey(string key)
		{
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
			if (UnityEngine.Application.isEditor)
			{
				return "Editor_" + key;
			}
#endif
			return key;
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			eventChannel.RemoveAllListeners();
			updateTimer.Stop();
			timerTooltip.GetComponent<Button>().onClick.RemoveListener(onTooltipClicked);
			if (tooltipInputButton != null)
			{
				UnityEngine.Object.Destroy(tooltipInputButton);
			}
		}
	}
}
