using MinigameFramework;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_UILevelUp : MonoBehaviour
	{
		private mg_pt_GameLogic m_gameLogic;

		private Animator m_animator;

		protected void Awake()
		{
			m_animator = GetComponent<Animator>();
			m_animator.enabled = false;
		}

		protected void Start()
		{
			m_gameLogic = MinigameManager.GetActive<mg_Pizzatron>().GameLogic;
			m_gameLogic.LevelUp = this;
		}

		public void ShowLevelUp()
		{
			m_gameLogic.Minigame.PlaySFX("mg_pt_sfx_level_up");
			m_animator.enabled = true;
			m_animator.Play(Animator.StringToHash("Base Layer.mg_pt_levelup"), 0, 0f);
		}

		private void OnAnimationEnded()
		{
			m_animator.enabled = false;
			m_gameLogic.ResetPizza();
		}
	}
}
