using MinigameFramework;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_Item_FruitObject : mg_ss_ItemObject
	{
		private Transform m_comboHolder;

		private SpriteRenderer m_comboRenderer;

		private float m_alphaCountDown;

		private GameObject m_highlight;

		private bool m_chaosItem;

		private bool m_partOfOrder;

		public override bool ChaosItem
		{
			get
			{
				return m_chaosItem;
			}
		}

		public Color Color
		{
			get
			{
				Color result;
				switch (base.ItemType)
				{
				case mg_ss_EItemTypes.APPLE:
					result = new Color(0.369f, 1f, 0.192f);
					break;
				case mg_ss_EItemTypes.BANANA:
					result = new Color(1f, 1f, 0f);
					break;
				case mg_ss_EItemTypes.BLACKBERRY:
					result = new Color(0.639f, 0.506f, 0.776f);
					break;
				case mg_ss_EItemTypes.BLUEBERRY:
					result = new Color(0.38f, 0.749f, 0.898f);
					break;
				case mg_ss_EItemTypes.FIG:
					result = new Color(0.961f, 1f, 0.486f);
					break;
				case mg_ss_EItemTypes.GRAPES:
					result = new Color(0.796f, 0.541f, 0.808f);
					break;
				case mg_ss_EItemTypes.MANGO:
					result = new Color(1f, 0.992f, 0.682f);
					break;
				case mg_ss_EItemTypes.ORANGE:
					result = new Color(1f, 0.788f, 0.2f);
					break;
				case mg_ss_EItemTypes.PEACH:
					result = new Color(1f, 0.8f, 0.78f);
					break;
				case mg_ss_EItemTypes.PINEAPPLE:
					result = new Color(0.996f, 0.969f, 0.522f);
					break;
				case mg_ss_EItemTypes.RASPBERRY:
					result = new Color(0.976f, 0.38f, 0.51f);
					break;
				case mg_ss_EItemTypes.STRAWBERRY:
					result = new Color(1f, 0.584f, 0.502f);
					break;
				default:
					result = new Color(1f, 1f, 1f);
					break;
				}
				return result;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			m_comboHolder = base.transform.Find("combo");
			m_highlight = base.transform.Find("highlight").gameObject;
		}

		public override void Initialize(mg_ss_EItemTypes p_itemType, mg_ss_IItemMovement p_movement, Vector2 p_spawnPointBottom, Vector2 p_spawnPointTop, float p_screenWidth, bool p_chaosItem)
		{
			m_chaosItem = p_chaosItem;
			base.Initialize(p_itemType, p_movement, p_spawnPointBottom, p_spawnPointTop, p_screenWidth, p_chaosItem);
			if (m_chaosItem)
			{
				SetInitialPosition(new Vector2(p_spawnPointBottom.x, Random.Range(p_spawnPointBottom.y, p_spawnPointTop.y)));
			}
		}

		public override void OnCollided(mg_ss_EPlayerAction p_playerAction)
		{
			ShowHighlight(false);
			m_partOfOrder = (!m_chaosItem && m_logic.ItemPartOfOrder(base.ItemType));
			if (IsItemOnConveyor())
			{
				UpdateAnimation(m_partOfOrder);
			}
			base.OnCollided(p_playerAction);
			m_logic.OnFruitCollision(this);
		}

		public override void UpdatePosition(float p_deltaTime, float p_conveyorSpeed)
		{
			base.UpdatePosition(p_deltaTime, p_conveyorSpeed);
			if (base.Animation == mg_ss_EItemAnimation.IDLE && !base.Collidable && IsItemOnConveyor())
			{
				UpdateAnimation(m_partOfOrder);
			}
		}

		private void UpdateAnimation(bool p_partOfOrder)
		{
			if (p_partOfOrder)
			{
				base.Animation = mg_ss_EItemAnimation.ORDER;
			}
			else
			{
				base.Animation = mg_ss_EItemAnimation.COLLIDED;
			}
		}

		public void ShowCombo(int p_combo)
		{
			GameObject combo = m_logic.Minigame.Resources.GetCombo(p_combo);
			MinigameSpriteHelper.AssignParentTransform(combo, m_comboHolder);
			combo.transform.localPosition = new Vector2(0f, 0f);
			m_comboRenderer = combo.GetComponent<SpriteRenderer>();
			m_alphaCountDown = 1f;
		}

		public override void MinigameUpdate(float p_deltaTime, float p_conveyorSpeed)
		{
			base.MinigameUpdate(p_deltaTime, p_conveyorSpeed);
			if (m_comboRenderer != null)
			{
				Color color = m_comboRenderer.color;
				color.a = Mathf.Lerp(1f, 0f, 1f - m_alphaCountDown / 1f);
				m_comboRenderer.color = color;
				m_alphaCountDown -= p_deltaTime;
			}
		}

		public override void PlayBounceSFX()
		{
			MinigameManager.GetActive().PlaySFX("mg_ss_sfx_fruit_bounce");
		}

		public override void ShowHighlight(bool p_show)
		{
			m_highlight.SetActive(p_show);
		}
	}
}
