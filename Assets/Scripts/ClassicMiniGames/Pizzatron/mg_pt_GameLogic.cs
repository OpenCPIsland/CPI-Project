using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_GameLogic
	{
		private mg_pt_Conveyor m_conveyor;

		private mg_pt_Pizza m_pizza;

		private mg_pt_ToppingBar m_toppingBar;

		private mg_pt_PenguinManager m_penguinManager;

		private mg_pt_BoardObject m_boardObject;

		private bool m_levelUpShown;

		public mg_pt_UILevelUp LevelUp;

		private bool m_isActive;

		private mg_pt_GameScreen m_screen;

		public mg_Pizzatron Minigame
		{
			get;
			private set;
		}

		public mg_pt_ComplexityData CurrentComplexity
		{
			get;
			private set;
		}

		public int OrdersCompleted
		{
			get;
			private set;
		}

		public mg_pt_Order Order
		{
			get;
			private set;
		}

		public int Lives
		{
			get;
			private set;
		}

		public bool EndlessRun
		{
			get;
			private set;
		}

		public mg_pt_GameLogic()
		{
			Minigame = MinigameManager.GetActive<mg_Pizzatron>();
			m_conveyor = new mg_pt_Conveyor();
			Order = new mg_pt_Order(this);
			m_pizza = new mg_pt_Pizza();
		}

		public void Destroy()
		{
		}

		public void Initialize(mg_pt_GameScreen p_screen)
		{
			m_isActive = true;
			m_screen = p_screen;
			Lives = 5;
			Minigame.SetLogic(this);
			m_penguinManager = m_screen.PenguinManager;
			m_penguinManager.Initialize(Minigame.Resources, Order);
			m_toppingBar = m_screen.ToppingBar;
			SetComplexity(Minigame.Resources.ComplexityDataHead);
			m_conveyor.Initialize(m_screen, Minigame.Resources.ConveyorSpeedData);
			Order.Randomize();
			m_pizza.Initialize(this, m_screen);
			m_toppingBar.Initialize(this);
			m_boardObject = m_screen.BoardObject;
			m_boardObject.Initialize(this);
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			if (m_isActive)
			{
				m_toppingBar.MinigameUpdate(p_deltaTime);
				m_penguinManager.MinigameUpdate(p_deltaTime);
				m_pizza.MinigameUpdate(p_deltaTime, m_conveyor.ConveyorSpeed);
				m_conveyor.MinigameUpdate(p_deltaTime, m_pizza.SpeedModifier);
			}
		}

		public bool OnToppingDropped(mg_pt_Topping p_topping)
		{
			return m_pizza.OnToppingDropped(p_topping);
		}

		public void OnToppingMoved(mg_pt_Topping p_topping)
		{
			m_pizza.OnToppingMoved(p_topping);
		}

		public void OnOrderReset()
		{
			m_penguinManager.OnOrderReset();
		}

		public void OnOrderGenerated()
		{
			m_penguinManager.OnOrderGenerated(CurrentComplexity.SpecialCustomerChance);
		}

		public void OnOrderFailed()
		{
			Lives--;
			m_pizza.State = mg_pt_EPizzaState.FAILED;
			m_conveyor.PizzaFailed();
			if (Lives <= 0)
			{
				OnGameOver();
			}
		}

		public void OnOrderCompleted()
		{
			OrdersCompleted++;
			m_conveyor.PizzaCompleted();
			CheckComplexityIncrease();
			if (Order.MysteryPizza)
			{
				Minigame.CoinsEarned += Mathf.Min(Order.GetToppingCount(), 15);
			}
			else
			{
				Minigame.CoinsEarned += 5;
			}
			m_pizza.State = mg_pt_EPizzaState.COMPLETE;
			Order.Randomize();
			if (OrdersCompleted == 40)
			{
				OnGameOver();
			}
		}

		private void CheckComplexityIncrease()
		{
			while (CurrentComplexity.NextData != null && CurrentComplexity.NextData.RequiredPizzas <= OrdersCompleted)
			{
				m_levelUpShown = false;
				SetComplexity(CurrentComplexity.NextData);
			}
		}

		private void SetComplexity(mg_pt_ComplexityData p_data)
		{
			CurrentComplexity = p_data;
			m_toppingBar.ShowToppings(CurrentComplexity.HighestTopping);
			m_conveyor.ComplexitySpeedMultiplier = CurrentComplexity.SpeedMultiplier;
		}

		public void OnOrderChanged()
		{
			m_penguinManager.OnOrderChanged();
		}

		public void OnPizzaExit()
		{
			if (!m_levelUpShown && CurrentComplexity.LevelUp)
			{
				LevelUp.ShowLevelUp();
				m_levelUpShown = true;
			}
			else
			{
				ResetPizza();
			}
		}

		public void ResetPizza()
		{
			m_pizza.Reset();
			if (Order.MysteryPizza)
			{
				Minigame.PlaySFX("mg_pt_sfx_mystery_pizza_start");
			}
		}

		private void OnGameOver()
		{
			DisableInput();
			m_isActive = false;
			UIManager.Instance.OpenScreen("mg_pt_ResultScreen", false, OnResultPopped, null);
			m_screen.gameObject.SetActive(false);
			Minigame.StopAllSFX();
			Minigame.PauseMusic();
			Minigame.PlaySFX("mg_pt_sfx_game_over");
		}

		private void OnResultPopped(UIControlBase p_screen)
		{
			Minigame.ResumeMusic();
			m_screen.gameObject.SetActive(true);
			EnableInput();
			mg_pt_ResultScreen mg_pt_ResultScreen = p_screen as mg_pt_ResultScreen;
			if (mg_pt_ResultScreen.Continue)
			{
				m_conveyor.PlaySFX();
				EndlessRun = true;
				m_isActive = true;
				m_boardObject.ForceUpdate();
			}
			else if (mg_pt_ResultScreen.Restart)
			{
				Minigame.ShowTitle();
			}
			else
			{
				MinigameManager.Instance.OnMinigameEnded();
			}
		}

		public void DisableInput()
		{
			m_toppingBar.DisableInput();
		}

		public void EnableInput()
		{
			m_toppingBar.EnableInput();
		}
	}
}
