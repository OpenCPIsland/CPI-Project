using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class NotificationContainer : MonoBehaviour
	{
		private void Start()
		{
			Service.Get<TrayNotificationManager>().SetParentRectTransform((RectTransform)base.transform);
		}
	}
}
