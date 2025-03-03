using System;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("Layout/Extensions/Radial Layout")]
	public class RadialLayout : LayoutGroup
	{
		public float fDistance;

		[Range(0f, 360f)]
		public float MinAngle;

		[Range(0f, 360f)]
		public float MaxAngle;

		[Range(0f, 360f)]
		public float StartAngle;

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
			float num = (MaxAngle - MinAngle) / (float)base.transform.childCount;
			float num2 = StartAngle;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				RectTransform rectTransform = (RectTransform)base.transform.GetChild(i);
				if (rectTransform != null)
				{
					m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.PivotX | DrivenTransformProperties.PivotY);
					Vector3 a = new Vector3(Mathf.Cos(num2 * ((float)Math.PI / 180f)), Mathf.Sin(num2 * ((float)Math.PI / 180f)), 0f);
					rectTransform.localPosition = a * fDistance;
					Vector2 vector2 = rectTransform.pivot = new Vector2(0.5f, 0.5f);
					vector2 = (rectTransform.anchorMin = (rectTransform.anchorMax = vector2));
					num2 += num;
				}
			}
		}
	}
}
