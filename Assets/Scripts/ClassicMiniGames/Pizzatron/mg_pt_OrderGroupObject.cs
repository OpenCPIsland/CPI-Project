using MinigameFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_OrderGroupObject : MonoBehaviour, IComparer<mg_pt_OrderToppingObject>
	{
		private int m_requiredCount;

		private List<mg_pt_OrderToppingObject> m_toppings;

		private GameObject m_completed;

		public mg_pt_EToppingType ToppingType
		{
			get;
			set;
		}

		public int RequiredCount
		{
			get
			{
				return m_requiredCount;
			}
			set
			{
				m_requiredCount = value;
				int i = 0;
				m_toppings.ForEach(delegate(mg_pt_OrderToppingObject topping)
				{
					topping.gameObject.SetActive(i++ < m_requiredCount);
				});
				m_completed.SetActive(false);
				m_toppings.ForEach(delegate(mg_pt_OrderToppingObject topping)
				{
					topping.Placed = false;
				});
			}
		}

		protected void Awake()
		{
			m_toppings = new List<mg_pt_OrderToppingObject>();
			mg_pt_OrderToppingObject[] componentsInChildren = GetComponentsInChildren<mg_pt_OrderToppingObject>();
			mg_pt_OrderToppingObject[] array = componentsInChildren;
			foreach (mg_pt_OrderToppingObject item in array)
			{
				m_toppings.Add(item);
			}
			m_toppings.Sort(this);
			m_completed = base.transform.Find("completed").gameObject;
		}

		public int Compare(mg_pt_OrderToppingObject p_x, mg_pt_OrderToppingObject p_y)
		{
			int num = Convert.ToInt32(p_x.name);
			int num2 = Convert.ToInt32(p_y.name);
			if (num > num2)
			{
				return 1;
			}
			return -1;
		}

		public void SetCurrentCount(int p_count)
		{
			int i = 0;
			m_toppings.ForEach(delegate(mg_pt_OrderToppingObject topping)
			{
				topping.Placed = (i++ < p_count);
			});
			bool activeSelf = m_completed.activeSelf;
			m_completed.SetActive(p_count >= RequiredCount && RequiredCount > 0);
			if (!activeSelf && m_completed.activeSelf)
			{
				MinigameManager.GetActive().PlaySFX("mg_pt_sfx_topping_complete");
			}
		}

		public void SetPosition(float p_x, float p_y)
		{
			Vector2 v = base.transform.localPosition;
			v.x = p_x;
			v.y = p_y;
			base.transform.localPosition = v;
		}
	}
}
