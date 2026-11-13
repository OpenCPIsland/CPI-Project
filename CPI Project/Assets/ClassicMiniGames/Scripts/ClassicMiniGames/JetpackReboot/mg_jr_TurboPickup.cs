using MinigameFramework;

namespace JetpackReboot
{
	public class mg_jr_TurboPickup : mg_jr_Collectable
	{
		public override void OnCollection()
		{
			mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
			active.Resources.ReturnPooledResource(base.gameObject);
			active.PlaySFX(mg_jr_Sound.PICKUP_JETPACK.ClipName());
		}
	}
}
