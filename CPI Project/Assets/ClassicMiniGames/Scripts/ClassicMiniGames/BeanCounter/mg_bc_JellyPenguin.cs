using System;
using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_JellyPenguin : mg_bc_Penguin
	{
		private SpriteRenderer[] m_tintSprites;

		public override void Awake()
		{
			base.Awake();
			m_tintSprites = new SpriteRenderer[5];
			SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>();
			SpriteRenderer[] array = componentsInChildren;
			foreach (SpriteRenderer spriteRenderer in array)
			{
				string name = spriteRenderer.gameObject.name;
				if (name.StartsWith("bag_tint_"))
				{
					int num = Convert.ToInt32(name.Substring(name.Length - 1));
					m_tintSprites[num - 1] = spriteRenderer;
				}
			}
		}

		protected override void OnCaughtBag(mg_bc_Bag _bag)
		{
			mg_bc_JellyBag component = _bag.GetComponent<mg_bc_JellyBag>();
			if (m_heldBagsStack.Count < 5)
			{
				m_tintSprites[m_heldBagsStack.Count].color = mg_bc_Constants.GetColorForJelly(component.Color);
			}
			base.OnCaughtBag(_bag);
		}
	}
}
