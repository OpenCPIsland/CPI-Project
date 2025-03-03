using JetpackReboot;
using MinigameFramework;

namespace JetpacReboot
{
	public class ReturnToPool : mg_jr_DestroyOnEvent
	{
		public override void DestroyThisGameObject()
		{
			mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
			active.Resources.ReturnPooledResource(base.gameObject);
		}
	}
}
