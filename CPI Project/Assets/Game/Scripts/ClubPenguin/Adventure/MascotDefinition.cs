using ClubPenguin.Core.StaticGameData;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClubPenguin.Adventure
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Mascot")]
	public class MascotDefinition : StaticGameDataDefinition
	{
		[Serializable]
		public struct QuestChapterData
		{
			public int Number;

			public bool IsPreviewChapter;

			[LocalizationToken]
			public string Name;
		}

		[LocalizationToken]
		[FormerlySerializedAs("DisplayTitle")]
		public string i18nDisplayTitle;

		[LocalizationToken]
		public string i18nAdventureLogTitleText;

		public string AbbreviatedName;

		public bool IsQuestGiver;

		public bool ShowComingSoonInLog;

		public QuestChapterData[] ChapterData = new QuestChapterData[0];

		public int QuestReminderTutorialId;

		public int QuestLogPriority;

		[LocalizationToken]
		public string GoForItNotificationText;

		[Header("Dialog")]
		public DialogList QuestGiverDialog;

		public DialogList QuestGiverDialogMemberLocked;

		public DialogList QuestGiverDialogLevelLocked;

		public DialogList QuestGiverDialogTimeLocked;

		public DialogList QuestGiverDialogAllComplete;

		public DialogList RandomQuestDialog;

		public DialogList RandomWorldDialog;

		[Header("Audio")]
		public string QuestIntroMusic;

		public string QuestEndMusic;

		[Header("GUI")]
		public Color TextColor;

		public Color DialogBubbleArrowColor;

		public Color NavigationArrowColor;

		public Color BannerTextColor;

		public Color CommunicatorBGColor;

		public Color CommunicatorBGShadowColor;

		public Color XPTintColor;

		public FontContentKey FontContentKey;

		public int SpeechBubbleFontSize;

		public int MessageBubbleFontSize;

		public SpriteContentKey BubbleContentKey;

		public SpriteContentKey HudAvatarContentKey;

		public SpriteContentKey CommunicatorIconContentKey;

		public PrefabContentKey RewardPopupXPContentKey;

		public PrefabContentKey RewardPopupChestContentKey;

		public SpriteContentKey QuestStatusIconContentKey;

		public SpriteContentKey ProgressionLockedIconContentKey;

		[Header("In World Location")]
		public Vector3 SpawnPlayerNearMascotPosition;

		public ZoneDefinition Zone;
	}
}
