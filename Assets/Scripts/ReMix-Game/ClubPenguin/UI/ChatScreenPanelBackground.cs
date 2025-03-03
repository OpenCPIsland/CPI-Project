using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ChatScreenPanelBackground : MonoBehaviour
	{
		[SerializeField]
		private GameObject instantContentParent;

		[SerializeField]
		private GameObject emoteContentParent;

		[SerializeField]
		private float minWidth;

		private float originalWidth;

		private ScrollRect instantContent;

		private ScrollRect emoteContent;

		private void Awake()
		{
			originalWidth = ((RectTransform)base.transform).rect.width;
		}

		private void Update()
		{
			if (instantContentParent.transform.childCount > 0)
			{
				if (instantContent == null)
				{
					instantContent = instantContentParent.GetComponentInChildren<ScrollRect>();
				}
				useContentWidth(instantContent);
			}
			else
			{
				instantContent = null;
			}
			if (emoteContentParent.transform.childCount > 0)
			{
				if (emoteContent == null)
				{
					emoteContent = emoteContentParent.GetComponentInChildren<ScrollRect>();
				}
				useContentWidth(emoteContent);
			}
			else
			{
				emoteContent = null;
			}
		}

		private void useContentWidth(ScrollRect scrollRect)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			float width = scrollRect.content.rect.width;
			if (width > minWidth)
			{
				if (width < originalWidth)
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
					((RectTransform)scrollRect.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
				}
				else
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalWidth);
					((RectTransform)scrollRect.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalWidth);
				}
			}
		}
	}
}
