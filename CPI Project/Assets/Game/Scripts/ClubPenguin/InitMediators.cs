using ClubPenguin.Adventure;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using ClubPenguin.DailyChallenge;
using ClubPenguin.Net;
using ClubPenguin.PartyGames;
using ClubPenguin.SceneLayoutSync;
using ClubPenguin.Tutorial;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitAdventureSystemAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitDailyChallengesServiceAction), typeof(InitContentSchedulerServiceAction), typeof(InitPartyGameServiceAction))]
	[RequireComponent(typeof(InitTrayNotificationsAction), typeof(InitGuiAction), typeof(InitDataModelAction))]
	[RequireComponent(typeof(InitTutorialServiceAction))]
	[RequireComponent(typeof(InitGameDataAction), typeof(InitSceneLayoutSyncServiceAction), typeof(InitNetworkControllerAction))]
	public class InitMediators : InitActionComponent
	{
		public NotificationMediator.BreadcrumbIdentifiers BreadcrumbIdentifiers;

		public override bool HasSecondPass
		{
			get
			{
				return false;
			}
		}

		public override bool HasCompletedPass
		{
			get
			{
				return false;
			}
		}

		public override IEnumerator PerformFirstPass()
		{
			new ZoneMediator(Service.Get<EventDispatcher>(), Service.Get<DailyChallengeService>(), Service.Get<ContentSchedulerService>(), Service.Get<PartyGameManager>());
			new QuestEventMediator(Service.Get<EventDispatcher>(), Service.Get<QuestService>());
			new NotificationMediator(Service.Get<EventDispatcher>(), Service.Get<CPDataEntityCollection>(), Service.Get<TrayNotificationManager>(), Service.Get<NotificationBreadcrumbController>(), BreadcrumbIdentifiers);
			new LocalPlayerDataMediator(Service.Get<CPDataEntityCollection>(), Service.Get<TutorialManager>());
			new IglooMediator(Service.Get<EventDispatcher>(), Service.Get<CPDataEntityCollection>(), Service.Get<SceneLayoutDataManager>(), Service.Get<SceneLayoutSyncService>(), Service.Get<INetworkServicesManager>());
			new InputMediator(Service.Get<EventDispatcher>());
			new AccessibilityMediator(Service.Get<EventDispatcher>());
			yield break;
		}
	}
}
