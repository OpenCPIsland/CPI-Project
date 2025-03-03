using MinigameFramework;

namespace SmoothieSmash
{
	public class mg_ss_TitleLogic
	{
		private mg_ss_ITitleScreen m_titleScreen;

		public mg_SmoothieSmash Minigame
		{
			get;
			private set;
		}

		public mg_ss_TitleLogic(mg_ss_ITitleScreen p_TitleScreen)
		{
			Minigame = MinigameManager.GetActive<mg_SmoothieSmash>();
			m_titleScreen = p_TitleScreen;
		}

		public void SwitchGameModes()
		{
			Minigame.SwitchGameModes();
			m_titleScreen.UpdateGameMode();
		}

		public void ShowInstructions()
		{
			Minigame.ShowInstructions();
		}

		public void PlayGame()
		{
			Minigame.ShowGame();
		}
	}
}
