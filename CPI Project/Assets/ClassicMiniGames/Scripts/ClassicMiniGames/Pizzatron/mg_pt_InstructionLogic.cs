using DisneyMobile.CoreUnitySystems;
using MinigameFramework;

namespace Pizzatron
{
	public class mg_pt_InstructionLogic
	{
		private mg_Pizzatron m_minigame;

		public bool IsCandy
		{
			get
			{
				return m_minigame.IsCandy;
			}
		}

		public mg_pt_InstructionLogic()
		{
			m_minigame = MinigameManager.GetActive<mg_Pizzatron>();
		}

		public void PlayGame()
		{
			m_minigame.LaunchGame();
		}

		public void CloseInstructions()
		{
			UIManager.Instance.PopScreen();
		}
	}
}
