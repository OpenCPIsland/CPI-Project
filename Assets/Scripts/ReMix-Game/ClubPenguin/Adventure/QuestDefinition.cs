using ClubPenguin.Core;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[CreateAssetMenu(menuName = "Definition/Quest")]
	public class QuestDefinition : ScriptableObject
	{
		[Serializable]
		public struct DQuestItem
		{
			public enum QuestItemClickAction
			{
				none,
				loadPopup,
				showTrayScreen,
				openChat,
				useConsumable,
				showImagePopup
			}

			[LocalizationToken]
			public string DisplayName;

			public string Name;

			public string ItemText;

			public SpriteContentKey ItemInventoryContentKey;

			public PrefabContentKey ItemUIContentKey;

			public bool IsHiddenWhenNotCollected;

			public QuestItemClickAction ClickAction;

			public string ClickActionArg;

			public PrefabContentKey PopupContentKey;

			public SpriteContentKey PopupImageContentKey;

			public DButton UseItemButton;
		}

		[Tooltip("This needs to be disabled and the quest exported before the quest is complete.  Enabled to test quest changes locally, and disable to have them run on the server")]
		public bool Prototyped = true;

		[Tooltip("Set this to true when overriding the FSM's starting objective for testing purposes, or risk having camera/tray issues upong starting quest.")]
		public bool OverridesFirstObjective = false;

		[Tooltip("Set to true if this quest can be replayed after completion.")]
		public bool IsReplayable = true;

		[Tooltip("Set to true if this quest can be paused.")]
		public bool IsPausable = true;

		public MascotDefinition Mascot;

		public int ChapterNumber;

		public int QuestNumber;

		[LocalizationToken]
		public string Title;

		[LocalizationToken]
		public string AbbreviatedTitle;

		[TextArea]
		public string Description;

		[LocalizationToken]
		public string SplashScreenText;

		public DDialogPanel[] Instructions;

		public DQuestItem[] QuestItems;

		[Header("Rewards")]
		public RewardDefinition StartReward;

		public RewardDefinition CompleteReward;

		public RewardDefinition[] ObjectiveRewards;

		public bool IsRewardPopupSupressed;

		public string RewardPopupPrefabOverride;

		public string RewardPopupMusicOverride;

		[Header("Requirements")]
		[Tooltip("The minimum progression level needed to start this quest")]
		public int LevelRequirement;

		[Tooltip("List of quests that need to be completed before this one can be started")]
		public QuestDefinition[] CompletedQuestRequirement;

		[Tooltip("The amount of time the user must wait after the last completed quest requirement is finished before this one can be started")]
		public TimeSpanUnityWrapper TimeLock;

		[Tooltip("The membership line")]
		public bool isMemberOnly = true;
	}
}
