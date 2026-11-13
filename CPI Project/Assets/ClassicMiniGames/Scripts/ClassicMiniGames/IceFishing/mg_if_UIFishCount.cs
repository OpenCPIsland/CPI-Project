using MinigameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace IceFishing
{
	public class mg_if_UIFishCount : MonoBehaviour
	{
		private mg_if_GameLogic m_logic;

		private Text m_label;

		private int m_fishDisplayed = -1;

		public void Start()
		{
			m_label = GetComponent<Text>();
			m_logic = MinigameManager.GetActive<mg_IceFishing>().Logic;
		}

		public void Update()
		{
			if (m_fishDisplayed != m_logic.FishCaught)
			{
				m_fishDisplayed = m_logic.FishCaught;
				m_label.text = m_fishDisplayed.ToString();
			}
		}
	}
}
