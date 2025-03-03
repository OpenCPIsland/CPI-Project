using ClubPenguin.Analytics;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin.UI
{
	public static class ForcedUpgrade
	{
		private const string FORCED_UPGRADE_TITLE_TOKEN = "GlobalUI.ErrorMessages.UpgradeNeeded.Title";

		private const string FORCED_UPGRADE_TOKEN = "GlobalUI.ErrorMessages.UpgradeNeeded";

		private const string LAUNCH_INSTALLER_TITLE_TOKEN = "LauncherUI.Note.NeedUpdate";

		private const string LAUNCH_INSTALLER_BODY_TOKEN = "GlobalUI.ErrorMessages.Update";

		private const string BLOCK_PLAYER_PROGRESS_TITLE_TOKEN = "GlobalUI.ErrorMessages.QuitGame.Title";

		private const string BLOCK_PLAYER_PROGRESS_TOKEN = "GlobalUI.ErrorMessages.QuitGame.Description\n\n";

		private const string UPGRADE_AVAILABLE_TITLE_TOKEN = "GlobalUI.Prompts.UpgradeAvailable.Title";

		private const string UPGRADE_AVAILABLE_TOKEN = "GlobalUI.Prompts.UpgradeAvailable";

		private static readonly SpriteContentKey iconForcedUpgradeKey = new SpriteContentKey("Images/Prompt_ForceUpgrade");

		private static readonly SpriteContentKey iconBlockPlayerProgressKey = new SpriteContentKey("Images/Prompt_ForceUpgrade");

		private static readonly SpriteContentKey iconUpgradeAvailableKey = new SpriteContentKey("Images/Prompt_ForceUpgrade");

		[Invokable("UI.OpenForcedUpgradePrompt")]
		public static void OpenForcedUpgradePrompt()
		{
			Content.LoadAsync(onForcedUpgradeIconLoaded, iconForcedUpgradeKey);
		}

		private static void onForcedUpgradeIconLoaded(string path, Sprite icon)
		{
			DPrompt data = new DPrompt("LauncherUI.Note.NeedUpdate", "GlobalUI.ErrorMessages.Update", DPrompt.ButtonFlags.OK, icon, true, false);
			Service.Get<ICPSwrveService>().Error("forcedupgrade_prompt", "upgrade_needed", null, SceneManager.GetActiveScene().name);
			Service.Get<ICPSwrveService>().Action("upgrade_prompt", "view");
			Service.Get<PromptManager>().ShowPrompt(data, onForcedUpgradePromptClosed);
		}

		private static void onForcedUpgradePromptClosed(DPrompt.ButtonFlags flags)
		{
			Service.Get<ICPSwrveService>().Action("upgrade_prompt", "mandatory");
			openAppUpdatePage();
			openBlockPlayerProgressPrompt();
		}

		private static void openAppUpdatePage()
		{
			IPlatformProcess platformProcess = PlatformProcessBuilder.BuildLauncherProcess();
			platformProcess.Execute();
			QuitHelper.Quit();
		}

		private static void openBlockPlayerProgressPrompt()
		{
			Content.LoadAsync(onBlockPlayerProgressIconLoaded, iconBlockPlayerProgressKey);
		}

		private static void onBlockPlayerProgressIconLoaded(string path, Sprite icon)
		{
			DPrompt data = new DPrompt("GlobalUI.ErrorMessages.QuitGame.Title", "GlobalUI.ErrorMessages.QuitGame.Description\n\n", DPrompt.ButtonFlags.None, icon, true, false);
			Service.Get<ICPSwrveService>().Error("forcedupgrade_prompt", "block_progress", null, SceneManager.GetActiveScene().name);
			Service.Get<PromptManager>().ShowPrompt(data, onBlockPlayerProgressPromptClosed);
		}

		private static void onBlockPlayerProgressPromptClosed(DPrompt.ButtonFlags flags)
		{
		}

		[Invokable("UI.OpenOptionalUpgradePrompt")]
		public static void OpenOptionalUpgradePrompt()
		{
			Content.LoadAsync(onOptionalUpgradeIconLoaded, iconUpgradeAvailableKey);
		}

		private static void onOptionalUpgradeIconLoaded(string path, Sprite icon)
		{
			DPrompt data = new DPrompt("GlobalUI.Prompts.UpgradeAvailable.Title", "GlobalUI.Prompts.UpgradeAvailable", DPrompt.ButtonFlags.NO | DPrompt.ButtonFlags.YES, icon, true, false);
			Service.Get<ICPSwrveService>().Action("upgrade_prompt", "view");
			Service.Get<PromptManager>().ShowPrompt(data, onOptionalUpgradePromptClosed);
		}

		private static void onOptionalUpgradePromptClosed(DPrompt.ButtonFlags flags)
		{
			if (flags == DPrompt.ButtonFlags.YES)
			{
				Service.Get<ICPSwrveService>().Action("upgrade_prompt", "yes");
				openAppUpdatePage();
			}
			else
			{
				Service.Get<ICPSwrveService>().Action("upgrade_prompt", "no");
			}
		}
	}
}
