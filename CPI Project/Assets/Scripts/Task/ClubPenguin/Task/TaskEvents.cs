namespace ClubPenguin.Task
{
	public static class TaskEvents
	{
		public struct TaskCompleted
		{
			public readonly Task Task;

			public TaskCompleted(Task task)
			{
				Task = task;
			}
		}

		public struct TaskUpdated
		{
			public readonly Task Task;

			public TaskUpdated(Task task)
			{
				Task = task;
			}
		}

		public struct TaskRewardClaimed
		{
			public readonly Task Task;

			public TaskRewardClaimed(Task task)
			{
				Task = task;
			}
		}
	}
}
