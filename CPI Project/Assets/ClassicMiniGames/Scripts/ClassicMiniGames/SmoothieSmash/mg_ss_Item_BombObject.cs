using MinigameFramework;

namespace SmoothieSmash
{
	public class mg_ss_Item_BombObject : mg_ss_ItemObject
	{
		private MinigameSFX m_fuseSFX;

		protected void Start()
		{
			m_fuseSFX = MinigameManager.GetActive().PlaySFX("mg_ss_sfx_bomb_fuse");
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			StopFuseSFX();
		}

		public override void OnCollided(mg_ss_EPlayerAction p_playerAction)
		{
			m_logic.ComboReset();
			base.OnCollided(p_playerAction);
			base.Animation = mg_ss_EItemAnimation.COLLIDED;
			m_logic.OnBombCollision(this);
			StopFuseSFX();
			MinigameManager.GetActive().PlaySFX("mg_ss_sfx_bomb_impact");
		}

		private void StopFuseSFX()
		{
			if (m_fuseSFX != null)
			{
				m_fuseSFX.Stop();
				m_fuseSFX = null;
			}
		}
	}
}
