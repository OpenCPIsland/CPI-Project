using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin.Analytics
{
	public class TechAnalytics : MonoBehaviour
	{
		public const string CONTEXT_FPS_MIN = "fps_min";

		public const string CONTEXT_FPS_MAX = "fps_max";

		public const string CONTEXT_FPS_AVERAGE = "fps_average";

		public const string CONTEXT_MAXMEMORY = "max_memory_usage";

		public const string CONTEXT_MEM_WARNING = "low_memory_event";

		public const string CONTEXT_NETWORK_LATENCY = "network_latency";

		private const float HEAP_SAMPLE_INTERVAL = 5f;

		private const ulong SIZE_OF_MB = 1048576uL;

		private Disney.Kelowna.Common.Performance perf;

		private float heapSize;

		private static Dictionary<string, CPSwrveTimer> pendingTimers;

		private void Awake()
		{
			perf = Service.Get<Disney.Kelowna.Common.Performance>();
			pendingTimers = new Dictionary<string, CPSwrveTimer>();
		}

		private void Start()
		{
			StartCoroutine(monitorHeap());
			StartCoroutine(waitForEventDispatcher());
		}

		public static void LogTimer(string timerID, int elapsedTime, string context, string stepName = null)
		{
			Service.Get<ICPSwrveService>().Timing(elapsedTime, context, null, stepName);
		}

		public static void LogAction(string context, string action, string location, string type = null)
		{
			Service.Get<ICPSwrveService>().Action("game." + context, action, location, type);
		}

		public static void StartTimer(string TimerID, string Context, string Message = null, string StepName = null)
		{
			if (pendingTimers.ContainsKey(TimerID))
			{
				pendingTimers.Remove(TimerID);
			}
			CPSwrveTimer value = new CPSwrveTimer(TimerID, Context, Message, StepName);
			pendingTimers.Add(TimerID, value);
		}

		public static void EndTimer(string TimerID)
		{
			CPSwrveTimer value;
			if (pendingTimers.TryGetValue(TimerID, out value))
			{
				int elapsedTime = (int)value.Timer.ElapsedMilliseconds / 1000;
				value.Timer.Stop();
				pendingTimers.Remove(TimerID);
				LogTimer(TimerID, elapsedTime, value.Context, value.StepName);
			}
		}

		public static void LogNetworkLatency(RollingStatistics gameServerStats, RollingStatistics webServiceStats)
		{
			if (gameServerStats != null && webServiceStats != null)
			{
				if (gameServerStats.SampleCount > 0)
				{
					LogAction("network_latency", "max_latency", "game_server", gameServerStats.Maximum.ToString());
					LogAction("network_latency", "min_latency", "game_server", gameServerStats.Minimum.ToString());
					LogAction("network_latency", "avg_latency", "game_server", gameServerStats.Average.ToString());
					LogAction("network_latency", "std_dev_latency", "game_server", gameServerStats.StandardDeviation.ToString());
				}
				if (webServiceStats.SampleCount > 0)
				{
					LogAction("network_latency", "max_latency", "web_service", webServiceStats.Maximum.ToString());
					LogAction("network_latency", "min_latency", "web_service", webServiceStats.Minimum.ToString());
					LogAction("network_latency", "avg_latency", "web_service", webServiceStats.Average.ToString());
					LogAction("network_latency", "std_dev_latency", "web_service", webServiceStats.StandardDeviation.ToString());
				}
			}
		}

		private IEnumerator waitForEventDispatcher()
		{
			while (!Service.IsSet<EventDispatcher>())
			{
				yield return null;
			}
			addEventListeners();
		}

		private void OnApplicationPause(bool isPaused)
		{
			if (!isPaused && Service.IsSet<EventDispatcher>())
			{
				StartTimer("from_background", "from_background");
				Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionResumedEvent>(onSessionResumed);
			}
		}

		private bool onSessionResumed(SessionEvents.SessionResumedEvent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.SessionResumedEvent>(onSessionResumed);
			EndTimer("from_background");
			return false;
		}

		private bool onSessionStarted(SessionEvents.SessionStartedEvent evt)
		{
			try
			{
				Crittercism.SetUsername(Service.Get<SessionManager>().LocalUser.DisplayName.Text);
			}
			catch (Exception)
			{
				Crittercism.SetUsername("<display name not set>");
			}
			return false;
		}

		private void addEventListeners()
		{
			Service.Get<EventDispatcher>().AddListener<SceneTransitionEvents.TransitionStart>(onSceneTransitionStart);
			Service.Get<EventDispatcher>().AddListener<SceneTransitionEvents.TransitionComplete>(onSceneTransitionComplete);
			Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionStartedEvent>(onSessionStarted);
			EnvironmentManager.LowMemoryEvent += onLowMemory;
		}

		private bool onSceneTransitionStart(SceneTransitionEvents.TransitionStart evt)
		{
			logSceneFPS();
			logSceneMemory();
			return false;
		}

		private bool onSceneTransitionComplete(SceneTransitionEvents.TransitionComplete evt)
		{
			perf.FramesPerSecond.ResetValues();
			perf.FrameTime.ResetValues();
			return false;
		}

		private void logSceneFPS()
		{
			Disney.Kelowna.Common.Performance.Metric<float> framesPerSecond = perf.FramesPerSecond;
			LogAction("fps_min", Mathf.RoundToInt(framesPerSecond.Min).ToString(), SceneManager.GetActiveScene().name);
			LogAction("fps_max", Mathf.RoundToInt(framesPerSecond.Max).ToString(), SceneManager.GetActiveScene().name);
			LogAction("fps_average", Mathf.RoundToInt((float)framesPerSecond.Average).ToString(), SceneManager.GetActiveScene().name);
		}

		private IEnumerator monitorHeap()
		{
			yield return new WaitForSeconds(5f);
			updateMaxHeapSize();
		}

		private void updateMaxHeapSize()
		{
			float num = (float)perf.ProcessUsedBytes.Value / 1048576f;
			if (num > heapSize)
			{
				heapSize = num;
			}
		}

		private void forceUpdateMaxHeapSize()
		{
			bool trackProcessUsedMemory = perf.TrackProcessUsedMemory;
			if (!trackProcessUsedMemory)
			{
				perf.TrackProcessUsedMemory = true;
				perf.ProcessUsedBytes.ForceUpdate();
			}
			updateMaxHeapSize();
			perf.TrackProcessUsedMemory = trackProcessUsedMemory;
		}

		private int roundToNearest10(float input)
		{
			return (int)Math.Round((double)input / 10.0) * 10;
		}

		private void logSceneMemory()
		{
			forceUpdateMaxHeapSize();
			int num = roundToNearest10(heapSize);
			bool flag = heapSize > 180f;
			LogAction("max_memory_usage", flag ? "true" : "false", SceneManager.GetActiveScene().name, num.ToString());
		}

		private void onLowMemory()
		{
			forceUpdateMaxHeapSize();
			int num = roundToNearest10(heapSize);
			bool flag = heapSize > 180f;
			LogAction("low_memory_event", flag ? "true" : "false", SceneManager.GetActiveScene().name, num.ToString());
		}
	}
}
