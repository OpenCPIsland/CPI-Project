using ClubPenguin.Benchmarking;
using ClubPenguin.Mix;
using ClubPenguin.Net;
using Disney.Kelowna.Common.Environment;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitEnvironmentManagerAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitNetworkControllerAction))]
	[RequireComponent(typeof(InitKeyChainManager))]
	public class InitBenchmarkingAction : InitActionComponent
	{
		private const float CONFIG_TIMEOUT_DURATION = 15f;

		public BenchmarkConfig Config;

		private SessionManager sessionManager;

		protected MixLoginCreateService mixLoginCreateService;

		private bool localPlayerDataReady;

		private float configTimeoutCurrent = 15f;

		private bool isConfiguring = false;

		private string[] environmentNames = Enum.GetNames(typeof(Disney.Kelowna.Common.Environment.Environment));

		private Array environmentValues = Enum.GetValues(typeof(Disney.Kelowna.Common.Environment.Environment));

		private int selectedEnvIndex = -1;

		private bool[] activeTests;

		public override bool HasSecondPass
		{
			get
			{
				return true;
			}
		}

		public override bool HasCompletedPass
		{
			get
			{
				return true;
			}
		}

		public override IEnumerator PerformFirstPass()
		{
			Service.Get<EventDispatcher>().AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
			if (Config == null)
			{
				throw new ArgumentNullException("Config", "Configuration is required for benchmarking");
			}
			while (configTimeoutCurrent > 0f || isConfiguring)
			{
				yield return null;
			}
		}

		public override IEnumerator PerformSecondPass()
		{
			NetworkServicesConfig networkConfig = NetworkController.GenerateNetworkServiceConfig(Config.ServerEnvironment);
			Service.Get<INetworkServicesManager>().Configure(networkConfig);
			Service.Get<LoginController>().SetNetworkConfig(networkConfig);
			mixLoginCreateService = Service.Get<MixLoginCreateService>();
			mixLoginCreateService.SetNetworkConfig(networkConfig);
			sessionManager = Service.Get<SessionManager>();
			mixLoginCreateService.OnLoginSuccess += HandleOnLoginSuccess;
			mixLoginCreateService.OnLoginFailed += HandleOnLoginFailed;
			login();
			while (!sessionManager.HasSession)
			{
				yield return null;
			}
			mixLoginCreateService.OnLoginSuccess -= HandleOnLoginSuccess;
			mixLoginCreateService.OnLoginFailed -= HandleOnLoginFailed;
			while (!localPlayerDataReady)
			{
				yield return null;
			}
		}

		private void login()
		{
			mixLoginCreateService.Login(Config.UserName, Config.Password);
		}

		private void HandleOnLoginSuccess(ISession session)
		{
			sessionManager.AddMixSession(session);
		}

		private void HandleOnLoginFailed(ILoginResult result)
		{
			Log.LogError(this, "Login Failed");
		}

		private bool onLocalPlayerDataReady(NetworkControllerEvents.LocalPlayerDataReadyEvent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
			localPlayerDataReady = true;
			return false;
		}

		public override void OnInitializationComplete()
		{
			BenchmarkRunner benchmarkRunner = Service.Get<GameObject>().AddComponent<BenchmarkRunner>();
			benchmarkRunner.RunTests(Config.Tests);
			Service.Get<GameStateController>().EnterGame();
		}

		public void OnGUI()
		{
			GUILayout.Space(75f);
			if (selectedEnvIndex == -1)
			{
				for (int i = 0; i < environmentValues.Length; i++)
				{
					Disney.Kelowna.Common.Environment.Environment environment = (Disney.Kelowna.Common.Environment.Environment)environmentValues.GetValue(i);
					if (environment == Disney.Kelowna.Common.Environment.Environment.QA)
					{
						selectedEnvIndex = i;
					}
				}
				activeTests = new bool[Config.Tests.Length].Select((bool test) => true).ToArray();
			}
			if (configTimeoutCurrent > 0f && !isConfiguring)
			{
				isConfiguring = GUILayout.Button("Configure Benchmarks", GUILayout.MinWidth(200f), GUILayout.MinHeight(100f));
				configTimeoutCurrent -= Time.deltaTime;
				GUILayout.Label(string.Format("Continuing in {0:F1} seconds...", configTimeoutCurrent));
			}
			else
			{
				if (!isConfiguring)
				{
					return;
				}
				Config.UserName = makeLabelledTextField("User Name:", Config.UserName);
				Config.Password = makeLabelledTextField("Password:", Config.Password);
				selectedEnvIndex = GUILayout.Toolbar(selectedEnvIndex, environmentNames, GUILayout.MinHeight(100f));
				Config.ServerEnvironment = (Disney.Kelowna.Common.Environment.Environment)environmentValues.GetValue(selectedEnvIndex);
				int num = Config.Tests.Length / 2;
				using (new GUILayout.HorizontalScope())
				{
					makeTestToggles(0, num);
					makeTestToggles(num, Config.Tests.Length);
				}
				if (!GUILayout.Button("Done", GUILayout.MinHeight(100f)))
				{
					return;
				}
				isConfiguring = false;
				configTimeoutCurrent = 0f;
				List<BenchmarkTest> list = new List<BenchmarkTest>();
				for (int i = 0; i < Config.Tests.Length; i++)
				{
					if (activeTests[i])
					{
						list.Add(Config.Tests[i]);
					}
				}
				Config.Tests = list.ToArray();
			}
		}

		private void makeTestToggles(int indexStart, int indexEnd)
		{
			using (new GUILayout.VerticalScope())
			{
				for (int i = indexStart; i < indexEnd; i++)
				{
					GUILayout.Space(10f);
					BenchmarkTest benchmarkTest = Config.Tests[i];
					activeTests[i] = GUILayout.Toggle(activeTests[i], benchmarkTest.name);
					GUILayout.Space(10f);
				}
			}
		}

		private static string makeLabelledTextField(string label, string text)
		{
			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Label(label);
				return GUILayout.TextField(text);
			}
		}
	}
}
