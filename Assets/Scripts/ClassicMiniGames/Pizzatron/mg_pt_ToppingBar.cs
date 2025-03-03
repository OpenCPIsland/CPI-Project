using System.Collections.Generic;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_ToppingBar : MonoBehaviour
	{
		private const string GO_SAUCE_01 = "sauce_01";

		private const string GO_SAUCE_02 = "sauce_02";

		private const string GO_TOPPING_CHEESE = "topping_cheese";

		private const string GO_TOPPING_01 = "topping_01";

		private const string GO_TOPPING_02 = "topping_02";

		private const string GO_TOPPING_03 = "topping_03";

		private const string GO_TOPPING_04 = "topping_04";

		private mg_pt_GameLogic m_gameLogic;

		private mg_pt_InputManager m_inputManager;

		private bool m_holdingSauce;

		private mg_pt_ToppingHolderObject[] m_toppingHolders;

		private List<mg_pt_Topping> m_toppings;

		protected void Awake()
		{
			m_toppingHolders = GetComponentsInChildren<mg_pt_ToppingHolderObject>();
			m_toppings = new List<mg_pt_Topping>();
		}

		public void Initialize(mg_pt_GameLogic p_gameLogic)
		{
			m_inputManager = new mg_pt_InputManager(p_gameLogic.Minigame.MainCamera, this);
			m_gameLogic = p_gameLogic;
			Initialize_Holders();
			Initialize_Toppings(m_gameLogic.Minigame.Resources);
		}

		protected void OnDestroy()
		{
			m_inputManager.TidyUp();
		}

		private void Initialize_Holders()
		{
			mg_pt_EResourceList mg_pt_EResourceList = mg_pt_EResourceList.NULL;
			mg_pt_EToppingType mg_pt_EToppingType = mg_pt_EToppingType.INVALID;
			string text = "";
			string text2 = "";
			mg_pt_ToppingHolderObject[] toppingHolders = m_toppingHolders;
			foreach (mg_pt_ToppingHolderObject mg_pt_ToppingHolderObject in toppingHolders)
			{
				text = "";
				text2 = "";
				switch (mg_pt_ToppingHolderObject.gameObject.name)
				{
				case "sauce_01":
					mg_pt_EResourceList = mg_pt_EResourceList.GAME_SAUCE_01;
					mg_pt_EToppingType = mg_pt_EToppingType.SAUCE_01;
					text = "mg_pt_sfx_sauce_start_01";
					text2 = "mg_pt_sfx_sauce_loop_01";
					break;
				case "sauce_02":
					mg_pt_EResourceList = mg_pt_EResourceList.GAME_SAUCE_02;
					mg_pt_EToppingType = mg_pt_EToppingType.SAUCE_02;
					text = "mg_pt_sfx_sauce_start_02";
					text2 = "mg_pt_sfx_sauce_loop_02";
					break;
				case "topping_cheese":
					mg_pt_EResourceList = mg_pt_EResourceList.GAME_HOLDER_CHEESE;
					mg_pt_EToppingType = mg_pt_EToppingType.CHEESE;
					break;
				case "topping_01":
					mg_pt_EResourceList = mg_pt_EResourceList.GAME_HOLDER_01;
					mg_pt_EToppingType = mg_pt_EToppingType.MIN_TOPPINGS;
					break;
				case "topping_02":
					mg_pt_EResourceList = mg_pt_EResourceList.GAME_HOLDER_02;
					mg_pt_EToppingType = mg_pt_EToppingType.TOPPING_02;
					break;
				case "topping_03":
					mg_pt_EResourceList = mg_pt_EResourceList.GAME_HOLDER_03;
					mg_pt_EToppingType = mg_pt_EToppingType.TOPPING_03;
					break;
				case "topping_04":
					mg_pt_EResourceList = mg_pt_EResourceList.GAME_HOLDER_04;
					mg_pt_EToppingType = mg_pt_EToppingType.TOPPING_04;
					break;
				default:
					mg_pt_EResourceList = mg_pt_EResourceList.NULL;
					mg_pt_EToppingType = mg_pt_EToppingType.INVALID;
					break;
				}
				mg_pt_ToppingHolderObject.Initialize(m_gameLogic.Minigame.Resources.GetInstancedResource(mg_pt_EResourceList), mg_pt_EToppingType, text, text2);
			}
		}

		private void Initialize_Toppings(mg_pt_Resources p_resources)
		{
			for (int i = 0; i < 60; i++)
			{
				m_toppings.Add(new mg_pt_Topping(p_resources));
			}
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			foreach (mg_pt_Topping topping in m_toppings)
			{
				topping.MinigameUpdate(p_deltaTime);
				if (topping.State == mg_pt_EToppingState.DROPPED)
				{
					m_gameLogic.OnToppingDropped(topping);
				}
				else if (m_holdingSauce && topping.IsSauce && topping.State == mg_pt_EToppingState.GRABBED)
				{
					m_gameLogic.OnToppingMoved(topping);
				}
			}
		}

		public void OnTouchStart(Vector2 p_point, int p_touchID)
		{
			if (OnTouchMove(p_point, p_touchID))
			{
				return;
			}
			mg_pt_ToppingHolderObject mg_pt_ToppingHolderObject = FindTopping(p_point);
			if (!(mg_pt_ToppingHolderObject != null))
			{
				return;
			}
			if (mg_pt_ToppingHolderObject.IsSauce)
			{
				if (!m_holdingSauce)
				{
					mg_pt_Topping mg_pt_Topping = GrabTopping(mg_pt_ToppingHolderObject.ToppingType, mg_pt_ToppingHolderObject.HeldTagSFX, p_point, p_touchID);
					if (mg_pt_Topping != null)
					{
						m_holdingSauce = true;
						mg_pt_ToppingHolderObject.OnGrabbed();
					}
				}
			}
			else
			{
				mg_pt_Topping mg_pt_Topping = GrabTopping(mg_pt_ToppingHolderObject.ToppingType, mg_pt_ToppingHolderObject.HeldTagSFX, p_point, p_touchID);
				if (mg_pt_Topping != null)
				{
					mg_pt_ToppingHolderObject.OnGrabbed();
				}
			}
		}

		public bool OnTouchMove(Vector2 p_point, int p_touchID)
		{
			bool result = false;
			mg_pt_Topping mg_pt_Topping = m_toppings.Find((mg_pt_Topping searchTopping) => searchTopping.TouchID == p_touchID && searchTopping.State == mg_pt_EToppingState.GRABBED);
			if (mg_pt_Topping != null)
			{
				mg_pt_Topping.UpdatePosition(p_point);
				m_gameLogic.OnToppingMoved(mg_pt_Topping);
				result = true;
			}
			return result;
		}

		public void OnTouchEnd(Vector2 p_point, int p_touchID)
		{
			mg_pt_Topping mg_pt_Topping = m_toppings.Find((mg_pt_Topping searchTopping) => searchTopping.TouchID == p_touchID && searchTopping.State == mg_pt_EToppingState.GRABBED);
			if (mg_pt_Topping != null)
			{
				mg_pt_Topping.UpdatePosition(p_point);
				DropTopping(mg_pt_Topping);
			}
		}

		private mg_pt_ToppingHolderObject FindTopping(Vector2 p_point)
		{
			mg_pt_ToppingHolderObject result = null;
			mg_pt_ToppingHolderObject[] toppingHolders = m_toppingHolders;
			foreach (mg_pt_ToppingHolderObject mg_pt_ToppingHolderObject in toppingHolders)
			{
				if (mg_pt_ToppingHolderObject.Clicked(p_point))
				{
					result = mg_pt_ToppingHolderObject;
					break;
				}
			}
			return result;
		}

		private mg_pt_Topping GrabTopping(mg_pt_EToppingType p_toppingType, string p_heldTagSFX, Vector2 p_position, int p_touchID)
		{
			mg_pt_Topping mg_pt_Topping = m_toppings.Find((mg_pt_Topping topping) => topping.State == mg_pt_EToppingState.IDLE);
			if (mg_pt_Topping != null)
			{
				mg_pt_Topping.SetTopping(p_toppingType, base.gameObject, p_heldTagSFX);
				mg_pt_Topping.OnGrabbed(p_position, p_touchID);
			}
			return mg_pt_Topping;
		}

		private void DropTopping(mg_pt_Topping p_topping)
		{
			p_topping.Drop();
			if (p_topping.IsSauce)
			{
				m_holdingSauce = false;
			}
			if (!m_gameLogic.OnToppingDropped(p_topping))
			{
				m_gameLogic.Minigame.PlaySFX("mg_pt_sfx_topping_release_0" + Random.Range(1, 4));
			}
		}

		public void ShowToppings(int p_highestTopping)
		{
			if (p_highestTopping > 0)
			{
				int num = 3 + p_highestTopping;
				for (int i = 3; i < num; i++)
				{
					m_toppingHolders[i].Show();
				}
			}
		}

		public void DisableInput()
		{
			m_inputManager.IsActive = false;
			List<mg_pt_Topping> list = m_toppings.FindAll((mg_pt_Topping searchTopping) => searchTopping.State == mg_pt_EToppingState.GRABBED);
			list.ForEach(delegate(mg_pt_Topping grabbedTopping)
			{
				DropTopping(grabbedTopping);
			});
		}

		public void EnableInput()
		{
			m_inputManager.IsActive = true;
		}
	}
}
