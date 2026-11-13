using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Breadcrumbs
{
	[ActionCategory("UI")]
	public class ShowTutorialBreadcrumbAction : FsmStateAction
	{
		public string[] BreadcrumbPath;

		public override void OnEnter()
		{
			TutorialBreadcrumbPath path = new TutorialBreadcrumbPath(BreadcrumbPath);
			Service.Get<TutorialBreadcrumbController>().AddBreadcrumb(path);
			Finish();
		}
	}
}
