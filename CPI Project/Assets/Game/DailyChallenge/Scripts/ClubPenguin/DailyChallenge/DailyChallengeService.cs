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

        private readonly bool offlineMode;

        private List<ScriptableObject> loadedDailies = new List<ScriptableObject>();

        private DatedManifestMap datedManifestMap;

        private TaskProgressList serverTaskProgress;

        private Dictionary<string, int> savedTaskCounters = new Dictionary<string, int>();

        private const string PREFS_RECENT_TASKS_KEY = "DailyChallenge_RecentTasksHistory";

        private const string PREFS_LAST_ASSIGNED_DATE_KEY = "DailyChallenge_LastAssignedDate";

        private const string PREFS_ACTIVE_TASKS_KEY = "DailyChallenge_ActiveTasks";

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

        public DailyChallengeService(DatedManifestMap datedManifestMap, bool offlineMode)
        {
            dispatcher = Service.Get<EventDispatcher>();
            dispatcher.AddListener<TaskNetworkServiceEvents.TaskCounterChanged>(onTaskCounterChanged);
            dispatcher.AddListener<TaskNetworkServiceEvents.DailyTaskProgressRecieved>(onTaskProgressRecieved);
            dispatcher.AddListener<TaskServiceEvents.TasksLoaded>(onTasksLoaded, EventDispatcher.Priority.FIRST);
            dispatcher.AddListener<TaskEvents.TaskCompleted>(onTaskComplete);
            dispatcher.AddListener<TaskEvents.TaskUpdated>(onTaskUpdated);
            taskService = Service.Get<TaskService>();
            serverTaskProgress = new TaskProgressList();
            this.datedManifestMap = datedManifestMap;
            this.offlineMode = offlineMode;
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
            savedTaskCounters.Clear();
            UnityEngine.Object manifest;
            DailyChallengeScheduleDefinition dailies = null;
            if (day != default(DateTime) && datedManifestMap.Map.TryGetValue(day.Date, out manifest))
            {
                dailies = manifest as DailyChallengeScheduleDefinition;
            }
            if (dailies != null)
            {
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

        private const string CLOTHING_CATALOG_TASK_NAME = "ClothingCatalogSubmission";

        private const int DAILY_CHALLENGE_TOTAL = 6;

        private const int DAILY_CHALLENGE_MASCOT_LIMIT = 2;

        private bool isExcludedGroup(TaskDefinition.TaskGroup group)
        {
            return group == TaskDefinition.TaskGroup.Teamwork || group == TaskDefinition.TaskGroup.Community;
        }

        private bool exceedsMascotTaskLimit(IEnumerable<string> taskNames)
        {
            Dictionary<string, int> mascotTaskCounts = new Dictionary<string, int>();
            foreach (string taskName in taskNames)
            {
                string mascotName = taskService.GetSubGroupByTaskName(taskName);
                int taskCount;
                mascotTaskCounts.TryGetValue(mascotName, out taskCount);
                taskCount++;
                if (taskCount > DAILY_CHALLENGE_MASCOT_LIMIT)
                {
                    return true;
                }
                mascotTaskCounts[mascotName] = taskCount;
            }
            return false;
        }

        private bool tryAddTask(string taskName, List<string> taskNames, HashSet<string> usedTaskNames, Dictionary<string, int> mascotTaskCounts, bool enforceMascotLimit)
        {
            if (usedTaskNames.Contains(taskName))
            {
                return false;
            }
            if (!enforceMascotLimit)
            {
                taskNames.Add(taskName);
                usedTaskNames.Add(taskName);
                return true;
            }
            string mascotName = taskService.GetSubGroupByTaskName(taskName);
            int taskCount;
            mascotTaskCounts.TryGetValue(mascotName, out taskCount);
            if (taskCount >= DAILY_CHALLENGE_MASCOT_LIMIT)
            {
                return false;
            }
            taskNames.Add(taskName);
            usedTaskNames.Add(taskName);
            mascotTaskCounts[mascotName] = taskCount + 1;
            return true;
        }

        private void setupTasks()
        {
            int targetCount = DAILY_CHALLENGE_TOTAL;
            bool enforceMascotLimit = offlineMode;
            string currentDateString = DateTime.UtcNow.ToString("yyyy-MM-dd");
            string storedDateString = PlayerPrefs.GetString(PREFS_LAST_ASSIGNED_DATE_KEY, string.Empty);
            string storedActiveTasks = PlayerPrefs.GetString(PREFS_ACTIVE_TASKS_KEY, string.Empty);

            if (storedDateString == currentDateString && !string.IsNullOrEmpty(storedActiveTasks))
            {
                string[] activeTaskArray = storedActiveTasks.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (activeTaskArray.Length == targetCount && (!enforceMascotLimit || !exceedsMascotTaskLimit(activeTaskArray)))
                {
                    NumberOfUpdates = activeTaskArray.Length;
                    taskService.LoadTasks(activeTaskArray);
                    return;
                }
            }

            Queue<string> recentTasksQueue = loadRecentTasksHistory();
            HashSet<string> recentTasksSet = new HashSet<string>(recentTasksQueue);

            List<string> finalTaskNames = new List<string>();
            HashSet<string> usedTaskNames = new HashSet<string>();
            Dictionary<string, int> mascotTaskCounts = new Dictionary<string, int>();

            for (int i = 0; i < loadedDailies.Count; i++)
            {
                if (finalTaskNames.Count >= targetCount)
                {
                    break;
                }

                DailyChallengeDefinition dailyChallengeDefinition = (DailyChallengeDefinition)loadedDailies[i];
                string taskName = dailyChallengeDefinition.TaskName();

                if (usedTaskNames.Contains(taskName))
                {
                    continue;
                }

                if (taskName == CLOTHING_CATALOG_TASK_NAME)
                {
                    tryAddTask(taskName, finalTaskNames, usedTaskNames, mascotTaskCounts, enforceMascotLimit);
                    continue;
                }

                TaskDefinition definition;
                if (!taskService.TryGetDefinition(taskName, out definition) || isExcludedGroup(definition.Group))
                {
                    continue;
                }

                tryAddTask(taskName, finalTaskNames, usedTaskNames, mascotTaskCounts, enforceMascotLimit);
            }

            if (finalTaskNames.Count < targetCount)
            {
                int shortfall = targetCount - finalTaskNames.Count;

                HashSet<string> exclusionSet = new HashSet<string>(usedTaskNames);
                foreach (string recentTask in recentTasksSet)
                {
                    exclusionSet.Add(recentTask);
                }

                List<string> candidates = taskService.GetTaskNamesByGroup(TaskDefinition.TaskGroup.Individual, exclusionSet, int.MaxValue);

                if (candidates.Count < shortfall || enforceMascotLimit)
                {
                    List<string> fallbackCandidates = taskService.GetTaskNamesByGroup(TaskDefinition.TaskGroup.Individual, usedTaskNames, int.MaxValue);
                    List<string> additionalCandidates = new List<string>();
                    for (int i = 0; i < fallbackCandidates.Count; i++)
                    {
                        if (!candidates.Contains(fallbackCandidates[i]))
                        {
                            additionalCandidates.Add(fallbackCandidates[i]);
                        }
                    }
                    shuffleList(additionalCandidates);
                    for (int i = 0; i < additionalCandidates.Count && (enforceMascotLimit || candidates.Count < shortfall); i++)
                    {
                        candidates.Add(additionalCandidates[i]);
                    }
                }

                shuffleList(candidates);

                for (int i = 0; i < candidates.Count && finalTaskNames.Count < targetCount; i++)
                {
                    tryAddTask(candidates[i], finalTaskNames, usedTaskNames, mascotTaskCounts, enforceMascotLimit);
                }
            }

            for (int i = 0; i < finalTaskNames.Count; i++)
            {
                recentTasksQueue.Enqueue(finalTaskNames[i]);
            }

            while (recentTasksQueue.Count > 42)
            {
                recentTasksQueue.Dequeue();
            }

            saveRecentTasksHistory(recentTasksQueue);
            PlayerPrefs.SetString(PREFS_LAST_ASSIGNED_DATE_KEY, currentDateString);
            PlayerPrefs.SetString(PREFS_ACTIVE_TASKS_KEY, string.Join(",", finalTaskNames.ToArray()));
            PlayerPrefs.Save();

            string[] array = finalTaskNames.ToArray();
            NumberOfUpdates = array.Length;
            taskService.LoadTasks(array);
        }

        private Queue<string> loadRecentTasksHistory()
        {
            Queue<string> queue = new Queue<string>();
            string savedData = PlayerPrefs.GetString(PREFS_RECENT_TASKS_KEY, string.Empty);
            if (!string.IsNullOrEmpty(savedData))
            {
                string[] entries = savedData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < entries.Length; i++)
                {
                    queue.Enqueue(entries[i]);
                }
            }
            return queue;
        }

        private void saveRecentTasksHistory(Queue<string> queue)
        {
            string savedData = string.Join(",", queue.ToArray());
            PlayerPrefs.SetString(PREFS_RECENT_TASKS_KEY, savedData);
        }

        private void shuffleList<T>(List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private bool onTasksLoaded(TaskServiceEvents.TasksLoaded evt)
        {
            for (int i = 0; i < serverTaskProgress.Count; i++)
            {
                if (evt.Tasks.ContainsKey(serverTaskProgress[i].taskId))
                {
                    applyStoredProgress(serverTaskProgress[i].taskId, serverTaskProgress[i].counter, serverTaskProgress[i].claimed);
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
            applyStoredProgress(evt.TaskId, evt.Counter, null);
            return false;
        }

        private bool onTaskProgressRecieved(TaskNetworkServiceEvents.DailyTaskProgressRecieved evt)
        {
            serverTaskProgress = evt.DailyTaskProgress;
            for (int i = 0; i < evt.DailyTaskProgress.Count; i++)
            {
                TaskProgress taskProgress = evt.DailyTaskProgress[i];
                applyStoredProgress(taskProgress.taskId, taskProgress.counter, taskProgress.claimed);
            }
            return false;
        }

        private void applyStoredProgress(string taskId, int counter, bool? claimed)
        {
            savedTaskCounters[taskId] = counter;
            taskService.SetTaskProgress(taskId, counter, claimed);
        }

        private bool onTaskUpdated(TaskEvents.TaskUpdated evt)
        {
            ClubPenguin.Task.Task task = evt.Task;
            int savedCounter;
            if (savedTaskCounters.TryGetValue(task.Id, out savedCounter) && savedCounter == task.Counter)
            {
                return false;
            }
            savedTaskCounters[task.Id] = task.Counter;
            Service.Get<INetworkServicesManager>().TaskService.SetProgress(task.Id, task.Counter);
            return false;
        }
    }
}