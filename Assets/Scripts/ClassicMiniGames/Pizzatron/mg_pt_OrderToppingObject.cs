using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_OrderToppingObject : MonoBehaviour
	{
		private SpriteRenderer m_filled;

		private SpriteRenderer m_empty;

		public bool Placed
		{
			set
			{
				m_filled.enabled = value;
				m_empty.enabled = !value;
			}
		}

		protected void Awake()
		{
			SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>();
			SpriteRenderer[] array = componentsInChildren;
			foreach (SpriteRenderer spriteRenderer in array)
			{
				if (spriteRenderer.name == "empty")
				{
					m_empty = spriteRenderer;
				}
				else if (spriteRenderer.name == "filled")
				{
					m_filled = spriteRenderer;
				}
			}
		}

		protected void Start()
		{
			Placed = false;
		}
	}
}
