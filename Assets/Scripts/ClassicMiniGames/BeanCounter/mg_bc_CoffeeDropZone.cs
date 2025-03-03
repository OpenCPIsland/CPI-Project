using System;
using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_CoffeeDropZone : mg_bc_DropZone
	{
		private Animator m_mainBagStack;

		private Animator m_secondBagStack;

		public override void Awake()
		{
			m_mainBagStack = base.transform.Find("storage_01").gameObject.GetComponent<Animator>();
			m_secondBagStack = base.transform.Find("storage2offset").GetChild(0).gameObject.GetComponent<Animator>();
			m_mainBagStack.SetInteger("num_bags", 0);
			m_secondBagStack.SetInteger("num_bags", 0);
			base.Awake();
		}

		protected override void SetBags(int _bags)
		{
			base.SetBags(_bags);
			int value = Math.Min(m_storedBags, 30);
			int value2 = Math.Min(30, Math.Max(m_storedBags - 30, 0));
			m_mainBagStack.SetInteger("num_bags", value);
			m_secondBagStack.SetInteger("num_bags", value2);
		}
	}
}
