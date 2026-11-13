using System;
using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	public class BenchmarkTest : ScriptableObject
	{
		public int PollTime = 30;

		public bool MemoryProfiling = true;

		public BenchmarkTestStage[] Stages;

		private int currentStageIndex;

		private Action<int> onFinishDelegate;

		private BenchmarkLogger logger;

		public virtual void Run(Action<int> onFinishDelegate)
		{
			logger = new BenchmarkLogger("Test:" + base.name);
			this.onFinishDelegate = onFinishDelegate;
			currentStageIndex = 0;
			runNextStage();
		}

		private void runNextStage()
		{
			if (currentStageIndex < Stages.Length)
			{
				Stages[currentStageIndex].Run(new BenchmarkRuntimeProfileEvents.RuntimeProfileStart(MemoryProfiling, PollTime), delegate(int exitStatus)
				{
					onStageFinished(exitStatus);
				});
			}
			else
			{
				onFinish();
			}
		}

		private void onStageFinished(int exitStatus)
		{
			if (exitStatus == 0)
			{
				currentStageIndex++;
				runNextStage();
			}
			else
			{
				onFinish(exitStatus);
			}
		}

		private void onFinish(int exitStatus = 0)
		{
			logger.Dispose();
			onFinishDelegate(exitStatus);
		}
	}
}
