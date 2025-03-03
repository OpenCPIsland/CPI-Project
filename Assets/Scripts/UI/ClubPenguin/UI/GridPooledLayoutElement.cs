using Disney.LaunchPadFramework;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(GridLayoutGroup))]
	public class GridPooledLayoutElement : AbstractPooledLayoutElement
	{
		private struct IntVector2
		{
			public int X;

			public int Y;

			public IntVector2(int x, int y)
			{
				X = x;
				Y = y;
			}
		}

		public int MinRows;

		public int MinColumns;

		private GridLayoutGroup _gridLayoutGroup;

		private GridLayoutGroup gridLayoutGroup
		{
			get
			{
				if (_gridLayoutGroup == null)
				{
					_gridLayoutGroup = GetComponent<GridLayoutGroup>();
				}
				return _gridLayoutGroup;
			}
		}

		public override float GetPreferredWidth(int elementCount, bool ignoreRestrictions = false)
		{
			IntVector2 dimensions = getDimensions(elementCount);
			int num = gridLayoutGroup.padding.left + gridLayoutGroup.padding.right;
			int num2 = (!ignoreRestrictions && MinColumns > 0) ? Math.Max(MinColumns, dimensions.X) : dimensions.X;
			float num3 = (float)num2 * gridLayoutGroup.cellSize.x;
			float num4 = (num2 > 0) ? (gridLayoutGroup.spacing.x * (float)(num2 - 1)) : 0f;
			return (float)num + num3 + num4;
		}

		public override float GetPreferredHeight(int elementCount)
		{
			IntVector2 dimensions = getDimensions(elementCount);
			int num = gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom;
			int num2 = (MinRows > 0) ? Math.Max(MinRows, dimensions.Y) : dimensions.Y;
			float num3 = (float)num2 * gridLayoutGroup.cellSize.y;
			float num4 = (num2 > 0) ? (gridLayoutGroup.spacing.y * (float)(num2 - 1)) : 0f;
			return (float)num + num3 + num4;
		}

		private IntVector2 getDimensions(int count)
		{
			int x = 0;
			int y = 0;
			int constraintCount = gridLayoutGroup.constraintCount;
			if (gridLayoutGroup.constraint != 0)
			{
				if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount && gridLayoutGroup.startAxis == GridLayoutGroup.Axis.Horizontal)
				{
					if (count <= constraintCount)
					{
						x = count;
						y = 1;
					}
					else
					{
						x = constraintCount;
						y = count / constraintCount + ((count % constraintCount > 0) ? 1 : 0);
					}
				}
				else if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount && gridLayoutGroup.startAxis == GridLayoutGroup.Axis.Vertical)
				{
					if (count <= constraintCount)
					{
						x = 1;
						y = count;
					}
					else
					{
						x = count / constraintCount + ((count % constraintCount > 0) ? 1 : 0);
						y = constraintCount;
					}
				}
				else
				{
					Log.LogError(this, "Does not support this combination of constraint type and start axis");
				}
			}
			else
			{
				Log.LogError(this, "Does not support flexible constraint count");
			}
			return new IntVector2(x, y);
		}
	}
}
