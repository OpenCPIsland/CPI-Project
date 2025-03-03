using DisneyMobile.CoreUnitySystems;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_DebugGoalBoost : MonoBehaviour
	{
		private mg_jr_UIGoalBar m_goalBar;

		public int m_goalNumber = 0;

		private void Awake()
		{
			m_goalBar = base.transform.parent.GetComponent<mg_jr_UIGoalBar>();
		}

		public void BoostGoal()
		{
			mg_jr_Goal goalToDisplay = m_goalBar.GoalToDisplay;
			float num = (float)(goalToDisplay.CurrentLevelTarget - 1) - goalToDisplay.Progress;
			if (num <= 0f)
			{
				DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "Don't reduce progress");
				return;
			}
			bool condition = goalToDisplay.AddToProgressAndCheckCompletion(num);
			m_goalBar.UpdateGoalDisplay();
			Assert.IsFalse(condition, "this shouldn't complete a goal");
		}
	}
}
