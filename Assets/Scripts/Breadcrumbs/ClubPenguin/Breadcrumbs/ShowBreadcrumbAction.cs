using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Breadcrumbs
{
	[ActionCategory("UI")]
	public class ShowBreadcrumbAction : FsmStateAction
	{
		public string BreadcrumbId;

		public int BreadcrumbCount = 1;

		public override void OnEnter()
		{
			Service.Get<NotificationBreadcrumbController>().AddBreadcrumb(BreadcrumbId, BreadcrumbCount);
			Finish();
		}
	}
}
