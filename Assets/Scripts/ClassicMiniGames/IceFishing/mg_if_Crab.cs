using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_Crab : mg_if_GameObject
	{
		private static string ANIM_STATE = "state";

		private Animator m_animator;

		private mg_if_XMovement m_XMovement;

		private mg_if_FishingRod m_fishingRod;

		private Transform m_checkPoint1;

		private Transform m_checkPoint2;

		private mg_if_EObjectsMovement m_movement;

		private float m_waitTime;

		private mg_if_ECrabState m_crabState;

		private mg_if_ECrabState CrabState
		{
			get
			{
				return m_crabState;
			}
			set
			{
				m_crabState = value;
				m_animator.SetInteger(ANIM_STATE, (int)value);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			m_animator = GetComponentInChildren<Animator>();
			m_XMovement = base.gameObject.GetComponent<mg_if_XMovement>();
			m_fishingRod = MinigameManager.GetActive<mg_IceFishing>().Logic.FishingRod;
		}

		public void Initialize(mg_if_EObjectsMovement p_movement)
		{
			m_movement = p_movement;
			mg_if_GameLogic logic = MinigameManager.GetActive<mg_IceFishing>().Logic;
			Vector2 v = base.transform.position;
			v.y = logic.TopZone.position.y;
			base.transform.position = v;
			GameObject gameObject = null;
			gameObject = ((p_movement != mg_if_EObjectsMovement.MOVEMENT_LEFT) ? logic.CrabStopLeft : logic.CrabStopRight);
			m_checkPoint1 = gameObject.transform.Find("mg_if_stop_1");
			m_checkPoint2 = gameObject.transform.Find("mg_if_stop_2");
		}

		public override void Spawn()
		{
			base.Spawn();
			m_XMovement.Initialize(m_movement, m_variables.CrabSpeed, m_variables.CrabSpeedRange);
			m_XMovement.SetInitialPos();
			CrabState = mg_if_ECrabState.STATE_ENTERING_1;
			m_waitTime = m_variables.CrabWaitTime;
			MinigameManager.GetActive().PlaySFX("mg_if_sfx_CrabWalk");
		}

		public override void MinigameUpdate(float p_deltaTime)
		{
			base.MinigameUpdate(p_deltaTime);
			switch (CrabState)
			{
			case mg_if_ECrabState.STATE_ENTERING_1:
				UpdateEntering(p_deltaTime, m_checkPoint1, mg_if_ECrabState.STATE_WAIT);
				break;
			case mg_if_ECrabState.STATE_WAIT:
				UpdateWaiting(p_deltaTime);
				break;
			case mg_if_ECrabState.STATE_ENTERING_2:
				UpdateEntering(p_deltaTime, m_checkPoint2, mg_if_ECrabState.STATE_CUTTING);
				break;
			case mg_if_ECrabState.STATE_EXITING:
				UpdateExiting(p_deltaTime);
				break;
			}
		}

		private void UpdateEntering(float p_deltaTime, Transform p_checkPoint, mg_if_ECrabState p_newState)
		{
			m_XMovement.MinigameUpdate(p_deltaTime);
			if (CheckAtCheckPoint(p_checkPoint.position.x))
			{
				CrabState = p_newState;
			}
		}

		private void UpdateWaiting(float p_deltaTime)
		{
			m_waitTime -= p_deltaTime;
			if (m_waitTime <= 0f)
			{
				CrabState = mg_if_ECrabState.STATE_BLINKING;
			}
		}

		private void UpdateExiting(float p_deltaTime)
		{
			m_XMovement.MinigameUpdate(p_deltaTime);
			if (m_XMovement.CheckOffEdge())
			{
				Despawn();
			}
		}

		private bool CheckAtCheckPoint(float p_checkPointX)
		{
			float num = 1f;
			if (m_XMovement.Movement == mg_if_EObjectsMovement.MOVEMENT_RIGHT)
			{
				num = -1f;
			}
			float num2 = base.transform.position.x - p_checkPointX;
			bool result = false;
			if (num2 * num <= 0f)
			{
				result = true;
			}
			return result;
		}

		public void OnBlinkAnimFinished()
		{
			CrabState = mg_if_ECrabState.STATE_ENTERING_2;
		}

		public void OnCutAnimFrame()
		{
			MinigameManager.GetActive().PlaySFX("mg_if_sfx_CrabPincer");
			if (!m_fishingRod.IsBroken && m_fishingRod.HookTransform.position.y <= base.transform.position.y)
			{
				MinigameManager.GetActive().PlaySFX("mg_if_sfx_CrabCutLine");
				m_fishingRod.CutLine(base.transform.position.y);
			}
		}

		public void OnCutAnimFinished()
		{
			CrabState = mg_if_ECrabState.STATE_EXITING;
			m_XMovement.SwapMovement();
		}
	}
}
