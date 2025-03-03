using ClubPenguin.Core.StaticGameData;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Task;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.DailyChallenge
{
	public class DailyChallengeService
	{
		public int NumberOfUpdates;

		private EventDispatcher dispatcher;

		private TaskService taskService;

		private List<ScriptableObject> loadedDailies = new List<ScriptableObject>();

		private DatedManifestMap datedManifestMap;

		private TaskProgressList serverTaskProgress;

		public bool HasUpdates
		{
			get
			{
				return NumberOfUpdates > 0;
			}
		}

		public bool IsDailiesLoaded
		{
			get
			{
				return loadedDailies.Count > 0;
			}
		}

		public DailyChallengeService(DatedManifestMap datedManifestMap)
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<TaskNetworkServiceEvents.TaskCounterChanged>(onTaskCounterChanged);
			dispatcher.AddListener<TaskNetworkServiceEvents.DailyTaskProgressRecieved>(onTaskProgressRecieved);
			dispatcher.AddListener<TaskServiceEvents.TasksLoaded>(onTasksLoaded, EventDispatcher.Priority.FIRST);
			dispatcher.AddListener<TaskEvents.TaskCompleted>(onTaskComplete);
			taskService = Service.Get<TaskService>();
			serverTaskProgress = new TaskProgressList();
			this.datedManifestMap = datedManifestMap;
		}

		public static string GetDateManifestMapPath()
		{
			return UriUtil.Combine(StaticGameDataUtils.GetPathFromResources(StaticGameDataUtils.GetDefinitionPath(typeof(DailyChallengeScheduleDefinition))), "Schedule");
		}

		public void ClearLoadedDailies()
		{
			loadedDailies.Clear();
		}

		public void ReloadChallenges(DateTime day)
		{
			CoroutineRunner.Start(reloadChallenges(day), this, "DailyChallengesService");
		}

		private IEnumerator reloadChallenges(DateTime day)
		{
			loadedDailies.Clear();
			if (day != default(DateTime))
			{
				DailyChallengeScheduleDefinition dailies = datedManifestMap.Map[day] as DailyChallengeScheduleDefinition;
				yield return loadSchedule(dailies);
			}
			else
			{
				Log.LogError(this, "No Daily Tasks scheduled for today");
			}
			setupTasks();
		}

		private IEnumerator loadSchedule(DailyChallengeScheduleDefinition schedule)
		{
			for (int i = 0; i < schedule.Assets.Length; i++)
			{
				DailyChallengeDefinitionContentKey daily = schedule.Assets[i];
				yield return loadScheduleDaily(daily.Key);
			}
		}

		private IEnumerator loadScheduleDaily(string path)
		{
			AssetRequest<ScriptableObject> assetRequest = null;
			try
			{
				assetRequest = Content.LoadAsync<ScriptableObject>(path);
			}
			catch (ContentManifestException)
			{
			}
			yield return assetRequest;
			loadedDailies.Add(assetRequest.Asset);
		}

		private void setupTasks()
		{
			string[] array = new string[loadedDailies.Count];
			for (int i = 0; i < loadedDailies.Count; i++)
			{
				DailyChallengeDefinition dailyChallengeDefinition = (DailyChallengeDefinition)loadedDailies[i];
				array[i] = dailyChallengeDefinition.TaskName();
			}
			NumberOfUpdates = loadedDailies.Count;
			taskService.LoadTasks(array);
		}

		private bool onTasksLoaded(TaskServiceEvents.TasksLoaded evt)
		{
			for (int i = 0; i < serverTaskProgress.Count; i++)
			{
				if (evt.Tasks.ContainsKey(serverTaskProgress[i].taskId))
				{
					taskService.SetTaskProgress(serverTaskProgress[i].taskId, serverTaskProgress[i].counter, serverTaskProgress[i].claimed);
				}
			}
			return false;
		}

		private bool onTaskComplete(TaskEvents.TaskCompleted evt)
		{
			NumberOfUpdates++;
			return false;
		}

		public void RecordShown()
		{
			NumberOfUpdates = 0;
		}

		public void ClaimTaskReward(ClubPenguin.Task.Task task)
		{
			Service.Get<INetworkServicesManager>().TaskService.ClaimReward(task.Id);
			taskService.SetRewardClaimed(task);
		}

		private bool onTaskCounterChanged(TaskNetworkServiceEvents.TaskCounterChanged evt)
		{
			taskService.SetTaskProgress(evt.TaskId, evt.Counter);
			return false;
		}

		private bool onTaskProgressRecieved(TaskNetworkServiceEvents.DailyTaskProgressRecieved evt)
		{
			serverTaskProgress = evt.DailyTaskProgress;
			for (int i = 0; i < evt.DailyTaskProgress.Count; i++)
			{
				TaskProgress taskProgress = evt.DailyTaskProgress[i];
				taskService.SetTaskProgress(taskProgress.taskId, taskProgress.counter, taskProgress.claimed);
			}
			return false;
		}
	}
}
