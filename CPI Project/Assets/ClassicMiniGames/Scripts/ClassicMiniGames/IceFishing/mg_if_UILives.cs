using MinigameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace IceFishing
{
	public class mg_if_UILives : MonoBehaviour
	{
		private mg_if_GameLogic m_logic;

		private Text m_label;

		private int m_livesDisplayed = -1;

		public void Start()
		{
			m_label = GetComponent<Text>();
			m_logic = MinigameManager.GetActive<mg_IceFishing>().Logic;
		}

		public void Update()
		{
			if (m_livesDisplayed != m_logic.Lives)
			{
				m_livesDisplayed = m_logic.Lives;
				m_label.text = m_livesDisplayed.ToString();
			}
		}
	}
}
