using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_SharkFar : mg_if_ObstacleHook
	{
		private mg_if_XMovement m_XMovement;

		private mg_if_YMovement m_YMovement;

		private mg_if_FishingRod m_fishingRod;

		private GameObject m_mouthOpen;

		private GameObject m_mouthClosed;

		protected override void Awake()
		{
			base.Awake();
			m_mouthOpen = base.transform.Find("mouth_open").gameObject;
			m_mouthClosed = base.transform.Find("mouth_closed").gameObject;
			m_XMovement = base.gameObject.GetComponent<mg_if_XMovement>();
			m_YMovement = base.gameObject.GetComponent<mg_if_YMovement>();
			m_fishingRod = MinigameManager.GetActive<mg_IceFishing>().Logic.FishingRod;
		}

		public void Initialize(mg_if_EObjectsMovement p_movement)
		{
			GameObject gameObject = base.transform.Find("object_size").gameObject;
			gameObject.SetActive(true);
			m_XMovement.Initialize(p_movement, m_variables.SharkFarSpeed, m_variables.SharkFarSpeedRange);
			m_YMovement.Initialize(0f, 0f);
			gameObject.SetActive(false);
			SetMouthState(false);
			m_XMovement.SetInitialPos();
			m_YMovement.SetInitialPos();
		}

		public override void Spawn()
		{
			base.Spawn();
			MinigameManager.GetActive().PlaySFX("mg_if_sfx_SharkSwim");
		}

		public override void MinigameUpdate(float p_deltaTime)
		{
			m_XMovement.MinigameUpdate(p_deltaTime);
			m_YMovement.MinigameUpdate(p_deltaTime);
			CalculateMouthState();
			if (m_XMovement.CheckOffEdge())
			{
				Despawn();
			}
		}

		private void CalculateMouthState()
		{
			bool mouthState = false;
			if (!m_fishingRod.IsBroken && !m_fishingRod.IsHookAboveWater && MovingTowardsHook())
			{
				float num = Vector2.Distance(m_fishingRod.HookTransform.position, base.transform.position);
				if (num <= m_variables.SharkFarMouthOpenDistance)
				{
					mouthState = true;
				}
			}
			SetMouthState(mouthState);
		}

		private bool MovingTowardsHook()
		{
			float num = 1f;
			if (m_XMovement.Movement == mg_if_EObjectsMovement.MOVEMENT_RIGHT)
			{
				num *= -1f;
			}
			return (m_fishingRod.HookTransform.position.x - base.transform.position.x) * num <= 0f;
		}

		public override void OnObstacleHitHook(mg_if_FishingRod p_rod)
		{
			MinigameManager.GetActive().PlaySFX("mg_if_sfx_SharkBite");
			p_rod.CutLine(base.transform.position.y);
		}

		private void SetMouthState(bool p_mouthOpen)
		{
			m_mouthOpen.SetActive(p_mouthOpen);
			m_mouthClosed.SetActive(!p_mouthOpen);
		}
	}
}
