using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ClubPenguin.Task
{
	public class TaskService
	{
		private const string CLOTHING_CATALOG_CHALLENGE_NAME = "ClothingCatalogSubmission";

		private readonly EventDispatcher dispatcher;

		private readonly Dictionary<string, TaskDefinition> knownTasks = new Dictionary<string, TaskDefinition>();

		private readonly Dictionary<string, Task> tasks = new Dictionary<string, Task>();

		private readonly Dictionary<string, string> taskToSubGroupMap = new Dictionary<string, string>();

		public TaskDefinition ClothingCatalogChallenge
		{
			get
			{
				if (knownTasks.ContainsKey("ClothingCatalogSubmission"))
				{
					return knownTasks["ClothingCatalogSubmission"];
				}
				return null;
			}
		}

		public bool HasLoadedTasks
		{
			get;
			private set;
		}

		public IEnumerable<Task> Tasks
		{
			get
			{
				return tasks.Values;
			}
		}

		public TaskService(Manifest manifest)
		{
			dispatcher = Service.Get<EventDispatcher>();
			ScriptableObject[] assets = manifest.Assets;
			foreach (ScriptableObject scriptableObject in assets)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(scriptableObject.name);
				ScriptableObject[] assets2 = ((Manifest)scriptableObject).Assets;
				foreach (ScriptableObject scriptableObject2 in assets2)
				{
					TaskDefinition value = (TaskDefinition)scriptableObject2;
					knownTasks[scriptableObject2.name] = value;
					taskToSubGroupMap[scriptableObject2.name] = fileNameWithoutExtension;
				}
			}
		}

		public string GetSubGroupByTaskName(string taskName)
		{
			return taskToSubGroupMap[taskName];
		}

		public void LoadTasks(string[] taskIds)
		{
			tasks.Clear();
			foreach (string key in taskIds)
			{
				if (knownTasks.ContainsKey(key))
				{
					TaskDefinition definition = knownTasks[key];
					Task value = new Task(definition);
					tasks[key] = value;
				}
			}
			HasLoadedTasks = true;
			dispatcher.DispatchEvent(new TaskServiceEvents.TasksLoaded(tasks));
		}

		public void SetTaskProgress(string taskId, int count, bool? claimed = null)
		{
			if (tasks.ContainsKey(taskId))
			{
				Task task = tasks[taskId];
				setTaskProgress(task, count, claimed);
			}
		}

		private void setTaskProgress(Task task, int count, bool? claimed = null)
		{
			task.SetCounter(count);
			if (claimed.HasValue)
			{
				task.IsRewardClaimed = claimed.Value;
			}
		}

		public void SetRewardClaimed(Task task)
		{
			task.IsRewardClaimed = true;
		}
	}
}
