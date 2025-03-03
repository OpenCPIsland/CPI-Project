using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_PersistActiveGoalsInPlayerPrefs : mg_jr_IActiveGoalPersistenceStrategy
	{
		private const string KEY_PREFIX = "mg_jr_activeGoal_number_";

		private const string DURATION_SUFIX = "_duration";

		private const string TYPE_SUFIX = "_progress";

		public void Save(mg_jr_GoalManager _goalManager)
		{
			IList<mg_jr_Goal> activeGoals = _goalManager.ActiveGoals;
			Assert.AreEqual(activeGoals.Count, 3, "Goal manager has unexpected number of active goals");
			for (int i = 0; i < activeGoals.Count; i++)
			{
				PlayerPrefs.SetInt("mg_jr_activeGoal_number_" + i + "_duration", (int)activeGoals[i].Duration);
				PlayerPrefs.SetInt("mg_jr_activeGoal_number_" + i + "_progress", (int)activeGoals[i].Type);
			}
		}

		public mg_jr_Goal.GoalType LoadGoalType(int _index)
		{
			Assert.IsTrue(_index >= 0 && _index < 3, "index out of range");
			return (mg_jr_Goal.GoalType)PlayerPrefs.GetInt("mg_jr_activeGoal_number_" + _index + "_progress");
		}

		public mg_jr_Goal.GoalDuration LoadGoalDuration(int _index)
		{
			Assert.IsTrue(_index >= 0 && _index < 3, "index out of range");
			return (mg_jr_Goal.GoalDuration)PlayerPrefs.GetInt("mg_jr_activeGoal_number_" + _index + "_duration");
		}
	}
}
