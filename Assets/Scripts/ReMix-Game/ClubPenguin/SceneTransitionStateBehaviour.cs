using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class SceneTransitionStateBehaviour : StateMachineBehaviour
	{
		public string TargetScene;

		public string LoadingScene;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (string.IsNullOrEmpty(TargetScene))
			{
				Service.Get<ZoneTransitionService>().LoadCurrentZone(LoadingScene);
			}
			else if (Service.Get<ZoneTransitionService>().GetZoneBySceneName(TargetScene) != null)
			{
				Service.Get<ZoneTransitionService>().LoadZone(TargetScene, LoadingScene);
			}
			else
			{
				Service.Get<SceneTransitionService>().LoadScene(TargetScene, LoadingScene);
			}
		}
	}
}
