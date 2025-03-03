using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_FishingRod : MonoBehaviour
	{
		private mg_if_GameLogic m_logic;

		private mg_if_FishingHook m_hook;

		private mg_if_FishingLine m_line;

		private mg_if_Penguin m_penguin;

		private mg_if_WormCan m_wormCan;

		private Camera m_mainCamera;

		private Vector2 m_destination;

		private float m_touchDistance;

		private float m_shockTimeRemaining;

		public mg_if_GameLogic Logic
		{
			get
			{
				return m_logic;
			}
		}

		public bool IsHookAboveWater
		{
			get
			{
				return m_hook.IsAboveWater;
			}
		}

		public bool IsBroken
		{
			get
			{
				return m_hook.IsBroken;
			}
		}

		public Transform HookTransform
		{
			get
			{
				return m_hook.transform;
			}
		}

		public void Start()
		{
			m_logic = MinigameManager.GetActive<mg_IceFishing>().Logic;
			m_hook = GetComponentInChildren<mg_if_FishingHook>();
			m_line = GetComponentInChildren<mg_if_FishingLine>();
			m_penguin = GetComponentInChildren<mg_if_Penguin>();
			m_wormCan = GetComponentInChildren<mg_if_WormCan>();
			m_mainCamera = MinigameManager.GetActive<mg_IceFishing>().MainCamera;
			m_destination = new Vector2(0f, -4f);
			m_touchDistance = m_hook.GetComponentInChildren<BoxCollider2D>().size.y * 1.5f;
			m_hook.Initialize(this, GetComponentInChildren<mg_if_WormDrop>());
			m_line.Initialize(this);
			m_penguin.Initialize(this);
			m_wormCan.UpdateWorms(m_logic.Lives);
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			m_hook.MinigameUpdate(p_deltaTime, m_destination);
			m_line.MinigameUpdate(p_deltaTime);
			m_penguin.MinigameUpdate(p_deltaTime);
			if (m_hook.State == mg_if_EHookState.SHOCKED)
			{
				m_shockTimeRemaining -= p_deltaTime;
				if (m_shockTimeRemaining <= 0f)
				{
					StopShockingRod();
				}
			}
		}

		internal void MoveLineTo(Vector3 screenPosition)
		{
			Vector2 vector = m_destination = CalculateDestination(screenPosition);
		}

		public void OnFishCaught()
		{
			m_logic.OnFishCaught();
		}

		private Vector2 CalculateDestination(Vector2 p_position)
		{
			Vector2 v = m_mainCamera.ScreenToWorldPoint(p_position);
			return base.transform.InverseTransformPoint(v);
		}

		public void ReleaseFish()
		{
			m_hook.ReleaseFish();
		}

		private void HookWorm()
		{
			m_wormCan.UpdateWorms(m_logic.Lives);
			m_hook.BaitHook();
		}

		public void ShockRod()
		{
			mg_IceFishing active = MinigameManager.GetActive<mg_IceFishing>();
			active.PlaySFX("mg_if_sfx_JellyfishElectrify");
			m_shockTimeRemaining = active.Resources.Variables.ShockTime;
			m_hook.Shock();
			m_line.Shock();
			m_penguin.Shock();
			m_logic.LoseLife();
		}

		private void StopShockingRod()
		{
			m_hook.StopShock();
			m_line.StopShock();
			m_penguin.StopShock();
		}

		public void CutLine(float p_posY)
		{
			m_destination.y = p_posY;
			m_destination = base.transform.InverseTransformPoint(m_destination);
			m_hook.DropHook();
			m_logic.LoseLife();
		}

		public void GainLife()
		{
			m_logic.GainLife();
			m_wormCan.UpdateWorms(m_logic.Lives);
		}

		public void OnTouchDown(Vector2 p_screenPos)
		{
			m_destination = CalculateDestination(p_screenPos);
		}

		public void OnSimpleTap(Vector2 p_screenPos, Transform p_topZone)
		{
			Vector2 destination = CalculateDestination(p_screenPos);
			Vector2 vector = base.transform.InverseTransformPoint(p_topZone.position);
			bool flag = destination.y >= vector.y;
			switch (m_hook.State)
			{
			case mg_if_EHookState.CAUGHT_YELLOW:
			case mg_if_EHookState.CAUGHT_GREY:
				if (flag && m_hook.IsAboveWater)
				{
					m_hook.CatchFish();
				}
				else if (destination.y > m_hook.transform.localPosition.y - m_touchDistance && destination.y < m_hook.transform.localPosition.y + m_touchDistance)
				{
					m_hook.ReleaseFish();
				}
				else
				{
					m_destination = destination;
				}
				break;
			case mg_if_EHookState.NO_HOOK:
			case mg_if_EHookState.NO_WORM:
				if (flag && m_hook.IsAboveWater)
				{
					HookWorm();
				}
				else
				{
					m_destination = destination;
				}
				break;
			default:
				m_destination = destination;
				break;
			}
		}
	}
}
