using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Image))]
	public class CurvedImage : BaseMeshEffect
	{
		public enum CurvedImageConstraint
		{
			None,
			Bottom,
			Top
		}

		public AnimationCurve imageCurve = AnimationCurve.Linear(0f, 0f, 1f, 10f);

		public float curveMultiplier = 1f;

		[Range(1f, 50f)]
		public int NumVertices = 1;

		public CurvedImageConstraint Constraint = CurvedImageConstraint.None;

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
			if (!IsActive() || currentVertCount == 0)
			{
				return;
			}
			if (GetComponent<Image>().type == Image.Type.Simple)
			{
				addVertices(vh);
			}
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				UIVertex vertex = default(UIVertex);
				vh.PopulateUIVertex(ref vertex, i);
				if ((Constraint != CurvedImageConstraint.Bottom || !(vertex.position.y < 0f)) && (Constraint != CurvedImageConstraint.Top || !(vertex.position.y > 0f)))
				{
					vertex.position.y += imageCurve.Evaluate(rectTrans.rect.width * rectTrans.pivot.x + vertex.position.x) * curveMultiplier;
					vh.SetUIVertex(vertex, i);
				}
			}
		}

		private void addVertices(VertexHelper vh)
		{
			List<UIVertex> list = new List<UIVertex>();
			vh.GetUIVertexStream(list);
			UIVertex bottomLeft = list[0];
			UIVertex topLeft = list[1];
			UIVertex topRight = list[2];
			UIVertex bottomRight = list[4];
			List<UIVertex> verticesForRect = getVerticesForRect(NumVertices, bottomLeft, topLeft, topRight, bottomRight);
			List<int> indicesForRect = getIndicesForRect(NumVertices);
			vh.Clear();
			vh.AddUIVertexStream(verticesForRect, indicesForRect);
		}

		private List<UIVertex> getVerticesForRect(int numVertices, UIVertex bottomLeft, UIVertex topLeft, UIVertex topRight, UIVertex bottomRight)
		{
			List<UIVertex> list = new List<UIVertex>();
			list.Add(bottomLeft);
			list.Add(topLeft);
			int i;
			for (i = 0; i < numVertices; i++)
			{
				if (i % 2 == 0)
				{
					list.Add(getVertexForPercent((float)(i + 1) / (float)(numVertices + 1), topLeft, topRight));
					list.Add(getVertexForPercent((float)(i + 1) / (float)(numVertices + 1), bottomLeft, bottomRight));
				}
				else
				{
					list.Add(getVertexForPercent((float)(i + 1) / (float)(numVertices + 1), bottomLeft, bottomRight));
					list.Add(getVertexForPercent((float)(i + 1) / (float)(numVertices + 1), topLeft, topRight));
				}
			}
			if (i % 2 == 1)
			{
				list.Add(bottomRight);
				list.Add(topRight);
			}
			else
			{
				list.Add(topRight);
				list.Add(bottomRight);
			}
			return list;
		}

		private List<int> getIndicesForRect(int numVertices)
		{
			List<int> list = new List<int>();
			int num = 0;
			for (int i = 0; i < numVertices + 1; i++)
			{
				if (i % 2 == 0)
				{
					list.Add(num);
					list.Add(num + 1);
					list.Add(num + 2);
					list.Add(num);
					list.Add(num + 2);
					list.Add(num + 3);
				}
				else
				{
					list.Add(num + 1);
					list.Add(num);
					list.Add(num + 3);
					list.Add(num + 1);
					list.Add(num + 3);
					list.Add(num + 2);
				}
				num += 2;
			}
			return list;
		}

		private UIVertex getVertexForPercent(float percent, UIVertex left, UIVertex right)
		{
			UIVertex result = left;
			result.position = new Vector3(left.position.x + (right.position.x - left.position.x) * percent, left.position.y, left.position.z);
			result.uv0 = new Vector2(left.uv0.x + (right.uv0.x - left.uv0.x) * percent, left.uv0.y);
			result.uv1 = new Vector2(left.uv1.x + (right.uv1.x - left.uv1.x) * percent, left.uv1.y);
			return result;
		}

		protected override void OnRectTransformDimensionsChange()
		{
			if (imageCurve != null && rectTrans != null && rectTrans.rect.width > float.Epsilon)
			{
				if (Mathf.Abs(imageCurve[imageCurve.length - 1].time - rectTrans.rect.width) > float.Epsilon)
				{
					Keyframe key = imageCurve[imageCurve.length - 1];
					key.time = rectTrans.rect.width;
					imageCurve.MoveKey(imageCurve.length - 1, key);
				}
				if (Mathf.Abs(imageCurve[0].time) > float.Epsilon)
				{
					Keyframe key = imageCurve[0];
					key.time = 0f;
					imageCurve.MoveKey(0, key);
				}
			}
		}
	}
}
