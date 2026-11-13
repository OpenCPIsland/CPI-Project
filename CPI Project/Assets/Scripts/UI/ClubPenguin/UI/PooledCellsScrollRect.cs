using Disney.LaunchPadFramework;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class PooledCellsScrollRect : AbstractPooledScrollRect
	{
		[Header("Number of scrollview pages that are in the item pool")]
		public float PagesToPool = 2.5f;

		private GridLayoutGroup gridLayoutGroup;

		protected override float rectSize
		{
			get
			{
				return base.rectTransform.rect.width;
			}
		}

		protected override float position
		{
			get
			{
				return scrollRect.content.anchoredPosition.x;
			}
		}

		protected override float sizeDelta
		{
			get
			{
				return scrollRect.content.sizeDelta.x;
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

		protected override int getPoolSize(GameObject item)
		{
			return (int)((float)gridLayoutGroup.constraintCount * PagesToPool * Mathf.Ceil(base.rectTransform.rect.width / gridLayoutGroup.cellSize.x));
		}

		protected override int getScrollPosition(Vector2 position)
		{
			return (int)(position.x * 100f);
		}

		protected override float getPosition(RectTransform item)
		{
			return item.localPosition.x + scrollRect.content.localPosition.x;
		}

		public override void CenterOnElement(int elementIndex)
		{
			if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount)
			{
				int num = elementIndex + 1;
				float num2 = gridLayoutGroup.padding.left;
				int num3 = num / gridLayoutGroup.constraintCount + ((num % gridLayoutGroup.constraintCount > 0) ? 1 : 0);
				int num4 = num3 - 1;
				num2 += (float)num4 * gridLayoutGroup.cellSize.x;
				num2 += (float)(num4 - 1) * gridLayoutGroup.spacing.x;
				float width = scrollRect.GetComponent<RectTransform>().rect.width;
				float num5 = (width - gridLayoutGroup.cellSize.x) / 2f;
				scrollRect.content.anchoredPosition = new Vector2(0f - num2 + num5, scrollRect.content.anchoredPosition.y);
				customStartPosition = scrollRect.content.anchoredPosition.x;
			}
			else
			{
				Log.LogError(this, "Does not support constraint types other than FixedRowCount");
			}
		}
	}
}
