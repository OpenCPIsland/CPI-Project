using MinigameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace IceFishing
{
	public class mg_if_UISpeechBubble : MonoBehaviour
	{
		private mg_if_GameLogic m_logic;

		private bool m_isActive;

		private Text m_label;

		private Image m_sprite;

		public void Start()
		{
			m_logic = MinigameManager.GetActive<mg_IceFishing>().Logic;
			m_label = GetComponentInChildren<Text>();
			m_sprite = GetComponentInChildren<Image>();
			SetActive(false);
		}

		public void Update()
		{
			bool flag = !m_logic.GameOver && m_logic.FishingRod.IsBroken;
			if (m_isActive != flag)
			{
				SetActive(flag);
			}
		}

		private void SetActive(bool p_active)
		{
			m_label.gameObject.SetActive(p_active);
			m_sprite.gameObject.SetActive(p_active);
			m_isActive = p_active;
		}
	}
}
