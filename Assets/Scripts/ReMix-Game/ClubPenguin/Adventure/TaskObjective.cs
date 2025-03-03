using ClubPenguin.Core;
using ClubPenguin.Task;
using Disney.LaunchPadFramework;
using HutongGames.PlayMaker;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest (Server)")]
	[HutongGames.PlayMaker.Tooltip("Have the server watch for the given set of watchers to complete this task")]
	public class TaskObjective : FsmStateAction, ServerVerifiableAction
	{
		private class ExportedQuestTask
		{
			public int threshold;

			public int comparison;

			public string zone;

			public List<ExportedTaskWatcher> watchers;
		}

		[HutongGames.PlayMaker.Tooltip("If this objective can only be completed in a single zone, set that here, otherwise leave it empty")]
		public ZoneDefinition Zone;

		[HutongGames.PlayMaker.Tooltip("The count of the watchers triggering to mark this task as complete")]
		[RequiredField]
		public int Threshold;

		[HutongGames.PlayMaker.Tooltip("The operator to use when checking the threshold value")]
		[RequiredField]
		public TaskDefinition.TaskComparison Comparison;

		[HutongGames.PlayMaker.Tooltip("The list of watchers that drive this task")]
		[RequiredField]
		public TaskWatcher[] Watchers;

		public object GetVerifiableParameters()
		{
			ExportedQuestTask exportedQuestTask = new ExportedQuestTask();
			exportedQuestTask.zone = Zone.ZoneName;
			exportedQuestTask.threshold = Threshold;
			exportedQuestTask.comparison = (int)Comparison;
			exportedQuestTask.watchers = new List<ExportedTaskWatcher>();
			for (int i = 0; i < Watchers.Length; i++)
			{
				TaskWatcher taskWatcher = Watchers[i];
				ExportedTaskWatcher exportedTaskWatcher = new ExportedTaskWatcher();
				if (!string.IsNullOrEmpty(taskWatcher.CriteriaSwitchName))
				{
					GameObject gameObject = GameObject.Find(taskWatcher.CriteriaSwitchName);
					if (gameObject == null)
					{
						Disney.LaunchPadFramework.Log.LogError(this, "Unable to find switch criteria object " + taskWatcher.CriteriaSwitchName + ". Will not be exported");
						continue;
					}
					Switch component = gameObject.GetComponent<Switch>();
					exportedTaskWatcher.criteriaSwitch = ExportedSwitch.Create(component);
				}
				exportedTaskWatcher.type = taskWatcher.GetWatcherType();
				exportedTaskWatcher.parameters = taskWatcher.GetExportParameters();
				exportedQuestTask.watchers.Add(exportedTaskWatcher);
			}
			return exportedQuestTask;
		}

		public string GetVerifiableType()
		{
			return "Task";
		}

		public override void OnEnter()
		{
			Finish();
		}
	}
}
