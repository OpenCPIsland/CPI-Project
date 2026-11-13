using BeanCounter;
using MinigameFramework;
using UnityEngine;
using UnityEngine.UI;

public class mg_bc_GoalUIScript : MonoBehaviour
{
	private Text m_label;

	private void Start()
	{
		m_label = base.gameObject.GetComponent<Text>();
	}

	private void Update()
	{
		mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
		if (active != null && active.GameLogic != null)
		{
			int goalProgress = active.GameLogic.GetGoalProgress();
			int goal = active.GameLogic.GetGoal();
			m_label.text = goalProgress + "/" + goal;
		}
	}
}
