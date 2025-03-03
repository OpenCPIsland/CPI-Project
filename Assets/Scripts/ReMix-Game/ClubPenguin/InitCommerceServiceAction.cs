using ClubPenguin.Commerce;
using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitGameDataAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitNetworkControllerAction))]
	[RequireComponent(typeof(InitContentSchedulerServiceAction))]
	public class InitCommerceServiceAction : InitActionComponent
	{
		public ScheduledEventDateDefinitionKey SupportWindow;

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
			CommerceService commerceService = new CommerceService();
			commerceService.Setup();
			Dictionary<int, ScheduledEventDateDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>();
			ScheduledEventDateDefinition value = null;
			dictionary.TryGetValue(SupportWindow.Id, out value);
			Service.Set(commerceService);
			Service.Set(new MembershipService(value));
			if (Service.Get<GameSettings>().FirstSession)
			{
				Service.Get<MembershipService>().AccountFunnelName = "account_creation";
			}
			else
			{
				Service.Get<MembershipService>().AccountFunnelName = "login";
			}
			yield break;
		}
	}
}
