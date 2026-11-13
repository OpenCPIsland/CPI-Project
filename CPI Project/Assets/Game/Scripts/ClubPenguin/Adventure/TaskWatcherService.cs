using ClubPenguin.Core;
using ClubPenguin.Task;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	// Switches the task watchers on so daily challenges can be finished by playing
	// Nothing ever did, the server used to watch the same events and send the counter
	// Which watchers a task has lives in content, at Tasks/<TaskId>/Watchers.Manifest
	public class TaskWatcherService
	{
		private static readonly ManifestContentKey WatchersKey = new ManifestContentKey("Tasks/*/Watchers.Manifest");

		private readonly EventDispatcher dispatcher;

		private readonly List<TaskWatcher> active = new List<TaskWatcher>();

		private Dictionary<string, ClubPenguin.Task.Task> tasks;

		private bool tasksReady;

		private bool playerReady;

		public TaskWatcherService()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<TaskServiceEvents.TasksLoaded>(onTasksLoaded);
			dispatcher.AddListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
			dispatcher.AddListener<SceneTransitionEvents.TransitionStart>(onTransitionStart);
		}

		private bool onTasksLoaded(TaskServiceEvents.TasksLoaded evt)
		{
			tasks = evt.Tasks;
			tasksReady = true;
			rebuild();
			return false;
		}

		private bool onLocalPlayerSpawned(PlayerSpawnedEvents.LocalPlayerSpawned evt)
		{
			playerReady = evt.LocalPlayerGameObject != null;
			rebuild();
			return false;
		}

		// A zone change, or a plain scene load like the clothing designer
		private bool onTransitionStart(SceneTransitionEvents.TransitionStart evt)
		{
			deactivate(false);
			playerReady = false;
			return false;
		}

		// Needs both, the day's tasks and a player in a loaded scene, either can land first
		private void rebuild()
		{
			if (!tasksReady || !playerReady || tasks == null)
			{
				return;
			}
			if (SceneRefs.ZoneLocalPlayerManager == null)
			{
				return;
			}
			deactivate(true);
			CoroutineRunner.StopAllForOwner(this);
			CoroutineRunner.Start(loadAndActivate(tasks), this, "TaskWatchers");
		}

		private IEnumerator loadAndActivate(Dictionary<string, ClubPenguin.Task.Task> loaded)
		{
			List<ClubPenguin.Task.Task> owners = new List<ClubPenguin.Task.Task>();
			List<AssetRequest<Manifest>> requests = new List<AssetRequest<Manifest>>();
			foreach (KeyValuePair<string, ClubPenguin.Task.Task> pair in loaded)
			{
				// One task has no watcher folder and a missing key throws
				AssetRequest<Manifest> request;
				if (!Content.TryLoadAsync(out request, WatchersKey, pair.Key))
				{
					Log.LogWarningFormatted(this, "No watchers in content for task {0}", pair.Key);
					continue;
				}
				owners.Add(pair.Value);
				requests.Add(request);
			}
			// Polled, a cancelled request never reports Finished and would hang this
			bool waiting = true;
			while (waiting)
			{
				waiting = false;
				for (int i = 0; i < requests.Count; i++)
				{
					if (!requests[i].Finished && !requests[i].Cancelled)
					{
						waiting = true;
						break;
					}
				}
				if (waiting)
				{
					yield return null;
				}
			}
			for (int i = 0; i < requests.Count; i++)
			{
				if (requests[i].Cancelled || requests[i].Asset == null)
				{
					continue;
				}
				activate(owners[i], requests[i].Asset);
			}
		}

		private void activate(ClubPenguin.Task.Task task, Manifest manifest)
		{
			if (manifest.Assets == null)
			{
				return;
			}
			for (int i = 0; i < manifest.Assets.Length; i++)
			{
				TaskWatcher source = manifest.Assets[i] as TaskWatcher;
				if (source == null)
				{
					continue;
				}
				// A copy, the watcher keeps state in its own fields and two tasks can share one
				TaskWatcher watcher = Object.Instantiate(source);
				watcher.Init(task);
				active.Add(watcher);
				try
				{
					watcher.OnActivate();
				}
				catch (System.Exception e)
				{
					Log.LogException(this, e);
				}
			}
		}

		// Leaving a scene only takes down what was watching the scene
		private void deactivate(bool includeSceneIndependent)
		{
			for (int i = active.Count - 1; i >= 0; i--)
			{
				TaskWatcher watcher = active[i];
				if (watcher == null)
				{
					active.RemoveAt(i);
					continue;
				}
				if (!includeSceneIndependent && watcher.SurvivesSceneChange)
				{
					continue;
				}
				try
				{
					watcher.OnDeactivate();
				}
				catch (System.Exception e)
				{
					Log.LogException(this, e);
				}
				active.RemoveAt(i);
				Object.Destroy(watcher);
			}
		}
	}
}
