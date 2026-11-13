using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WatcherSwitch : Switch
{
	public List<TaskWatcher> enableWatchers = new List<TaskWatcher>();

	public List<TaskWatcher> disableWatchers = new List<TaskWatcher>();

	private EventDispatcher dispatcher;

	private LocomotionEventBroadcaster locomotionBroadcaster;

	private readonly List<Switch> watchedSwitches = new List<Switch>();

	private void Start()
	{
		dispatcher = Service.Get<EventDispatcher>();
		dispatcher.AddListener<ClubPenguin.ActionSequencerEvents.ActionSequenceStarted>(onActionSequenceStarted);
		addSwitchListeners(enableWatchers);
		addSwitchListeners(disableWatchers);
		if (hasWatcherType(enableWatchers, "action") || hasWatcherType(disableWatchers, "action"))
		{
			if (ClubPenguin.SceneRefs.ZoneLocalPlayerManager != null && ClubPenguin.SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject != null)
			{
				locomotionBroadcaster = ClubPenguin.SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<LocomotionEventBroadcaster>();
			}
			if (locomotionBroadcaster != null)
			{
				locomotionBroadcaster.OnDoActionEvent += onDoAction;
			}
			else
			{
				Log.LogWarning(this, "Action watchers require a local player's LocomotionEventBroadcaster.");
			}
		}
	}

	private void OnDestroy()
	{
		if (dispatcher != null)
		{
			dispatcher.RemoveListener<ClubPenguin.ActionSequencerEvents.ActionSequenceStarted>(onActionSequenceStarted);
		}
		if (locomotionBroadcaster != null)
		{
			locomotionBroadcaster.OnDoActionEvent -= onDoAction;
		}
		for (int i = 0; i < watchedSwitches.Count; i++)
		{
			if (watchedSwitches[i] != null)
			{
				watchedSwitches[i].StateChanged -= onSwitchStateChanged;
			}
		}
	}

	private bool onActionSequenceStarted(ClubPenguin.ActionSequencerEvents.ActionSequenceStarted evt)
	{
		if (matchesWatcher(enableWatchers, evt.actionGameObject))
		{
			Change(true);
		}
		if (matchesWatcher(disableWatchers, evt.actionGameObject))
		{
			Change(false);
		}
		return false;
	}

	private void addSwitchListeners(List<TaskWatcher> watcherDefinitions)
	{
		for (int i = 0; i < watcherDefinitions.Count; i++)
		{
			SwitchWatcher watcherDefinition = watcherDefinitions[i] as SwitchWatcher;
			if (watcherDefinition == null)
			{
				continue;
			}
			GameObject switchObject = GameObject.Find(watcherDefinition.SwitchName);
			Switch watchedSwitch = (switchObject != null) ? switchObject.GetComponent<Switch>() : null;
			if (watchedSwitch == null)
			{
				Log.LogWarningFormatted(this, "Unable to find switch {0} for WatcherSwitch {1}", watcherDefinition.SwitchName, base.name);
				continue;
			}
			if (!watchedSwitches.Contains(watchedSwitch))
			{
				watchedSwitch.StateChanged += onSwitchStateChanged;
				watchedSwitches.Add(watchedSwitch);
			}
		}
	}

	private void onSwitchStateChanged(Switch watchedSwitch, bool value)
	{
		if (!value)
		{
			return;
		}
		if (matchesSwitch(enableWatchers, watchedSwitch))
		{
			Change(true);
		}
		if (matchesSwitch(disableWatchers, watchedSwitch))
		{
			Change(false);
		}
	}

	private void onDoAction(LocomotionController.LocomotionAction action, object userData = null)
	{
		if (matchesAction(enableWatchers, action))
		{
			Change(true);
		}
		if (matchesAction(disableWatchers, action))
		{
			Change(false);
		}
	}

	private bool hasWatcherType(List<TaskWatcher> watcherDefinitions, string watcherType)
	{
		for (int i = 0; i < watcherDefinitions.Count; i++)
		{
			if (watcherDefinitions[i] != null && watcherDefinitions[i].GetWatcherType() == watcherType)
			{
				return true;
			}
		}
		return false;
	}

	private bool matchesSwitch(List<TaskWatcher> watcherDefinitions, Switch watchedSwitch)
	{
		if (watchedSwitch == null)
		{
			return false;
		}
		for (int i = 0; i < watcherDefinitions.Count; i++)
		{
			SwitchWatcher watcherDefinition = watcherDefinitions[i] as SwitchWatcher;
			if (watcherDefinition != null && string.Equals(watcherDefinition.SwitchName, watchedSwitch.name, StringComparison.Ordinal))
			{
				return true;
			}
		}
		return false;
	}

	private bool matchesAction(List<TaskWatcher> watcherDefinitions, LocomotionController.LocomotionAction action)
	{
		for (int i = 0; i < watcherDefinitions.Count; i++)
		{
			ActionEventWatcher watcherDefinition = watcherDefinitions[i] as ActionEventWatcher;
			if (watcherDefinition != null && watcherDefinition.MatchesAction(action))
			{
				return true;
			}
		}
		return false;
	}

	private bool matchesWatcher(List<TaskWatcher> watcherDefinitions, GameObject actionGameObject)
	{
		if (actionGameObject == null)
		{
			return false;
		}
		string actionPath = actionGameObject.GetPath();
		for (int i = 0; i < watcherDefinitions.Count; i++)
		{
			TaskWatcher watcherDefinition = watcherDefinitions[i];
			if (watcherDefinition != null && watcherDefinition.GetWatcherType() == "interaction" && string.Equals(watcherDefinition.GetExportParameters() as string, actionPath, StringComparison.Ordinal))
			{
				return true;
			}
		}
		return false;
	}

	public override object GetSwitchParameters()
	{
		Dictionary<string, List<ExportedTaskWatcher>> dictionary = new Dictionary<string, List<ExportedTaskWatcher>>();
		List<ExportedTaskWatcher> list = new List<ExportedTaskWatcher>();
		foreach (TaskWatcher enableWatcher in enableWatchers)
		{
			ExportedTaskWatcher exportedTaskWatcher = exportTaskWatcher(enableWatcher);
			if (exportedTaskWatcher != null)
			{
				list.Add(exportedTaskWatcher);
			}
		}
		dictionary.Add("enable", list);
		List<ExportedTaskWatcher> list2 = new List<ExportedTaskWatcher>();
		foreach (TaskWatcher disableWatcher in disableWatchers)
		{
			ExportedTaskWatcher exportedTaskWatcher = exportTaskWatcher(disableWatcher);
			if (exportedTaskWatcher != null)
			{
				list2.Add(exportedTaskWatcher);
			}
		}
		dictionary.Add("disable", list2);
		return dictionary;
	}

	private ExportedTaskWatcher exportTaskWatcher(TaskWatcher watcherDef)
	{
		ExportedTaskWatcher exportedTaskWatcher = new ExportedTaskWatcher();
		if (!string.IsNullOrEmpty(watcherDef.CriteriaSwitchName))
		{
			GameObject gameObject = GameObject.Find(watcherDef.CriteriaSwitchName);
			if (gameObject == null)
			{
				Log.LogError(this, "Unable to find switch criteria object " + watcherDef.CriteriaSwitchName + " for WatcherSwitch " + base.name + ". Will not be exported");
				return null;
			}
			Switch component = gameObject.GetComponent<Switch>();
			exportedTaskWatcher.criteriaSwitch = ExportedSwitch.Create(component);
		}
		exportedTaskWatcher.type = watcherDef.GetWatcherType();
		exportedTaskWatcher.parameters = watcherDef.GetExportParameters();
		return exportedTaskWatcher;
	}

	public override string GetSwitchType()
	{
		return "watcher";
	}
}
