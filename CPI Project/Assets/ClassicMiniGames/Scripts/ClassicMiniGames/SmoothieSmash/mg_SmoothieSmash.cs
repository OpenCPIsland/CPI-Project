using DevonLocalization.Core;
using Disney.MobileNetwork;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using UnityEngine;

namespace SmoothieSmash
{
	[RequireComponent(typeof(SmoothieSmashInputObserver))]
	public class mg_SmoothieSmash : Minigame
	{
		public mg_ss_Resources Resources
		{
			get;
			private set;
		}

		public mg_ss_EGameMode GameMode
		{
			get;
			private set;
		}

		public mg_ss_GameLogic GameLogic
		{
			get;
			private set;
		}

		public SmoothieSmashInputObserver InputObserver
		{
			get;
			private set;
		}

		protected void OnDestroy()
		{
			RemoveLogic();
			Resources.UnloadAllResources();
		}

		public override void Awake()
		{
			base.Awake();
			InputObserver = GetComponent<SmoothieSmashInputObserver>();
		}

		protected void Start()
		{
			Resources = new mg_ss_Resources();
			Resources.LoadResources();
			GameMode = mg_ss_EGameMode.NORMAL;
			SetMainCamera("mg_ss_MainCamera");
			ShowTitle();
		}

		public override void MinigameUpdate(float p_deltaTime)
		{
			if (GameLogic != null)
			{
				GameLogic.MinigameUpdate(p_deltaTime);
			}
		}

		private void RemoveLogic()
		{
			if (GameLogic != null)
			{
				GameLogic.Destroy();
			}
			GameLogic = null;
		}

		public void ShowTitle()
		{
			RemoveLogic();
			PopAllScreens();
			UIManager.Instance.OpenScreen("mg_ss_TitleScreen", false, null, null);
		}

		public void ShowInstructions()
		{
			string screenname = "mg_ss_InstructionNormalScreen";
			if (GameMode == mg_ss_EGameMode.SURVIVAL)
			{
				screenname = "mg_ss_InstructionSurvivalScreen";
			}
			UIManager.Instance.OpenScreen(screenname, false, null, null);
		}

		public void ShowGame()
		{
			Resources.LoadGameResources(GameMode);
			PopAllScreens();
			if (GameMode == mg_ss_EGameMode.NORMAL)
			{
				UIManager.Instance.OpenScreen("mg_ss_GameNormalScreen", false, null, null);
			}
			else
			{
				UIManager.Instance.OpenScreen("mg_ss_GameSurvivalScreen", false, null, null);
			}
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
				return Service.Get<Localizer>().GetTokenTranslation("Activity.SmoothieSmash.Title");
			}
			return "Smoothie Smash";
		}

		public void SwitchGameModes()
		{
			if (GameMode == mg_ss_EGameMode.NORMAL)
			{
				GameMode = mg_ss_EGameMode.SURVIVAL;
			}
			else
			{
				GameMode = mg_ss_EGameMode.NORMAL;
			}
		}

		public void SetLogic(mg_ss_GameLogic p_logic, mg_ss_GameScreen p_screen)
		{
			GameLogic = p_logic;
			GameLogic.Initialize(p_screen);
		}

		public override void OnQuit()
		{
			if (GameLogic != null)
			{
				base.CoinsEarned += GameLogic.Scoring.Coins;
			}
			base.OnQuit();
		}
	}
}
