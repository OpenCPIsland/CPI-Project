namespace ClubPenguin.Net
{
	public static class TutorialServiceEvents
	{
		public struct TutorialReceived
		{
			public readonly byte[] TutorialBytes;

			public TutorialReceived(byte[] tutorialBytes)
			{
				TutorialBytes = tutorialBytes;
			}
		}
	}
}
