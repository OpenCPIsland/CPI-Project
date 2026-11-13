using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	public abstract class BenchmarkTestStage : ScriptableObject
	{
		private Action<int> onFinishDelegate;

		protected BenchmarkLogger logger;

		public void Run(BenchmarkRuntimeProfileEvents.RuntimeProfileStart profileStartEvent, Action<int> onFinishDelegate)
		{
			logger = new BenchmarkLogger("Stage:" + base.name);
			Service.Get<EventDispatcher>().DispatchEvent(profileStartEvent);
			this.onFinishDelegate = onFinishDelegate;
			try
			{
				setup();
				performBenchmark();
			}
			catch (Exception ex)
			{
				logger.Log("EXCEPTION: " + ex.Message);
				onFinish(1);
			}
		}

		protected virtual void setup()
		{
		}

		protected abstract void performBenchmark();

		protected virtual void teardown()
		{
		}

		public void onFinish(int exitStatus = 0)
		{
			teardown();
			Service.Get<EventDispatcher>().DispatchEvent(default(BenchmarkRuntimeProfileEvents.RuntimeProfileReset));
			logger.Dispose();
			onFinishDelegate(exitStatus);
		}
	}
}
