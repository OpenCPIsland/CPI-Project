using MinigameFramework;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Goal
	{
		public enum GoalDuration
		{
			SINGLE_RUN,
			MULTIPLE_RUNS,
			MAX
		}

		public enum GoalType
		{
			DISTANCE_TRAVELLED,
			COLLECT_COINS,
			USE_TURBO,
			DESTROY_OBSTACLES,
			PLAY_GAMES,
			COLLECT_ROBOTS,
			LOSE_ROBOTS,
			MAX
		}

		public const int MAX_GOAL_LEVEL = 5;

		private mg_jr_IGoalPersistenceStrategy m_persistenceStrat = new mg_jr_PersistGoalsInPlayerPrefs();

		private mg_jr_Resources m_resources;

		private string pluralDescription;

		private string singularDescription;

		private int[] m_goalAmounts;

		private int[] m_goalRewards;

		public GoalType Type
		{
			get;
			private set;
		}

		public GoalDuration Duration
		{
			get;
			private set;
		}

		public string GoalName
		{
			get
			{
				return Type.ToString() + "_" + Duration;
			}
		}

		public float Progress
		{
			get;
			private set;
		}

		public float ProgressPercentage
		{
			get
			{
				return Mathf.Clamp01(Progress / (float)m_goalAmounts[CurrentLevel]);
			}
		}

		public int CurrentLevelTarget
		{
			get
			{
				return m_goalAmounts[CurrentLevel];
			}
		}

		public int RewardForCurrentLevel
		{
			get
			{
				return m_goalRewards[CurrentLevel];
			}
		}

		public int CurrentLevel
		{
			get;
			private set;
		}

		public bool IsCurrentLevelComplete
		{
			get
			{
				return Progress >= (float)m_goalAmounts[CurrentLevel];
			}
		}

		public mg_jr_Goal(GoalType _goalType, GoalDuration _duration, int[] _objectives, int[] _rewards, string pluralDescription, string singularDescription)
		{
			Assert.IsTrue(_objectives.Length == 5, "Must provide exactly " + 5 + " objectives");
			Assert.IsTrue(_rewards.Length == 5, "Must provide exactly " + 5 + " rewards");
			Type = _goalType;
			Duration = _duration;
			m_goalAmounts = _objectives;
			m_goalRewards = _rewards;
			this.pluralDescription = pluralDescription;
			this.singularDescription = singularDescription;
			CurrentLevel = m_persistenceStrat.LoadLevelFor(this);
			Progress = m_persistenceStrat.LoadProgresFor(this);
			m_resources = MinigameManager.GetActive<mg_JetpackReboot>().Resources;
		}

		public void SaveGoalProgress()
		{
			m_persistenceStrat.SaveGoalProgressFor(this);
		}

		public string TextDescriptionOfGoal()
		{
			string text = "";
			text = ((m_goalAmounts[CurrentLevel] != 1) ? pluralDescription : singularDescription);
			return string.Format(text, CurrentLevelTarget);
		}

		public string TextDescriptionOfProgress()
		{
			return Mathf.Clamp((int)Progress, 0, m_goalAmounts[CurrentLevel]) + "/" + m_goalAmounts[CurrentLevel];
		}

		public void ResetCurrentProgress()
		{
			Progress = 0f;
		}

		public bool AddToProgressAndCheckCompletion(float _amount)
		{
			bool isCurrentLevelComplete = IsCurrentLevelComplete;
			Progress += _amount;
			return !isCurrentLevelComplete && IsCurrentLevelComplete;
		}

		public void Complete()
		{
			Assert.IsTrue(IsCurrentLevelComplete, "Level must be complete");
			CurrentLevel++;
			if (CurrentLevel >= 5)
			{
				CurrentLevel = 0;
			}
			ResetCurrentProgress();
		}
	}
}
