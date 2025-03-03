using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using System.Collections.Generic;
using UnityEngine;

public class WatcherSwitch : Switch
{
	public List<TaskWatcher> enableWatchers = new List<TaskWatcher>();

	public List<TaskWatcher> disableWatchers = new List<TaskWatcher>();

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
