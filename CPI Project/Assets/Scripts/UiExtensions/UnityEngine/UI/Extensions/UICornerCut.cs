namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/Cut Corners")]
	public class UICornerCut : UIPrimitiveBase
	{
		public Vector2 cornerSize = new Vector2(16f, 16f);

		[Header("Corners to cut")]
		public bool cutUL = true;

		public bool cutUR;

		public bool cutLL;

		public bool cutLR;

		[Tooltip("Up-Down colors become Left-Right colors")]
		public bool makeColumns = false;

		[Header("Color the cut bars differently")]
		public bool useColorUp;

		public Color32 colorUp = Color.blue;

		public bool useColorDown;

		public Color32 colorDown = Color.green;

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			Rect rect = base.rectTransform.rect;
			Rect rect2 = rect;
			Color32 color = this.color;
			bool flag = cutUL | cutUR;
			bool flag2 = cutLL | cutLR;
			bool flag3 = cutLL | cutUL;
			bool flag4 = cutLR | cutUR;
			if (!(flag || flag2) || !(cornerSize.sqrMagnitude > 0f))
			{
				return;
			}
			vh.Clear();
			if (flag3)
			{
				rect2.xMin += cornerSize.x;
			}
			if (flag2)
			{
				rect2.yMin += cornerSize.y;
			}
			if (flag)
			{
				rect2.yMax -= cornerSize.y;
			}
			if (flag4)
			{
				rect2.xMax -= cornerSize.x;
			}
			Vector2 vector;
			Vector2 vector2;
			Vector2 vector3;
			Vector2 vector4;
			if (makeColumns)
			{
				vector = new Vector2(rect.xMin, (!cutUL) ? rect.yMax : rect2.yMax);
				vector2 = new Vector2(rect.xMax, (!cutUR) ? rect.yMax : rect2.yMax);
				vector3 = new Vector2(rect.xMin, (!cutLL) ? rect.yMin : rect2.yMin);
				vector4 = new Vector2(rect.xMax, (!cutLR) ? rect.yMin : rect2.yMin);
				if (flag3)
				{
					AddSquare(vector3, vector, new Vector2(rect2.xMin, rect.yMax), new Vector2(rect2.xMin, rect.yMin), rect, (!useColorUp) ? color : colorUp, vh);
				}
				if (flag4)
				{
					AddSquare(vector2, vector4, new Vector2(rect2.xMax, rect.yMin), new Vector2(rect2.xMax, rect.yMax), rect, (!useColorDown) ? color : colorDown, vh);
				}
			}
			else
			{
				vector = new Vector2((!cutUL) ? rect.xMin : rect2.xMin, rect.yMax);
				vector2 = new Vector2((!cutUR) ? rect.xMax : rect2.xMax, rect.yMax);
				vector3 = new Vector2((!cutLL) ? rect.xMin : rect2.xMin, rect.yMin);
				vector4 = new Vector2((!cutLR) ? rect.xMax : rect2.xMax, rect.yMin);
				if (flag2)
				{
					AddSquare(vector4, vector3, new Vector2(rect.xMin, rect2.yMin), new Vector2(rect.xMax, rect2.yMin), rect, (!useColorDown) ? color : colorDown, vh);
				}
				if (flag)
				{
					AddSquare(vector, vector2, new Vector2(rect.xMax, rect2.yMax), new Vector2(rect.xMin, rect2.yMax), rect, (!useColorUp) ? color : colorUp, vh);
				}
			}
			if (makeColumns)
			{
				AddSquare(new Rect(rect2.xMin, rect.yMin, rect2.width, rect.height), rect, color, vh);
			}
			else
			{
				AddSquare(new Rect(rect.xMin, rect2.yMin, rect.width, rect2.height), rect, color, vh);
			}
		}

		private static void AddSquare(Rect rect, Rect rectUV, Color32 color32, VertexHelper vh)
		{
			int num = AddVert(rect.xMin, rect.yMin, rectUV, color32, vh);
			int idx = AddVert(rect.xMin, rect.yMax, rectUV, color32, vh);
			int num2 = AddVert(rect.xMax, rect.yMax, rectUV, color32, vh);
			int idx2 = AddVert(rect.xMax, rect.yMin, rectUV, color32, vh);
			vh.AddTriangle(num, idx, num2);
			vh.AddTriangle(num2, idx2, num);
		}

		private static void AddSquare(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Rect rectUV, Color32 color32, VertexHelper vh)
		{
			int num = AddVert(a.x, a.y, rectUV, color32, vh);
			int idx = AddVert(b.x, b.y, rectUV, color32, vh);
			int num2 = AddVert(c.x, c.y, rectUV, color32, vh);
			int idx2 = AddVert(d.x, d.y, rectUV, color32, vh);
			vh.AddTriangle(num, idx, num2);
			vh.AddTriangle(num2, idx2, num);
		}

		private static int AddVert(float x, float y, Rect area, Color32 color32, VertexHelper vh)
		{
			vh.AddVert(uv0: new Vector2(Mathf.InverseLerp(area.xMin, area.xMax, x), Mathf.InverseLerp(area.yMin, area.yMax, y)), position: new Vector3(x, y), color: color32);
			return vh.currentVertCount - 1;
		}
	}
}
