using ClubPenguin.Core;
using System;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[Serializable]
	public class SavedOutfitWatcher : TaskWatcher
	{
		[Tooltip("Enforce that a saved outfit needs to have a certain number of pieces to it to count for the challenge")]
		public int MinPartCount = 0;

		public override object GetExportParameters()
		{
			return MinPartCount;
		}

		public override string GetWatcherType()
		{
			return "savedOutfit";
		}
	}
}
