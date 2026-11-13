using ClubPenguin.Core;
using ClubPenguin.Task;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[Serializable]
	[CreateAssetMenu(menuName = "Watcher/Task/TaskCompletion")]
	public class TaskCompletionWatcher : TaskWatcher
	{
		public override bool SurvivesSceneChange
		{
			get
			{
				return true;
			}
		}

		public override void OnActivate()
		{
			base.OnActivate();
			base.dispatcher.AddListener<TaskEvents.TaskCompleted>(onTaskCompleted);
			recount();
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			base.dispatcher.RemoveListener<TaskEvents.TaskCompleted>(onTaskCompleted);
		}

		private bool onTaskCompleted(TaskEvents.TaskCompleted evt)
		{
			if (evt.Task.Definition.Category != TaskDefinition.TaskCategory.TaskCompletion)
			{
				recount();
			}
			return false;
		}

		// Counts what is finished, not how many times it saw a task complete
		// Restoring saved progress raises TaskCompleted again and would add one per login
		private void recount()
		{
			ClubPenguin.Task.Task self = base.task as ClubPenguin.Task.Task;
			if (self == null)
			{
				return;
			}
			int done = 0;
			foreach (ClubPenguin.Task.Task other in Service.Get<TaskService>().Tasks)
			{
				if (other.Definition.Category != TaskDefinition.TaskCategory.TaskCompletion && other.IsComplete)
				{
					done++;
				}
			}
			if (self.Definition.CounterMax > 0 && done > self.Definition.CounterMax)
			{
				done = self.Definition.CounterMax;
			}
			// Only up, the day's progress arrives one task at a time
			if (done > self.Counter)
			{
				self.SetCounter(done);
			}
		}

		public override object GetExportParameters()
		{
			return "none";
		}

		public override string GetWatcherType()
		{
			return "taskCompletion";
		}
	}
}
