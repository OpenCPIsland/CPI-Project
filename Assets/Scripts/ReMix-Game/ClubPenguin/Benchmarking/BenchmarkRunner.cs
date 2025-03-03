using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin.Benchmarking
{
	public class BenchmarkRunner : MonoBehaviour
	{
		private BenchmarkTest[] tests;

		private int currentTestIndex;

		private BenchmarkLogger logger;

		public bool IsRunning
		{
			get;
			private set;
		}

		public void Start()
		{
			base.gameObject.AddComponent<BenchmarkRuntimeProfiler>();
			Service.Get<EventDispatcher>().AddListener<LoadingController.TimeoutEvent>(onLoadingTimeout);
		}

		public void RunTests(BenchmarkTest[] tests)
		{
			logger = new BenchmarkLogger("BenchmarkRun");
			logger.Print("benchmark-date> " + DateTime.Now);
			logger.Print("benchmark-device-model> " + SystemInfo.deviceModel);
			IsRunning = true;
			this.tests = tests;
			currentTestIndex = 0;
			runNextTest();
		}

		private void runNextTest()
		{
			if (currentTestIndex < tests.Length)
			{
				tests[currentTestIndex++].Run(delegate(int exitStatus)
				{
					onTestFinished(exitStatus);
				});
			}
			else
			{
				onFinish();
			}
		}

		private void onTestFinished(int exitStatus)
		{
			if (exitStatus == 0)
			{
				runNextTest();
			}
			else
			{
				onFinish(exitStatus);
			}
		}

		private void onFinish(int exitStatus = 0)
		{
			logger.Print("benchmark-exit-status> " + exitStatus);
			logger.Dispose();
			IsRunning = false;
			Scene sceneByPath = SceneManager.GetSceneByPath("Assets/Game/Core/Tests/BenchmarkingTests/Scenes/EmptyScene.unity");
			if (sceneByPath.buildIndex >= 0)
			{
				SceneManager.LoadScene(sceneByPath.buildIndex);
			}
		}

		private bool onLoadingTimeout(LoadingController.TimeoutEvent evt)
		{
			logger.Print("ERROR: Loading new scene timed out after " + evt.ElapsedTime + " seconds.");
			onFinish(-1);
			Service.Get<LoadingController>().ClearAllLoadingSystems();
			return true;
		}
	}
}
