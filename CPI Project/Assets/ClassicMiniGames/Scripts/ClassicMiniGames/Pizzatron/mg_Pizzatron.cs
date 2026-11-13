using DevonLocalization.Core;
using Disney.MobileNetwork;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;

namespace Pizzatron
{
	public class mg_Pizzatron : Minigame
	{
		public mg_pt_Resources Resources
		{
			get;
			private set;
		}

		public bool IsCandy
		{
			get;
			set;
		}

		public mg_pt_GameLogic GameLogic
		{
			get;
			private set;
		}

		private void Start()
		{
			IsCandy = false;
			Resources = new mg_pt_Resources();
			Resources.LoadResources();
			SetMainCamera("mg_pt_MainCamera");
			ShowTitle();
		}

		private void OnDestroy()
		{
			Resources.UnloadAllResources();
		}

		public void ShowTitle()
		{
			RemoveLogic();
			PopAllScreens();
			UIManager.Instance.OpenScreen("mg_pt_TitleScreen", false, null, null);
		}

		public void LaunchGame()
		{
			Resources.LoadGameResources(IsCandy);
			PopAllScreens();
			UIManager.Instance.OpenScreen("mg_pt_GameScreen", false, null, null);
		}

		public void ShowInstructions()
		{
			UIManager.Instance.OpenScreen("mg_pt_InstructionsScreen", false, null, null);
		}

		public override void MinigameUpdate(float p_deltaTime)
		{
			if (GameLogic != null)
			{
				GameLogic.MinigameUpdate(p_deltaTime);
			}
		}

		public override string GetMinigameName()
		{
			if (Service.IsSet<Localizer>())
			{
				return Service.Get<Localizer>().GetTokenTranslation("Activity.Pizzatron.Title");
			}
			return "Pizzatron 3000";
		}

		private void PopAllScreens()
		{
			while (UIManager.Instance.ScreenCount > 0)
			{
				UIManager.Instance.PopScreen();
			}
		}

		public void SetLogic(mg_pt_GameLogic p_logic)
		{
			GameLogic = p_logic;
		}

		private void RemoveLogic()
		{
			if (GameLogic != null)
			{
				GameLogic.Destroy();
			}
			GameLogic = null;
		}

		protected override void OnPause()
		{
			if (GameLogic != null)
			{
				GameLogic.DisableInput();
			}
			base.OnPause();
		}

		protected override void OnResume()
		{
			base.OnResume();
			if (GameLogic != null)
			{
				GameLogic.EnableInput();
			}
		}
	}
}
