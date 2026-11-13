using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public abstract class AbstractAdjustForNotification : MonoBehaviour
	{
		private TrayNotificationManager trayNotificationManager;

		protected bool isDown = false;

		protected abstract void start();

		protected abstract void doMoveUp(float height);

		protected abstract void doMoveDown(float height);

		private void Start()
		{
			trayNotificationManager = Service.Get<TrayNotificationManager>();
			TrayNotificationManager obj = trayNotificationManager;
			obj.NotificationStart = (Action<DNotification>)Delegate.Combine(obj.NotificationStart, new Action<DNotification>(onNotificationStart));
			TrayNotificationManager obj2 = trayNotificationManager;
			obj2.NotificationDismissed = (Action<NotificationCompleteEnum, DNotification>)Delegate.Combine(obj2.NotificationDismissed, new Action<NotificationCompleteEnum, DNotification>(onNotificationDismissed));
			start();
			if (trayNotificationManager.IsShowingNotification && trayNotificationManager.DisplayingNotificationData.AdjustRectPositionForNotification && trayNotificationManager.DisplayingNotificationData.Type != DNotification.NotificationType.DailyComplete && trayNotificationManager.DisplayingNotificationData.Type != DNotification.NotificationType.ActivityTracker)
			{
				CoroutineRunner.Start(moveDownDelayed(), this, "");
			}
		}

		private void OnDestroy()
		{
			TrayNotificationManager obj = trayNotificationManager;
			obj.NotificationStart = (Action<DNotification>)Delegate.Remove(obj.NotificationStart, new Action<DNotification>(onNotificationStart));
			TrayNotificationManager obj2 = trayNotificationManager;
			obj2.NotificationDismissed = (Action<NotificationCompleteEnum, DNotification>)Delegate.Remove(obj2.NotificationDismissed, new Action<NotificationCompleteEnum, DNotification>(onNotificationDismissed));
		}

		private void onNotificationStart(DNotification notificationData)
		{
			if (notificationData.AdjustRectPositionForNotification && notificationData.Type != DNotification.NotificationType.DailyComplete && notificationData.Type != DNotification.NotificationType.ActivityTracker)
			{
				moveDown();
			}
		}

		private void onNotificationDismissed(NotificationCompleteEnum complete, DNotification notificationData)
		{
			if (notificationData.AdjustRectPositionForNotification && notificationData.Type != DNotification.NotificationType.DailyComplete && notificationData.Type != DNotification.NotificationType.ActivityTracker)
			{
				moveUp();
			}
		}

		private void moveUp()
		{
			if (isDown)
			{
				isDown = false;
				doMoveUp(trayNotificationManager.NotificationParentTransform.rect.height);
			}
		}

		private void moveDown()
		{
			if (!isDown)
			{
				isDown = true;
				doMoveDown(trayNotificationManager.NotificationParentTransform.rect.height);
			}
		}

		private IEnumerator moveDownDelayed()
		{
			yield return new WaitForEndOfFrame();
			moveDown();
		}
	}
}
