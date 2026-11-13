using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class GridLayoutScaler : MonoBehaviour
	{
		public GridLayoutScalerProperty[] ScaleProperties;

		public GridLayoutGroup GridLayout;

		public void Init(int numElements)
		{
			if (ScaleProperties.Length > 0 && numElements > 0)
			{
				int num = Mathf.Min(ScaleProperties.Length - 1, numElements - 1);
				applyScaleProperty(ScaleProperties[num]);
			}
		}

		private void applyScaleProperty(GridLayoutScalerProperty scaleProperty)
		{
			GridLayout.cellSize = scaleProperty.CellSize;
			GridLayout.spacing = scaleProperty.Spacing;
			GridLayout.constraintCount = scaleProperty.ConstraintCount;
			LayoutRebuilder.MarkLayoutForRebuild(GridLayout.GetComponent<RectTransform>());
		}
	}
}
