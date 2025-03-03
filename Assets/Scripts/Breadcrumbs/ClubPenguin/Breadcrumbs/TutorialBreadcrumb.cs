using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Breadcrumbs
{
	public class TutorialBreadcrumb : MonoBehaviour
	{
		private const string ANIMATOR_IS_SHOWING_BREADCRUMB = "isBreadcrumb";

		private static PrefabContentKey arrowPrefabKey = new PrefabContentKey("QuestNavigationPrefabs/BreadcrumbNavigationArrow");

		public string BreadcrumbId;

		protected TutorialBreadcrumbController tutorialBreadcrumbController;

		private GameObject breadcrumbArrow;

		private bool isShowing = false;

		public event Action<GameObject> OnBreadcrumbShown;

		private void OnEnable()
		{
			if (BreadcrumbId != "")
			{
				getController().BreadcrumbIsOnScreen(BreadcrumbId, true);
				init();
				Content.LoadAsync<GameObject>(arrowPrefabKey.Key, onArrowPrefabLoaded);
			}
		}

		public void SetBreadcrumbId(string id)
		{
			BreadcrumbId = id;
			if (BreadcrumbId != "")
			{
				getController().BreadcrumbIsOnScreen(BreadcrumbId, true);
				init();
			}
		}

		protected void init()
		{
			if (getController().IsLastShowingBreadcrumb(BreadcrumbId))
			{
				showBreadcrumb();
			}
			else
			{
				hideBreadcrumb();
			}
		}

		protected void onBreadcrumbsUpdated()
		{
			if (getController().IsLastShowingBreadcrumb(BreadcrumbId))
			{
				showBreadcrumb();
			}
			else
			{
				hideBreadcrumb();
			}
		}

		protected void showBreadcrumb()
		{
			isShowing = true;
			if (breadcrumbArrow == null)
			{
				Content.LoadAsync<GameObject>(arrowPrefabKey.Key, onArrowPrefabLoaded);
			}
		}

		private void onArrowPrefabLoaded(string path, GameObject prefab)
		{
			if (isShowing && breadcrumbArrow == null)
			{
				breadcrumbArrow = UnityEngine.Object.Instantiate(prefab);
				breadcrumbArrow.transform.SetParent(base.transform, false);
				if (this.OnBreadcrumbShown != null)
				{
					this.OnBreadcrumbShown(breadcrumbArrow);
				}
			}
		}

		protected void hideBreadcrumb()
		{
			isShowing = false;
			if (breadcrumbArrow != null)
			{
				UnityEngine.Object.Destroy(breadcrumbArrow);
			}
		}

		private TutorialBreadcrumbController getController()
		{
			if (tutorialBreadcrumbController == null)
			{
				tutorialBreadcrumbController = Service.Get<TutorialBreadcrumbController>();
				tutorialBreadcrumbController.OnBreadcrumbsUpdated += onBreadcrumbsUpdated;
			}
			return tutorialBreadcrumbController;
		}

		private void OnDestroy()
		{
			getController().OnBreadcrumbsUpdated -= onBreadcrumbsUpdated;
			getController().BreadcrumbIsOnScreen(BreadcrumbId, false);
			this.OnBreadcrumbShown = null;
		}
	}
}
