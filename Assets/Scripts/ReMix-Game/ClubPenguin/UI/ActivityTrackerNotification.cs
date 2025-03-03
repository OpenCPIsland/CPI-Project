using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.CellPhone;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Animator))]
	public class ActivityTrackerNotification : TrayNotification
	{
		public Text CoinRewardText;

		private ActivityNotificationData notificationData;

		private Mascot mascot;

		public override void Show(DNotification data)
		{
			base.Show(data);
			notificationData = (ActivityNotificationData)data.DataPayload;
			CoinRewardText.text = notificationData.CoinReward.ToString();
			if (notificationData != null)
			{
			}
		}

		public void OnGoTherePressed()
		{
			Service.Get<TrayNotificationManager>().DismissCurrentNotification(false);
			Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.ClaimQuickNotificationRewardSuccess>(onClaimQuickNotificationRewardSuccess);
			Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.ClaimQuickNotificationRewardFailed>(onClaimQuickNotificationRewardFail);
			Service.Get<INetworkServicesManager>().RewardService.ClaimQuickNotificationReward();
		}

		private bool onClaimQuickNotificationRewardSuccess(RewardServiceEvents.ClaimQuickNotificationRewardSuccess evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.ClaimQuickNotificationRewardSuccess>(onClaimQuickNotificationRewardSuccess);
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.ClaimQuickNotificationRewardFailed>(onClaimQuickNotificationRewardFail);
			goToActivityNotification(evt.Reward);
			return false;
		}

		private bool onClaimQuickNotificationRewardFail(RewardServiceEvents.ClaimQuickNotificationRewardFailed evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.ClaimQuickNotificationRewardSuccess>(onClaimQuickNotificationRewardSuccess);
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.ClaimQuickNotificationRewardFailed>(onClaimQuickNotificationRewardFail);
			goToActivityNotification(null);
			return false;
		}

		private void goToActivityNotification(Reward reward)
		{
			if (notificationData.Definition is CellPhoneQuestActivityDefinition)
			{
				CellPhoneQuestActivityDefinition cellPhoneQuestActivityDefinition = (CellPhoneQuestActivityDefinition)notificationData.Definition;
				Mascot mascot = Service.Get<MascotService>().GetMascot(cellPhoneQuestActivityDefinition.Quest.Mascot.name);
				Vector3 spawnPlayerNearMascotPosition = mascot.Definition.SpawnPlayerNearMascotPosition;
				ZoneDefinition zone = mascot.Definition.Zone;
				goToLocationInZone(spawnPlayerNearMascotPosition, zone.ZoneName, notificationData.Definition, reward);
			}
			else if (notificationData.Definition.GetType() == typeof(CellPhoneLocationActivityDefinition) || notificationData.Definition.GetType().IsSubclassOf(typeof(CellPhoneLocationActivityDefinition)))
			{
				goToLocationInZone((CellPhoneLocationActivityDefinition)notificationData.Definition, reward);
			}
		}

		private void goToLocationInZone(CellPhoneLocationActivityDefinition definition, Reward reward)
		{
			goToLocationInZone(definition.LocationInZone, definition.Scene.SceneName, definition, reward);
		}

		private void goToLocationInZone(Vector3 location, string sceneName, CellPhoneActivityDefinition definition, Reward reward)
		{
			PlayerSpawnPositionManager component = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PlayerSpawnPositionManager>();
			if (component != null)
			{
				SpawnedAction spawnedAction = new SpawnedAction();
				spawnedAction.Action = SpawnedAction.SPAWNED_ACTION.None;
				component.SpawnPlayer(new SpawnPlayerParams.SpawnPlayerParamsBuilder(location).SceneName(sceneName).SpawnedAction(spawnedAction).PendingReward(reward)
					.Build());
			}
			Service.Get<ICPSwrveService>().Action("activity_tracker", "quick_go", definition.GetType().ToString(), definition.name);
		}
	}
}
