using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_Order
	{
		private mg_pt_GameLogic m_gameLogic;

		private List<mg_pt_OrderComponent> m_allToppings;

		private List<mg_pt_OrderComponent> m_sauces;

		private mg_pt_OrderComponent m_cheese;

		private List<mg_pt_OrderComponent> m_ingredients;

		private int m_pizzaBaseCount;

		public bool MysteryPizza
		{
			get;
			private set;
		}

		public bool Completed
		{
			get;
			private set;
		}

		public bool Failed
		{
			get;
			private set;
		}

		public int CoinsToAward
		{
			get;
			set;
		}

		public mg_pt_EResourceList PizzaBase
		{
			get
			{
				int num = (!MysteryPizza) ? UnityEngine.Random.Range(0, m_pizzaBaseCount) : 0;
				return (mg_pt_EResourceList)(51 + num);
			}
		}

		public mg_pt_Order(mg_pt_GameLogic p_gameLogic)
		{
			m_gameLogic = p_gameLogic;
			m_pizzaBaseCount = m_gameLogic.Minigame.Resources.PizzaBaseCount;
			m_sauces = new List<mg_pt_OrderComponent>();
			m_sauces.Add(new mg_pt_OrderComponent(mg_pt_EToppingType.SAUCE_01));
			m_sauces.Add(new mg_pt_OrderComponent(mg_pt_EToppingType.SAUCE_02));
			m_cheese = new mg_pt_OrderComponent(mg_pt_EToppingType.CHEESE);
			m_ingredients = new List<mg_pt_OrderComponent>();
			m_ingredients.Add(new mg_pt_OrderComponent(mg_pt_EToppingType.MIN_TOPPINGS));
			m_ingredients.Add(new mg_pt_OrderComponent(mg_pt_EToppingType.TOPPING_02));
			m_ingredients.Add(new mg_pt_OrderComponent(mg_pt_EToppingType.TOPPING_03));
			m_ingredients.Add(new mg_pt_OrderComponent(mg_pt_EToppingType.TOPPING_04));
			m_allToppings = new List<mg_pt_OrderComponent>();
			List<mg_pt_OrderComponent> sauces = m_sauces;
			Action<mg_pt_OrderComponent> action = delegate(mg_pt_OrderComponent sauce)
			{
				m_allToppings.Add(sauce);
			};
			sauces.ForEach(action);
			m_allToppings.Add(m_cheese);
			m_ingredients.ForEach(delegate(mg_pt_OrderComponent ingredient)
			{
				m_allToppings.Add(ingredient);
			});
		}

		public void Reset()
		{
			CoinsToAward = 0;
			m_allToppings.ForEach(delegate(mg_pt_OrderComponent topping)
			{
				topping.Reset();
			});
			m_gameLogic.OnOrderReset();
			Completed = false;
			Failed = false;
		}

		public void ResetSauce()
		{
			m_sauces.ForEach(delegate(mg_pt_OrderComponent sauce)
			{
				sauce.Reset();
			});
			m_gameLogic.OnOrderChanged();
		}

		public void Randomize()
		{
			m_allToppings.ForEach(delegate(mg_pt_OrderComponent topping)
			{
				topping.RequiredCount = 0;
			});
			MysteryPizza = (m_gameLogic.CurrentComplexity.MysteryChance > 0 && UnityEngine.Random.Range(0, 100) < m_gameLogic.CurrentComplexity.MysteryChance);
			if (!MysteryPizza)
			{
				if (UnityEngine.Random.Range(0, 100) < m_gameLogic.CurrentComplexity.CheeseChance)
				{
					m_cheese.RequiredCount = 1;
				}
				if (m_gameLogic.CurrentComplexity.RequireSauce)
				{
					m_sauces[UnityEngine.Random.Range(0, m_sauces.Count)].RequiredCount = 1;
				}
				if (m_gameLogic.CurrentComplexity.HighestTopping > 0)
				{
					RandomToppings(m_gameLogic.CurrentComplexity.HighestTopping, m_gameLogic.CurrentComplexity.ToppingTypes, m_gameLogic.CurrentComplexity.NumToppings);
				}
			}
			m_gameLogic.OnOrderGenerated();
		}

		private void RandomToppings(int p_maxIndex, int p_numberTypes, int p_totalToppings = 5, int p_maxPerTopping = 5)
		{
			int num = 0;
			int num2 = p_totalToppings;
			List<mg_pt_OrderComponent> list = m_ingredients.FindAll((mg_pt_OrderComponent topping) => (int)topping.Topping < 3 + p_maxIndex);
			while (list.Count > p_numberTypes)
			{
				list.RemoveAt(UnityEngine.Random.Range(0, list.Count));
			}
			while (--num2 >= 0)
			{
				num = UnityEngine.Random.Range(0, list.Count);
				list[num].RequiredCount++;
				if (list[num].RequiredCount >= p_maxPerTopping)
				{
					list.RemoveAt(num);
				}
				if (list.Count == 0)
				{
					break;
				}
			}
		}

		public void AddTopping(mg_pt_Topping p_topping)
		{
			mg_pt_OrderComponent mg_pt_OrderComponent = m_allToppings.Find((mg_pt_OrderComponent topping) => topping.Topping == p_topping.Type);
			if (mg_pt_OrderComponent != null)
			{
				if (p_topping.IsSauce)
				{
					mg_pt_OrderComponent.CurrentCount = 1;
					int coins = 0;
					m_sauces.ForEach(delegate(mg_pt_OrderComponent sauce)
					{
						coins += sauce.CoinsEarnt;
					});
					if (coins == 0)
					{
						mg_pt_OrderComponent.CoinsEarnt++;
						CoinsToAward++;
					}
				}
				else if (p_topping.IsCheese)
				{
					mg_pt_OrderComponent.CurrentCount = 1;
					if (mg_pt_OrderComponent.CoinsEarnt == 0)
					{
						mg_pt_OrderComponent.CoinsEarnt++;
						CoinsToAward++;
					}
				}
				else
				{
					mg_pt_OrderComponent.CurrentCount++;
					CoinsToAward++;
				}
				if (p_topping.IsSauce)
				{
					if (mg_pt_OrderComponent.RequiredCount > 0 || MysteryPizza)
					{
						m_gameLogic.Minigame.PlaySFX("mg_pt_sfx_topping_drop");
					}
				}
				else
				{
					m_gameLogic.Minigame.PlaySFX((mg_pt_OrderComponent.RequiredCount > 0 || MysteryPizza) ? "mg_pt_sfx_topping_drop" : "mg_pt_sfx_topping_error");
				}
			}
			m_gameLogic.OnOrderChanged();
			CheckCompletion();
		}

		private void CheckCompletion()
		{
			if (Completed || Failed)
			{
				return;
			}
			if (MysteryPizza)
			{
				if (GetToppingCount() >= 15)
				{
					MarkCompleted();
				}
				return;
			}
			CheckFailure();
			if (!Failed)
			{
				bool completed = true;
				m_allToppings.ForEach(delegate(mg_pt_OrderComponent topping)
				{
					completed &= topping.Completed;
				});
				if (completed)
				{
					MarkCompleted();
				}
			}
		}

		private void CheckFailure()
		{
			bool failed = false;
			failed |= m_cheese.Failed;
			m_ingredients.ForEach(delegate(mg_pt_OrderComponent ingredient)
			{
				failed |= ingredient.Failed;
			});
			if (failed)
			{
				MarkFailed();
			}
		}

		private void MarkFailed()
		{
			Failed = true;
			m_gameLogic.OnOrderFailed();
		}

		private void MarkCompleted()
		{
			if (MysteryPizza)
			{
				m_gameLogic.Minigame.PlaySFX("mg_pt_sfx_mystery_pizza_end");
			}
			Completed = true;
			m_gameLogic.OnOrderCompleted();
		}

		public int GetRequiredCount(mg_pt_EToppingType p_toppingType)
		{
			return m_allToppings.Find((mg_pt_OrderComponent topping) => topping.Topping == p_toppingType).RequiredCount;
		}

		public int GetCurrentCount(mg_pt_EToppingType p_toppingType)
		{
			return m_allToppings.Find((mg_pt_OrderComponent topping) => topping.Topping == p_toppingType).CurrentCount;
		}

		public int GetToppingCount()
		{
			int count = 0;
			m_allToppings.ForEach(delegate(mg_pt_OrderComponent topping)
			{
				count += topping.CurrentCount;
			});
			return count;
		}
	}
}
