using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class VerticalScrollingLayoutElementPool : AbstractScrollingLayoutElementPool
	{
		protected override float getViewportSize()
		{
			return ((RectTransform)base.transform).rect.height;
		}

		protected override float getContainerMinBounds(RectTransform container)
		{
			return container.localPosition.y + ElementScrollRect.content.anchoredPosition.y;
		}

		protected override float getContainerMaxBounds(RectTransform container)
		{
			return getContainerMinBounds(container) + container.rect.height;
		}

		protected override void setUpElement(LayoutElement layoutElement, int count, int prefabIndex = 0, bool ignoreSizeRestrictions = false, Vector2 additionalPadding = default(Vector2))
		{
			float preferredHeight = pooledLayoutElements[prefabIndex].GetPreferredHeight(count);
			layoutElement.flexibleWidth = 1f;
			layoutElement.preferredHeight = preferredHeight;
		}

		protected override void setUpSpacer(LayoutElement layoutElement, float spacing)
		{
			layoutElement.preferredHeight = spacing;
			layoutElement.flexibleWidth = 1f;
		}

		public override void CenterOnElement(int elementIndex)
		{
			float num = ElementLayoutGroup.padding.top;
			int num2 = 0;
			float num3 = 0f;
			for (int i = 0; i < ElementLayoutGroup.transform.childCount; i++)
			{
				RectTransform rectTransform = ElementLayoutGroup.transform.GetChild(i) as RectTransform;
				float height = rectTransform.rect.height;
				if (rectTransform.GetComponentInChildren<AbstractPooledLayoutElement>() != null)
				{
					if (num2 == elementIndex)
					{
						num3 = height;
						break;
					}
					num2++;
				}
				num += height;
			}
			float num4 = (scrollViewportSize - num3) / 2f;
			ElementScrollRect.content.anchoredPosition = new Vector2(ElementScrollRect.content.anchoredPosition.x, 0f - num + num4);
		}
	}
}
