using System.Linq;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_PizzaObject : MonoBehaviour
	{
		private const float SPLOTCH_RADIUS_PERCENT = 0.2f;

		private const int TOTAL_POINTS = 21;

		private const float SAUCE_POINT_SPACING = 0.1f;

		private const float SAUCE_POINT_HALF_SIZE = 0.4f;

		private int m_halfPoints;

		private float m_gameXSpacing;

		private float m_gameYSpacing;

		private float m_xRadius;

		private float m_yRadius;

		private static string GO_CHEESE = "Cheese";

		private static string GO_BASE = "Base";

		private GameObject m_cheese;

		private SpriteRenderer m_baseRenderer;

		private MeshFilter m_meshFilter;

		private MeshRenderer m_meshRenderer;

		private mg_pt_PizzaPoint[,] m_pizzaPoints;

		private int m_totalPizzaPoints;

		private int m_saucePointsAdded;

		private bool m_sauceCompleted;

		protected void Awake()
		{
			m_cheese = base.transform.Find(GO_CHEESE).gameObject;
			m_baseRenderer = base.transform.Find(GO_BASE).GetComponent<SpriteRenderer>();
			m_meshFilter = base.transform.GetComponentInChildren<MeshFilter>();
			m_meshRenderer = base.transform.GetComponentInChildren<MeshRenderer>();
			m_meshRenderer.sortingOrder = -79;
			m_halfPoints = 10;
		}

		protected void Start()
		{
			m_cheese.SetActive(false);
			m_meshFilter.mesh = new Mesh();
			m_meshFilter.mesh.vertices = new Vector3[0];
			m_meshFilter.mesh.triangles = new int[0];
			Texture2D texture2D = (Texture2D)m_meshRenderer.material.GetTexture("_MainTex");
			float num = (float)texture2D.width * 0.5f / 10f;
			float num2 = (float)texture2D.height * 0.5f / 10f;
			m_gameXSpacing = m_baseRenderer.sprite.bounds.size.x * 0.5f / 10f;
			m_gameYSpacing = m_baseRenderer.sprite.bounds.size.y * 0.5f / 10f;
			m_xRadius = m_baseRenderer.sprite.bounds.size.x * 0.2f;
			m_yRadius = m_baseRenderer.sprite.bounds.size.y * 0.2f;
			m_pizzaPoints = new mg_pt_PizzaPoint[21, 21];
			for (int i = 0; i < 21; i++)
			{
				for (int j = 0; j < 21; j++)
				{
					float a = texture2D.GetPixel(Mathf.RoundToInt((float)i * num), Mathf.RoundToInt((float)j * num2)).a;
					m_pizzaPoints[i, j] = new mg_pt_PizzaPoint
					{
						Transparent = (texture2D.GetPixel(Mathf.RoundToInt((float)i * num), Mathf.RoundToInt((float)j * num2)).a < 0.1f),
						SauceAdded = false
					};
					if (!m_pizzaPoints[i, j].Transparent)
					{
						m_totalPizzaPoints++;
					}
				}
			}
		}

		protected void Update()
		{
			m_meshRenderer.material.SetMatrix("_ScaleTransform", Matrix4x4.TRS(base.transform.position, Quaternion.identity, Vector3.one) * Matrix4x4.Scale(m_baseRenderer.bounds.size));
		}

		public bool ContainsPoint(Vector2 p_globalPoint)
		{
			Vector2 p_localPoint = base.transform.InverseTransformPoint(p_globalPoint);
			mg_pt_PizzaPoint mg_pt_PizzaPoint = CalculatePizzaPoint(p_localPoint);
			bool result = false;
			if (mg_pt_PizzaPoint != null && !mg_pt_PizzaPoint.Transparent)
			{
				result = true;
			}
			return result;
		}

		private bool WillAddSauce(mg_pt_Topping p_sauce)
		{
			bool result = false;
			Vector2 p_localPoint = base.transform.InverseTransformPoint(p_sauce.SaucePosition);
			mg_pt_PizzaPoint mg_pt_PizzaPoint = null;
			float num = Mathf.Pow(m_xRadius * base.transform.lossyScale.x, 2f);
			float num2 = Mathf.Pow(m_yRadius * base.transform.lossyScale.y, 2f);
			float num3 = 0f - m_xRadius + p_localPoint.x;
			float num4 = 0f - m_yRadius + p_localPoint.y;
			float num5 = num4;
			Vector2 vector = CalculatePizzaPointIndex(p_localPoint);
			vector.y = Mathf.Abs(vector.y);
			while (num3 <= m_xRadius + p_localPoint.x)
			{
				float num6 = num3 * base.transform.lossyScale.x;
				float num7 = Mathf.Pow(num6 - p_localPoint.x, 2f);
				for (; num4 <= p_localPoint.y; num4 += m_gameYSpacing)
				{
					float num8 = num4 * base.transform.lossyScale.y;
					float num9 = Mathf.Pow(num8 - p_localPoint.y, 2f);
					float num10 = num7 / num + num9 / num2;
					if (!(num10 <= 1f))
					{
						continue;
					}
					Vector2 vector2 = CalculatePizzaPointIndex(new Vector2(num6, num8));
					int num11 = (int)(vector.y - vector2.y);
					for (int i = (int)vector2.y; (float)i <= vector.y + (float)num11; i++)
					{
						mg_pt_PizzaPoint = GetPizzaPoint(new Vector2(vector2.x, i));
						if (mg_pt_PizzaPoint != null && !mg_pt_PizzaPoint.Transparent && !mg_pt_PizzaPoint.SauceAdded)
						{
							result = true;
							mg_pt_PizzaPoint.SauceAdded = true;
							m_saucePointsAdded++;
						}
					}
					break;
				}
				num3 += m_gameXSpacing;
				num4 = num5;
			}
			return result;
		}

		public void AddSauceAt(mg_pt_Pizza p_pizza, mg_pt_Topping p_sauce)
		{
			if (!m_sauceCompleted && WillAddSauce(p_sauce))
			{
				Vector2 v = CalculateSauceRendererPoint(p_sauce.SaucePosition);
				int num = m_meshFilter.mesh.vertices.Length;
				int num2 = 41;
				float num3 = 360f / (float)(num2 - 1);
				Vector3[] array = new Vector3[num2];
				int[] array2 = new int[num2 * 3];
				array[0] = v;
				for (int i = 1; i < num2; i++)
				{
					Vector3 vector = Quaternion.AngleAxis(num3 * (float)(i - 1), Vector3.back) * Vector3.up;
					vector.x *= 0.2f;
					vector.y *= 0.2f;
					vector.x += v.x;
					vector.y += v.y;
					array[i] = vector;
				}
				for (int i = 0; i + 2 < num2; i++)
				{
					int num4 = i * 3;
					array2[num4] = num;
					array2[num4 + 1] = num + i + 1;
					array2[num4 + 2] = num + i + 2;
				}
				int num5 = array2.Length - 3;
				array2[num5] = num;
				array2[num5 + 1] = num + num2 - 1;
				array2[num5 + 2] = num + 1;
				m_meshFilter.mesh.vertices = m_meshFilter.mesh.vertices.Concat(array).ToArray();
				m_meshFilter.mesh.triangles = m_meshFilter.mesh.triangles.Concat(array2).ToArray();
				if ((float)m_totalPizzaPoints * 0.85f <= (float)m_saucePointsAdded)
				{
					CompleteSauce(p_pizza, p_sauce);
				}
				m_meshFilter.mesh.bounds = new Bounds(Vector3.zero, new Vector3(500f, 500f, 500f));
			}
		}

		private void CompleteSauce(mg_pt_Pizza p_pizza, mg_pt_Topping p_sauce)
		{
			p_pizza.SauceAdded(p_sauce);
			ClearSauce(m_meshRenderer.material.GetColor("_Color"));
			Vector2 vector = new Vector2(0f, 0f);
			m_meshFilter.mesh.vertices = new Vector3[5]
			{
				vector,
				vector + new Vector2(-0.5f, 0.5f),
				vector + new Vector2(0.5f, 0.5f),
				vector + new Vector2(0.5f, -0.5f),
				vector + new Vector2(-0.5f, -0.5f)
			}.ToArray();
			m_meshFilter.mesh.triangles = new int[12]
			{
				0,
				1,
				2,
				0,
				2,
				3,
				0,
				3,
				4,
				0,
				4,
				1
			}.ToArray();
			m_sauceCompleted = true;
		}

		private Vector2 CalculateSauceRendererPoint(Vector2 p_globalPoint)
		{
			Vector2 result = base.transform.InverseTransformPoint(p_globalPoint);
			result.x /= m_baseRenderer.bounds.size.x;
			result.y /= m_baseRenderer.bounds.size.y;
			return result;
		}

		private mg_pt_PizzaPoint CalculatePizzaPoint(Vector2 p_localPoint)
		{
			Vector2 p_index = CalculatePizzaPointIndex(p_localPoint);
			return GetPizzaPoint(p_index);
		}

		private Vector2 CalculatePizzaPointIndex(Vector2 p_localPoint)
		{
			int num = Mathf.RoundToInt(p_localPoint.x / m_gameXSpacing) + m_halfPoints;
			int num2 = Mathf.RoundToInt(p_localPoint.y / m_gameYSpacing) + m_halfPoints;
			return new Vector2(num, num2);
		}

		private mg_pt_PizzaPoint GetPizzaPoint(Vector2 p_index)
		{
			mg_pt_PizzaPoint result = null;
			if (p_index.x >= 0f && p_index.x < 21f && p_index.y >= 0f && p_index.y < 21f)
			{
				result = m_pizzaPoints[(int)p_index.x, (int)p_index.y];
			}
			return result;
		}

		public void ClearSauce(Color p_sauceColor)
		{
			m_meshFilter.mesh.Clear();
			m_meshRenderer.material.SetColor("_Color", p_sauceColor);
			m_sauceCompleted = false;
			m_saucePointsAdded = 0;
			for (int i = 0; i < m_pizzaPoints.GetLength(0); i++)
			{
				for (int j = 0; j < m_pizzaPoints.GetLength(1); j++)
				{
					m_pizzaPoints[i, j].SauceAdded = false;
				}
			}
		}

		public void ShowCheese()
		{
			m_cheese.SetActive(true);
		}
	}
}
