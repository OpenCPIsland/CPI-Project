namespace ClubPenguin.Adventure
{
	public struct QuestHint
	{
		public string HintText;

		public string MascotName;

		public QuestHintWaitType WaitType;

		public float WaitTime;

		public bool Repeat;

		public QuestHint(string hintText, string mascotName, QuestHintWaitType waitType, float waitTime, bool repeat)
		{
			HintText = hintText;
			MascotName = mascotName;
			WaitType = waitType;
			WaitTime = waitTime;
			Repeat = repeat;
		}
	}
}
