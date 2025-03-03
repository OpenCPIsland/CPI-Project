using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_JellyBag : mg_bc_Bag
	{
		private SpriteRenderer m_tint;

		public mg_bc_EJellyColors Color
		{
			get;
			private set;
		}

		public void SetColor(mg_bc_EJellyColors _color)
		{
			Color = _color;
			if (m_tint != null)
			{
				m_tint.color = mg_bc_Constants.GetColorForJelly(Color);
			}
		}

		public override void Awake()
		{
			base.Awake();
			m_tint = base.transform.GetChild(0).Find("tint").gameObject.GetComponent<SpriteRenderer>();
		}
	}
}
