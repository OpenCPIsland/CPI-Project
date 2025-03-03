using ClubPenguin.Adventure;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class QuestAdventuresLog : MonoBehaviour
	{
		public ScrollRect ScrollRect;

		public Transform ScrollContent;

		private static PrefabContentKey chapterItemContentKey = new PrefabContentKey("ScreenQuestsAdventuresPrefabs/QuestLogChapterItem_*");

		private static PrefabContentKey chapterItemPreviewContentKey = new PrefabContentKey("ScreenQuestsAdventuresPrefabs/QuestLogChapterItem_*_ComingSoon");

		private Mascot currentMascot;

		private void Start()
		{
			currentMascot = Service.Get<MascotService>().ActiveMascot;
			if (currentMascot == null)
			{
				string currentMascotID = GetComponentInParent<QuestsScreenController>().CurrentMascotID;
				if (!string.IsNullOrEmpty(currentMascotID))
				{
					currentMascot = Service.Get<MascotService>().GetMascot(currentMascotID);
				}
			}
			if (currentMascot != null)
			{
				CoroutineRunner.Start(loadChapterItems(), this, "LoadQuestLogChapterItems");
			}
		}

		private IEnumerator loadChapterItems()
		{
			MascotDefinition.QuestChapterData[] chapters = currentMascot.Definition.ChapterData;
			AssetRequest<GameObject> chapterItemRequest = Content.LoadAsync(new PrefabContentKey(chapterItemContentKey, currentMascot.AbbreviatedName));
			yield return chapterItemRequest;
			bool hasPreviewItem = false;
			for (int i = 0; i < chapters.Length; i++)
			{
				if (chapters[i].IsPreviewChapter)
				{
					hasPreviewItem = true;
					break;
				}
			}
			AssetRequest<GameObject> chapterPreviewItemRequest = null;
			if (hasPreviewItem)
			{
				chapterPreviewItemRequest = Content.LoadAsync(new PrefabContentKey(chapterItemPreviewContentKey, currentMascot.AbbreviatedName));
				yield return chapterItemRequest;
			}
			for (int i = 0; i < chapters.Length; i++)
			{
				GameObject gameObject = chapters[i].IsPreviewChapter ? Object.Instantiate(chapterPreviewItemRequest.Asset, ScrollContent, false) : Object.Instantiate(chapterItemRequest.Asset, ScrollContent, false);
				QuestLogChapterItem component = gameObject.GetComponent<QuestLogChapterItem>();
				component.LoadChapterData(chapters[i], currentMascot.Definition);
			}
			if (chapters.Length == 1)
			{
				ScrollContent.GetComponent<ContentSizeFitter>().enabled = false;
				ScrollRect.GetComponent<ScrollRect>().enabled = false;
				ScrollContent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ScrollRect.GetComponent<RectTransform>().rect.width);
				ScrollContent.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
				ScrollRect.normalizedPosition = new Vector2(0.5f, 0.5f);
			}
		}
	}
}
