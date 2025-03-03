using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class HorizontalScrollingLayoutElementPool : AbstractScrollingLayoutElementPool
	{
		[SerializeField]
		private bool flexibleHeight = false;

		protected override float getViewportSize()
		{
			return ((RectTransform)base.transform).rect.width;
		}

		protected override float getContainerMinBounds(RectTransform container)
		{
			return container.localPosition.x + ElementScrollRect.content.anchoredPosition.x;
		}

		protected override float getContainerMaxBounds(RectTransform container)
		{
			return getContainerMinBounds(container) + container.rect.width;
		}

		protected override void setUpElement(LayoutElement layoutElement, int count, int prefabIndex = 0, bool ignoreSizeRestrictions = false, Vector2 additionalPadding = default(Vector2))
		{
			float preferredWidth = pooledLayoutElements[prefabIndex].GetPreferredWidth(count, ignoreSizeRestrictions);
			layoutElement.preferredWidth = preferredWidth + additionalPadding.x;
			if (flexibleHeight)
			{
				layoutElement.flexibleHeight = 1f;
			}
			else
			{
				float num = layoutElement.preferredHeight = pooledLayoutElements[prefabIndex].GetPreferredHeight(count);
			}
		}

		protected override void setUpSpacer(LayoutElement layoutElement, float spacing)
		{
			layoutElement.preferredWidth = spacing;
			layoutElement.flexibleHeight = 1f;
		}

		public override void CenterOnElement(int elementIndex)
		{
			float num = ElementLayoutGroup.padding.left;
			int num2 = 0;
			float num3 = 0f;
			for (int i = 0; i < ElementLayoutGroup.transform.childCount; i++)
			{
				RectTransform rectTransform = ElementLayoutGroup.transform.GetChild(i) as RectTransform;
				float width = rectTransform.rect.width;
				if (rectTransform.GetComponentInChildren<AbstractPooledLayoutElement>() != null)
				{
					if (num2 == elementIndex)
					{
						num3 = width;
						break;
					}
					num2++;
				}
				num += width;
			}
			float num4 = (scrollViewportSize - num3) / 2f;
			ElementScrollRect.content.anchoredPosition = new Vector2(0f - num + num4, ElementScrollRect.content.anchoredPosition.y);
		}
	}
}
