using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_Pizza
	{
		private mg_pt_GameLogic m_gameLogic;

		private List<mg_pt_Topping> m_toppings;

		private mg_pt_GameScreen m_gameScreen;

		private mg_pt_EToppingType m_sauceBeingAdded;

		private mg_pt_PizzaObject m_pizzaObject;

		private List<mg_pt_CoinObject> m_coins;

		private int m_totalCoinsSpawned;

		public float SpeedModifier
		{
			get;
			private set;
		}

		public mg_pt_EPizzaState State
		{
			get;
			set;
		}

		public void Initialize(mg_pt_GameLogic p_gameLogic, mg_pt_GameScreen p_gameScreen)
		{
			m_toppings = new List<mg_pt_Topping>();
			m_coins = new List<mg_pt_CoinObject>();
			m_gameLogic = p_gameLogic;
			m_gameScreen = p_gameScreen;
			Reset();
		}

		public void Reset()
		{
			m_totalCoinsSpawned = 0;
			m_gameLogic.Order.Reset();
			SpeedModifier = 1f;
			State = mg_pt_EPizzaState.ACTIVE;
			m_toppings.ForEach(delegate(mg_pt_Topping topping)
			{
				topping.Reset();
			});
			m_toppings.Clear();
			if (m_pizzaObject != null)
			{
				Object.Destroy(m_pizzaObject.gameObject);
			}
			m_pizzaObject = m_gameLogic.Minigame.Resources.GetInstancedResource(m_gameLogic.Order.PizzaBase).GetComponent<mg_pt_PizzaObject>();
			MinigameSpriteHelper.AssignParentTransform(m_pizzaObject.gameObject, m_gameScreen.GameTransform);
			m_pizzaObject.transform.position = m_gameScreen.PizzaSpawnStart.position;
			m_sauceBeingAdded = mg_pt_EToppingType.INVALID;
		}

		public void MinigameUpdate(float p_deltaTime, float p_conveyorSpeed)
		{
			float speedModifier = SpeedModifier;
			SpeedModifier = 1f;
			if (State != mg_pt_EPizzaState.INVALID && m_pizzaObject != null)
			{
				if (m_pizzaObject.transform.position.x < m_gameScreen.PizzaSpeedPoint.position.x)
				{
					SpeedModifier = 3f;
				}
				else if (State == mg_pt_EPizzaState.COMPLETE || State == mg_pt_EPizzaState.FAILED)
				{
					SpeedModifier = Mathf.Min(speedModifier + 5f * p_deltaTime, 15f);
				}
				Vector2 v = m_pizzaObject.transform.localPosition;
				v.x += p_deltaTime * p_conveyorSpeed * SpeedModifier;
				m_pizzaObject.transform.localPosition = v;
				CheckExit();
			}
			m_coins.ForEach(delegate(mg_pt_CoinObject coin)
			{
				coin.MinigameUpdate(p_deltaTime);
			});
		}

		private void CheckExit()
		{
			if (!(m_pizzaObject.transform.position.x > m_gameScreen.PizzaSpawnEnd.position.x))
			{
				return;
			}
			if (State == mg_pt_EPizzaState.ACTIVE)
			{
				if (m_gameLogic.Order.MysteryPizza)
				{
					m_gameLogic.OnOrderCompleted();
				}
				else
				{
					m_gameLogic.OnOrderFailed();
				}
			}
			State = mg_pt_EPizzaState.INVALID;
			m_gameLogic.OnPizzaExit();
		}

		public bool OnToppingDropped(mg_pt_Topping p_topping)
		{
			bool result = false;
			if (State == mg_pt_EPizzaState.ACTIVE && !p_topping.IsSauce && m_pizzaObject.ContainsPoint(p_topping.Position))
			{
				AddTopping(p_topping);
				result = true;
			}
			return result;
		}

		public void OnToppingMoved(mg_pt_Topping p_topping)
		{
			if (State == mg_pt_EPizzaState.ACTIVE && p_topping.IsSauce && m_pizzaObject.ContainsPoint(p_topping.SaucePosition))
			{
				if (m_sauceBeingAdded != p_topping.Type)
				{
					m_pizzaObject.ClearSauce(m_gameLogic.Minigame.Resources.GetSauceColor(p_topping.Type));
					m_gameLogic.Order.ResetSauce();
					m_sauceBeingAdded = p_topping.Type;
				}
				m_pizzaObject.AddSauceAt(this, p_topping);
			}
		}

		public void SauceAdded(mg_pt_Topping p_sauce)
		{
			UpdateTopping(p_sauce);
		}

		private void AddTopping(mg_pt_Topping p_topping)
		{
			UpdateTopping(p_topping);
			if (p_topping.IsCheese)
			{
				m_pizzaObject.ShowCheese();
			}
			p_topping.Place(m_pizzaObject.gameObject);
			if (p_topping.State == mg_pt_EToppingState.PLACED)
			{
				m_toppings.Add(p_topping);
			}
		}

		private void UpdateTopping(mg_pt_Topping p_topping)
		{
			bool mysteryPizza = m_gameLogic.Order.MysteryPizza;
			m_gameLogic.Order.AddTopping(p_topping);
			if (mysteryPizza)
			{
				AddCoin(p_topping);
			}
		}

		private void AddCoin(mg_pt_Topping p_topping)
		{
			while (m_gameLogic.Order.CoinsToAward > 0)
			{
				mg_pt_CoinObject mg_pt_CoinObject = m_coins.Find((mg_pt_CoinObject coin) => coin.State == mg_pt_ECoinState.INACTIVE);
				if (mg_pt_CoinObject == null)
				{
					GameObject instancedResource = m_gameLogic.Minigame.Resources.GetInstancedResource(mg_pt_EResourceList.GAME_COIN);
					MinigameSpriteHelper.AssignParent(instancedResource, m_gameScreen.GameTransform.gameObject);
					mg_pt_CoinObject = instancedResource.GetComponent<mg_pt_CoinObject>();
					m_coins.Add(mg_pt_CoinObject);
				}
				mg_pt_CoinObject.Spawn(p_topping.IsSauce ? p_topping.SaucePosition : p_topping.Position);
				m_gameLogic.Order.CoinsToAward--;
				m_totalCoinsSpawned++;
			}
			m_gameLogic.Order.CoinsToAward = 0;
		}
	}
}
