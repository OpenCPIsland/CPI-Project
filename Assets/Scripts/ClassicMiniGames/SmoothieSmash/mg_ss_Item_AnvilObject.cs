using MinigameFramework;

namespace SmoothieSmash
{
	public class mg_ss_Item_AnvilObject : mg_ss_ItemObject
	{
		public override bool ToeStub
		{
			get
			{
				return true;
			}
		}

		public override void PlayBounceSFX()
		{
			MinigameManager.GetActive().PlaySFX("mg_ss_sfx_anvil_bounce");
		}

		public override void OnCollided(mg_ss_EPlayerAction p_playerAction)
		{
			m_logic.ComboReset();
			base.OnCollided(p_playerAction);
			m_logic.OnAnvilCollision(this);
			MinigameManager.GetActive().PlaySFX("mg_ss_sfx_anvil_impact");
		}
	}
}
