using ClubPenguin.Core;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class Mascot
	{
		public struct WantsToTalkBehaviours
		{
			public bool ZoomIn;

			public bool ZoomOut;

			public bool LowerTray;

			public bool RestoreTray;

			public bool ShowIndicator;

			public bool SuppressQuestNotifier;

			public bool RestoreQuestNotifier;

			public bool MoveToTalkSpot;

			public bool OverrideInteracteeTxform;

			public Transform DesiredInteracteeTxform;

			public void Reset()
			{
				ZoomIn = true;
				ZoomOut = true;
				LowerTray = true;
				RestoreTray = true;
				ShowIndicator = true;
				SuppressQuestNotifier = false;
				RestoreQuestNotifier = false;
				MoveToTalkSpot = true;
				OverrideInteracteeTxform = false;
				DesiredInteracteeTxform = null;
			}
		}

		public readonly MascotDefinition Definition;

		public readonly List<Quest> AvailableQuests = new List<Quest>();

		public readonly List<Quest> ResumableQuests = new List<Quest>();

		public QuestDefinition[] KnownQuests;

		public bool WantsToTalk;

		public WantsToTalkBehaviours InteractionBehaviours;

		public bool IsTalking;

		public DialogList ActiveQuestDialog;

		public DialogList RandomWorldDialogOverride;

		public string Name
		{
			get
			{
				return Definition.name;
			}
		}

		public string AbbreviatedName
		{
			get
			{
				return Definition.AbbreviatedName;
			}
		}

		public bool IsQuestGiver
		{
			get
			{
				return Definition.IsQuestGiver;
			}
		}

		public bool IsDefaultInteractDisabled
		{
			get;
			set;
		}

		public Mascot(MascotDefinition definition)
		{
			Definition = definition;
			InteractionBehaviours.Reset();
		}

		public bool HasAvailableQuests(bool countCompleted = false)
		{
			bool result = false;
			for (int i = 0; i < AvailableQuests.Count; i++)
			{
				if (AvailableQuests[i].State == Quest.QuestState.Available && (AvailableQuests[i].TimesCompleted == 0 || countCompleted) && (!AvailableQuests[i].Definition.isMemberOnly || Service.Get<CPDataEntityCollection>().IsLocalPlayerMember()))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public int GetHighestCompletedQuest()
		{
			int result = 0;
			for (int i = 0; i < AvailableQuests.Count; i++)
			{
				if (AvailableQuests[i].State == Quest.QuestState.Available && AvailableQuests[i].TimesCompleted > 0)
				{
					result = i + 1;
				}
			}
			return result;
		}

		public void ClearQuests()
		{
			AvailableQuests.Clear();
			ResumableQuests.Clear();
		}

		public QuestDefinition GetNextAvailableQuest(int chapterNumber = -1)
		{
			Quest quest = null;
			QuestDefinition result = null;
			List<QuestDefinition> validQuests = GetValidQuests(chapterNumber);
			for (int i = 0; i < validQuests.Count; i++)
			{
				QuestDefinition questDefinition = validQuests[i];
				foreach (Quest availableQuest in AvailableQuests)
				{
					if (availableQuest.Definition.name == questDefinition.name)
					{
						quest = availableQuest;
						break;
					}
				}
				if (quest == null || quest.State == Quest.QuestState.Locked || quest.TimesCompleted == 0)
				{
					result = questDefinition;
					break;
				}
				quest = null;
			}
			return result;
		}

		public List<QuestDefinition> GetValidQuests(int chapterNumber = -1)
		{
			List<QuestDefinition> list = new List<QuestDefinition>();
			for (int i = 0; i < KnownQuests.Length; i++)
			{
				QuestDefinition questDefinition = KnownQuests[i];
				if (!questDefinition.Prototyped && (chapterNumber == -1 || questDefinition.ChapterNumber == chapterNumber))
				{
					list.Add(KnownQuests[i]);
				}
			}
			return list;
		}
	}
}
