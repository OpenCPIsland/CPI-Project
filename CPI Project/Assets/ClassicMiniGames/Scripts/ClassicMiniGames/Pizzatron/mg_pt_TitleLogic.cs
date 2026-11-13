using MinigameFramework;

namespace Pizzatron
{
	public class mg_pt_TitleLogic
	{
		public mg_Pizzatron Minigame
		{
			get;
			private set;
		}

		public bool IsCandy
		{
			get
			{
				return Minigame.IsCandy;
			}
			set
			{
				Minigame.IsCandy = value;
			}
		}

		public mg_pt_TitleLogic()
		{
			Minigame = MinigameManager.GetActive<mg_Pizzatron>();
		}

		public void OnPlayClicked()
		{
			Minigame.LaunchGame();
		}

		public void OnInstructionsClicked()
		{
			Minigame.ShowInstructions();
		}
	}
}
