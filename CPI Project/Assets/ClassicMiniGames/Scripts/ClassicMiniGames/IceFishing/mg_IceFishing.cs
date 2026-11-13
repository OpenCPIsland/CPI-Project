using DevonLocalization.Core;
using Disney.MobileNetwork;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;

namespace IceFishing
{
	public class mg_IceFishing : Minigame
	{
		internal mg_if_Resources Resources
		{
			get;
			private set;
		}

		public mg_if_GameLogic Logic
		{
			get;
			private set;
		}

		private void Start()
		{
			Resources = new mg_if_Resources();
			Resources.LoadResources();
			SetMainCamera("mg_if_MainCamera");
			ShowTitle();
		}

		private void OnDestroy()
		{
			Resources.UnloadAllResources();
		}

		public override void MinigameUpdate(float p_deltaTime)
		{
			if (Logic != null)
			{
				Logic.MinigameUpdate(p_deltaTime);
			}
		}

		public void ShowTitle()
		{
			PopAllScreens();
			UIManager.Instance.OpenScreen("mg_if_TitleScreen", false, null, null);
			Logic = null;
		}

		public void ShowGame()
		{
			Resources.LoadGameResources();
			PopAllScreens();
			UIManager.Instance.OpenScreen("mg_if_GameScreen", false, null, null);
			Logic = base.transform.GetComponentInChildren<mg_if_GameLogic>();
		}

		private void PopAllScreens()
		{
			while (UIManager.Instance.ScreenCount > 0)
			{
				UIManager.Instance.PopScreen();
			}
		}

		public override string GetMinigameName()
		{
			if (Service.IsSet<Localizer>())
			{
				return Service.Get<Localizer>().GetTokenTranslation("Activity.IceFishing.Title");
			}
			return "Ice Fishing";
		}
	}
}
