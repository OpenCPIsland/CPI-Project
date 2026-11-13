using ClubPenguin.Breadcrumbs;
using ClubPenguin.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(TutorialBreadcrumb))]
	public class TutorialBreadcrumbVisibilityController : MonoBehaviour
	{
		public bool CheckX;

		public bool CheckY;

		public bool CheckMinAndMax;

		private TutorialBreadcrumb tutorialBreadcrumb;

		private Image breadcrumbArrowImage;

		private RectTransformBoundsVisibility rectTransformBoundsVisibility;

		private void Awake()
		{
			tutorialBreadcrumb = GetComponent<TutorialBreadcrumb>();
			tutorialBreadcrumb.OnBreadcrumbShown += onBreadcrumbShown;
		}

		private void onBreadcrumbShown(GameObject breadcrumbArrow)
		{
			breadcrumbArrowImage = breadcrumbArrow.GetComponent<Image>();
			rectTransformBoundsVisibility = breadcrumbArrow.AddComponent<RectTransformBoundsVisibility>();
			rectTransformBoundsVisibility.Viewport = (GetComponentInParent<ScrollRect>().transform as RectTransform);
			rectTransformBoundsVisibility.CheckX = CheckX;
			rectTransformBoundsVisibility.CheckY = CheckY;
			rectTransformBoundsVisibility.CheckMinAndMax = CheckMinAndMax;
			rectTransformBoundsVisibility.OnVisibilityChanged += onVisibilityChanged;
			onVisibilityChanged(rectTransformBoundsVisibility.IsVisible);
		}

		private void onVisibilityChanged(bool isVisible)
		{
			breadcrumbArrowImage.enabled = isVisible;
		}

		private void OnDestroy()
		{
			if (tutorialBreadcrumb != null)
			{
				tutorialBreadcrumb.OnBreadcrumbShown -= onBreadcrumbShown;
			}
			if (rectTransformBoundsVisibility != null)
			{
				rectTransformBoundsVisibility.OnVisibilityChanged -= onVisibilityChanged;
			}
		}
	}
}
