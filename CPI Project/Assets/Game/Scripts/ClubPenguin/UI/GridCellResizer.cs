using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(GridLayoutGroup))]
	[RequireComponent(typeof(RectTransform))]
	public class GridCellResizer : MonoBehaviour
	{
		private GridLayoutGroup gridLayoutGroup;

		private RectTransform rectTransform;

		private float aspectRatio;

		private void Awake()
		{
			gridLayoutGroup = GetComponent<GridLayoutGroup>();
			rectTransform = GetComponent<RectTransform>();
			aspectRatio = gridLayoutGroup.cellSize.x / gridLayoutGroup.cellSize.y;
			resizeGridCellSize();
		}

		private void OnRectTransformDimensionsChange()
		{
			resizeGridCellSize();
		}

		private void resizeGridCellSize()
		{
			if (gridLayoutGroup != null && rectTransform != null && rectTransform.rect.height > 0f && Math.Abs(gridLayoutGroup.cellSize.y - rectTransform.rect.height) > float.Epsilon)
			{
				Vector2 cellSize = default(Vector2);
				cellSize.y = rectTransform.rect.height;
				cellSize.x = rectTransform.rect.height * aspectRatio;
				gridLayoutGroup.cellSize = cellSize;
			}
		}
	}
}
