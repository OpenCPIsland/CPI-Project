using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_Fish : mg_if_GameObject
	{
		private mg_if_XMovement m_XMovement;

		private mg_if_YMovement m_YMovement;

		private Vector2 m_boundSize;

		private mg_if_FishingHook m_hook;

		private Vector2 m_releaseStart;

		private Vector2 m_releaseEnd;

		private float m_releaseTime;

		public mg_if_EFishState FishState
		{
			get;
			private set;
		}

		protected override void Awake()
		{
			base.Awake();
			m_XMovement = GetComponent<mg_if_XMovement>();
			m_YMovement = GetComponent<mg_if_YMovement>();
		}

		protected override void Start()
		{
			m_boundSize = GetComponentInChildren<Renderer>().bounds.size;
			base.Start();
		}

		private void reset()
		{
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			eulerAngles.z = 0f;
			base.transform.rotation = Quaternion.Euler(eulerAngles);
			FishState = mg_if_EFishState.STATE_SWIMMING;
			m_hook = null;
		}

		public override void Spawn()
		{
			base.Spawn();
			reset();
			m_XMovement.Initialize(mg_if_EObjectsMovement.MOVEMENT_AUTO, m_variables.FishSpeedX, m_variables.FishSpeedRangeX);
			m_XMovement.SetInitialPos();
			m_YMovement.Initialize(m_variables.FishSpeedY, m_variables.FishSpeedRangeY);
			m_YMovement.SetInitialPos();
		}

		public override void MinigameUpdate(float p_deltaTime)
		{
			base.MinigameUpdate(p_deltaTime);
			switch (FishState)
			{
			case mg_if_EFishState.STATE_SWIMMING:
				UpdateSwimming(p_deltaTime);
				break;
			case mg_if_EFishState.STATE_HOOKED:
				UpdateHooked();
				break;
			case mg_if_EFishState.STATE_RELEASED:
				UpdateReleased(p_deltaTime);
				break;
			}
		}

		private void UpdateSwimming(float p_deltaTime)
		{
			m_XMovement.MinigameUpdate(p_deltaTime);
			m_YMovement.MinigameUpdate(p_deltaTime);
			if (m_XMovement.CheckOffEdge())
			{
				Despawn();
			}
		}

		private void UpdateHooked()
		{
			Vector2 v = base.transform.position;
			v.x = m_hook.transform.position.x - m_boundSize.y * 0.05f * base.transform.localScale.x;
			v.y = m_hook.transform.position.y - m_boundSize.x * 0.53f;
			base.transform.position = v;
		}

		private void UpdateReleased(float p_deltaTime)
		{
			m_releaseTime += p_deltaTime;
			float t = m_releaseTime / m_variables.FishReleaseTime;
			base.transform.position = Vector2.Lerp(m_releaseStart, m_releaseEnd, t);
			if (m_releaseTime > m_variables.FishReleaseTime)
			{
				Despawn();
			}
		}

		public void OnHooked(mg_if_FishingHook p_hook)
		{
			MinigameManager.GetActive().PlaySFX("mg_if_sfx_CatchFish");
			FishState = mg_if_EFishState.STATE_HOOKED;
			m_hook = p_hook;
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			eulerAngles.z = 90f * base.transform.localScale.x;
			base.transform.rotation = Quaternion.Euler(eulerAngles);
			UpdateHooked();
		}

		public void OnCaught()
		{
			Despawn();
		}

		public void OnReleased()
		{
			MinigameManager.GetActive().PlaySFX("mg_if_sfx_FishDrop");
			FishState = mg_if_EFishState.STATE_RELEASED;
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			eulerAngles.z *= -1f;
			base.transform.rotation = Quaternion.Euler(eulerAngles);
			m_releaseTime = 0f;
			m_releaseStart = base.transform.position;
			m_releaseEnd = new Vector2(m_releaseStart.x, m_releaseStart.y - MinigameManager.GetActive<mg_IceFishing>().MainCamera.orthographicSize);
		}
	}
}
