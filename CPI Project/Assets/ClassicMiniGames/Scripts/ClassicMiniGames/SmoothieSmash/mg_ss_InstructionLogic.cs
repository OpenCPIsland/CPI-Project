using DisneyMobile.CoreUnitySystems;
using MinigameFramework;

namespace SmoothieSmash
{
	public class mg_ss_InstructionLogic
	{
		private mg_SmoothieSmash m_minigame;

		public mg_ss_InstructionLogic()
		{
			m_minigame = MinigameManager.GetActive<mg_SmoothieSmash>();
		}

		public void PlayGame()
		{
			m_minigame.ShowGame();
		}

		public void CloseInstructions()
		{
			UIManager.Instance.PopScreen();
		}
	}
}
