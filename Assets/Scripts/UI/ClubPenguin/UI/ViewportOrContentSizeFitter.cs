using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(ContentSizeFitter))]
	public class ViewportOrContentSizeFitter : MonoBehaviour, ILayoutSelfController, ILayoutController
	{
		public RectTransform Viewport;

		public bool ControlWidth;

		public bool ControlHeight;

		private RectTransform rectTransform;

		private ContentSizeFitter contentSizeFitter;

		private bool doRebuild;

		private void Awake()
		{
			contentSizeFitter = GetComponent<ContentSizeFitter>();
			rectTransform = (base.transform as RectTransform);
		}

		private void Update()
		{
			if (doRebuild)
			{
				LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
				doRebuild = false;
			}
		}

		public void SetLayoutHorizontal()
		{
			RectTransform rectTransform = (Viewport != null) ? Viewport : (base.transform.parent as RectTransform);
			if (ControlWidth)
			{
				if (LayoutUtility.GetPreferredWidth(this.rectTransform) < rectTransform.rect.width)
				{
					doRebuild = (contentSizeFitter.horizontalFit != ContentSizeFitter.FitMode.Unconstrained);
					contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
				}
				else
				{
					doRebuild = (contentSizeFitter.horizontalFit != ContentSizeFitter.FitMode.PreferredSize);
					contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
				}
			}
		}

		public void SetLayoutVertical()
		{
			RectTransform rectTransform = (Viewport != null) ? Viewport : (base.transform.parent as RectTransform);
			if (ControlHeight)
			{
				if (LayoutUtility.GetPreferredHeight(this.rectTransform) < rectTransform.rect.height)
				{
					doRebuild = (contentSizeFitter.verticalFit != ContentSizeFitter.FitMode.Unconstrained);
					contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
				}
				else
				{
					doRebuild = (contentSizeFitter.verticalFit != ContentSizeFitter.FitMode.PreferredSize);
					contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
				}
			}
		}
	}
}
