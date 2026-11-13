using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(GridLayoutGroup))]
	public class PhysicalSizeGridScaler : PhysicalSizeScaler
	{
		public float CellWidth_Iphone5 = 0f;

		public float CellWidth_Ipad = 0f;

		public float CellHeight_Iphone5 = 0f;

		public float CellHeight_Ipad = 0f;

		public float CellSpacing_Iphone5 = 0f;

		public float CellSpacing_Ipad = 0f;

		private GridLayoutGroup gridLayoutGroup;

		private void Start()
		{
			gridLayoutGroup = GetComponent<GridLayoutGroup>();
			ApplyScale();
		}

		private void ApplyScale()
		{
			Vector2 targetCellDimensions = GetTargetCellDimensions();
			Vector2 targetCellSpacingDimensions = GetTargetCellSpacingDimensions();
			gridLayoutGroup.cellSize = targetCellDimensions;
			gridLayoutGroup.spacing = targetCellSpacingDimensions;
		}

		private Vector2 GetTargetCellDimensions()
		{
			Vector2 result = default(Vector2);
			float deviceSize = GetDeviceSize();
			float num = NormalizeScaleProperty(CellWidth_Ipad, CellWidth_Iphone5);
			float num2 = NormalizeScaleProperty(CellHeight_Ipad, CellHeight_Iphone5);
			result.x = CellWidth_Iphone5 + (deviceSize - 4f) * num;
			result.y = CellHeight_Iphone5 + (deviceSize - 4f) * num2;
			return result;
		}

		private Vector2 GetTargetCellSpacingDimensions()
		{
			Vector2 result = default(Vector2);
			float deviceSize = GetDeviceSize();
			float num = NormalizeScaleProperty(CellSpacing_Ipad, CellSpacing_Iphone5);
			result.x = CellSpacing_Iphone5 + (deviceSize - 4f) * num;
			result.y = result.x;
			return result;
		}
	}
}
