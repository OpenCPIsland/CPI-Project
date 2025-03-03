using MinigameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace SmoothieSmash
{
	public class mg_ss_UIGoldenApple : MonoBehaviour
	{
		private mg_ss_ChaosManager m_chaosManager;

		private int m_applesDisplayed;

		private Text m_text;

		protected void Awake()
		{
			m_text = GetComponentInChildren<Text>();
		}

		protected void Start()
		{
			m_chaosManager = MinigameManager.GetActive<mg_SmoothieSmash>().GameLogic.ChaosManager;
			m_applesDisplayed = m_chaosManager.ApplesCollected;
			UpdateText(m_applesDisplayed);
		}

		protected void Update()
		{
			if (m_chaosManager.Activated)
			{
				float num = m_chaosManager.ActivationTimer / m_chaosManager.ChaosTotalTime;
				float f = (1f - num) / 0.2f;
				UpdateText((int)Mathf.Ceil(f));
			}
			else if (m_applesDisplayed != m_chaosManager.ApplesCollected)
			{
				m_applesDisplayed = m_chaosManager.ApplesCollected;
				UpdateText(m_applesDisplayed);
			}
		}

		private void UpdateText(int p_apples)
		{
			string text = p_apples + " / " + 5;
			m_text.text = text;
		}
	}
}
