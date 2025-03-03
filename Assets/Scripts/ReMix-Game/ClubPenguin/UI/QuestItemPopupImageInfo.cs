using System;

namespace ClubPenguin.UI
{
	[Serializable]
	public class QuestItemPopupImageInfo
	{
		public string ItemImagePath;

		public string ItemNameText;

		public string i18nItemNameText;

		public int ItemCount;

		public QuestItemPopupImageInfo()
		{
			ItemImagePath = "";
			ItemNameText = "";
			i18nItemNameText = "";
			ItemCount = 0;
		}

		public QuestItemPopupImageInfo(string itemImagePath, string itemNameText = "", int itemCount = 0)
		{
			ItemImagePath = itemImagePath;
			ItemNameText = itemNameText;
			i18nItemNameText = itemNameText;
			ItemCount = itemCount;
		}
	}
}
