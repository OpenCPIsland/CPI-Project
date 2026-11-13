using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using System.Collections.Generic;

namespace ClubPenguin.Adventure
{
	[ActionCategory("World")]
	public class ChangeSceneAction : FsmStateAction
	{
		public string TargetScene;

		public string LoadingSplashScreenOverride;

		public string LoadingScene = "Loading";

		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(LoadingSplashScreenOverride))
			{
				loadSceneWithArgs();
			}
			else
			{
				loadScene();
			}
			Finish();
		}

		private void loadScene()
		{
			if (Service.Get<ZoneTransitionService>().GetZoneBySceneName(TargetScene) != null)
			{
				Service.Get<ZoneTransitionService>().LoadZone(TargetScene, LoadingScene);
			}
			else
			{
				Service.Get<SceneTransitionService>().LoadScene(TargetScene, LoadingScene);
			}
		}

		private void loadSceneWithArgs()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(SceneTransitionService.SceneArgs.LoadingScreenOverride.ToString(), LoadingSplashScreenOverride);
			if (Service.Get<ZoneTransitionService>().GetZoneBySceneName(TargetScene) != null)
			{
				Service.Get<ZoneTransitionService>().LoadZone(TargetScene, LoadingScene, null, null, dictionary);
			}
			else
			{
				Service.Get<SceneTransitionService>().LoadScene(TargetScene, LoadingScene, dictionary);
			}
		}
	}
}
