using MinigameFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_BoardObject : MonoBehaviour, IComparer<mg_pt_OrderToppingObject>
	{
		private mg_pt_GameLogic m_gameLogic;

		private List<mg_pt_OrderToppingObject> m_lives;

		private GameObject m_text;

		private mg_pt_BoardPizzaObject m_pizza;

		private int m_livesShown;

		private int m_ordersCompleted;

		protected void Awake()
		{
			m_lives = new List<mg_pt_OrderToppingObject>();
			mg_pt_OrderToppingObject[] componentsInChildren = GetComponentsInChildren<mg_pt_OrderToppingObject>();
			mg_pt_OrderToppingObject[] array = componentsInChildren;
			foreach (mg_pt_OrderToppingObject item in array)
			{
				m_lives.Add(item);
			}
			m_lives.Sort(this);
			m_text = base.transform.Find("text").gameObject;
			m_text.GetComponent<Renderer>().sortingOrder = -90;
			m_ordersCompleted = -1;
		}

		public void Initialize(mg_pt_GameLogic p_gameLogic)
		{
			m_gameLogic = p_gameLogic;
			GameObject instancedResource = m_gameLogic.Minigame.Resources.GetInstancedResource(mg_pt_EResourceList.GAME_PIZZA_BOARD);
			m_pizza = instancedResource.GetComponent<mg_pt_BoardPizzaObject>();
			MinigameSpriteHelper.AssignParentPositionReset(instancedResource, base.transform.Find("pizza").gameObject);
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

		protected void Update()
		{
			UpdateLives();
			UpdateOrdersCompleted();
		}

		private void UpdateLives()
		{
			if (m_gameLogic.Lives != m_livesShown)
			{
				m_livesShown = m_gameLogic.Lives;
				int i = 5;
				m_lives.ForEach(delegate(mg_pt_OrderToppingObject life)
				{
					life.Placed = (i-- > m_livesShown);
				});
			}
		}

		private void UpdateText()
		{
			string text = m_ordersCompleted.ToString();
			if (!m_gameLogic.EndlessRun)
			{
				text = text + " / " + 40;
			}
			m_text.GetComponent<TextMesh>().text = text;
		}

		private void UpdateOrdersCompleted()
		{
			if (m_ordersCompleted != m_gameLogic.OrdersCompleted)
			{
				UpdatePizza();
			}
		}

		private void UpdatePizza()
		{
			m_ordersCompleted = m_gameLogic.OrdersCompleted;
			UpdateText();
			if (!m_gameLogic.EndlessRun)
			{
				m_pizza.SetPercentage((float)m_ordersCompleted / 40f);
			}
		}

		public void ForceUpdate()
		{
			UpdatePizza();
		}
	}
}
