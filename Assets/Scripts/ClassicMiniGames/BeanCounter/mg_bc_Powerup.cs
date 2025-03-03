using MinigameFramework;

namespace BeanCounter
{
	public class mg_bc_Powerup : mg_bc_FlyingObject
	{
		public mg_bc_EPowerupType PowerupType;

		protected override void OnHitGround()
		{
			base.OnHitGround();
			m_state = mg_bc_EObjectState.STATE_TO_DESTROY;
		}

		public override void OnCaught()
		{
			base.OnCaught();
			string name = "";
			switch (PowerupType)
			{
			case mg_bc_EPowerupType.EXTRA_LIFE:
				name = "mg_bc_sfx_BeanCounters_1UP";
				break;
			case mg_bc_EPowerupType.INVINCIBILITY:
				name = "mg_bc_sfx_ShieldPowerUp";
				break;
			}
			MinigameManager.GetActive().PlaySFX(name);
		}
	}
}
