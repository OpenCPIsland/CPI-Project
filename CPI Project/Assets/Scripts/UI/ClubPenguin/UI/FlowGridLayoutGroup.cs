using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[AddComponentMenu("Layout/Extensions/Flow Grid Layout Group")]
	public class FlowGridLayoutGroup : LayoutGroup
	{
		public float PreferredCellWidth = 0f;

		public float PreferredCellHeight = 0f;

		public float SpacingX = 0f;

		public float SpacingY = 0f;

		public bool ExpandHorizontalSpacing = false;

		public bool ChildForceExpandWidth = false;

		public bool ChildForceExpandHeight = false;

		private float layoutHeight;

		private float calculatedRowChildWidth;

		private float calculatedRowChildHeight;

		private readonly IList<RectTransform> rowList = new List<RectTransform>();

		private readonly IList<float> positionXOffsets = new List<float>();

		protected bool IsCenterAlign
		{
			get
			{
				return base.childAlignment == TextAnchor.LowerCenter || base.childAlignment == TextAnchor.MiddleCenter || base.childAlignment == TextAnchor.UpperCenter;
			}
		}

		protected bool IsRightAlign
		{
			get
			{
				return base.childAlignment == TextAnchor.LowerRight || base.childAlignment == TextAnchor.MiddleRight || base.childAlignment == TextAnchor.UpperRight;
			}
		}

		protected bool IsMiddleAlign
		{
			get
			{
				return base.childAlignment == TextAnchor.MiddleLeft || base.childAlignment == TextAnchor.MiddleRight || base.childAlignment == TextAnchor.MiddleCenter;
			}
		}

		protected bool IsLowerAlign
		{
			get
			{
				return base.childAlignment == TextAnchor.LowerLeft || base.childAlignment == TextAnchor.LowerRight || base.childAlignment == TextAnchor.LowerCenter;
			}
		}

		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			float totalMin = GetMaximumChildWidth() + (float)base.padding.left + (float)base.padding.right;
			SetLayoutInputForAxis(totalMin, -1f, -1f, 0);
		}

		public override void SetLayoutHorizontal()
		{
			SetLayout(base.rectTransform.rect.width, 0, false);
		}

		public override void SetLayoutVertical()
		{
			SetLayout(base.rectTransform.rect.width, 1, false);
		}

		public override void CalculateLayoutInputVertical()
		{
			layoutHeight = SetLayout(base.rectTransform.rect.width, 1, true);
		}

		public float SetLayout(float width, int axis, bool layoutInput)
		{
			rowList.Clear();
			positionXOffsets.Clear();
			float height = base.rectTransform.rect.height;
			float num = base.rectTransform.rect.width - (float)base.padding.left - (float)base.padding.right;
			float num2 = IsLowerAlign ? base.padding.bottom : base.padding.top;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = Mathf.Min(PreferredCellWidth, num);
			for (int i = 0; i < base.rectChildren.Count; i++)
			{
				int index = IsLowerAlign ? (base.rectChildren.Count - 1 - i) : i;
				RectTransform item = base.rectChildren[index];
				if (num3 + num5 > num)
				{
					num3 -= SpacingX;
					if (!layoutInput)
					{
						LayoutRow(num3, num4, num, base.padding.left, CalculateRowVerticalOffset(height, num2, num4), axis);
					}
					rowList.Clear();
					num2 += num4;
					num2 += SpacingY;
					num4 = 0f;
					num3 = 0f;
				}
				num3 += num5;
				rowList.Add(item);
				if (PreferredCellHeight > num4)
				{
					num4 = PreferredCellHeight;
				}
				if (i < base.rectChildren.Count - 1)
				{
					num3 += SpacingX;
				}
			}
			if (!layoutInput)
			{
				num3 -= SpacingX;
				float yOffset = CalculateRowVerticalOffset(height, num2, num4);
				LayoutRow(num3, num4, num - ((rowList.Count > 1) ? SpacingX : 0f), base.padding.left, yOffset, axis, true);
			}
			rowList.Clear();
			num2 += num4;
			num2 += (float)(IsLowerAlign ? base.padding.top : base.padding.bottom);
			if (layoutInput && axis == 1)
			{
				SetLayoutInputForAxis(num2, num2, -1f, axis);
			}
			return num2;
		}

		protected void LayoutRow(float rowWidth, float rowHeight, float maxWidth, float xOffset, float yOffset, int axis, bool lastRow = false)
		{
			if (positionXOffsets.Count == 0)
			{
				layoutFirstRow(rowWidth, rowHeight, maxWidth, xOffset, yOffset, axis);
			}
			else
			{
				layoutRow(yOffset, axis);
			}
		}

		private void layoutFirstRow(float rowWidth, float rowHeight, float maxWidth, float xOffset, float yOffset, int axis)
		{
			int count = rowList.Count;
			float num = xOffset;
			if (!ChildForceExpandWidth && IsCenterAlign)
			{
				num += (maxWidth - rowWidth) * 0.5f;
			}
			else if (!ChildForceExpandWidth && IsRightAlign)
			{
				num += maxWidth - rowWidth;
			}
			float num2 = 0f;
			float num3 = 0f;
			if (ChildForceExpandWidth)
			{
				num2 = (maxWidth - rowWidth) / (float)count;
			}
			else if (ExpandHorizontalSpacing)
			{
				num3 = (maxWidth - rowWidth) / (float)(count - 1);
				if (count > 1)
				{
					if (IsCenterAlign)
					{
						num -= num3 * 0.5f * (float)(count - 1);
					}
					else if (IsRightAlign)
					{
						num -= num3 * (float)(count - 1);
					}
				}
			}
			calculatedRowChildWidth = Mathf.Min(PreferredCellWidth + num2, maxWidth);
			calculatedRowChildHeight = (ChildForceExpandHeight ? rowHeight : PreferredCellHeight);
			for (int i = 0; i < count; i++)
			{
				int index = IsLowerAlign ? (count - 1 - i) : i;
				RectTransform rect = rowList[index];
				float num4 = yOffset;
				if (IsMiddleAlign)
				{
					num4 += (rowHeight - calculatedRowChildHeight) * 0.5f;
				}
				else if (IsLowerAlign)
				{
					num4 += rowHeight - calculatedRowChildHeight;
				}
				if (ExpandHorizontalSpacing && i > 0)
				{
					num += num3;
				}
				if (axis == 0)
				{
					SetChildAlongAxis(rect, 0, num, calculatedRowChildWidth);
				}
				else
				{
					SetChildAlongAxis(rect, 1, num4, calculatedRowChildHeight);
				}
				positionXOffsets.Add(num);
				if (i < count - 1)
				{
					num += calculatedRowChildWidth + SpacingX;
				}
			}
		}

		private void layoutRow(float yOffset, int axis)
		{
			int count = rowList.Count;
			int num = IsRightAlign ? (positionXOffsets.Count - count) : 0;
			for (int i = 0; i < count; i++)
			{
				int index = IsLowerAlign ? (count - 1 - i) : i;
				RectTransform rect = rowList[index];
				if (axis == 0)
				{
					SetChildAlongAxis(rect, 0, positionXOffsets[num], calculatedRowChildWidth);
				}
				else
				{
					SetChildAlongAxis(rect, 1, yOffset, calculatedRowChildHeight);
				}
				num++;
			}
		}

		private float CalculateRowVerticalOffset(float groupHeight, float yOffset, float currentRowHeight)
		{
			if (IsLowerAlign)
			{
				return groupHeight - yOffset - currentRowHeight;
			}
			if (IsMiddleAlign)
			{
				return groupHeight * 0.5f - layoutHeight * 0.5f + yOffset;
			}
			return yOffset;
		}

		public float GetMaximumChildWidth()
		{
			float num = 0f;
			for (int i = 0; i < base.rectChildren.Count; i++)
			{
				float minWidth = LayoutUtility.GetMinWidth(base.rectChildren[i]);
				num = Mathf.Max(minWidth, num);
			}
			return num;
		}
	}
}
