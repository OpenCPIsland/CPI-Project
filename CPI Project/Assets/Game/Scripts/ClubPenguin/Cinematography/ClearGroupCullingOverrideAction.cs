using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Cinematography
{
	[ActionCategory("Cinematography")]
	public class ClearGroupCullingOverrideAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(CinematographyEvents.ClearGroupCullingOverride));
			Finish();
		}
	}
}
