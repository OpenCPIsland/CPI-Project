using System;
using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	public abstract class BenchmarkingTestStage : ScriptableObject
	{
		private Action onFinishDelegate;

		private BenchmarkLogger logger;

		public void Run(Action onFinishDelegate)
		{
			logger = new BenchmarkLogger("Stage:" + base.name);
			this.onFinishDelegate = onFinishDelegate;
			performBenchmark();
		}

		protected abstract void performBenchmark();

		public void onFinish()
		{
			logger.Dispose();
			onFinishDelegate();
		}
	}
}
