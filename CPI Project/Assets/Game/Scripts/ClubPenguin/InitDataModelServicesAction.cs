using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitNetworkControllerAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitTrayNotificationsAction))]
	[RequireComponent(typeof(InitLocalizerSetupAction))]
	[RequireComponent(typeof(InitDataModelAction))]
	public class InitDataModelServicesAction : InitActionComponent
	{
		public StaticBreadcrumbDefinitionKey FriendAddedBreadcrumb;

		public StaticBreadcrumbDefinitionKey FriendRequestBreadcrumb;

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
			GameObject gameObject = Service.Get<GameObject>();
			List<AbstractDataModelService> list = new List<AbstractDataModelService>();
			list.Add(gameObject.AddComponent<ControlsService>());
			list.Add(gameObject.AddComponent<FullScreenChatService>());
			list.Add(gameObject.AddComponent<ChatActivityService>());
			list.Add(gameObject.AddComponent<FriendsDataModelService>());
			list.Add(gameObject.AddComponent<FriendsNotificationService>());
			FriendsBreadcrumbsService friendsBreadcrumbsService = gameObject.AddComponent<FriendsBreadcrumbsService>();
			friendsBreadcrumbsService.Init(FriendAddedBreadcrumb, FriendRequestBreadcrumb);
			list.Add(friendsBreadcrumbsService);
			list.Add(gameObject.AddComponent<OtherPlayerDataModelService>());
			list.Add(gameObject.AddComponent<PlayerStatusDataModelService>());
			list.Add(gameObject.AddComponent<SystemBarsDataModelService>());
			list.Add(gameObject.AddComponent<PlayerCardService>());
			list.Add(gameObject.AddComponent<MembershipExpiringHandler>());
			OtherPlayerDetailsRequestBatcher instance = new OtherPlayerDetailsRequestBatcher();
			Service.Set(instance);
			List<AbstractDataModelAdder> list2 = new List<AbstractDataModelAdder>();
			list2.Add(new DataModelAdder<MainNavData>("MainNavData"));
			list2.Add(new DataModelAdder<UpgradeAvailablePromptData>("UpgradeAvailablePrompt"));
			CPDataEntityCollection dataEntityCollection = Service.Get<CPDataEntityCollection>();
			for (int i = 0; i < list.Count; i++)
			{
				list[i].SetDataEntityCollection(dataEntityCollection);
			}
			for (int i = 0; i < list2.Count; i++)
			{
				list2[i].AddComponent(dataEntityCollection);
			}
			yield break;
		}
	}
}
