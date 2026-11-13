namespace ClubPenguin.Adventure
{
	public class QuestItem
	{
		public enum QuestItemState
		{
			Collected,
			NotCollected,
			Interactive
		}

		public int ItemCount = 0;

		public readonly QuestDefinition.DQuestItem DataModel;

		public QuestItemState State;

		public QuestItem(QuestDefinition.DQuestItem dataModel)
		{
			DataModel = dataModel;
			State = QuestItemState.NotCollected;
		}
	}
}
