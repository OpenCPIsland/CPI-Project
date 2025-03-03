using Disney.LaunchPadFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class VerticalGridPooledScrollRect : AbstractPooledScrollRect
	{
		protected const int POOL_SIZE_PADDING_FACTOR = 6;

		public RectTransform ParentRectTransform;

		private GridLayoutGroup gridLayoutGroup;

		protected override float rectSize
		{
			get
			{
				return base.rectTransform.rect.height;
			}
		}

		protected override float position
		{
			get
			{
				return scrollRect.content.anchoredPosition.y;
			}
		}

		protected override float sizeDelta
		{
			get
			{
				return scrollRect.content.sizeDelta.y;
			}
		}

		protected override void awake()
		{
			gridLayoutGroup = scrollRect.content.GetComponent<GridLayoutGroup>();
			if (gridLayoutGroup == null)
			{
				Log.LogError(this, "This script requires ScrollRect Content to have a GridLayoutGroup component.");
			}
		}

		private IEnumerator Start()
		{
			LayoutRebuilder.MarkLayoutForRebuild(base.transform as RectTransform);
			yield return new WaitForEndOfFrame();
			gridLayoutGroup.constraintCount = getConstraintCount(ParentRectTransform.rect.width);
		}

		private int getConstraintCount(float rectWidth)
		{
			float num = (float)(gridLayoutGroup.padding.left + gridLayoutGroup.padding.right) + gridLayoutGroup.cellSize.x;
			int num2 = 0;
			for (; num < rectWidth; num += gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x)
			{
				num2++;
			}
			return num2;
		}

		protected override int getPoolSize(GameObject item)
		{
			int num = (int)(base.rectTransform.rect.height / gridLayoutGroup.cellSize.y) * gridLayoutGroup.constraintCount * 6;
			if (num < 1)
			{
				Log.LogErrorFormatted(this, "Calculated negative pool size ({0})\nRect. Height = {1}\nCell size = {2}\nConstraint count = {3}", num, base.rectTransform.rect.height, gridLayoutGroup.cellSize.y, gridLayoutGroup.constraintCount);
				num = 1;
			}
			if (num > 1000)
			{
				Log.LogErrorFormatted(this, "Calculated excessive pool size ({0})\nRect. Height = {1}\nCell size = {2}\nConstraint count = {3}", num, base.rectTransform.rect.height, gridLayoutGroup.cellSize.y, gridLayoutGroup.constraintCount);
				num = 1000;
			}
			return num;
		}

		protected override int getScrollPosition(Vector2 position)
		{
			return (int)(position.y * 100f);
		}

		protected override float getPosition(RectTransform item)
		{
			return item.localPosition.y + scrollRect.content.localPosition.y;
		}

		public override void CenterOnElement(int elementIndex)
		{
			if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
			{
				int num = elementIndex + 1;
				float num2 = gridLayoutGroup.padding.top;
				int num3 = num / gridLayoutGroup.constraintCount + ((num % gridLayoutGroup.constraintCount > 0) ? 1 : 0);
				int num4 = num3 - 1;
				num2 += (float)num4 * gridLayoutGroup.cellSize.y;
				num2 += (float)(num4 - 1) * gridLayoutGroup.spacing.y;
				float height = scrollRect.GetComponent<RectTransform>().rect.height;
				float num5 = (height - gridLayoutGroup.cellSize.y) / 2f;
				scrollRect.content.anchoredPosition = new Vector2(0f - num2 + num5, scrollRect.content.anchoredPosition.x);
			}
			else
			{
				Log.LogError(this, "Does not support constraint types other than FixedColumnCount");
			}
		}
	}
}
