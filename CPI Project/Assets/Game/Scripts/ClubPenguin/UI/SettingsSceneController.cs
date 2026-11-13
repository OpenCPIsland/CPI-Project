using Disney.Kelowna.Common;
using Disney.Kelowna.Common.GameObjectTree;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class SettingsSceneController : MonoBehaviour
	{
		public TreeController TreeController;

		private LoadingController loadingController;

		private void Awake()
		{
			loadingController = Service.Get<LoadingController>();
			TreeController.OnLoadingContent += onTreeControllerLoadingContent;
			TreeController.OnContentLoaded += onTreeControllerContentLoaded;
		}

		private void OnDestroy()
		{
			TreeController.OnLoadingContent -= onTreeControllerLoadingContent;
			TreeController.OnContentLoaded -= onTreeControllerContentLoaded;
			if (loadingController.HasLoadingSystem(this))
			{
				loadingController.RemoveLoadingSystem(this);
			}
		}

		private void onTreeControllerLoadingContent(TreeController treeController)
		{
			if (TreeController.Equals(treeController) && !loadingController.HasLoadingSystem(this))
			{
				loadingController.AddLoadingSystem(this);
			}
		}

		private void onTreeControllerContentLoaded(TreeController treeController)
		{
			if (TreeController.Equals(treeController) && loadingController.HasLoadingSystem(this))
			{
				loadingController.RemoveLoadingSystem(this);
			}
		}
	}
}
