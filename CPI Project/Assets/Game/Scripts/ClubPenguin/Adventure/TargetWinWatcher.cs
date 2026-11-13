using ClubPenguin.Core;
using System;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[Serializable]
	[CreateAssetMenu(menuName = "Watcher/Targets/TargetWin")]
	public class TargetWinWatcher : TaskWatcher
	{
		public override object GetExportParameters()
		{
			return "none";
		}

		public override string GetWatcherType()
		{
			return "targetplaygroundwin";
		}
	}
}
