using MinigameFramework;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_Item_GoldenAppleObject : mg_ss_ItemObject
	{
		public override bool PowerUp
		{
			get
			{
				return true;
			}
		}

		public override void Initialize(mg_ss_EItemTypes p_itemType, mg_ss_IItemMovement p_movement, Vector2 p_spawnPointBottom, Vector2 p_spawnPointTop, float p_screenWidth, bool p_chaosItem)
		{
			base.Initialize(p_itemType, p_movement, p_spawnPointBottom, p_spawnPointTop, p_screenWidth, p_chaosItem);
			SetInitialPosition(p_spawnPointTop);
		}

		public override void OnCollided(mg_ss_EPlayerAction p_playerAction)
		{
			MinigameManager.GetActive().PlaySFX("mg_ss_sfx_golden_apple_collect");
			base.OnCollided(p_playerAction);
			m_logic.OnGoldenAppleCollision(this);
			Camera mainCamera = MinigameManager.GetActive().MainCamera;
			Vector2 p_targetPosition = new Vector2(0f - mainCamera.aspect * mainCamera.orthographicSize, mainCamera.orthographicSize);
			p_targetPosition.x /= base.transform.lossyScale.x;
			p_targetPosition.x -= -0.714f;
			p_targetPosition.y /= base.transform.lossyScale.y;
			p_targetPosition.y -= 1.16f;
			m_movement = new mg_ss_ItemMovement_PowerupExit(p_targetPosition);
			m_movement.Initialize(this);
		}

		protected override void CheckOffScreen()
		{
			if (base.Collidable)
			{
				base.CheckOffScreen();
			}
		}
	}
}
