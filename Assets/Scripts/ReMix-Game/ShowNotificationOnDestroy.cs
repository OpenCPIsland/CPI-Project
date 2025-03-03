using ClubPenguin.UI;
using Disney.MobileNetwork;
using UnityEngine;

public class ShowNotificationOnDestroy : MonoBehaviour
{
	public string NotificationText;

	public void OnDestroy()
	{
		DNotification dNotification = new DNotification();
		dNotification.PrefabLocation = TrayNotificationManager.NonDisruptiveNotificationContentKey;
		dNotification.Message = NotificationText;
		dNotification.ContainsButtons = false;
		dNotification.PopUpDelayTime = 3f;
		Service.Get<TrayNotificationManager>().ShowNotification(dNotification);
	}
}
