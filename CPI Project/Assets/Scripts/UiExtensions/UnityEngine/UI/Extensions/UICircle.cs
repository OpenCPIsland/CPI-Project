using System;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/UI Circle")]
	public class UICircle : UIPrimitiveBase
	{
		[Tooltip("The circular fill percentage of the primitive, affected by FixedToSegments")]
		[Range(0f, 100f)]
		public int fillPercent = 100;

		[Tooltip("Should the primitive fill draw by segments or absolute percentage")]
		public bool FixedToSegments = false;

		[Tooltip("Draw the primitive filled or as a line")]
		public bool fill = true;

		[Tooltip("If not filled, the thickness of the primitive line")]
		public float thickness = 5f;

		[Tooltip("The number of segments to draw the primitive, more segments = smoother primitive")]
		[Range(0f, 360f)]
		public int segments = 360;

		private void Update()
		{
			thickness = Mathf.Clamp(thickness, 0f, base.rectTransform.rect.width / 2f);
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			Vector2 pivot = base.rectTransform.pivot;
			float outer = (0f - pivot.x) * base.rectTransform.rect.width;
			Vector2 pivot2 = base.rectTransform.pivot;
			float inner = (0f - pivot2.x) * base.rectTransform.rect.width + thickness;
			vh.Clear();
			Vector2 prevX = Vector2.zero;
			Vector2 prevY = Vector2.zero;
			Vector2 vector = new Vector2(0f, 0f);
			Vector2 vector2 = new Vector2(0f, 1f);
			Vector2 vector3 = new Vector2(1f, 1f);
			Vector2 vector4 = new Vector2(1f, 0f);
			Vector2 pos;
			Vector2 pos2;
			Vector2 pos3;
			Vector2 pos4;
			if (FixedToSegments)
			{
				float num = (float)fillPercent / 100f;
				float num2 = 360f / (float)segments;
				int num3 = (int)((float)(segments + 1) * num);
				for (int i = 0; i < num3; i++)
				{
					float f = (float)Math.PI / 180f * ((float)i * num2);
					float c = Mathf.Cos(f);
					float s = Mathf.Sin(f);
					vector = new Vector2(0f, 1f);
					vector2 = new Vector2(1f, 1f);
					vector3 = new Vector2(1f, 0f);
					vector4 = new Vector2(0f, 0f);
					StepThroughPointsAndFill(outer, inner, ref prevX, ref prevY, out pos, out pos2, out pos3, out pos4, c, s);
					vh.AddUIVertexQuad(SetVbo(new Vector2[4]
					{
						pos,
						pos2,
						pos3,
						pos4
					}, new Vector2[4]
					{
						vector,
						vector2,
						vector3,
						vector4
					}));
				}
				return;
			}
			float width = base.rectTransform.rect.width;
			float height = base.rectTransform.rect.height;
			float num4 = (float)fillPercent / 100f * ((float)Math.PI * 2f) / (float)segments;
			float num5 = 0f;
			for (int j = 0; j < segments + 1; j++)
			{
				float c2 = Mathf.Cos(num5);
				float s2 = Mathf.Sin(num5);
				StepThroughPointsAndFill(outer, inner, ref prevX, ref prevY, out pos, out pos2, out pos3, out pos4, c2, s2);
				vector = new Vector2(pos.x / width + 0.5f, pos.y / height + 0.5f);
				vector2 = new Vector2(pos2.x / width + 0.5f, pos2.y / height + 0.5f);
				vector3 = new Vector2(pos3.x / width + 0.5f, pos3.y / height + 0.5f);
				vector4 = new Vector2(pos4.x / width + 0.5f, pos4.y / height + 0.5f);
				vh.AddUIVertexQuad(SetVbo(new Vector2[4]
				{
					pos,
					pos2,
					pos3,
					pos4
				}, new Vector2[4]
				{
					vector,
					vector2,
					vector3,
					vector4
				}));
				num5 += num4;
			}
		}

		private void StepThroughPointsAndFill(float outer, float inner, ref Vector2 prevX, ref Vector2 prevY, out Vector2 pos0, out Vector2 pos1, out Vector2 pos2, out Vector2 pos3, float c, float s)
		{
			pos0 = prevX;
			pos1 = new Vector2(outer * c, outer * s);
			if (fill)
			{
				pos2 = Vector2.zero;
				pos3 = Vector2.zero;
			}
			else
			{
				pos2 = new Vector2(inner * c, inner * s);
				pos3 = prevY;
			}
			prevX = pos1;
			prevY = pos2;
		}
	}
}
