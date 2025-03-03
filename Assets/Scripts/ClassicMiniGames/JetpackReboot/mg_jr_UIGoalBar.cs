using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace JetpackReboot
{
	public class mg_jr_UIGoalBar : MonoBehaviour
	{
		private Image m_progressMask;

		private Text m_progressLabel;

		private Text m_goalDescription;

		private Text m_awardAmount;

		private Animator m_goalSparkle;

		private mg_jr_Goal m_goalToDisplay;

		public mg_jr_Goal GoalToDisplay
		{
			get
			{
				return m_goalToDisplay;
			}
			set
			{
				m_goalToDisplay = value;
				UpdateGoalDisplay();
			}
		}

		private void Awake()
		{
			Text[] componentsInChildren = base.gameObject.GetComponentsInChildren<Text>();
			Text[] array = componentsInChildren;
			foreach (Text text in array)
			{
				if (text.gameObject.name == "mg_jr_AwardAmount")
				{
					m_awardAmount = text;
				}
				else if (text.gameObject.name == "mg_jr_GoalDescription")
				{
					m_goalDescription = text;
				}
				else if (text.gameObject.name == "mg_jr_GoalProgressLabel")
				{
					m_progressLabel = text;
				}
			}
			Transform transform = base.transform.Find("mg_jr_GoalSparkle");
			Assert.NotNull(transform, "sparkle transform not found");
			m_goalSparkle = transform.GetComponent<Animator>();
			Assert.NotNull(m_awardAmount, "Award label not found");
			Assert.NotNull(m_goalDescription, "Goal description label not found");
			Assert.NotNull(m_progressLabel, "Goal progress label not found");
			Transform transform2 = base.transform.Find("mg_jr_GoalProgressMask");
			m_progressMask = transform2.GetComponent<Image>();
		}

		public void UpdateGoalDisplay()
		{
			m_progressLabel.text = GoalToDisplay.TextDescriptionOfProgress();
			m_awardAmount.text = string.Concat(GoalToDisplay.RewardForCurrentLevel);
			m_goalDescription.text = GoalToDisplay.TextDescriptionOfGoal();
			m_progressMask.fillAmount = 1f - GoalToDisplay.ProgressPercentage;
			if (GoalToDisplay.IsCurrentLevelComplete)
			{
				m_goalSparkle.SetBool("Sparkle", true);
			}
			else
			{
				m_goalSparkle.SetBool("Sparkle", false);
			}
		}
	}
}
