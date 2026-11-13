using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/UILineTextureRenderer")]
	public class UILineTextureRenderer : UIPrimitiveBase
	{
		[SerializeField]
		private Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

		[SerializeField]
		private Vector2[] m_points;

		public float LineThickness = 2f;

		public bool UseMargins;

		public Vector2 Margin;

		public bool relativeSize;

		public Rect uvRect
		{
			get
			{
				return m_UVRect;
			}
			set
			{
				if (!(m_UVRect == value))
				{
					m_UVRect = value;
					SetVerticesDirty();
				}
			}
		}

		public Vector2[] Points
		{
			get
			{
				return m_points;
			}
			set
			{
				if (m_points != value)
				{
					m_points = value;
					SetAllDirty();
				}
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (m_points == null || m_points.Length < 2)
			{
				m_points = new Vector2[2]
				{
					new Vector2(0f, 0f),
					new Vector2(1f, 1f)
				};
			}
			int num = 24;
			float num2 = base.rectTransform.rect.width;
			float num3 = base.rectTransform.rect.height;
			Vector2 pivot = base.rectTransform.pivot;
			float num4 = (0f - pivot.x) * base.rectTransform.rect.width;
			Vector2 pivot2 = base.rectTransform.pivot;
			float num5 = (0f - pivot2.y) * base.rectTransform.rect.height;
			if (!relativeSize)
			{
				num2 = 1f;
				num3 = 1f;
			}
			List<Vector2> list = new List<Vector2>();
			list.Add(m_points[0]);
			Vector2 item = m_points[0] + (m_points[1] - m_points[0]).normalized * num;
			list.Add(item);
			for (int i = 1; i < m_points.Length - 1; i++)
			{
				list.Add(m_points[i]);
			}
			item = m_points[m_points.Length - 1] - (m_points[m_points.Length - 1] - m_points[m_points.Length - 2]).normalized * num;
			list.Add(item);
			list.Add(m_points[m_points.Length - 1]);
			Vector2[] array = list.ToArray();
			if (UseMargins)
			{
				num2 -= Margin.x;
				num3 -= Margin.y;
				num4 += Margin.x / 2f;
				num5 += Margin.y / 2f;
			}
			vh.Clear();
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			for (int j = 1; j < array.Length; j++)
			{
				Vector2 vector3 = array[j - 1];
				Vector2 vector4 = array[j];
				vector3 = new Vector2(vector3.x * num2 + num4, vector3.y * num3 + num5);
				vector4 = new Vector2(vector4.x * num2 + num4, vector4.y * num3 + num5);
				float z = Mathf.Atan2(vector4.y - vector3.y, vector4.x - vector3.x) * 180f / (float)Math.PI;
				Vector2 v = vector3 + new Vector2(0f, (0f - LineThickness) / 2f);
				Vector2 v2 = vector3 + new Vector2(0f, LineThickness / 2f);
				Vector2 v3 = vector4 + new Vector2(0f, LineThickness / 2f);
				Vector2 v4 = vector4 + new Vector2(0f, (0f - LineThickness) / 2f);
				v = RotatePointAroundPivot(v, vector3, new Vector3(0f, 0f, z));
				v2 = RotatePointAroundPivot(v2, vector3, new Vector3(0f, 0f, z));
				v3 = RotatePointAroundPivot(v3, vector4, new Vector3(0f, 0f, z));
				v4 = RotatePointAroundPivot(v4, vector4, new Vector3(0f, 0f, z));
				Vector2 zero = Vector2.zero;
				Vector2 vector5 = new Vector2(0f, 1f);
				Vector2 vector6 = new Vector2(0.5f, 0f);
				Vector2 vector7 = new Vector2(0.5f, 1f);
				Vector2 vector8 = new Vector2(1f, 0f);
				Vector2 vector9 = new Vector2(1f, 1f);
				Vector2[] uvs = new Vector2[4]
				{
					vector6,
					vector7,
					vector7,
					vector6
				};
				if (j > 1)
				{
					vh.AddUIVertexQuad(SetVbo(new Vector2[4]
					{
						vector,
						vector2,
						v,
						v2
					}, uvs));
				}
				if (j == 1)
				{
					uvs = new Vector2[4]
					{
						zero,
						vector5,
						vector7,
						vector6
					};
				}
				else if (j == array.Length - 1)
				{
					uvs = new Vector2[4]
					{
						vector6,
						vector7,
						vector9,
						vector8
					};
				}
				vh.AddUIVertexQuad(SetVbo(new Vector2[4]
				{
					v,
					v2,
					v3,
					v4
				}, uvs));
				vector = v3;
				vector2 = v4;
			}
		}

		public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
		{
			Vector3 point2 = point - pivot;
			point2 = Quaternion.Euler(angles) * point2;
			point = point2 + pivot;
			return point;
		}
	}
}
