using MinigameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace SmoothieSmash
{
	public class mg_ss_UIScore : MonoBehaviour
	{
		private int m_scoreDisplayed;

		private mg_ss_Score m_scoring;

		private Text m_text;

		protected void Awake()
		{
			m_text = GetComponentInChildren<Text>();
		}

		protected void Start()
		{
			m_scoring = MinigameManager.GetActive<mg_SmoothieSmash>().GameLogic.Scoring;
		}

		protected void Update()
		{
			if (m_scoring.Score != m_scoreDisplayed)
			{
				m_scoreDisplayed = m_scoring.Score;
				m_text.text = m_scoreDisplayed.ToString();
			}
		}
	}
}
