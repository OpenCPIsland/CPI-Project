using ClubPenguin.Mix;
using ClubPenguin.Net;
using ClubPenguin.Net.Client;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitContentSchedulerServiceAction))]
	[RequireComponent(typeof(InitLocalizerSetupAction))]
	[RequireComponent(typeof(InitApteligentAction))]
	[RequireComponent(typeof(InitContentSystemAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitDataModelAction))]
	[RequireComponent(typeof(InitKeyChainManager))]
	public class InitNetworkControllerAction : InitActionComponent
	{
		public FTUEConfig FTUEConfig;

		public GameSceneConfig GameSceneConfig;

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
			GameSettings gameSettings = Service.Get<GameSettings>();
			bool offlineMode = gameSettings.OfflineMode;
			Service.Set(GameSceneConfig);
			Service.Set(new OfflineDatabase());
			Service.Set((IOfflineDefinitionLoader)new OfflineDefinitionLoader());
			Service.Set((IOfflineRoomFactory)new OfflineRoomFactory());
			SessionManager instance = (!offlineMode || !string.IsNullOrEmpty(gameSettings.MixAPIHostUrl)) ? new SessionManager() : new OfflineSessionManager();
			Service.Set(instance);
			MixLoginCreateService instance2 = (!offlineMode || !string.IsNullOrEmpty(gameSettings.MixAPIHostUrl)) ? new MixLoginCreateService() : new OfflineLoginCreateService();
			Service.Set(instance2);
			Service.Set(new LoginController());
			NetworkController networkController = new NetworkController(Service.Get<CoroutineRunner>(), offlineMode);
			networkController.SetGameConfig(FTUEConfig, GameSceneConfig);
			Service.Set(networkController);
			if (Service.IsSet<IStandaloneErrorLogger>())
			{
				((StandaloneErrorLogger)Service.Get<IStandaloneErrorLogger>()).DiagnosticsService = Service.Get<INetworkServicesManager>().DiagnosticsService;
			}
			yield break;
		}
	}
}
