using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Text), typeof(RectTransform))]
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
					vertex.position.y += curveForText.Evaluate(rectTrans.rect.width * rectTrans.pivot.x + vertex.position.x) * curveMultiplier;
					vh.SetUIVertex(vertex, i);
				}
			}
		}

		protected override void OnRectTransformDimensionsChange()
		{
			if (curveForText != null && rectTrans != null)
			{
				Keyframe key = curveForText[curveForText.length - 1];
				key.time = rectTrans.rect.width;
				curveForText.MoveKey(curveForText.length - 1, key);
			}
		}
	}
}
