namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform), typeof(Graphic))]
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/Effects/Extensions/Flippable")]
	public class UIFlippable : MonoBehaviour, IMeshModifier
	{
		[SerializeField]
		private bool m_Horizontal = false;

		[SerializeField]
		private bool m_Veritical = false;

		public bool horizontal
		{
			get
			{
				return m_Horizontal;
			}
			set
			{
				m_Horizontal = value;
			}
		}

		public bool vertical
		{
			get
			{
				return m_Veritical;
			}
			set
			{
				m_Veritical = value;
			}
		}

		protected void OnValidate()
		{
			GetComponent<Graphic>().SetVerticesDirty();
		}

		public void ModifyMesh(VertexHelper verts)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			for (int i = 0; i < verts.currentVertCount; i++)
			{
				UIVertex vertex = default(UIVertex);
				verts.PopulateUIVertex(ref vertex, i);
				float x2;
				if (m_Horizontal)
				{
					float x = vertex.position.x;
					Vector2 center = rectTransform.rect.center;
					x2 = x + (center.x - vertex.position.x) * 2f;
				}
				else
				{
					x2 = vertex.position.x;
				}
				float y2;
				if (m_Veritical)
				{
					float y = vertex.position.y;
					Vector2 center2 = rectTransform.rect.center;
					y2 = y + (center2.y - vertex.position.y) * 2f;
				}
				else
				{
					y2 = vertex.position.y;
				}
				vertex.position = new Vector3(x2, y2, vertex.position.z);
				verts.SetUIVertex(vertex, i);
			}
		}

		public void ModifyMesh(Mesh mesh)
		{
		}
	}
}
