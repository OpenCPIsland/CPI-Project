using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.UI.Example
{
	public class TrayNotification_Example : MonoBehaviour
	{
		public GameObject GUITray;

		public RectTransform NotificationParent;

		private TrayNotificationManager manager;

		private DNotification noButtonNotification;

		private DNotification buttonsNotification;

		private int noteCount = 0;

		private void Start()
		{
			setExampleData();
			manager = Service.Get<TrayNotificationManager>();
			manager.SetParentRectTransform(NotificationParent);
			GUITray.SetActive(false);
		}

		private void setExampleData()
		{
			noButtonNotification = new DNotification();
			noButtonNotification.Message = "Test notification without buttons.";
			noButtonNotification.ContainsButtons = false;
			noButtonNotification.PopUpDelayTime = 5f;
			buttonsNotification = new DNotification();
			buttonsNotification.Message = "Test notification with buttons!";
			buttonsNotification.ContainsButtons = true;
			buttonsNotification.PopUpDelayTime = 10f;
		}

		public void ShowNoButtonsNotification()
		{
			GUITray.SetActive(true);
			noteCount = 1;
			TrayNotificationManager trayNotificationManager = manager;
			trayNotificationManager.NotificationDismissed = (Action<NotificationCompleteEnum, DNotification>)Delegate.Combine(trayNotificationManager.NotificationDismissed, new Action<NotificationCompleteEnum, DNotification>(notificationButtonPressed));
			manager.ShowNotification(noButtonNotification);
		}

		public void ShowButtonsNotification()
		{
			GUITray.SetActive(true);
			noteCount = 1;
			TrayNotificationManager trayNotificationManager = manager;
			trayNotificationManager.NotificationDismissed = (Action<NotificationCompleteEnum, DNotification>)Delegate.Combine(trayNotificationManager.NotificationDismissed, new Action<NotificationCompleteEnum, DNotification>(notificationButtonPressed));
			manager.ShowNotification(buttonsNotification);
		}

		public void Stack2Notifications()
		{
			GUITray.SetActive(true);
			noteCount = 2;
			TrayNotificationManager trayNotificationManager = manager;
			trayNotificationManager.NotificationDismissed = (Action<NotificationCompleteEnum, DNotification>)Delegate.Combine(trayNotificationManager.NotificationDismissed, new Action<NotificationCompleteEnum, DNotification>(notificationButtonPressed));
			manager.ShowNotification(noButtonNotification);
			manager.ShowNotification(buttonsNotification);
		}

		private void notificationButtonPressed(NotificationCompleteEnum type, DNotification data)
		{
			noteCount--;
			if (noteCount == 0)
			{
				TrayNotificationManager trayNotificationManager = manager;
				trayNotificationManager.NotificationDismissed = (Action<NotificationCompleteEnum, DNotification>)Delegate.Remove(trayNotificationManager.NotificationDismissed, new Action<NotificationCompleteEnum, DNotification>(notificationButtonPressed));
				GUITray.SetActive(false);
			}
		}
	}
}
