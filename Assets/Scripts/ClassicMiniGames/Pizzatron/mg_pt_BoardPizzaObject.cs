using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_BoardPizzaObject : MonoBehaviour
	{
		private Sprite m_fullPizza;

		private SpriteRenderer m_renderer;

		protected void Awake()
		{
			m_renderer = base.transform.Find("full").GetComponent<SpriteRenderer>();
			m_fullPizza = m_renderer.sprite;
			m_renderer.sprite = null;
		}

		public void SetPercentage(float p_percentageFill)
		{
			Rect rect = new Rect(m_fullPizza.rect.x, m_fullPizza.rect.y, m_fullPizza.rect.width * p_percentageFill, m_fullPizza.rect.height);
			m_renderer.sprite = Sprite.Create(m_fullPizza.texture, rect, new Vector2(0f, 0.5f));
		}
	}
}
