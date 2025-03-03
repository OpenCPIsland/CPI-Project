using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Breadcrumbs
{
	[ActionCategory("UI")]
	public class HideTutorialBreadcrumbAction : FsmStateAction
	{
		public string BreadcrumbId;

		public override void OnEnter()
		{
			Service.Get<TutorialBreadcrumbController>().RemoveBreadcrumb(BreadcrumbId);
			Finish();
		}
	}
}
