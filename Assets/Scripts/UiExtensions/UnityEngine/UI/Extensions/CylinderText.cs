namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(Text), typeof(RectTransform))]
	[AddComponentMenu("UI/Effects/Extensions/Cylinder Text")]
	public class CylinderText : BaseMeshEffect
	{
		public float radius;

		private RectTransform rectTrans;

		protected override void Awake()
		{
			base.Awake();
			rectTrans = GetComponent<RectTransform>();
			OnRectTransformDimensionsChange();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			rectTrans = GetComponent<RectTransform>();
			OnRectTransformDimensionsChange();
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (!IsActive())
			{
				return;
			}
			int currentVertCount = vh.currentVertCount;
			if (IsActive() && currentVertCount != 0)
			{
				for (int i = 0; i < vh.currentVertCount; i++)
				{
					UIVertex vertex = default(UIVertex);
					vh.PopulateUIVertex(ref vertex, i);
					float x = vertex.position.x;
					vertex.position.z = (0f - radius) * Mathf.Cos(x / radius);
					vertex.position.x = radius * Mathf.Sin(x / radius);
					vh.SetUIVertex(vertex, i);
				}
			}
		}
	}
}
