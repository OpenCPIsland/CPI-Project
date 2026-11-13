using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Cinematography
{
	[Note("Sets a list of culling groups to always be on instead of relying on their switches")]
	[ActionCategory("Cinematography")]
	public class GroupCullingOverrideAction : FsmStateAction
	{
		public string[] GroupNames;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new CinematographyEvents.SetGroupCullingOverride(GroupNames));
			Finish();
		}
	}
}
