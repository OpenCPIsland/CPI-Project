using System.Collections.Generic;

namespace ClubPenguin.Task
{
	public static class TaskServiceEvents
	{
		public struct TasksLoaded
		{
			public readonly Dictionary<string, Task> Tasks;

			public TasksLoaded(Dictionary<string, Task> tasks)
			{
				Tasks = tasks;
			}
		}
	}
}
