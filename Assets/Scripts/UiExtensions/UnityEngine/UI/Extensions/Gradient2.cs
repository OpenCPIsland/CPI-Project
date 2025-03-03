using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Effects/Extensions/Gradient2")]
	public class Gradient2 : BaseMeshEffect
	{
		public enum Type
		{
			Horizontal,
			Vertical
		}

		public enum Blend
		{
			Override,
			Add,
			Multiply
		}

		[SerializeField]
		private Type _gradientType;

		[SerializeField]
		private Blend _blendMode = Blend.Multiply;

		[SerializeField]
		[Range(-1f, 1f)]
		private float _offset = 0f;

		[SerializeField]
		private UnityEngine.Gradient _effectGradient = new UnityEngine.Gradient
		{
			colorKeys = new GradientColorKey[2]
			{
				new GradientColorKey(Color.black, 0f),
				new GradientColorKey(Color.white, 1f)
			}
		};

		public Blend BlendMode
		{
			get
			{
				return _blendMode;
			}
			set
			{
				_blendMode = value;
			}
		}

		public UnityEngine.Gradient EffectGradient
		{
			get
			{
				return _effectGradient;
			}
			set
			{
				_effectGradient = value;
			}
		}

		public Type GradientType
		{
			get
			{
				return _gradientType;
			}
			set
			{
				_gradientType = value;
			}
		}

		public float Offset
		{
			get
			{
				return _offset;
			}
			set
			{
				_offset = value;
			}
		}

		public override void ModifyMesh(VertexHelper helper)
		{
			if (!IsActive() || helper.currentVertCount == 0)
			{
				return;
			}
			List<UIVertex> list = new List<UIVertex>();
			helper.GetUIVertexStream(list);
			int count = list.Count;
			switch (GradientType)
			{
			case Type.Horizontal:
			{
				UIVertex uIVertex4 = list[0];
				float num6 = uIVertex4.position.x;
				UIVertex uIVertex5 = list[0];
				float num7 = uIVertex5.position.x;
				float num8 = 0f;
				for (int num9 = count - 1; num9 >= 1; num9--)
				{
					UIVertex uIVertex6 = list[num9];
					num8 = uIVertex6.position.x;
					if (num8 > num7)
					{
						num7 = num8;
					}
					else if (num8 < num6)
					{
						num6 = num8;
					}
				}
				float num10 = 1f / (num7 - num6);
				UIVertex vertex2 = default(UIVertex);
				for (int j = 0; j < helper.currentVertCount; j++)
				{
					helper.PopulateUIVertex(ref vertex2, j);
					vertex2.color = BlendColor(vertex2.color, EffectGradient.Evaluate((vertex2.position.x - num6) * num10 - Offset));
					helper.SetUIVertex(vertex2, j);
				}
				break;
			}
			case Type.Vertical:
			{
				UIVertex uIVertex = list[0];
				float num = uIVertex.position.y;
				UIVertex uIVertex2 = list[0];
				float num2 = uIVertex2.position.y;
				float num3 = 0f;
				for (int num4 = count - 1; num4 >= 1; num4--)
				{
					UIVertex uIVertex3 = list[num4];
					num3 = uIVertex3.position.y;
					if (num3 > num2)
					{
						num2 = num3;
					}
					else if (num3 < num)
					{
						num = num3;
					}
				}
				float num5 = 1f / (num2 - num);
				UIVertex vertex = default(UIVertex);
				for (int i = 0; i < helper.currentVertCount; i++)
				{
					helper.PopulateUIVertex(ref vertex, i);
					vertex.color = BlendColor(vertex.color, EffectGradient.Evaluate((vertex.position.y - num) * num5 - Offset));
					helper.SetUIVertex(vertex, i);
				}
				break;
			}
			}
		}

		private Color BlendColor(Color colorA, Color colorB)
		{
			switch (BlendMode)
			{
			default:
				return colorB;
			case Blend.Add:
				return colorA + colorB;
			case Blend.Multiply:
				return colorA * colorB;
			}
		}
	}
}
