using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.NPC;
using ClubPenguin.Progression;
using ClubPenguin.Rewards;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneActivityScreenDailySpinWidget : MonoBehaviour, ICellPhoneAcitivtyScreenWidget
	{
		public const string DAILY_SPIN_ACITIVITY_DEFINITION_KEY = "Definitions/CellPhoneActivities/DailySpin/CellPhoneActivity_DailySpin";

		private const int TICK_OFF_INDEX = 0;

		private const int TICK_ON_INDEX = 1;

		private const string DAILY_SPIN_BUTTON_ID = "DailySpinButton";

		private const string HOURS_LETTER_TOKEN = "Cellphone.DailySpin.SpinTimer.Hours.Letter";

		private const string MINUTES_LETTER_TOKEN = "Cellphone.DailySpin.SpinTime.Minutes.Letter";

		private const string TITLE_TEXT_TOKEN = "Cellphone.DailySpin.Unlock";

		public DailySpinWheel DailySpinWheel;

		public GameObject ChestLevelContainer;

		public Text ChestLevelNameText;

		public Text ChestLevelValueText;

		public GameObject SpinTimerContainer;

		public Text SpinTimerHours;

		public Text SpinTimerMinutes;

		public SpriteSelector ChestSpriteSelector;

		public GameObject TickPanel;

		public GameObject SpinButton;

		public Text SpinButtonText;

		public DailySpinRewardScreen RewardScreen;

		public DailySpinRewardPopup RewardPopup;

		public GameObject SpinScreen;

		public Text TitleText;

		public float CoinXpRewardDelay = 2f;

		public float RewardShowDelay = 1f;

		public float ShowChestDelay = 1.5f;

		public float MoveToBottomDelay = 1.5f;

		private DailySpinTick[] ticks;

		private CellPhoneDailySpinActivityDefinition widgetData;

		private DailySpinEntityData spinData;

		private DailySpinEntityData showingSpinData;

		private CellPhoneDailySpinActivityDefinition.ChestDefinition currentChestDefinition;

		private EventDispatcher dispatcher;

		private CPDataEntityCollection dataEntityCollection;

		private Localizer localizer;

		private Reward lastSpinReward;

		private Reward lastSpinChestReward;

		private int lastSpinOutcomeId;

		private bool showingRewardChest;

		private Color SPIN_BUTTON_TEXT_DISABLED_COLOR = new Color(0.8f, 0.8f, 0.8f);

		public CellPhoneDailySpinActivityDefinition WidgetData
		{
			get
			{
				return widgetData;
			}
		}

		private void Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			localizer = Service.Get<Localizer>();
			ChestLevelContainer.SetActive(true);
			SpinTimerContainer.SetActive(false);
			DailySpinRewardScreen rewardScreen = RewardScreen;
			rewardScreen.RewardScreenComplete = (System.Action)Delegate.Combine(rewardScreen.RewardScreenComplete, new System.Action(OnRewardScreenComplete));
			DailySpinRewardPopup rewardPopup = RewardPopup;
			rewardPopup.RewardPopupComplete = (System.Action)Delegate.Combine(rewardPopup.RewardPopupComplete, new System.Action(OnRewardPopupComplete));
			DailySpinWheel.SetOverlayState(DailySpinWheel.WheelOverlayState.Center);
			ticks = TickPanel.GetComponentsInChildren<DailySpinTick>();
			SetSpinButtonEnabled(false);
			Content.LoadAsync<ScriptableObject>("Definitions/CellPhoneActivities/DailySpin/CellPhoneActivity_DailySpin", onActivityDefinitionLoaded);
			ClubPenguin.Core.SceneRefs.Set(this);
		}

		private void OnDestroy()
		{
			dispatcher.RemoveListener<RewardServiceEvents.ClaimDailySpinRewardSuccess>(onClaimSuccess);
			dispatcher.RemoveListener<RewardServiceEvents.ClaimDailySpinRewardFailed>(onClaimFail);
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("DailySpinButton"));
			DailySpinWheel dailySpinWheel = DailySpinWheel;
			dailySpinWheel.OnSpinComplete = (System.Action)Delegate.Remove(dailySpinWheel.OnSpinComplete, new System.Action(onSpinComplete));
			DailySpinRewardScreen rewardScreen = RewardScreen;
			rewardScreen.RewardScreenComplete = (System.Action)Delegate.Remove(rewardScreen.RewardScreenComplete, new System.Action(OnRewardScreenComplete));
			CoroutineRunner.StopAllForOwner(this);
			ClubPenguin.Core.SceneRefs.Remove(this);
		}

		public void SetWidgetData(CellPhoneActivityDefinition widgetData)
		{
		}

		private void onActivityDefinitionLoaded(string Path, ScriptableObject definition)
		{
			widgetData = (definition as CellPhoneDailySpinActivityDefinition);
			loadPlayerSpinData();
		}

		private void loadPlayerSpinData()
		{
			dataEntityCollection.When<DailySpinEntityData>(dataEntityCollection.LocalPlayerHandle, onSpinDataAdded);
		}

		private void onSpinDataAdded(DailySpinEntityData spinData)
		{
			for (int i = 0; i < widgetData.ChestDefinitions.Count; i++)
			{
				if (widgetData.ChestDefinitions[i].ChestId == spinData.CurrentChestId)
				{
					currentChestDefinition = widgetData.ChestDefinitions[i];
					break;
				}
			}
			ChestSpriteSelector.Select(currentChestDefinition.ChestId);
			ChestLevelNameText.text = localizer.GetTokenTranslation(currentChestDefinition.ChestNameToken);
			if (currentChestDefinition.NumChestsToNextLevel == -1)
			{
				ChestLevelValueText.gameObject.SetActive(false);
			}
			else
			{
				ChestLevelValueText.text = string.Format(" {0}/{1}", spinData.NumChestsReceivedOfCurrentChestId + 1, currentChestDefinition.NumChestsToNextLevel);
			}
			TitleText.text = string.Format(localizer.GetTokenTranslation("Cellphone.DailySpin.Unlock"), localizer.GetTokenTranslation(currentChestDefinition.ChestTypeToken));
			int numPunchesPerChest = currentChestDefinition.NumPunchesPerChest;
			SetChestTicks(numPunchesPerChest, spinData.NumPunchesOnCurrentChest);
			DailySpinWheel dailySpinWheel = DailySpinWheel;
			dailySpinWheel.OnSpinComplete = (System.Action)Delegate.Combine(dailySpinWheel.OnSpinComplete, new System.Action(onSpinComplete));
			if (!CheckLastSpinTime(spinData.TimeOfLastSpinInMilliseconds))
			{
				if (showingSpinData != null)
				{
					SetChestTicks(currentChestDefinition.NumPunchesPerChest, showingSpinData.NumPunchesOnCurrentChest);
				}
				DailySpinWheel.SetOverlayState(DailySpinWheel.WheelOverlayState.Full);
			}
			this.spinData = spinData;
			showingSpinData = new DailySpinEntityData();
			showingSpinData.CurrentChestId = spinData.CurrentChestId;
			showingSpinData.NumChestsReceivedOfCurrentChestId = spinData.NumChestsReceivedOfCurrentChestId;
			showingSpinData.NumPunchesOnCurrentChest = spinData.NumPunchesOnCurrentChest;
			showingSpinData.TimeOfLastSpinInMilliseconds = spinData.TimeOfLastSpinInMilliseconds;
			DailySpinWheel.SetWidgetData(widgetData, currentChestDefinition.ChestId);
		}

		public void OnSpinPressed()
		{
			dispatcher.AddListener<RewardServiceEvents.ClaimDailySpinRewardSuccess>(onClaimSuccess);
			dispatcher.AddListener<RewardServiceEvents.ClaimDailySpinRewardFailed>(onClaimFail);
			Service.Get<INetworkServicesManager>().RewardService.ClaimDailySpinReward();
			SetSpinButtonEnabled(false);
		}

		private bool onClaimSuccess(RewardServiceEvents.ClaimDailySpinRewardSuccess evt)
		{
			dispatcher.RemoveListener<RewardServiceEvents.ClaimDailySpinRewardSuccess>(onClaimSuccess);
			dispatcher.RemoveListener<RewardServiceEvents.ClaimDailySpinRewardFailed>(onClaimFail);
			lastSpinReward = evt.Reward;
			lastSpinChestReward = evt.ChestReward;
			lastSpinOutcomeId = evt.SpinOutcomeId;
			spinData.TimeOfLastSpinInMilliseconds = Service.Get<ContentSchedulerService>().ScheduledEventDate().GetTimeInMilliseconds();
			showingSpinData.TimeOfLastSpinInMilliseconds = spinData.TimeOfLastSpinInMilliseconds;
			updateSpinDataFromSpin();
			DailySpinWheel.StartSpin(evt.SpinOutcomeId);
			DailySpinWheel.SetOverlayState(DailySpinWheel.WheelOverlayState.None);
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyRewardEarned(RewardSource.CLAIMABLE_REWARD, "daily_spin", evt.Reward, false));
			if (evt.ChestReward != null)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyRewardEarned(RewardSource.CLAIMABLE_REWARD, "daily_spin", evt.ChestReward, false));
			}
			logSpinBI(lastSpinReward, lastSpinChestReward);
			return false;
		}

		private void updateSpinDataFromSpin()
		{
			if ((lastSpinChestReward != null && !lastSpinChestReward.isEmpty()) || lastSpinOutcomeId == widgetData.ChestSpinOutcomeId)
			{
				spinData.NumChestsReceivedOfCurrentChestId++;
				if (spinData.NumChestsReceivedOfCurrentChestId == currentChestDefinition.NumChestsToNextLevel)
				{
					spinData.CurrentChestId++;
					spinData.NumChestsReceivedOfCurrentChestId = 0;
				}
				spinData.NumPunchesOnCurrentChest = 0;
			}
			else
			{
				spinData.NumPunchesOnCurrentChest++;
			}
		}

		private bool onClaimFail(RewardServiceEvents.ClaimDailySpinRewardFailed evt)
		{
			dispatcher.RemoveListener<RewardServiceEvents.ClaimDailySpinRewardSuccess>(onClaimSuccess);
			dispatcher.RemoveListener<RewardServiceEvents.ClaimDailySpinRewardFailed>(onClaimFail);
			SetSpinButtonEnabled(true);
			return false;
		}

		public void SetChestTicks(int totalSpots, int numTicks, bool showHighlight = false)
		{
			for (int i = 0; i < ticks.Length; i++)
			{
				if (i < totalSpots)
				{
					if (i < numTicks)
					{
						if (showHighlight && ticks[i].State != DailySpinTick.TickState.On)
						{
							ticks[i].SetState(DailySpinTick.TickState.Highlighted);
						}
						else
						{
							ticks[i].SetState(DailySpinTick.TickState.On);
						}
					}
					else
					{
						ticks[i].SetState(DailySpinTick.TickState.Off);
					}
				}
				else
				{
					ticks[i].SetState(DailySpinTick.TickState.Hidden);
				}
			}
		}

		private void onSpinComplete()
		{
			if (lastSpinOutcomeId == widgetData.ChestSpinOutcomeId)
			{
				showingSpinData.NumPunchesOnCurrentChest = currentChestDefinition.NumPunchesPerChest;
			}
			else
			{
				showingSpinData.NumPunchesOnCurrentChest++;
			}
			CoroutineRunner.Start(showSpinRewards(), this, "DailySpinShowSpinRewards");
		}

		private IEnumerator showSpinRewards()
		{
			yield return new WaitForSeconds(RewardShowDelay);
			if (lastSpinOutcomeId == widgetData.ChestSpinOutcomeId)
			{
				OnChestReceived();
				DailySpinWheel.SetOverlayState(DailySpinWheel.WheelOverlayState.Full);
			}
			else if (lastSpinOutcomeId == widgetData.RespinSpinOutcomeId)
			{
				showCoinXPReward(lastSpinReward);
			}
			else
			{
				showCoinXPReward(lastSpinReward);
			}
		}

		private void showCoinXPReward(Reward reward)
		{
			CoinsData component = dataEntityCollection.GetComponent<CoinsData>(dataEntityCollection.LocalPlayerHandle);
			CoinReward rewardable;
			if (reward.TryGetValue(out rewardable) && rewardable.Coins > 0)
			{
				component.RemoveCoins(rewardable.Coins);
				component.AddCoins(rewardable.Coins);
			}
			MascotXPReward rewardable2;
			if (reward.TryGetValue(out rewardable2))
			{
				using (Dictionary<string, int>.Enumerator enumerator = rewardable2.XP.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<string, int> current = enumerator.Current;
						ProgressionService progressionService = Service.Get<ProgressionService>();
						if (!progressionService.IsMascotMaxLevel(current.Key))
						{
							dispatcher.DispatchEvent(new RewardEvents.AddXP(current.Key, progressionService.GetMascotXP(current.Key, -current.Value), progressionService.GetMascotXP(current.Key), current.Value, true));
						}
					}
				}
			}
			RewardPopup.ShowReward(reward, lastSpinOutcomeId == widgetData.RespinSpinOutcomeId);
		}

		private void OnChestReceived()
		{
			showingSpinData.NumChestsReceivedOfCurrentChestId++;
			bool flag = showingSpinData.NumChestsReceivedOfCurrentChestId == currentChestDefinition.NumChestsToNextLevel;
			SpinScreen.SetActive(false);
			Reward chestReward = (lastSpinChestReward != null && !lastSpinChestReward.isEmpty()) ? lastSpinChestReward : lastSpinReward;
			RewardScreen.ShowChest(chestReward, widgetData, showingSpinData, currentChestDefinition, flag);
			if (!flag)
			{
				return;
			}
			showingSpinData.CurrentChestId++;
			showingSpinData.NumChestsReceivedOfCurrentChestId = 0;
			for (int i = 0; i < widgetData.ChestDefinitions.Count; i++)
			{
				if (widgetData.ChestDefinitions[i].ChestId == showingSpinData.CurrentChestId)
				{
					currentChestDefinition = widgetData.ChestDefinitions[i];
					break;
				}
			}
			ChestSpriteSelector.Select(currentChestDefinition.ChestId);
			DailySpinWheel.SetCurrentChestId(currentChestDefinition.ChestId);
		}

		private bool CheckLastSpinTime(long lastSpinTime)
		{
			bool result = false;
			DateTime value = Service.Get<ContentSchedulerService>().ScheduledEventDate();
			DateTime dateTime = lastSpinTime.MsToDateTime();
			if (lastSpinTime == 0 || value.Date > dateTime.Date)
			{
				result = true;
				SetSpinButtonEnabled(true);
			}
			else
			{
				TimeSpan timeSpan = new DateTime(value.Year, value.Month, value.Day, 23, 59, 59).Subtract(value);
				updateCountdownText(timeSpan);
				CoroutineRunner.Start(runTimerCountdown(timeSpan), this, "DailySpinCountdown");
				SpinTimerContainer.SetActive(true);
				ChestLevelContainer.SetActive(false);
				SetSpinButtonEnabled(false);
			}
			return result;
		}

		private IEnumerator runTimerCountdown(TimeSpan countdownTime)
		{
			TimeSpan updateInterval = TimeSpan.FromSeconds(10.0);
			while (countdownTime > TimeSpan.Zero)
			{
				yield return new WaitForSecondsRealtime(10f);
				countdownTime = countdownTime.Subtract(updateInterval);
				updateCountdownText(countdownTime);
			}
			SpinTimerContainer.SetActive(false);
			ChestLevelContainer.SetActive(true);
			SetSpinButtonEnabled(true);
			DailySpinWheel.SetOverlayState(DailySpinWheel.WheelOverlayState.Center);
		}

		private void updateCountdownText(TimeSpan timeLeft)
		{
			SpinTimerHours.text = timeLeft.Hours.ToString();
			SpinTimerMinutes.text = timeLeft.Minutes.ToString();
		}

		private void OnRewardScreenComplete()
		{
			showingSpinData.NumPunchesOnCurrentChest = 0;
			SetChestTicks(currentChestDefinition.NumPunchesPerChest, showingSpinData.NumPunchesOnCurrentChest);
			if (currentChestDefinition.NumChestsToNextLevel == -1)
			{
				ChestLevelValueText.gameObject.SetActive(false);
			}
			else
			{
				ChestLevelValueText.text = string.Format(" {0}/{1}", showingSpinData.NumChestsReceivedOfCurrentChestId, currentChestDefinition.NumChestsToNextLevel);
			}
			CheckLastSpinTime(showingSpinData.TimeOfLastSpinInMilliseconds);
			if (lastSpinOutcomeId == widgetData.RespinSpinOutcomeId)
			{
				DailySpinWheel.SetOverlayState(DailySpinWheel.WheelOverlayState.Center);
				SetSpinButtonEnabled(true);
			}
			else
			{
				DailySpinWheel.SetOverlayState(DailySpinWheel.WheelOverlayState.Full);
			}
			SpinScreen.SetActive(true);
		}

		private void OnRewardPopupComplete()
		{
			if (showingSpinData.NumPunchesOnCurrentChest == currentChestDefinition.NumPunchesPerChest)
			{
				CoroutineRunner.Start(OnChestReceivedDelayed(), this, "OnChestReceivedDelayed");
			}
			else if (lastSpinOutcomeId == widgetData.RespinSpinOutcomeId)
			{
				SetSpinButtonEnabled(true);
				DailySpinWheel.SetOverlayState(DailySpinWheel.WheelOverlayState.Center);
			}
			else
			{
				DailySpinWheel.SetOverlayState(DailySpinWheel.WheelOverlayState.Full);
			}
			SetChestTicks(currentChestDefinition.NumPunchesPerChest, showingSpinData.NumPunchesOnCurrentChest, true);
		}

		private IEnumerator OnChestReceivedDelayed()
		{
			yield return new WaitForSeconds(ShowChestDelay);
			OnChestReceived();
		}

		private void SetSpinButtonEnabled(bool enabled)
		{
			if (enabled)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("DailySpinButton"));
				SpinButtonText.color = Color.white;
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("DailySpinButton"));
				SpinButtonText.color = SPIN_BUTTON_TEXT_DISABLED_COLOR;
			}
			SpinButton.GetComponent<Animator>().enabled = enabled;
		}

		private void logSpinBI(Reward reward, Reward chestReward)
		{
			string tier = "";
			CoinReward rewardable;
			MascotXPReward rewardable2;
			if (lastSpinOutcomeId == widgetData.ChestSpinOutcomeId)
			{
				tier = "Chest";
			}
			else if (reward.TryGetValue(out rewardable) && rewardable.Coins > 0)
			{
				tier = "Coins";
			}
			else if (reward.TryGetValue(out rewardable2))
			{
				using (Dictionary<string, int>.Enumerator enumerator = rewardable2.XP.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<string, int> current = enumerator.Current;
						ProgressionService progressionService = Service.Get<ProgressionService>();
						tier = ((!progressionService.IsMascotMaxLevel(current.Key)) ? "XP" : "Party Blaster");
					}
				}
			}
			Service.Get<ICPSwrveService>().Action("daily_spin", currentChestDefinition.ChestNameToken, tier);
		}
	}
}
