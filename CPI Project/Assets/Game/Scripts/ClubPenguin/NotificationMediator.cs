using ClubPenguin.Breadcrumbs;
using ClubPenguin.ClothingDesigner;
using ClubPenguin.Consumable;
using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.Durable;
using ClubPenguin.Igloo;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Props;
using ClubPenguin.Rewards;
using ClubPenguin.Task;
using ClubPenguin.Tubes;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class NotificationMediator
	{
		[Serializable]
		public struct BreadcrumbIdentifiers
		{
			public StaticBreadcrumbDefinitionKey DailyChallenge;

			public StaticBreadcrumbDefinitionKey Gear;

			public PersistentBreadcrumbTypeDefinitionKey GearType;

			public StaticBreadcrumbDefinitionKey Template;

			public PersistentBreadcrumbTypeDefinitionKey TemplateType;

			public StaticBreadcrumbDefinitionKey Inventory;

			public PersistentBreadcrumbTypeDefinitionKey InventoryType;

			public StaticBreadcrumbDefinitionKey Fabric;

			public PersistentBreadcrumbTypeDefinitionKey FabricType;

			public StaticBreadcrumbDefinitionKey Decal;

			public PersistentBreadcrumbTypeDefinitionKey DecalType;

			public StaticBreadcrumbDefinitionKey Consumable;

			public PersistentBreadcrumbTypeDefinitionKey ConsumableType;

			public StaticBreadcrumbDefinitionKey Tube;

			public PersistentBreadcrumbTypeDefinitionKey TubeType;

			public StaticBreadcrumbDefinitionKey PartyGame;

			public PersistentBreadcrumbTypeDefinitionKey PartyGameType;

			public StaticBreadcrumbDefinitionKey Decoration;

			public PersistentBreadcrumbTypeDefinitionKey DecorationType;

			public StaticBreadcrumbDefinitionKey Structure;

			public PersistentBreadcrumbTypeDefinitionKey StructureType;

			public StaticBreadcrumbDefinitionKey MusicTrack;

			public PersistentBreadcrumbTypeDefinitionKey MusicTrackType;

			public StaticBreadcrumbDefinitionKey Lighting;

			public PersistentBreadcrumbTypeDefinitionKey LightingType;
		}

		private const float TASK_NOTIFICATION_DELAY_TIME = 1.3f;

		private const float TASK_POPUP_DISPLAY_TIME = 3.7f;

		private CPDataEntityCollection dataEntityCollection;

		private NotificationBreadcrumbController notificationBreadcrumbController;

		private TrayNotificationManager trayNotificationManager;

		private BreadcrumbIdentifiers breadcrumbIdentifiers;

		private List<string> tasksAlreadyShown = new List<string>();

		public NotificationMediator(EventDispatcher eventDispatcher, CPDataEntityCollection dataEntityCollection, TrayNotificationManager trayNotificationManager, NotificationBreadcrumbController notificationBreadcrumbController, BreadcrumbIdentifiers breadcrumbIdentifiers)
		{
			this.dataEntityCollection = dataEntityCollection;
			this.trayNotificationManager = trayNotificationManager;
			this.notificationBreadcrumbController = notificationBreadcrumbController;
			this.breadcrumbIdentifiers = breadcrumbIdentifiers;
			eventDispatcher.AddListener<TaskServiceEvents.TasksLoaded>(onTasksLoaded);
			eventDispatcher.AddListener<TaskEvents.TaskCompleted>(onTaskComplete);
			eventDispatcher.AddListener<RewardEvents.RewardPopupComplete>(onRewardPopupComplete);
			eventDispatcher.AddListener<RewardServiceEvents.MyRewardEarned>(onMyRewardEarned);
			eventDispatcher.AddListener<DisneyStoreEvents.PurchaseComplete>(onDisneyStorePurchase);
			eventDispatcher.AddListener<SessionEvents.SessionEndedEvent>(onSessionEnded);
			eventDispatcher.AddListener<IglooUIEvents.ShowNotification>(onShowIglooNotification);
		}

		private bool onDisneyStorePurchase(DisneyStoreEvents.PurchaseComplete evt)
		{
			addBreadcrumbsForReward(evt.Reward);
			return false;
		}

		private bool onMyRewardEarned(RewardServiceEvents.MyRewardEarned evt)
		{
			if (evt.Source == RewardSource.QUEST_OBJECTIVE || evt.Source == RewardSource.CLAIMABLE_REWARD)
			{
				addBreadcrumbsForReward(evt.Reward);
			}
			return false;
		}

		private bool onRewardPopupComplete(RewardEvents.RewardPopupComplete evt)
		{
			if (evt.RewardPopupData.PopupType != DRewardPopup.RewardPopupType.replay && evt.RewardPopupData.RewardData != null)
			{
				addBreadcrumbsForReward(evt.RewardPopupData.RewardData);
			}
			return false;
		}

		private void addBreadcrumbsForReward(Reward reward)
		{
			addBreadCrumbsForReward<PropDefinition, DurableReward, int>(reward, breadcrumbIdentifiers.GearType, breadcrumbIdentifiers.Gear);
			addBreadCrumbsForReward<TemplateDefinition, EquipmentTemplateReward, int>(reward, breadcrumbIdentifiers.TemplateType, breadcrumbIdentifiers.Template);
			addBreadCrumbsForReward<FabricDefinition, FabricReward, int>(reward, breadcrumbIdentifiers.FabricType, breadcrumbIdentifiers.Fabric);
			addBreadCrumbsForReward<DecalDefinition, DecalReward, int>(reward, breadcrumbIdentifiers.DecalType, breadcrumbIdentifiers.Decal);
			addBreadCrumbsForReward<TubeDefinition, TubeReward, int>(reward, breadcrumbIdentifiers.TubeType, breadcrumbIdentifiers.Tube);
			addBreadCrumbsForReward<DecorationDefinition, DecorationReward, int>(reward, breadcrumbIdentifiers.DecorationType, breadcrumbIdentifiers.Decoration);
			addBreadCrumbsForReward<StructureDefinition, StructureReward, int>(reward, breadcrumbIdentifiers.StructureType, breadcrumbIdentifiers.Structure);
			addBreadCrumbsForReward<MusicTrackDefinition, MusicTrackReward, int>(reward, breadcrumbIdentifiers.MusicTrackType, breadcrumbIdentifiers.MusicTrack);
			addBreadCrumbsForReward<LightingDefinition, LightingReward, int>(reward, breadcrumbIdentifiers.LightingType, breadcrumbIdentifiers.Lighting);
			EquipmentInstanceReward rewardable;
			if (reward.TryGetValue(out rewardable) && !rewardable.IsEmpty())
			{
				bool flag = false;
				for (int i = 0; i < rewardable.EquipmentInstances.Count; i++)
				{
					if (dataEntityCollection.IsLocalPlayerMember() || !RewardUtils.IsMemberLockableItemMemberOnly<TemplateDefinition, int>(rewardable.EquipmentInstances[i].definitionId))
					{
						notificationBreadcrumbController.AddPersistentBreadcrumb(breadcrumbIdentifiers.InventoryType, rewardable.EquipmentInstances[i].equipmentId.ToString());
						flag = true;
					}
				}
				if (flag)
				{
					notificationBreadcrumbController.AddBreadcrumb(breadcrumbIdentifiers.Inventory);
				}
			}
			ConsumableInstanceReward rewardable2;
			if (!reward.TryGetValue(out rewardable2) || rewardable2.IsEmpty())
			{
				return;
			}
			Dictionary<string, int>.Enumerator enumerator = rewardable2.Consumables.GetEnumerator();
			bool flag2 = false;
			bool flag3 = false;
			while (enumerator.MoveNext())
			{
				if (dataEntityCollection.IsLocalPlayerMember() || !RewardUtils.IsMemberLockableItemMemberOnly<PropDefinition, int>(RewardUtils.GetConsumableIdByServerName(enumerator.Current.Key)))
				{
					PersistentBreadcrumbTypeDefinitionKey type;
					if (RewardUtils.IsConsumablePartyGame(enumerator.Current.Key))
					{
						type = breadcrumbIdentifiers.PartyGameType;
						flag3 = true;
					}
					else
					{
						type = breadcrumbIdentifiers.ConsumableType;
						flag2 = true;
					}
					notificationBreadcrumbController.AddPersistentBreadcrumb(type, RewardUtils.GetConsumableIdByServerName(enumerator.Current.Key).ToString());
				}
			}
			if (flag2)
			{
				notificationBreadcrumbController.AddBreadcrumb(breadcrumbIdentifiers.Consumable);
			}
			if (flag3)
			{
				notificationBreadcrumbController.AddBreadcrumb(breadcrumbIdentifiers.PartyGame);
			}
		}

		private void addBreadCrumbsForReward<TItemDefinition, URewardable, VIdType>(Reward reward, PersistentBreadcrumbTypeDefinitionKey singleBreadcrumbId, StaticBreadcrumbDefinitionKey groupBreadcrumbId) where TItemDefinition : IMemberLocked where URewardable : AbstractListReward<VIdType>
		{
			URewardable rewardable;
			if (reward.TryGetValue(out rewardable) && !rewardable.IsEmpty())
			{
				bool flag = false;
				List<VIdType> list = (List<VIdType>)rewardable.Reward;
				foreach (VIdType item in list)
				{
					if (dataEntityCollection.IsLocalPlayerMember() || !RewardUtils.IsMemberLockableItemMemberOnly<TItemDefinition, VIdType>(item))
					{
						notificationBreadcrumbController.AddPersistentBreadcrumb(singleBreadcrumbId, item.ToString());
						flag = true;
					}
				}
				if (flag)
				{
					notificationBreadcrumbController.AddBreadcrumb(groupBreadcrumbId);
				}
			}
		}

		private bool onTasksLoaded(TaskServiceEvents.TasksLoaded evt)
		{
			foreach (ClubPenguin.Task.Task value in evt.Tasks.Values)
			{
				if (value.IsComplete && value.IsRewardClaimed && !tasksAlreadyShown.Contains(value.Id))
				{
					tasksAlreadyShown.Add(value.Id);
				}
			}
			return false;
		}

		private bool onTaskComplete(TaskEvents.TaskCompleted evt)
		{
			CoroutineRunner.Start(showNotification(evt.Task), this, "ShowTaskNotificationDelay");
			return false;
		}

		private IEnumerator showNotification(ClubPenguin.Task.Task task)
		{
			yield return new WaitForSeconds(1.3f);
			if (!tasksAlreadyShown.Contains(task.Id))
			{
				tasksAlreadyShown.Add(task.Id);
				DNotification dNotification = new DNotification();
				dNotification.PopUpDelayTime = 3.7f;
				dNotification.DataPayload = task;
				dNotification.PrefabLocation = TrayNotificationManager.CellPhoneDailyNotificationContentKey;
				dNotification.Message = Service.Get<Localizer>().GetTokenTranslation(task.Definition.CompletionMessage);
				dNotification.Type = DNotification.NotificationType.DailyComplete;
				trayNotificationManager.ShowNotification(dNotification);
			}
		}

		private bool onSessionEnded(SessionEvents.SessionEndedEvent evt)
		{
			tasksAlreadyShown.Clear();
			notificationBreadcrumbController.ClearAllBreadcrumbs();
			return false;
		}

		private bool onShowIglooNotification(IglooUIEvents.ShowNotification evt)
		{
			DNotification dNotification = new DNotification();
			dNotification.PopUpDelayTime = evt.DisplayTime;
			dNotification.PrefabLocation = evt.Prefab;
			dNotification.Message = evt.TranslatedText;
			dNotification.AdjustRectPositionForNotification = evt.AdjustRectPositionForNotification;
			dNotification.ShowAfterSceneLoad = evt.ShowAfterSceneLoad;
			dNotification.PersistBetweenScenes = false;
			dNotification.HeaderTint = new Color32(75, 102, 184, byte.MaxValue);
			trayNotificationManager.ShowNotification(dNotification);
			return false;
		}
	}
}
