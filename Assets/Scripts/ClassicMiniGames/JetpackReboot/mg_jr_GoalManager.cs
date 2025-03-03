using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_GoalManager : MonoBehaviour
	{
		public delegate void GoalCompleted(mg_jr_Goal _completed);

		public const int MAX_ACTIVE_GOALS = 3;

		private Dictionary<mg_jr_Goal.GoalType, mg_jr_Goal> m_singleRunGoals = new Dictionary<mg_jr_Goal.GoalType, mg_jr_Goal>();

		private Dictionary<mg_jr_Goal.GoalType, mg_jr_Goal> m_multiRunGoals = new Dictionary<mg_jr_Goal.GoalType, mg_jr_Goal>();

		private mg_jr_GoalFactory m_goalFactory = new mg_jr_GoalFactory();

		private List<mg_jr_Goal> m_activeGoals = new List<mg_jr_Goal>();

		private mg_jr_IActiveGoalPersistenceStrategy m_persistenceStrat = new mg_jr_PersistActiveGoalsInPlayerPrefs();

		public bool AreAnyActiveGoalsComplete
		{
			get
			{
				bool result = false;
				foreach (mg_jr_Goal activeGoal in m_activeGoals)
				{
					if (activeGoal.IsCurrentLevelComplete)
					{
						result = true;
					}
				}
				return result;
			}
		}

		public ReadOnlyCollection<mg_jr_Goal> ActiveGoals
		{
			get
			{
				return m_activeGoals.AsReadOnly();
			}
		}

		public event GoalCompleted OnGoalCompleted;

		private void Awake()
		{
			CreateAllGoals();
			LoadActiveGoals();
		}

		private void CreateAllGoals()
		{
			mg_jr_Goal.GoalDuration duration = mg_jr_Goal.GoalDuration.SINGLE_RUN;
			CreateAGoal(mg_jr_Goal.GoalType.COLLECT_COINS, duration);
			CreateAGoal(mg_jr_Goal.GoalType.DISTANCE_TRAVELLED, duration);
			CreateAGoal(mg_jr_Goal.GoalType.USE_TURBO, duration);
			CreateAGoal(mg_jr_Goal.GoalType.DESTROY_OBSTACLES, duration);
			duration = mg_jr_Goal.GoalDuration.MULTIPLE_RUNS;
			CreateAGoal(mg_jr_Goal.GoalType.COLLECT_COINS, duration);
			CreateAGoal(mg_jr_Goal.GoalType.DISTANCE_TRAVELLED, duration);
			CreateAGoal(mg_jr_Goal.GoalType.USE_TURBO, duration);
			CreateAGoal(mg_jr_Goal.GoalType.DESTROY_OBSTACLES, duration);
			CreateAGoal(mg_jr_Goal.GoalType.PLAY_GAMES, duration);
			CreateAGoal(mg_jr_Goal.GoalType.COLLECT_ROBOTS, duration);
			CreateAGoal(mg_jr_Goal.GoalType.LOSE_ROBOTS, duration);
		}

		private void CreateAGoal(mg_jr_Goal.GoalType _type, mg_jr_Goal.GoalDuration _duration)
		{
			mg_jr_Goal value = m_goalFactory.MakeGoal(_type, _duration);
			switch (_duration)
			{
			case mg_jr_Goal.GoalDuration.SINGLE_RUN:
				m_singleRunGoals.Add(_type, value);
				break;
			case mg_jr_Goal.GoalDuration.MULTIPLE_RUNS:
				m_multiRunGoals.Add(_type, value);
				break;
			default:
				Assert.IsTrue(false, "Not a valid goal duration");
				break;
			}
		}

		private void LoadActiveGoals()
		{
			for (int i = 0; i < 3; i++)
			{
				mg_jr_Goal.GoalDuration goalDuration = m_persistenceStrat.LoadGoalDuration(i);
				mg_jr_Goal.GoalType goalType = m_persistenceStrat.LoadGoalType(i);
				if (IsGoalActive(goalType, goalDuration))
				{
					m_activeGoals.Add(PickNewGoal());
					continue;
				}
				switch (goalDuration)
				{
				case mg_jr_Goal.GoalDuration.SINGLE_RUN:
					m_activeGoals.Add(m_singleRunGoals[goalType]);
					break;
				case mg_jr_Goal.GoalDuration.MULTIPLE_RUNS:
					m_activeGoals.Add(m_multiRunGoals[goalType]);
					break;
				default:
					Assert.IsTrue(false, "Unexpected duration type");
					break;
				}
			}
		}

		public void SaveGoalProgress()
		{
			foreach (mg_jr_Goal value in m_singleRunGoals.Values)
			{
				value.SaveGoalProgress();
			}
			foreach (mg_jr_Goal value2 in m_multiRunGoals.Values)
			{
				value2.SaveGoalProgress();
			}
			m_persistenceStrat.Save(this);
		}

		public void ResetRunGoalProgress()
		{
			foreach (mg_jr_Goal activeGoal in m_activeGoals)
			{
				if (activeGoal.Duration == mg_jr_Goal.GoalDuration.SINGLE_RUN)
				{
					activeGoal.ResetCurrentProgress();
				}
			}
		}

		public void ReplaceCompletedGoals()
		{
			for (int i = 0; i < m_activeGoals.Count; i++)
			{
				mg_jr_Goal mg_jr_Goal = m_activeGoals[i];
				if (mg_jr_Goal.IsCurrentLevelComplete)
				{
					mg_jr_Goal item = PickNewGoal();
					mg_jr_Goal.Complete();
					m_activeGoals.RemoveAt(i);
					m_activeGoals.Insert(i, item);
				}
			}
		}

		public void AddToProgress(mg_jr_Goal.GoalType _type, float _amount)
		{
			foreach (mg_jr_Goal activeGoal in m_activeGoals)
			{
				if (activeGoal.Type == _type && activeGoal.AddToProgressAndCheckCompletion(_amount) && this.OnGoalCompleted != null)
				{
					this.OnGoalCompleted(activeGoal);
				}
			}
		}

		private mg_jr_Goal PickNewGoal()
		{
			List<mg_jr_Goal> list = new List<mg_jr_Goal>();
			foreach (mg_jr_Goal value in m_singleRunGoals.Values)
			{
				if (!IsGoalActive(value))
				{
					list.Add(value);
				}
			}
			foreach (mg_jr_Goal value2 in m_multiRunGoals.Values)
			{
				if (!IsGoalActive(value2))
				{
					list.Add(value2);
				}
			}
			return list[Random.Range(0, list.Count)];
		}

		private bool IsGoalActive(mg_jr_Goal _toCheck)
		{
			return m_activeGoals.Contains(_toCheck);
		}

		private bool IsGoalActive(mg_jr_Goal.GoalType _type, mg_jr_Goal.GoalDuration _duration)
		{
			bool result = false;
			foreach (mg_jr_Goal activeGoal in m_activeGoals)
			{
				if (activeGoal.Type == _type && activeGoal.Duration == _duration)
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}
}
