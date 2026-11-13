using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Breadcrumbs
{
	[ActionCategory("UI")]
	public class HideBreadcrumbAction : FsmStateAction
	{
		public string BreadcrumbId;

		public int BreadcrumbCount = 1;

		public bool ClearAll = false;

		public override void OnEnter()
		{
			if (ClearAll)
			{
				Service.Get<NotificationBreadcrumbController>().ResetBreadcrumbs(BreadcrumbId);
			}
			else
			{
				Service.Get<NotificationBreadcrumbController>().RemoveBreadcrumb(BreadcrumbId, BreadcrumbCount);
			}
			Finish();
		}
	}
}
