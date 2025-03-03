using DevonLocalization.Core;
using Disney.MobileNetwork;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;

namespace PuffleRoundup
{
	public class mg_PuffleRoundup : Minigame
	{
		private mg_pr_Resources m_resources = new mg_pr_Resources();

		internal mg_pr_Resources Resources
		{
			get
			{
				return m_resources;
			}
		}

		private void Start()
		{
			m_resources.LoadResources();
			SetMainCamera("mg_pr_MainCamera");
			MinigameManager.GetActive().PlaySFX("mg_pr_sfx_main");
			UIManager.Instance.OpenScreen("mg_pr_TitleScreen", false, null, null);
		}

		private void OnDestroy()
		{
			Resources.UnloadAllResources();
		}

		public override void MinigameUpdate(float _deltaTime)
		{
		}

		public override string GetMinigameName()
		{
			if (Service.IsSet<Localizer>())
			{
				return Service.Get<Localizer>().GetTokenTranslation("Activity.MiniGames.PuffleRoundup");
			}
			return "Puffle Roundup";
		}
	}
}
