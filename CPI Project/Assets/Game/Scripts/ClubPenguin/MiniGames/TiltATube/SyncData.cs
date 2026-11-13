using UnityEngine;

namespace ClubPenguin.MiniGames.TiltATube
{
	public class SyncData : Component
	{
		public string methodName;

		public int timeAdjust;

		public SyncData(string methodName = "", int timeAdjust = 0)
		{
			this.methodName = methodName;
			this.timeAdjust = timeAdjust;
		}
	}
}
