#define ENABLE_PROFILER
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	public class BenchmarkRuntimeProfiler : MonoBehaviour
	{
		private BenchmarkLogger logger;

		private bool profiling = false;

		private bool memProfiling = false;

		private int frameCount = 0;

		private int pollRate = 30;

		private StringBuilder messageBuilder = new StringBuilder();

		public void Start()
		{
			logger = new BenchmarkLogger("RuntimeMemoryLogger");
		}

		public void OnEnable()
		{
			Service.Get<EventDispatcher>().AddListener<BenchmarkRuntimeProfileEvents.RuntimeProfileStart>(onRuntimeProfileStart);
			Service.Get<EventDispatcher>().AddListener<BenchmarkRuntimeProfileEvents.RuntimeProfileReset>(onRuntimeProfileReset);
		}

		public void OnDisable()
		{
			Service.Get<EventDispatcher>().RemoveListener<BenchmarkRuntimeProfileEvents.RuntimeProfileStart>(onRuntimeProfileStart);
			Service.Get<EventDispatcher>().RemoveListener<BenchmarkRuntimeProfileEvents.RuntimeProfileReset>(onRuntimeProfileReset);
		}

		private bool onRuntimeProfileStart(BenchmarkRuntimeProfileEvents.RuntimeProfileStart startEvent)
		{
			profiling = true;
			pollRate = startEvent.PollRate;
			memProfiling = startEvent.PollMemory;
			frameCount = 0;
			Disney.Kelowna.Common.Performance performance = Service.Get<Disney.Kelowna.Common.Performance>();
			performance.AutomaticMemorySampling = false;
			performance.ResetMetrics();
			logger.Print("Runtime Profiling Starting. Profiling Memory = " + memProfiling);
			return false;
		}

		private bool onRuntimeProfileReset(BenchmarkRuntimeProfileEvents.RuntimeProfileReset resetEvent)
		{
			if (profiling)
			{
				logFrametime();
			}
			profiling = false;
			memProfiling = false;
			return false;
		}

		public void LateUpdate()
		{
			if (memProfiling)
			{
				if (frameCount % pollRate == 0)
				{
					logMemory();
				}
				frameCount++;
			}
		}

		private void logFrametime()
		{
			messageBuilder.Length = 0;
			Disney.Kelowna.Common.Performance performance = Service.Get<Disney.Kelowna.Common.Performance>();
			messageBuilder.Append("runtime-frametime> avg: ");
			messageBuilder.Append(performance.FrameTime.Average);
			messageBuilder.Append(" max: ");
			messageBuilder.Append(performance.FrameTime.Max);
			messageBuilder.Append(" min: ");
			messageBuilder.Append(performance.FrameTime.Min);
			logger.Print(messageBuilder.ToString());
		}

		private void logMemory()
		{
			messageBuilder.Length = 0;
			messageBuilder.Append("runtime-memory>");
			Disney.Kelowna.Common.Performance performance = Service.Get<Disney.Kelowna.Common.Performance>();
			performance.UpdateAssetMemoryUsage();
			Dictionary<Disney.Kelowna.Common.Performance.MemoryType, Disney.Kelowna.Common.Performance.Metric<int>>.Enumerator memoryMetricsEnumerator = performance.MemoryMetricsEnumerator;
			while (memoryMetricsEnumerator.MoveNext())
			{
				KeyValuePair<Disney.Kelowna.Common.Performance.MemoryType, Disney.Kelowna.Common.Performance.Metric<int>> current = memoryMetricsEnumerator.Current;
				messageBuilder.Append(" ");
				messageBuilder.Append(current.Key);
				messageBuilder.Append(": ");
				messageBuilder.Append(current.Value.Value);
			}
			if ((float)performance.TotalMemoryUsed.Value > 0f)
			{
				messageBuilder.Append(" Total: ");
				messageBuilder.Append(performance.TotalMemoryUsed.Value);
			}
			ulong processUsedBytes = Service.Get<MemoryMonitorManager>().GetProcessUsedBytes();
			messageBuilder.Append(" Native: ");
			messageBuilder.Append(processUsedBytes);
			logger.Print(messageBuilder.ToString());
		}
	}
}
