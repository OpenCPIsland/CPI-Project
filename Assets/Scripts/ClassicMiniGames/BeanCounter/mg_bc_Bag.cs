using MinigameFramework;

namespace BeanCounter
{
	public class mg_bc_Bag : mg_bc_FlyingObject
	{
		public override void OnCaught()
		{
			base.OnCaught();
			MinigameManager.GetActive().PlaySFX("mg_bc_sfx_BagCatch");
			base.gameObject.SetActive(false);
			m_state = mg_bc_EObjectState.STATE_HELD;
		}

		protected override void OnHitGround()
		{
			base.OnHitGround();
			MinigameManager.GetActive().PlaySFX("mg_bc_sfx_BagImpactGround");
		}
	}
}
