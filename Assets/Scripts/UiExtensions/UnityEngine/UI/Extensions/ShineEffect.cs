namespace UnityEngine.UI.Extensions
{
	public class ShineEffect : MaskableGraphic
	{
		[SerializeField]
		private float yoffset = -1f;

		[SerializeField]
		private float width = 1f;

		public float Yoffset
		{
			get
			{
				return yoffset;
			}
			set
			{
				SetVerticesDirty();
				yoffset = value;
			}
		}

		public float Width
		{
			get
			{
				return width;
			}
			set
			{
				SetAllDirty();
				width = value;
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			Rect pixelAdjustedRect = GetPixelAdjustedRect();
			Vector4 vector = new Vector4(pixelAdjustedRect.x, pixelAdjustedRect.y, pixelAdjustedRect.x + pixelAdjustedRect.width, pixelAdjustedRect.y + pixelAdjustedRect.height);
			float num = (vector.w - vector.y) * 2f;
			Color32 color = this.color;
			vh.Clear();
			color.a = 0;
			vh.AddVert(new Vector3(vector.x - 50f, width * vector.y + yoffset * num), color, new Vector2(0f, 0f));
			vh.AddVert(new Vector3(vector.z + 50f, width * vector.y + yoffset * num), color, new Vector2(1f, 0f));
			Color color2 = this.color;
			color.a = (byte)(color2.a * 255f);
			vh.AddVert(new Vector3(vector.x - 50f, width * (vector.y / 4f) + yoffset * num), color, new Vector2(0f, 1f));
			vh.AddVert(new Vector3(vector.z + 50f, width * (vector.y / 4f) + yoffset * num), color, new Vector2(1f, 1f));
			Color color3 = this.color;
			color.a = (byte)(color3.a * 255f);
			vh.AddVert(new Vector3(vector.x - 50f, width * (vector.w / 4f) + yoffset * num), color, new Vector2(0f, 1f));
			vh.AddVert(new Vector3(vector.z + 50f, width * (vector.w / 4f) + yoffset * num), color, new Vector2(1f, 1f));
			Color color4 = this.color;
			color.a = (byte)(color4.a * 255f);
			color.a = 0;
			vh.AddVert(new Vector3(vector.x - 50f, width * vector.w + yoffset * num), color, new Vector2(0f, 1f));
			vh.AddVert(new Vector3(vector.z + 50f, width * vector.w + yoffset * num), color, new Vector2(1f, 1f));
			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 1);
			vh.AddTriangle(2, 3, 4);
			vh.AddTriangle(4, 5, 3);
			vh.AddTriangle(4, 5, 6);
			vh.AddTriangle(6, 7, 5);
		}

		public void Triangulate(VertexHelper vh)
		{
			int num = vh.currentVertCount - 2;
			Debug.Log(num);
			for (int i = 0; i <= num / 2 + 1; i += 2)
			{
				vh.AddTriangle(i, i + 1, i + 2);
				vh.AddTriangle(i + 2, i + 3, i + 1);
			}
		}
	}
}
