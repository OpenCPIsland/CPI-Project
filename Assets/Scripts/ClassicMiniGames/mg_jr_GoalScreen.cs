using DisneyMobile.CoreUnitySystems;
using JetpackReboot;
using MinigameFramework;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class mg_jr_GoalScreen : MinigameScreen
{
	private const float WAIT_TIME_BEFORE_BEGIN_REPLACE = 1f;

	private Button m_continueButton = null;

	private List<mg_jr_UIGoalBar> m_goalBars = new List<mg_jr_UIGoalBar>();

	private List<mg_jr_FlyByPenguin> m_flyByPenguins = new List<mg_jr_FlyByPenguin>();

	private mg_jr_GoalManager m_goalManager;

	private LinkedList<int> m_completedGoalIndexes = new LinkedList<int>();

	protected override void Awake()
	{
		base.Awake();
		base.HasPauseButton = false;
		base.ShouldShowPauseOver = false;
	}

	private IEnumerator Start()
	{
		mg_JetpackReboot minigame = MinigameManager.GetActive<mg_JetpackReboot>();
		m_goalManager = minigame.GoalManager;
		m_goalBars.AddRange(from x in GetComponentsInChildren<mg_jr_UIGoalBar>()
			orderby x.transform.position.y
			select x);
		IList<mg_jr_Goal> goals = m_goalManager.ActiveGoals;
		Assert.AreEqual(m_goalBars.Count, goals.Count, "Active goal count does not equal available goal bars");
		m_flyByPenguins.AddRange(from x in GetComponentsInChildren<mg_jr_FlyByPenguin>(true)
			orderby x.transform.position.y
			select x);
		Assert.AreEqual(m_flyByPenguins.Count, m_goalBars.Count, "Active flyby penguin count does not equal available goal bars");
		foreach (mg_jr_FlyByPenguin flyByPenguin in m_flyByPenguins)
		{
			flyByPenguin.FlyByCompleted += ReplaceCompletedGoal;
		}
		for (int i = 0; i < goals.Count; i++)
		{
			Assert.NotNull(goals[i], "Null goal in goal list");
			m_goalBars[i].GoalToDisplay = goals[i];
			if (goals[i].IsCurrentLevelComplete)
			{
				minigame.GameLogic.Player.AwardBonusCoins(goals[i].RewardForCurrentLevel);
				m_completedGoalIndexes.AddLast(i);
			}
		}
		minigame.MusicManager.SelectTrack(mg_jr_Sound.MENU_MUSIC_AMBIENT.ClipName());
		m_goalManager.ReplaceCompletedGoals();
		yield return new WaitForSeconds(1f);
		ReplaceCompletedGoal();
	}

	private void ReplaceCompletedGoal()
	{
		if (m_completedGoalIndexes.First != null)
		{
			int value = m_completedGoalIndexes.First.Value;
			m_completedGoalIndexes.RemoveFirst();
			mg_jr_UIGoalBar mg_jr_UIGoalBar = m_goalBars[value];
			mg_jr_UIGoalBar.GoalToDisplay = m_goalManager.ActiveGoals[value];
			m_flyByPenguins[value].AttachGoalBar(mg_jr_UIGoalBar);
			m_flyByPenguins[value].PerformFlyBy();
			MinigameManager.GetActive().PlaySFX(mg_jr_Sound.GOAL_FLY_BY.ClipName());
		}
		else
		{
			m_continueButton.gameObject.SetActive(true);
		}
	}

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		base.LoadUI(propertyList);
		m_continueButton = GetComponentInChildren<Button>();
		m_continueButton.gameObject.SetActive(false);
		m_continueButton.onClick.AddListener(OnResumeClicked);
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_continueButton.onClick.RemoveListener(OnResumeClicked);
	}

	private void OnResumeClicked()
	{
		MinigameManager.GetActive().PlaySFX("mg_jr_sfx_UISelect");
		UIManager.Instance.PopScreen();
		UIManager.Instance.OpenScreen("mg_jr_ResultScreen", false, null, null);
	}
}
