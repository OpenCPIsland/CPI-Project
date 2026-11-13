namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(Text), typeof(RectTransform))]
	[AddComponentMenu("UI/Effects/Extensions/Curved Text")]
	public class CurvedText : BaseMeshEffect
	{
		public AnimationCurve curveForText = AnimationCurve.Linear(0f, 0f, 1f, 10f);

		public float curveMultiplier = 1f;

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
			int currentVertCount = vh.currentVertCount;
			if (IsActive() && currentVertCount != 0)
			{
				for (int i = 0; i < vh.currentVertCount; i++)
				{
					UIVertex vertex = default(UIVertex);
					vh.PopulateUIVertex(ref vertex, i);
					 Vector3 position =  vertex.position;
					float y = position.y;
					AnimationCurve animationCurve = curveForText;
					float width = rectTrans.rect.width;
					Vector2 pivot = rectTrans.pivot;
					position.y = y + animationCurve.Evaluate(width * pivot.x + vertex.position.x) * curveMultiplier;
					vh.SetUIVertex(vertex, i);
				}
			}
		}

		protected override void OnRectTransformDimensionsChange()
		{
			Keyframe key = curveForText[curveForText.length - 1];
			key.time = rectTrans.rect.width;
			curveForText.MoveKey(curveForText.length - 1, key);
		}
	}
}
