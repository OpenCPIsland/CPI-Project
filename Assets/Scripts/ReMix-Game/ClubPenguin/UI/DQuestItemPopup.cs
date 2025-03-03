using System;

namespace ClubPenguin.UI
{
	[Serializable]
	public class DQuestItemPopup
	{
		public string Message;

		public string NotificationMessage;

		public QuestItemPopupImageInfo[] ItemInfos;

		public DQuestItemPopup()
		{
			Message = "";
			NotificationMessage = "";
			ItemInfos = null;
		}

		public override string ToString()
		{
			return string.Format("[DQuestItemPopup: Message={0}, NotificationMessage={2},  ItemImageLocation1={3}, ItemImageLocation2={4}, ItemImageLocation3={5}]", Message, NotificationMessage, ItemInfos[0].ItemImagePath, ItemInfos[1].ItemImagePath, ItemInfos[2].ItemImagePath);
		}
	}
}
