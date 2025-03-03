using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.Task
{
	[Serializable]
	public abstract class TaskDefinition : StaticGameDataDefinition, IMemberLocked
	{
		public enum TaskComparison
		{
			LessThan,
			Equals,
			GreaterThan
		}

		public enum TaskCategory
		{
			Collect,
			Make,
			Chat,
			Emote,
			Express,
			Action,
			Interact,
			Explore,
			Supplies,
			Sharing,
			Prop,
			Puzzle,
			Roleplay,
			TaskCompletion
		}

		public enum TaskGroup
		{
			Individual,
			Teamwork,
			Community
		}

		public enum TaskDifficulty
		{
			Easy,
			Medium,
			Hard
		}

		[SerializeField]
		private bool isMemberOnly = true;

		public TaskCategory Category;

		public TaskDifficulty Difficulty;

		[Header("Level Lock Info")]
		public LockType LevelType = LockType.PenguinLevel;

		[Range(0f, 999f)]
		public int LevelRequired;

		[LocalizationToken]
		[Header(" ")]
		public string Title;

		[LocalizationToken]
		public string Description;

		public int Threshold;

		public TaskGroup Group = TaskGroup.Individual;

		[Tooltip("Maximum value displayed on Task GUI")]
		public int CounterMax;

		public TaskComparison Comparison;

		[Header("Rewards")]
		public RewardDefinition Reward;

		[LocalizationToken]
		public string CompletionMessage;

		[Header("GUI")]
		public PrefabContentKey TaskLogItemContentKey;

		public PrefabContentKey DailyChallengeItemContentKey;

		public bool IsMemberOnly
		{
			get
			{
				return isMemberOnly;
			}
			set
			{
				isMemberOnly = value;
			}
		}
	}
}
