namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("Layout/Extensions/Curved Layout")]
	public class CurvedLayout : LayoutGroup
	{
		public Vector3 CurveOffset;

		[Tooltip("axis along which to place the items, Normalized before use")]
		public Vector3 itemAxis;

		[Tooltip("size of each item along the Normalized axis")]
		public float itemSize;

		public float centerpoint = 0.5f;

		protected override void OnEnable()
		{
			base.OnEnable();
			CalculateRadial();
		}

		public override void SetLayoutHorizontal()
		{
		}

		public override void SetLayoutVertical()
		{
		}

		public override void CalculateLayoutInputVertical()
		{
			CalculateRadial();
		}

		public override void CalculateLayoutInputHorizontal()
		{
			CalculateRadial();
		}

		private void CalculateRadial()
		{
			m_Tracker.Clear();
			if (base.transform.childCount == 0)
			{
				return;
			}
			Vector2 pivot = new Vector2((float)((int)base.childAlignment % 3) * 0.5f, (float)((int)base.childAlignment / 3) * 0.5f);
			Vector3 a = new Vector3(GetStartOffset(0, GetTotalPreferredSize(0)), GetStartOffset(1, GetTotalPreferredSize(1)), 0f);
			float num = 0f;
			float num2 = 1f / (float)base.transform.childCount;
			Vector3 b = itemAxis.normalized * itemSize;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				RectTransform rectTransform = (RectTransform)base.transform.GetChild(i);
				if (rectTransform != null)
				{
					m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.PivotX | DrivenTransformProperties.PivotY);
					Vector3 a2 = a + b;
					a = (rectTransform.localPosition = a2 + (num - centerpoint) * CurveOffset);
					rectTransform.pivot = pivot;
					Vector2 vector4 = rectTransform.anchorMin = (rectTransform.anchorMax = new Vector2(0.5f, 0.5f));
					num += num2;
				}
			}
		}
	}
}
