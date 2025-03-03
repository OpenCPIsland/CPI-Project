using Disney.MobileNetwork;

namespace ClubPenguin.Actions
{
	public class OpenSceneAction : Action
	{
		public string TargetScene;

		public string LoadingScene;

		protected override void CopyTo(Action _openScene)
		{
			OpenSceneAction openSceneAction = _openScene as OpenSceneAction;
			openSceneAction.TargetScene = TargetScene;
			openSceneAction.LoadingScene = LoadingScene;
			base.CopyTo(_openScene);
		}

		protected override void Update()
		{
			if (Service.Get<ZoneTransitionService>().GetZoneBySceneName(TargetScene) != null)
			{
				Service.Get<ZoneTransitionService>().LoadZone(TargetScene, LoadingScene);
			}
			else
			{
				Service.Get<SceneTransitionService>().LoadScene(TargetScene, LoadingScene);
			}
			Completed();
		}
	}
}
