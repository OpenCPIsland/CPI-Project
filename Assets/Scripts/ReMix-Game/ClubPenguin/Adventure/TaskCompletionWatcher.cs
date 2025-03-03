using ClubPenguin.Core;
using System;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[Serializable]
	[CreateAssetMenu(menuName = "Watcher/Task/TaskCompletion")]
	public class TaskCompletionWatcher : TaskWatcher
	{
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
