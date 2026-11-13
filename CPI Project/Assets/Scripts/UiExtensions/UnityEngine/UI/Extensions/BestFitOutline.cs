using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Effects/Extensions/BestFit Outline")]
	public class BestFitOutline : Shadow
	{
		protected BestFitOutline()
		{
		}

		public override void ModifyMesh(Mesh mesh)
		{
			if (IsActive())
			{
				List<UIVertex> list = new List<UIVertex>();
				using (VertexHelper vertexHelper = new VertexHelper(mesh))
				{
					vertexHelper.GetUIVertexStream(list);
				}
				Text component = GetComponent<Text>();
				float num = 1f;
				if ((bool)component && component.resizeTextForBestFit)
				{
					num = (float)component.cachedTextGenerator.fontSizeUsedForBestFit / (float)(component.resizeTextMaxSize - 1);
				}
				int num2 = 0;
				int count = list.Count;
				Color32 color = base.effectColor;
				int start = num2;
				int count2 = list.Count;
				Vector2 effectDistance = base.effectDistance;
				float x = effectDistance.x * num;
				Vector2 effectDistance2 = base.effectDistance;
				ApplyShadowZeroAlloc(list, color, start, count2, x, effectDistance2.y * num);
				num2 = count;
				count = list.Count;
				Color32 color2 = base.effectColor;
				int start2 = num2;
				int count3 = list.Count;
				Vector2 effectDistance3 = base.effectDistance;
				float x2 = effectDistance3.x * num;
				Vector2 effectDistance4 = base.effectDistance;
				ApplyShadowZeroAlloc(list, color2, start2, count3, x2, (0f - effectDistance4.y) * num);
				num2 = count;
				count = list.Count;
				Color32 color3 = base.effectColor;
				int start3 = num2;
				int count4 = list.Count;
				Vector2 effectDistance5 = base.effectDistance;
				float x3 = (0f - effectDistance5.x) * num;
				Vector2 effectDistance6 = base.effectDistance;
				ApplyShadowZeroAlloc(list, color3, start3, count4, x3, effectDistance6.y * num);
				num2 = count;
				count = list.Count;
				Color32 color4 = base.effectColor;
				int start4 = num2;
				int count5 = list.Count;
				Vector2 effectDistance7 = base.effectDistance;
				float x4 = (0f - effectDistance7.x) * num;
				Vector2 effectDistance8 = base.effectDistance;
				ApplyShadowZeroAlloc(list, color4, start4, count5, x4, (0f - effectDistance8.y) * num);
				using (VertexHelper vertexHelper2 = new VertexHelper())
				{
					vertexHelper2.AddUIVertexTriangleStream(list);
					vertexHelper2.FillMesh(mesh);
				}
			}
		}
	}
}
