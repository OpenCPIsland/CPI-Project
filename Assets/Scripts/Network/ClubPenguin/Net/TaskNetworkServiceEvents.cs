using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net
{
	public static class TaskNetworkServiceEvents
	{
		public struct DailyTaskProgressRecieved
		{
			public readonly TaskProgressList DailyTaskProgress;

			public DailyTaskProgressRecieved(TaskProgressList progressList)
			{
				DailyTaskProgress = progressList;
			}
		}

		public struct TaskCounterChanged
		{
			public readonly int Counter;

			public readonly string TaskId;

			public TaskCounterChanged(string taskId, int counter)
			{
				TaskId = taskId;
				Counter = counter;
			}
		}
	}
}
