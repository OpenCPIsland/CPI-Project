using MinigameFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_Penguin : MonoBehaviour
	{
		private const int STARTING_LIVES = 3;

		private const int MAX_BAGS = 5;

		private const float DEATH_COOLDOWN = 3f;

		private const float SHIELD_DURATION = 14f;

		private const float SHIELD_FLASH_TIME = 3f;

		private const float SHIELD_FLASH_COUNT = 3f;

		private static int POOF_IDLE_HASH = Animator.StringToHash("Base Layer.mg_bc_poof_off");

		private float m_deadCooldown = 0f;

		private bool m_canCatch = true;

		private bool m_isShielded;

		private float m_shieldTimer;

		protected Stack<mg_bc_Bag> m_heldBagsStack;

		private float m_leftLimit;

		private float m_rightLimit;

		private Animator m_animator;

		private GameObject m_warning;

		private GameObject m_sweat;

		private GameObject m_shield;

		private GameObject m_poof;

		private GameObject m_explosion;

		private BoxCollider2D m_box;

		public mg_bc_UIValue<int> Lives
		{
			get;
			private set;
		}

		public bool IsDead
		{
			get;
			private set;
		}

		public mg_bc_GameLogic GameLogic
		{
			get;
			set;
		}

		public mg_bc_Penguin()
		{
			Lives = new mg_bc_UIValue<int>();
			Lives.SetValue(3);
		}

		public virtual void Awake()
		{
			m_heldBagsStack = new Stack<mg_bc_Bag>();
			m_warning = base.gameObject.transform.Find("mg_bc_warning").gameObject;
			m_sweat = base.gameObject.transform.Find("mg_bc_sweat").gameObject;
			m_shield = base.gameObject.transform.Find("mg_bc_shield").gameObject;
			m_poof = base.gameObject.transform.Find("mg_bc_poof").gameObject;
			m_explosion = base.gameObject.transform.Find("mg_bc_explosion").gameObject;
			m_animator = base.gameObject.GetComponent<Animator>();
			m_box = base.gameObject.GetComponent<BoxCollider2D>();
			m_rightLimit -= m_box.size.x;
			base.gameObject.transform.Find("penguin_tint").GetComponent<SpriteRenderer>().color = MinigameManager.Instance.GetPenguinColor();
			ClearBags();
			RemoveShield();
		}

		private void UpdateBagCount()
		{
			int count = m_heldBagsStack.Count;
			m_warning.SetActive(count == 5);
			m_sweat.SetActive(count == 5);
			if (count == 5)
			{
				MinigameManager.GetActive().PlaySFX("mg_bc_sfx_PlayerOverloadBagsStart");
				MinigameManager.GetActive().PlaySFX("mg_bc_sfx_PlayerOverloadBagsLoop");
			}
			else
			{
				MinigameManager.GetActive().StopSFX("mg_bc_sfx_PlayerOverloadBagsLoop");
			}
			if (count > 5)
			{
				MinigameManager.GetActive().PlaySFX("mg_bc_sfx_BagOverloadPlayer");
				Die(mg_bc_EPenguinDeaths.DEATH_TOO_MANY_BAGS);
			}
			else
			{
				m_animator.SetInteger("held_bags", count);
			}
		}

		private void Die(mg_bc_EPenguinDeaths _type)
		{
			m_deadCooldown = 0f;
			ClearBags();
			RemoveShield();
			IsDead = true;
			m_animator.SetBool("is_dead", true);
			m_animator.SetInteger("death_type", (int)_type);
			GameLogic.OnPenguinDeath();
		}

		private void Revive()
		{
			Lives.SetValue(Lives.Value - 1);
			IsDead = false;
			ClearBags();
			m_animator.SetBool("is_dead", false);
			m_animator.SetInteger("death_type", 1);
			GameLogic.OnPenguinRevive();
		}

		public void Move(float _x)
		{
			if (!IsDead)
			{
				Vector3 localPosition = base.gameObject.transform.localPosition;
				localPosition.x += _x;
				if (localPosition.x <= m_leftLimit)
				{
					localPosition.x = m_leftLimit;
				}
				else if (localPosition.x >= m_rightLimit)
				{
					localPosition.x = m_rightLimit;
				}
				base.gameObject.transform.localPosition = localPosition;
			}
		}

		internal void MoveTo(Vector3 worldPoint)
		{
			if (!IsDead)
			{
				Vector3 localPosition = base.gameObject.transform.localPosition;
				localPosition.x = worldPoint.x;
				if (localPosition.x <= m_leftLimit)
				{
					localPosition.x = m_leftLimit;
				}
				else if (localPosition.x >= m_rightLimit)
				{
					localPosition.x = m_rightLimit;
				}
				base.gameObject.transform.localPosition = localPosition;
			}
		}

		internal void PenguinUpdate(float _deltaTime)
		{
			if (IsDead)
			{
				float deadCooldown = m_deadCooldown;
				m_deadCooldown += _deltaTime;
				int num = (int)(3f - deadCooldown + 1f);
				int num2 = (int)(3f - m_deadCooldown + 1f);
				if (num != num2 && num2 > 0)
				{
					MinigameManager.GetActive().PlaySFX("mg_bc_sfx_UICountdown321");
					GameLogic.NoticeDisplayer.ShowMessage(num2 + "\nTry Again...", 1f);
				}
				if (m_deadCooldown >= 3f)
				{
					Revive();
				}
			}
			else if (m_isShielded)
			{
				m_shieldTimer -= _deltaTime;
				if (m_shieldTimer <= 0f)
				{
					RemoveShield();
				}
				else if (m_shieldTimer < 3f)
				{
					SpriteRenderer component = m_shield.GetComponent<SpriteRenderer>();
					Color color = component.color;
					color.a = 0.5f * (Mathf.Sin(5.23f * m_shieldTimer - (float)Math.PI / 2f) + 1f);
					component.color = color;
				}
			}
		}

		private void OnTriggerEnter2D(Collider2D _collider)
		{
			if (!IsDead && m_canCatch)
			{
				mg_bc_Hazard component = _collider.gameObject.GetComponent<mg_bc_Hazard>();
				if (component != null)
				{
					OnHitHazard(component);
				}
				mg_bc_Bag component2 = _collider.gameObject.GetComponent<mg_bc_Bag>();
				if (component2 != null)
				{
					OnCaughtBag(component2);
				}
				mg_bc_Powerup component3 = _collider.gameObject.GetComponent<mg_bc_Powerup>();
				if (component3 != null)
				{
					OnCaughtPowerup(component3);
				}
			}
		}

		private void OnCaughtPowerup(mg_bc_Powerup _powerup)
		{
			switch (_powerup.PowerupType)
			{
			case mg_bc_EPowerupType.EXTRA_LIFE:
				Lives.SetValue(Lives.Value + 1);
				GameLogic.OnPenguinGainLife();
				break;
			case mg_bc_EPowerupType.INVINCIBILITY:
				StartShield();
				break;
			}
			_powerup.OnCaught();
			_powerup.Destroy();
		}

		protected virtual void OnCaughtBag(mg_bc_Bag _bag)
		{
			if (_bag.State != mg_bc_EObjectState.STATE_HELD)
			{
				m_heldBagsStack.Push(_bag);
				UpdateBagCount();
				_bag.OnCaught();
				if (!IsDead)
				{
					mg_bc_ScoreController.Instance.OnBagCaught();
				}
				else
				{
					_bag.Destroy();
				}
			}
		}

		private void OnHitHazard(mg_bc_Hazard _hazard)
		{
			if (!m_isShielded)
			{
				switch (_hazard.HazardType)
				{
				case mg_bc_EHazardType.HAZARD_ANVIL:
					Die(mg_bc_EPenguinDeaths.DEATH_ANVIL);
					break;
				case mg_bc_EHazardType.HAZARD_FISH:
					Die(mg_bc_EPenguinDeaths.DEATH_FISH);
					break;
				case mg_bc_EHazardType.HAZARD_FLOWERS:
					Die(mg_bc_EPenguinDeaths.DEATH_POT);
					break;
				}
				_hazard.OnCaught();
				Animator component = m_explosion.GetComponent<Animator>();
				component.SetTrigger("start");
			}
			else
			{
				Animator component2 = m_poof.GetComponent<Animator>();
				if (component2.GetCurrentAnimatorStateInfo(0).fullPathHash == POOF_IDLE_HASH)
				{
					m_poof.GetComponent<Animator>().SetTrigger("start");
				}
			}
			_hazard.Destroy();
		}

		internal void SetMovementLimits(float _left, float _right)
		{
			m_leftLimit = _left;
			m_rightLimit = _right;
		}

		internal virtual mg_bc_Bag RemoveBag()
		{
			mg_bc_Bag result = null;
			if (m_heldBagsStack.Count > 0)
			{
				result = m_heldBagsStack.Pop();
				UpdateBagCount();
			}
			return result;
		}

		private void StartShield()
		{
			m_shield.SetActive(true);
			m_isShielded = true;
			m_shieldTimer = 14f;
			SpriteRenderer component = m_shield.GetComponent<SpriteRenderer>();
			Color color = component.color;
			color.a = 1f;
			component.color = color;
		}

		private void RemoveShield()
		{
			m_shield.SetActive(false);
			m_isShielded = false;
		}

		internal void ClearBags()
		{
			while (m_heldBagsStack.Count > 0)
			{
				m_heldBagsStack.Pop().Destroy();
			}
			UpdateBagCount();
		}

		internal void DisableCollisions()
		{
			m_canCatch = false;
		}

		internal void EnableCollisions()
		{
			m_canCatch = true;
		}

		internal void OnDropAllowanceExceeded()
		{
			ClearBags();
			Die(mg_bc_EPenguinDeaths.DEATH_DROPS_EXCEEDED);
		}
	}
}
