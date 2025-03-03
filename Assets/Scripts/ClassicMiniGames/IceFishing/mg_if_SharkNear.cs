using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_SharkNear : mg_if_GameObject
	{
		private mg_if_SpawnManager m_spawnManager;

		private mg_if_XMovement m_XMovement;

		public mg_if_EObjectsMovement Movement
		{
			get
			{
				return m_XMovement.Movement;
			}
		}

		public void Initialize(mg_if_SpawnManager p_spawnManager)
		{
			m_spawnManager = p_spawnManager;
		}

		public override void Spawn()
		{
			base.Spawn();
			m_XMovement = base.gameObject.GetComponent<mg_if_XMovement>();
			m_XMovement.Initialize(mg_if_EObjectsMovement.MOVEMENT_AUTO, m_variables.SharkNearSpeed, m_variables.SharkNearSpeedRange);
			m_XMovement.SetInitialPos();
			Vector2 v = base.transform.position;
			v.y = 0f - MinigameManager.GetActive<mg_IceFishing>().MainCamera.orthographicSize;
			base.transform.position = v;
			MinigameManager.GetActive().PlaySFX("mg_if_sfx_SharkSwim");
		}

		public override void MinigameUpdate(float p_deltaTime)
		{
			m_XMovement.MinigameUpdate(p_deltaTime);
			if (m_XMovement.CheckOffEdge())
			{
				Despawn();
				mg_if_EObjectsMovement p_movement = mg_if_EObjectsMovement.MOVEMENT_LEFT;
				if (m_XMovement.Movement == mg_if_EObjectsMovement.MOVEMENT_LEFT)
				{
					p_movement = mg_if_EObjectsMovement.MOVEMENT_RIGHT;
				}
				m_spawnManager.SpawnShark(p_movement);
			}
		}
	}
}
