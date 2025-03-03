using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ClubPenguin.CellPhone
{
	public class ActivityNotificationManager : MonoBehaviour
	{
		private readonly ActivityNotificationScheduleContentKey scheduleKey = new ActivityNotificationScheduleContentKey("Definitions/ActivityNotificationSchedule/ActivityNotificationSchedule");

		private readonly PrefabContentKey notificationKey = new PrefabContentKey("Prefabs/Notifications/ActivityTrackerNotificationPanel");

		public float NotificationShowTime = 5f;

		private List<CellPhoneRecurringLocationActivityDefinition> recurringActivityDefinitions;

		private List<ActivityNotificationScheduleBlock> orderedNotificationBlocks;

		private int nextNotificationTime;

		private int nextNotificationIndex;

		private ActivityNotificationSchedule schedule;

		private Timer scheduleTimer;

		private TrayNotificationManager trayNotificationManager;

		private Localizer localizer;

		public ActivityNotificationSchedule Schedule
		{
			get
			{
				return schedule;
			}
		}

		private void Start()
		{
			trayNotificationManager = Service.Get<TrayNotificationManager>();
			localizer = Service.Get<Localizer>();
			scheduleTimer = new Timer(1f, true, delegate
			{
				onTimerTick();
			});
			Content.LoadAsync(onScheduleLoaded, scheduleKey);
			loadActivityScreenDefinition();
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		private void onScheduleLoaded(string path, ActivityNotificationSchedule schedule)
		{
			this.schedule = schedule;
			orderedNotificationBlocks = new List<ActivityNotificationScheduleBlock>();
			orderedNotificationBlocks.AddRange(schedule.NotificationBlocks);
			orderedNotificationBlocks.Sort((ActivityNotificationScheduleBlock a, ActivityNotificationScheduleBlock b) => a.TriggerTime.CompareTo(b.TriggerTime));
			int currentMinuteInDay = getCurrentMinuteInDay();
			for (int i = 0; i < orderedNotificationBlocks.Count; i++)
			{
				if (orderedNotificationBlocks[i].TriggerTime > currentMinuteInDay)
				{
					nextNotificationTime = orderedNotificationBlocks[i].TriggerTime;
					nextNotificationIndex = i;
					break;
				}
			}
			CoroutineRunner.Start(scheduleTimer.Start(), this, "ActivityNotificationTimer");
		}

		private void onTimerTick()
		{
			int currentMinuteInDay = getCurrentMinuteInDay();
			if (currentMinuteInDay >= nextNotificationTime)
			{
				triggerNotification(orderedNotificationBlocks[nextNotificationIndex]);
				nextNotificationIndex++;
				if (nextNotificationIndex >= orderedNotificationBlocks.Count)
				{
					nextNotificationIndex = 0;
				}
				nextNotificationTime = orderedNotificationBlocks[nextNotificationIndex].TriggerTime;
			}
		}

		private void triggerNotification(ActivityNotificationScheduleBlock notificationBlock)
		{
			int num = generateRandomNotificationIndex(notificationBlock.Notifications.Length);
			int num2 = num;
			CellPhoneActivityDefinition cellPhoneActivityDefinition = null;
			do
			{
				cellPhoneActivityDefinition = notificationBlock.Notifications[num];
				if (cellPhoneActivityDefinition.GetType() == typeof(CellPhoneScheduledLocationActivityDefinition) || cellPhoneActivityDefinition.GetType().IsSubclassOf(typeof(CellPhoneScheduledLocationActivityDefinition)))
				{
					CellPhoneScheduledLocationActivityDefinition cellPhoneScheduledLocationActivityDefinition = (CellPhoneScheduledLocationActivityDefinition)cellPhoneActivityDefinition;
					DateTime target = Service.Get<ContentSchedulerService>().ScheduledEventDate();
					if (cellPhoneScheduledLocationActivityDefinition is CellPhoneRecurringLocationActivityDefinition)
					{
						cellPhoneScheduledLocationActivityDefinition = CellPhoneActivityScreenRecurringWidgetLoader.GetActiveRecurringActivityDefinition(recurringActivityDefinitions);
					}
					if (cellPhoneScheduledLocationActivityDefinition != null && DateTimeUtils.DoesDateFallBetween(target, cellPhoneScheduledLocationActivityDefinition.GetStartingDate().Date, cellPhoneScheduledLocationActivityDefinition.GetEndingDate().Date))
					{
						break;
					}
					cellPhoneActivityDefinition = null;
					num++;
					if (num >= notificationBlock.Notifications.Length)
					{
						num = 0;
					}
					continue;
				}
				break;
			}
			while (num != num2);
			if (!(cellPhoneActivityDefinition != null))
			{
				return;
			}
			string message = "";
			if (cellPhoneActivityDefinition is CellPhoneQuestActivityDefinition)
			{
				CellPhoneQuestActivityDefinition cellPhoneQuestActivityDefinition = (CellPhoneQuestActivityDefinition)cellPhoneActivityDefinition;
				cellPhoneQuestActivityDefinition.Quest = getAvailableQuest();
				if (cellPhoneQuestActivityDefinition.Quest != null)
				{
					message = localizer.GetTokenTranslation(cellPhoneQuestActivityDefinition.Quest.Mascot.GoForItNotificationText);
				}
			}
			else
			{
				message = localizer.GetTokenTranslation(cellPhoneActivityDefinition.NotificationMessageToken);
			}
			if (checkNotificationIsAllowed(cellPhoneActivityDefinition))
			{
				ActivityNotificationData dataPayload = null;
				CoinReward rewardable;
				if (schedule.NotificationReward.ToReward().TryGetValue(out rewardable))
				{
					dataPayload = new ActivityNotificationData(rewardable.Coins, cellPhoneActivityDefinition);
				}
				DNotification dNotification = new DNotification();
				dNotification.PopUpDelayTime = NotificationShowTime;
				dNotification.DataPayload = dataPayload;
				dNotification.PrefabLocation = notificationKey;
				dNotification.Message = message;
				dNotification.Type = DNotification.NotificationType.ActivityTracker;
				dNotification.PersistBetweenScenes = false;
				trayNotificationManager.ShowNotification(dNotification);
			}
		}

		private QuestDefinition getAvailableQuest()
		{
			List<Mascot> list = Service.Get<MascotService>().Mascots.ToList();
			QuestDefinition questDefinition = null;
			do
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				Mascot mascot = list[index];
				list.RemoveAt(index);
				questDefinition = mascot.GetNextAvailableQuest();
			}
			while (questDefinition == null && list.Count > 0);
			return questDefinition;
		}

		private bool checkNotificationIsAllowed(CellPhoneActivityDefinition definition)
		{
			return (!definition.IsMemberOnly || Service.Get<CPDataEntityCollection>().IsLocalPlayerMember()) && Service.Get<QuestService>().ActiveQuest == null && !Service.Get<UIElementDisablerManager>().IsUIElementDisabled("CellphoneButton") && base.gameObject.activeSelf && (!(definition is CellPhoneQuestActivityDefinition) || (definition is CellPhoneQuestActivityDefinition && ((CellPhoneQuestActivityDefinition)definition).Quest != null));
		}

		private int getCurrentMinuteInDay()
		{
			DateTime dateTime = Service.Get<ContentSchedulerService>().PresentTime();
			return dateTime.Minute + dateTime.Hour * 60;
		}

		private int generateRandomNotificationIndex(int numOptions)
		{
			DateTime dateTime = Service.Get<ContentSchedulerService>().PresentTime();
			return (dateTime.Minute + dateTime.Hour + dateTime.Day) % numOptions;
		}

		private void loadActivityScreenDefinition()
		{
			Content.LoadAsync<ScriptableObject>("Definitions/CellPhone/CellPhoneActivityScreenDefinition", onActivityScreenDefinitionLoaded);
		}

		private void onActivityScreenDefinitionLoaded(string Path, ScriptableObject definition)
		{
			recurringActivityDefinitions = ((CellPhoneActivityScreenDefinition)definition).ScheduledRecurringActivities;
		}
	}
}
