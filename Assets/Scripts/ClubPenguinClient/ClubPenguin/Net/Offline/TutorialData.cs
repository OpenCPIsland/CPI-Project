namespace ClubPenguin.Net.Offline
{
	public struct TutorialData : IOfflineData
	{
		public sbyte[] Bytes;

		public void Init()
		{
			Bytes = new sbyte[64];
		}
	}
}
