using DevonLocalization.Core;
using Disney.MobileNetwork;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using UnityEngine;

namespace BeanCounter
{
	public class mg_BeanCounter : Minigame
	{
		private mg_bc_Resouces m_resources = new mg_bc_Resouces();

		private mg_bc_GameLogic m_gameLogic;

		internal mg_bc_Resouces Resources
		{
			get
			{
				return m_resources;
			}
		}

		internal mg_bc_GameLogic GameLogic
		{
			get
			{
				return m_gameLogic;
			}
		}

		public mg_bc_InputManager InputManager
		{
			get;
			set;
		}

		public mg_bc_EGameMode GameMode
		{
			get;
			set;
		}

		private void Start()
		{
			GameMode = mg_bc_EGameMode.COFFEE_NORMAL;
			m_resources.LoadResources();
			m_resources.GetInstancedResource(mg_bc_EResourceList.TITLE_ASSET_SOUNDS);
			SetMainCamera("mg_bc_MainCamera");
			InputManager.Prepare(base.MainCamera);
			UIManager.Instance.OpenScreen("mg_bc_TitleScreen", false, null, null);
		}

		public override void MinigameUpdate(float _deltaTime)
		{
			if (m_gameLogic != null)
			{
				m_gameLogic.MinigameUpdate(_deltaTime);
			}
		}

		public void StartGame()
		{
			m_resources.GetInstancedResource(mg_bc_EResourceList.GAME_ASSET_GAME_LOGIC);
		}

		public void OnDestroy()
		{
			InputManager.TidyUp(base.MainCamera);
			InputManager = null;
			m_resources.UnloadAllResources();
		}

		internal void SetLogic(mg_bc_GameLogic _logic)
		{
			m_gameLogic = _logic;
			m_gameLogic.Initialize();
		}

		protected override void OnPause()
		{
			base.OnPause();
			if (InputManager != null)
			{
				InputManager.IsActive = false;
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
			if (InputManager != null)
			{
				InputManager.IsActive = true;
			}
		}

		public void ReturnToTitle()
		{
			if (m_gameLogic != null)
			{
				Object.Destroy(m_gameLogic.gameObject);
				m_gameLogic = null;
				UIManager.Instance.PopScreen();
			}
			UIManager.Instance.OpenScreen("mg_bc_TitleScreen", false, null, null);
		}

		public override string GetMinigameName()
		{
			if (Service.IsSet<Localizer>())
			{
				return Service.Get<Localizer>().GetTokenTranslation("Activity.BeanCounter.Title");
			}
			return "Bean Counters";
		}

		public void LaunchGame()
		{
			MinigameManager.GetActive().PlaySFX("mg_bc_sfx_UIGameStart");
			MinigameManager.GetActive<mg_BeanCounter>().Resources.LoadGameResources(GameMode);
			UIManager.Instance.OpenScreen("mg_bc_GameScreen", false, null, null);
		}
	}
}
