using ClubPenguin.UI;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class DismissNotificationAction : FsmStateAction
	{
		public bool dismissAll = false;

		public override void OnEnter()
		{
			if (dismissAll)
			{
				Service.Get<TrayNotificationManager>().DismissAllNotifications();
			}
			else
			{
				Service.Get<TrayNotificationManager>().DismissCurrentNotification();
			}
			Finish();
		}
	}
}
