using MinigameFramework;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_PlayerObject : MonoBehaviour
	{
		[Range(0.1f, 10f)]
		public float KeyboardHorizonatlSpeedModifier = 5.95f;

		private mg_ss_PlayerLogic m_playerLogic;

		private Vector2 m_startingPos;

		private Transform m_gameZoneLeft;

		private Transform m_gameZoneRight;

		private Transform m_shadow;

		private Animator m_animator;

		private float m_conveyorPosY;

		private float m_dyingTimer;

		protected void Awake()
		{
			m_animator = GetComponentInChildren<Animator>();
			base.transform.Find("mg_ss_player_tint").GetComponent<SpriteRenderer>().color = MinigameManager.Instance.GetPenguinColor();
			m_shadow = base.transform.parent.Find("mg_ss_penguin_shadow");
		}

		public void Initialize(mg_ss_PlayerLogic p_logic, Transform p_zoneLeft, Transform p_zoneRight)
		{
			m_playerLogic = p_logic;
			m_gameZoneLeft = p_zoneLeft;
			m_gameZoneRight = p_zoneRight;
			m_conveyorPosY = base.transform.InverseTransformPoint(m_playerLogic.ConveyorWorldPosition).y;
		}

		protected void Start()
		{
			m_startingPos = base.transform.localPosition;
			Reset();
		}

		public void Reset()
		{
			base.transform.localPosition = m_startingPos;
			m_animator.Play("mg_ss_player_jump_bottom", 0, 1f);
			base.transform.localScale = new Vector2(1f, 1f);
			m_shadow.gameObject.SetActive(true);
			UpdateShadow();
		}

		public void UpdatePosY(float p_deltaTime)
		{
			Vector2 v = base.transform.localPosition;
			v.y += m_playerLogic.Velocity * p_deltaTime;
			base.transform.localPosition = v;
			if (m_playerLogic.State == mg_ss_EPlayerState.FALLING)
			{
				m_playerLogic.ConveyorCollidedWith = (v.y.CompareTo(m_conveyorPosY) < 0);
			}
			UpdateShadow();
		}

		public void UpdatePosX(float p_deltaTime, Vector2 steering)
		{
			if (steering.x > 0f)
			{
				UpdateDirectionScale(1);
			}
			else
			{
				UpdateDirectionScale(-1);
			}
			Vector2 v = base.transform.position;
			v.x += steering.x * p_deltaTime * KeyboardHorizonatlSpeedModifier;
			CheckOutOfZone(v.x);
			base.transform.position = v;
		}

		public void UpdatePosX(float p_deltaTime)
		{
			Vector2 v = base.transform.position;
			float num = 5.95f * p_deltaTime;
			if (Mathf.Abs(m_playerLogic.WorldTouch.x - v.x) < num)
			{
				v.x = m_playerLogic.WorldTouch.x;
			}
			else if (v.x < m_playerLogic.WorldTouch.x)
			{
				v.x += num;
				v.x = Mathf.Min(m_playerLogic.WorldTouch.x, v.x);
				UpdateDirectionScale(1);
			}
			else
			{
				v.x -= num;
				v.x = Mathf.Max(m_playerLogic.WorldTouch.x, v.x);
				UpdateDirectionScale(-1);
			}
			v.x = CheckOutOfZone(v.x);
			base.transform.position = v;
		}

		private void UpdateShadow()
		{
			Vector2 v = m_shadow.localPosition;
			v.x = base.transform.localPosition.x;
			m_shadow.localPosition = v;
			float num = base.transform.localPosition.y - m_conveyorPosY;
			float num2 = Mathf.Max(0f, 1f - num / 4f);
			Vector2 v2 = m_shadow.localScale;
			v2.x = num2;
			v2.y = num2;
			m_shadow.localScale = v2;
		}

		public void UpdateDyingScale(float p_deltaTime)
		{
			m_dyingTimer += p_deltaTime;
			float num = 1f + m_dyingTimer / 0.7f * 1f;
			bool flag = base.transform.localScale.x < 0f;
			Vector2 v = base.transform.localScale;
			v.y = num;
			v.x = num;
			if (flag)
			{
				v.x *= -1f;
			}
			base.transform.localScale = v;
			float num2 = m_dyingTimer - 0.5f;
			if (num2 >= 0f)
			{
				Vector2 v2 = base.transform.localPosition;
				v2.y -= p_deltaTime / 0.199999988f * 3f;
				base.transform.localPosition = v2;
			}
		}

		private float CheckOutOfZone(float p_newPosX)
		{
			float num = p_newPosX;
			if (num < m_gameZoneLeft.position.x)
			{
				num = m_gameZoneLeft.position.x;
			}
			else if (num > m_gameZoneRight.position.x)
			{
				num = m_gameZoneRight.position.x;
			}
			return num;
		}

		private void UpdateDirectionScale(int p_scale)
		{
			Vector2 v = base.transform.localScale;
			v.x = p_scale;
			base.transform.localScale = v;
		}

		public void SetFalling()
		{
			if (m_playerLogic.Action == mg_ss_EPlayerAction.SMASHING)
			{
				m_animator.Play("mg_ss_player_smash_top");
			}
			else
			{
				m_animator.Play("mg_ss_player_jump_top");
			}
		}

		public void SetCollision()
		{
			if (m_playerLogic.Action == mg_ss_EPlayerAction.SMASHING)
			{
				m_animator.Play("mg_ss_player_smash_bottom");
			}
			else
			{
				m_animator.Play("mg_ss_player_jump_bottom");
			}
		}

		public void SetDying()
		{
			m_animator.Play("mg_ss_player_die");
			m_dyingTimer = 0f;
			m_shadow.gameObject.SetActive(false);
		}

		protected void OnTriggerEnter2D(Collider2D p_other)
		{
			m_playerLogic.OnTriggerEnter2D(p_other);
		}

		protected void OnTriggerStay2D(Collider2D p_other)
		{
			m_playerLogic.OnTriggerStay2D(p_other);
		}

		private void OnJumpBottonEvent()
		{
			m_playerLogic.CollisionAnimFinished();
		}

		private void OnDieFinishedEvent()
		{
			m_playerLogic.FinishedDying();
		}

		private void OnSmashBottomEvent()
		{
			m_animator.Play("mg_ss_player_jump_bottom", 0, 0.5f);
			m_playerLogic.CollisionAnimFinished();
		}

		public void PlayToeStub()
		{
			m_animator.Play("mg_ss_player_toestub");
		}
	}
}
