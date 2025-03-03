using ClubPenguin.Benchmarking;
using ClubPenguin.Core;
using ClubPenguin.DailyChallenge;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.Environment;
using Disney.Kelowna.Common.Tests;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin.Tests
{
	public abstract class BaseZoneTranstionTest : BaseIntegrationTest
	{
		private bool IsBeginRecieved = false;

		private bool IsRequestReceived = false;

		private bool IsDoneReceived = false;

		public Canvas SpashScreenCanvas;

		public float NetworkLagTime = 1f;

		protected ZoneDefinition TestZoneDefinition;

		protected CPDataEntityCollection dataEntityCollection;

		protected ZoneTransitionService zts;

		protected EventDispatcher eventDispatcher;

		public Environment Environment = Environment.DEV;

		protected override IEnumerator setup()
		{
			yield return base.setup();
			eventDispatcher = Service.Get<EventDispatcher>();
			Service.Set(SpashScreenCanvas);
			Service.Set(SpashScreenCanvas.gameObject.AddComponent<LoadingController>());
			base.gameObject.AddComponent<KeyChainManager>();
			Service.Set(new GameSettings());
			initDataModel();
			initContentAction();
			yield return StartTestCoroutine(initSchedulerAction(), this, "initSchedulerAction");
			initNetworkServices();
			SceneTransitionService sts = base.gameObject.AddComponent<SceneTransitionService>();
			sts.LoadingMode = LoadSceneMode.Additive;
			Service.Set(sts);
			TestZoneDefinition = ScriptableObject.CreateInstance<ZoneDefinition>();
			TestZoneDefinition.ZoneName = "EmptySceneForLoadingTests";
			TestZoneDefinition.SceneName = "EmptySceneForLoadingTests";
			TestZoneDefinition.SceneFilePath = "Assets/Game/Core/Tests/IntegrationTests/ZoneAndSceneTransitionTests/EmptySceneForLoadingTests.unity";
			Manifest zoneManifest = ScriptableObject.CreateInstance<Manifest>();
			zoneManifest.Assets = new ScriptableObject[1]
			{
				TestZoneDefinition
			};
			zts = base.gameObject.AddComponent<ZoneTransitionService>();
			zts.SetZonesFromManifest(zoneManifest);
			Service.Set(zts);
			yield return null;
		}

		protected override IEnumerator runTest()
		{
			if (NetworkLagTime > 0f)
			{
				yield return new WaitForSeconds(NetworkLagTime);
			}
			sendMockSelfJoinedRoomEvent();
			while (!isTestComplete())
			{
				yield return null;
			}
		}

		public void initContentAction()
		{
			ContentManifest manifest = ContentManifestUtility.FromDefinitionFile("Configuration/embedded_content_manifest");
			Content instance = new Content(manifest);
			Service.Set(instance);
		}

		public void initDataModel()
		{
			Service.Set((CPDataEntityCollection)new CPDataEntityCollectionImpl());
		}

		public IEnumerator initSchedulerAction()
		{
			string dailyChallengesScheduleManifestPath = DailyChallengeService.GetDateManifestMapPath();
			AssetRequest<DatedManifestMap> scheduleAssetRequest = Content.LoadAsync<DatedManifestMap>(dailyChallengesScheduleManifestPath);
			yield return scheduleAssetRequest;
			Service.Set(new ContentSchedulerService(scheduleAssetRequest.Asset.Map.Keys, -8, null));
		}

		public void initNetworkServices()
		{
			SwappableNetworkServicesManager swappableNetworkServicesManager = new SwappableNetworkServicesManager();
			NetworkServicesManager networkServicesManager2 = (NetworkServicesManager)(swappableNetworkServicesManager.NetworkServicesManager = new NetworkServicesManager(this, NetworkController.GenerateNetworkServiceConfig(Environment), false));
		}

		private void sendMockSelfJoinedRoomEvent()
		{
			ZoneId zoneId = new ZoneId();
			zoneId.name = TestZoneDefinition.ZoneName;
			WorldServiceEvents.SelfRoomJoinedEvent evt = new WorldServiceEvents.SelfRoomJoinedEvent(1234L, TestZoneDefinition.ZoneName, new RoomIdentifier("TestWorld", Language.en_US, zoneId, ""), null, null, false);
			eventDispatcher.DispatchEvent(evt);
		}

		protected bool onZoneTransition(ZoneTransitionEvents.ZoneTransition evt)
		{
			if (evt.State == ZoneTransitionEvents.ZoneTransition.States.Begin)
			{
				IsBeginRecieved = true;
				IntegrationTestEx.FailIf(IsRequestReceived, "Request State recieved before a Begin State");
				IntegrationTestEx.FailIf(IsDoneReceived, "Done State recieved before a Begin State");
			}
			if (evt.State == ZoneTransitionEvents.ZoneTransition.States.Request)
			{
				IsRequestReceived = true;
				IntegrationTestEx.FailIf(!IsBeginRecieved, "Begin State NOT recieved before a Request State");
				IntegrationTestEx.FailIf(IsDoneReceived, "Done State recieved before a Request State");
			}
			if (evt.State == ZoneTransitionEvents.ZoneTransition.States.Done)
			{
				IsDoneReceived = true;
				IntegrationTestEx.FailIf(!IsBeginRecieved, "Begin State NOT recieved before a Done State");
				IntegrationTestEx.FailIf(!IsRequestReceived, "Request State NOT recieved before a Done State");
			}
			return false;
		}

		protected bool isTestComplete()
		{
			return IsBeginRecieved && IsRequestReceived && IsDoneReceived;
		}
	}
}
