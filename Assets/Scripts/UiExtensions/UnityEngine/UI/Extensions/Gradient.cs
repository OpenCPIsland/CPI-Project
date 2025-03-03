using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Effects/Extensions/Gradient")]
	public class Gradient : BaseMeshEffect
	{
		public GradientMode gradientMode = GradientMode.Global;

		public GradientDir gradientDir = GradientDir.Vertical;

		public bool overwriteAllColor = false;

		public Color vertex1 = Color.white;

		public Color vertex2 = Color.black;

		private Graphic targetGraphic;

		protected override void Start()
		{
			targetGraphic = GetComponent<Graphic>();
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			int currentVertCount = vh.currentVertCount;
			if (!IsActive() || currentVertCount == 0)
			{
				return;
			}
			List<UIVertex> list = new List<UIVertex>();
			vh.GetUIVertexStream(list);
			UIVertex vertex = default(UIVertex);
			if (gradientMode == GradientMode.Global)
			{
				if (gradientDir == GradientDir.DiagonalLeftToRight || gradientDir == GradientDir.DiagonalRightToLeft)
				{
					gradientDir = GradientDir.Vertical;
				}
				float num;
				if (gradientDir == GradientDir.Vertical)
				{
					UIVertex uIVertex = list[list.Count - 1];
					num = uIVertex.position.y;
				}
				else
				{
					UIVertex uIVertex2 = list[list.Count - 1];
					num = uIVertex2.position.x;
				}
				float num2 = num;
				float num3;
				if (gradientDir == GradientDir.Vertical)
				{
					UIVertex uIVertex3 = list[0];
					num3 = uIVertex3.position.y;
				}
				else
				{
					UIVertex uIVertex4 = list[0];
					num3 = uIVertex4.position.x;
				}
				float num4 = num3;
				float num5 = num4 - num2;
				for (int i = 0; i < currentVertCount; i++)
				{
					vh.PopulateUIVertex(ref vertex, i);
					if (overwriteAllColor || !(vertex.color != targetGraphic.color))
					{
						vertex.color *= Color.Lerp(vertex2, vertex1, (((gradientDir != 0) ? vertex.position.x : vertex.position.y) - num2) / num5);
						vh.SetUIVertex(vertex, i);
					}
				}
				return;
			}
			for (int j = 0; j < currentVertCount; j++)
			{
				vh.PopulateUIVertex(ref vertex, j);
				if (overwriteAllColor || CompareCarefully(vertex.color, targetGraphic.color))
				{
					switch (gradientDir)
					{
					case GradientDir.Vertical:
						vertex.color *= ((j % 4 != 0 && (j - 1) % 4 != 0) ? vertex2 : vertex1);
						break;
					case GradientDir.Horizontal:
						vertex.color *= ((j % 4 != 0 && (j - 3) % 4 != 0) ? vertex2 : vertex1);
						break;
					case GradientDir.DiagonalLeftToRight:
						vertex.color *= ((j % 4 == 0) ? vertex1 : (((j - 2) % 4 != 0) ? Color.Lerp(vertex2, vertex1, 0.5f) : vertex2));
						break;
					case GradientDir.DiagonalRightToLeft:
						vertex.color *= (((j - 1) % 4 == 0) ? vertex1 : (((j - 3) % 4 != 0) ? Color.Lerp(vertex2, vertex1, 0.5f) : vertex2));
						break;
					}
					vh.SetUIVertex(vertex, j);
				}
			}
		}

		private bool CompareCarefully(Color col1, Color col2)
		{
			if (Mathf.Abs(col1.r - col2.r) < 0.003f && Mathf.Abs(col1.g - col2.g) < 0.003f && Mathf.Abs(col1.b - col2.b) < 0.003f && Mathf.Abs(col1.a - col2.a) < 0.003f)
			{
				return true;
			}
			return false;
		}
	}
}
