using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(GridLayoutGroup))]
	public class ResponsiveGridContentFitter : MonoBehaviour
	{
		private GridLayoutGroup gridLayoutGroup;

		private RectTransform parentScrollRect;

		private float previousScrollRectSize = 0f;

		private bool isHorizontalResizing;

		private bool isInitialized;

		private float itemSize = 0f;

		private float originalItemSize = 0f;

		private void Start()
		{
			gridLayoutGroup = GetComponent<GridLayoutGroup>();
			parentScrollRect = (RectTransform)base.transform.parent;
			if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.Flexible || gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount)
			{
				isHorizontalResizing = true;
				gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
			}
			else
			{
				isHorizontalResizing = false;
				gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			}
		}

		private int calculateConstraintCount(float parentScrollRectSize, float padding, float spacing)
		{
			float num = parentScrollRectSize - padding - itemSize;
			float num2 = itemSize + spacing;
			int num3 = Mathf.FloorToInt(num / num2);
			return num3 + 1;
		}

		private void updateRows()
		{
			int num = calculateConstraintCount(parentScrollRect.rect.height, gridLayoutGroup.padding.top, gridLayoutGroup.spacing.y);
			if (gridLayoutGroup.constraintCount != num)
			{
				gridLayoutGroup.constraintCount = num;
			}
			checkRowOverflow();
		}

		private void checkRowOverflow()
		{
			if (gridLayoutGroup.constraintCount == 1)
			{
				if (Math.Abs(previousScrollRectSize - 0f) > float.Epsilon && gridLayoutGroup.cellSize.y > previousScrollRectSize)
				{
					float num = gridLayoutGroup.cellSize.x / gridLayoutGroup.cellSize.y;
					Vector2 cellSize = new Vector2(previousScrollRectSize * num, previousScrollRectSize);
					gridLayoutGroup.cellSize = cellSize;
				}
				else if (Math.Abs(originalItemSize - 0f) > float.Epsilon && gridLayoutGroup.cellSize.y < originalItemSize)
				{
					float num = gridLayoutGroup.cellSize.x / gridLayoutGroup.cellSize.y;
					Vector2 cellSize = new Vector2(originalItemSize * num, originalItemSize);
					gridLayoutGroup.cellSize = cellSize;
				}
			}
		}

		private void updateColumns()
		{
			int num = calculateConstraintCount(parentScrollRect.rect.width, gridLayoutGroup.padding.left, gridLayoutGroup.spacing.x);
			if (gridLayoutGroup.constraintCount != num)
			{
				gridLayoutGroup.constraintCount = num;
			}
			base.transform.localPosition = Vector3.zero;
		}

		private void Update()
		{
			if (!isInitialized)
			{
				previousScrollRectSize = (isHorizontalResizing ? parentScrollRect.rect.height : parentScrollRect.rect.width);
				itemSize = (isHorizontalResizing ? gridLayoutGroup.cellSize.x : gridLayoutGroup.cellSize.y);
				originalItemSize = itemSize;
				if (Math.Abs(itemSize - 0f) > float.Epsilon)
				{
					isInitialized = true;
					if (base.transform.childCount > 0)
					{
						updateRows();
					}
				}
			}
			else if (isHorizontalResizing)
			{
				if (Math.Abs(previousScrollRectSize - parentScrollRect.rect.height) > float.Epsilon)
				{
					previousScrollRectSize = parentScrollRect.rect.height;
					updateRows();
				}
			}
			else if (Math.Abs(previousScrollRectSize - parentScrollRect.rect.width) > float.Epsilon)
			{
				previousScrollRectSize = parentScrollRect.rect.width;
				updateRows();
			}
		}
	}
}
