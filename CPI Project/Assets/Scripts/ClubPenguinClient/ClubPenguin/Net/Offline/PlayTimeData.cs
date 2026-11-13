namespace ClubPenguin.Net.Offline
{
	[UnityEngine.Scripting.Preserve]
	public struct PlayTimeData : IOfflineData
	{
		[UnityEngine.Scripting.Preserve]
		public long TotalSeconds;

		public void Init()
		{
			TotalSeconds = 0L;
		}
	}
}
