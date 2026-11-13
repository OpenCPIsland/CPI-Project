using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(GridLayoutGroup))]
	public class ResponsiveGridCentering : MonoBehaviour
	{
		private GridLayoutGroup gridLayoutGroup;

		private RectTransform parentRectTransform;

		private RectTransform rectTransform;

		private float paddingSize;

		private float spacingSize;

		private float itemSize;

		private TextAnchor originalChildAlignment;

		private Vector2[] originalAnchors = new Vector2[2];

		private Vector2 originalPivot;

		private int childCount;

		private bool isHorizontalResizing;

		private void Start()
		{
			gridLayoutGroup = GetComponent<GridLayoutGroup>();
			originalChildAlignment = gridLayoutGroup.childAlignment;
			rectTransform = GetComponent<RectTransform>();
			originalAnchors[0] = rectTransform.anchorMin;
			originalAnchors[1] = rectTransform.anchorMax;
			originalPivot = rectTransform.pivot;
			parentRectTransform = (RectTransform)base.transform.parent;
			if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.Flexible || gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount)
			{
				isHorizontalResizing = true;
			}
			else
			{
				isHorizontalResizing = false;
			}
		}

		private void LateUpdate()
		{
			if (itemSize == 0f)
			{
				if (base.transform.childCount > 0)
				{
					childCount = base.transform.childCount;
					itemSize = retrieveItemSize();
					paddingSize = retrievePaddingSize();
					spacingSize = retrieveSpacingSize();
					updateAlignment();
				}
			}
			else if (childCount != base.transform.childCount)
			{
				childCount = base.transform.childCount;
				updateAlignment();
			}
		}

		private void updateAlignment()
		{
			if (calculateShouldCenterChildren())
			{
				gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
				rectTransform.anchorMax = new Vector2(0.5f, 1f);
				rectTransform.anchorMin = new Vector2(0.5f, 0f);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
			}
			else
			{
				gridLayoutGroup.childAlignment = originalChildAlignment;
				rectTransform.anchorMax = originalAnchors[1];
				rectTransform.anchorMin = originalAnchors[0];
				rectTransform.pivot = originalPivot;
			}
		}

		private bool calculateShouldCenterChildren()
		{
			bool result = false;
			float num = itemSize + paddingSize + spacingSize;
			float num2 = num * (float)childCount / (float)gridLayoutGroup.constraintCount;
			if (num2 < retrieveParentSize())
			{
				result = true;
			}
			return result;
		}

		private float retrieveParentSize()
		{
			return isHorizontalResizing ? parentRectTransform.rect.width : parentRectTransform.rect.height;
		}

		private float retrieveItemSize()
		{
			return isHorizontalResizing ? gridLayoutGroup.cellSize.x : gridLayoutGroup.cellSize.y;
		}

		private float retrievePaddingSize()
		{
			return isHorizontalResizing ? gridLayoutGroup.padding.right : gridLayoutGroup.padding.bottom;
		}

		private float retrieveSpacingSize()
		{
			return isHorizontalResizing ? gridLayoutGroup.spacing.x : gridLayoutGroup.spacing.y;
		}
	}
}
