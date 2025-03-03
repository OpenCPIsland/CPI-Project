using MinigameFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_OrderBubbleObject : MonoBehaviour
	{
		private const float TOPPING_SIZE = 1.2f;

		private const float HALF_TOPPING_SIZE = 0.6f;

		private const float TAIL_SIZE = 0.7f;

		private mg_pt_EOrderBubbleState m_state;

		private float m_currentAlpha;

		private SpriteRenderer m_largeBubble;

		private SpriteRenderer m_smallBubble;

		private bool m_smallDisplay;

		private mg_pt_Order m_order;

		private List<mg_pt_OrderGroupObject> m_toppings;

		private SpriteRenderer[] m_images;

		private GameObject m_mysteryPizza;

		private GameObject m_mysteryText;

		protected void Awake()
		{
			m_state = mg_pt_EOrderBubbleState.IDLE;
			m_largeBubble = base.transform.Find("large").GetComponent<SpriteRenderer>();
			m_smallBubble = base.transform.Find("small").GetComponent<SpriteRenderer>();
		}

		protected void Start()
		{
			m_state = mg_pt_EOrderBubbleState.BLINKING_OUT;
			SetAlpha(0f);
			SetScale(0f);
		}

		public void Initialize(mg_pt_Resources p_resources, mg_pt_Order p_order)
		{
			m_order = p_order;
			m_mysteryPizza = p_resources.GetInstancedResource(mg_pt_EResourceList.GAME_ORDER_MYSTERY);
			m_mysteryPizza.SetActive(false);
			MinigameSpriteHelper.AssignParent(m_mysteryPizza, base.gameObject);
			m_mysteryPizza.transform.localPosition = new Vector2(0f, 0.3f);
			m_mysteryText = m_mysteryPizza.transform.Find("text").gameObject;
			m_mysteryText.transform.GetComponent<Renderer>().sortingOrder = -97;
			m_toppings = new List<mg_pt_OrderGroupObject>();
			m_toppings.Add(p_resources.GetInstancedResource(mg_pt_EResourceList.GAME_ORDER_SAUCE_01).GetComponent<mg_pt_OrderGroupObject>());
			m_toppings.Add(p_resources.GetInstancedResource(mg_pt_EResourceList.GAME_ORDER_SAUCE_02).GetComponent<mg_pt_OrderGroupObject>());
			m_toppings.Add(p_resources.GetInstancedResource(mg_pt_EResourceList.GAME_ORDER_CHEESE).GetComponent<mg_pt_OrderGroupObject>());
			m_toppings.Add(p_resources.GetInstancedResource(mg_pt_EResourceList.GAME_ORDER_TOPPING_01).GetComponent<mg_pt_OrderGroupObject>());
			m_toppings.Add(p_resources.GetInstancedResource(mg_pt_EResourceList.GAME_ORDER_TOPPING_02).GetComponent<mg_pt_OrderGroupObject>());
			m_toppings.Add(p_resources.GetInstancedResource(mg_pt_EResourceList.GAME_ORDER_TOPPING_03).GetComponent<mg_pt_OrderGroupObject>());
			m_toppings.Add(p_resources.GetInstancedResource(mg_pt_EResourceList.GAME_ORDER_TOPPING_04).GetComponent<mg_pt_OrderGroupObject>());
			m_toppings.ForEach(delegate(mg_pt_OrderGroupObject topping)
			{
				MinigameSpriteHelper.AssignParent(topping.gameObject, base.gameObject);
			});
			int i = 0;
			m_toppings.ForEach(delegate(mg_pt_OrderGroupObject topping)
			{
				topping.ToppingType = (mg_pt_EToppingType)(i++);
			});
			m_images = GetComponentsInChildren<SpriteRenderer>(true);
		}

		public void NewOrder()
		{
			m_state = mg_pt_EOrderBubbleState.FADING_OUT;
			m_mysteryText.SetActive(false);
		}

		private void ShowOrder()
		{
			int numToppings = 0;
			m_toppings.ForEach(delegate(mg_pt_OrderGroupObject topping)
			{
				numToppings += Convert.ToInt32(ShowOrderTopping(topping));
			});
			m_mysteryPizza.SetActive(m_order.MysteryPizza);
			m_smallDisplay = (numToppings <= 3 && !m_order.MysteryPizza);
			m_largeBubble.gameObject.SetActive(!m_smallDisplay);
			m_smallBubble.gameObject.SetActive(m_smallDisplay);
			if (!m_order.MysteryPizza)
			{
				RepositionOrder(numToppings);
			}
		}

		private bool ShowOrderTopping(mg_pt_OrderGroupObject p_toppingGroup)
		{
			int requiredCount = m_order.GetRequiredCount(p_toppingGroup.ToppingType);
			p_toppingGroup.SetCurrentCount(m_order.GetCurrentCount(p_toppingGroup.ToppingType));
			p_toppingGroup.RequiredCount = requiredCount;
			return requiredCount > 0;
		}

		public void UpdateOrder()
		{
			if (m_state == mg_pt_EOrderBubbleState.IDLE)
			{
				int i = 0;
				m_toppings.ForEach(delegate(mg_pt_OrderGroupObject topping)
				{
					topping.SetCurrentCount(m_order.GetCurrentCount((mg_pt_EToppingType)(i++)));
				});
			}
		}

		private void RepositionOrder(int p_numToppings)
		{
			if (m_smallDisplay)
			{
				PositionInRow(m_smallBubble, 0f, m_toppings.FindAll((mg_pt_OrderGroupObject topping) => topping.RequiredCount > 0));
			}
			else
			{
				RepositionLarge(p_numToppings);
			}
		}

		private void RepositionLarge(int p_numToppings)
		{
			List<mg_pt_OrderGroupObject> list = m_toppings.FindAll((mg_pt_OrderGroupObject topping) => topping.RequiredCount > 0);
			int num = (p_numToppings > 5) ? 3 : 2;
			PositionInRow(m_largeBubble, 0.7f, list.GetRange(0, num));
			PositionInRow(m_largeBubble, -0.4f, list.GetRange(num, p_numToppings - num));
		}

		private void PositionInRow(SpriteRenderer p_bubble, float p_yPos, List<mg_pt_OrderGroupObject> p_requiredToppings)
		{
			float num = p_bubble.sprite.bounds.size.x - 0.7f;
			float num2 = (num - 1.2f * (float)p_requiredToppings.Count) / (float)(p_requiredToppings.Count + 1);
			float num3 = num * -0.5f;
			foreach (mg_pt_OrderGroupObject p_requiredTopping in p_requiredToppings)
			{
				p_requiredTopping.SetPosition(num3 + num2 + 0.6f, p_yPos);
				num3 += num2 + 1.2f;
			}
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			switch (m_state)
			{
			case mg_pt_EOrderBubbleState.FADING_OUT:
				UpdateFadingOut(p_deltaTime);
				break;
			case mg_pt_EOrderBubbleState.BLINKING_OUT:
				UpdateBlinkingOut(p_deltaTime);
				break;
			case mg_pt_EOrderBubbleState.BLINKING_IN:
				UpdateBlinkingIn(p_deltaTime);
				break;
			case mg_pt_EOrderBubbleState.BLINKING_IN_SHRINK:
				UpdateBlinkingInShrink(p_deltaTime);
				break;
			case mg_pt_EOrderBubbleState.FADING_IN:
				UpdateFadingIn(p_deltaTime);
				break;
			}
		}

		private void UpdateFadingOut(float p_deltaTime)
		{
			float num = p_deltaTime * 5f;
			float currentAlpha = m_currentAlpha;
			currentAlpha -= ((num > currentAlpha) ? currentAlpha : num);
			SetAlpha(currentAlpha);
			if (currentAlpha == 0f)
			{
				MinigameManager.GetActive().PlaySFX("mg_pt_sfx_chef_bubble_pop");
				m_state = mg_pt_EOrderBubbleState.BLINKING_OUT;
			}
		}

		private void UpdateBlinkingOut(float p_deltaTime)
		{
			float num = p_deltaTime * 5f;
			float y = base.transform.localScale.y;
			y -= ((num > y) ? y : num);
			SetScale(y);
			if (y == 0f)
			{
				ShowOrder();
				m_state = mg_pt_EOrderBubbleState.BLINKING_IN;
			}
		}

		private void UpdateBlinkingIn(float p_deltaTime)
		{
			float num = p_deltaTime * 5f;
			float y = base.transform.localScale.y;
			y += num;
			y = ((y > 1.2f) ? 1.2f : y);
			SetScale(y);
			if (y == 1.2f)
			{
				m_state = mg_pt_EOrderBubbleState.BLINKING_IN_SHRINK;
			}
		}

		private void UpdateBlinkingInShrink(float p_deltaTime)
		{
			float num = p_deltaTime * 5f;
			float y = base.transform.localScale.y;
			y -= num;
			y = ((y < 1f) ? 1f : y);
			SetScale(y);
			if (y == 1f)
			{
				m_state = mg_pt_EOrderBubbleState.FADING_IN;
				m_mysteryText.SetActive(true);
			}
		}

		private void UpdateFadingIn(float p_deltaTime)
		{
			float num = p_deltaTime * 5f;
			float currentAlpha = m_currentAlpha;
			currentAlpha += num;
			currentAlpha = ((currentAlpha > 1f) ? 1f : currentAlpha);
			SetAlpha(currentAlpha);
			if (currentAlpha == 1f)
			{
				m_state = mg_pt_EOrderBubbleState.IDLE;
			}
		}

		private void SetAlpha(float p_alpha)
		{
			m_currentAlpha = p_alpha;
			SpriteRenderer[] images = m_images;
			foreach (SpriteRenderer spriteRenderer in images)
			{
				Color color = spriteRenderer.color;
				color.a = p_alpha;
				spriteRenderer.color = color;
			}
		}

		private void SetScale(float p_scale)
		{
			Vector2 v = base.transform.localScale;
			v.y = p_scale;
			v.x = p_scale;
			base.transform.localScale = v;
		}
	}
}
