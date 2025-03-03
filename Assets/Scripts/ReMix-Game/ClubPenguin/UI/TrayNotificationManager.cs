using ClubPenguin.CellPhone;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native.iOS;
using System;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class TrayNotificationManager
	{
		public static PrefabContentKey NonDisruptiveNotificationContentKey = new PrefabContentKey("Prefabs/Notifications/NonDisruptiveNotificationPanel");

		public static PrefabContentKey CelebrationNotificationContentKey = new PrefabContentKey("Prefabs/Notifications/CelebrationNotificationPanel");

		public static PrefabContentKey MemberNotificationContentKey = new PrefabContentKey("Prefabs/Notifications/MembershipNotificationPanel");

		public static PrefabContentKey CellPhoneDailyNotificationContentKey = new PrefabContentKey("Prefabs/Notifications/CellPhoneDailyNotification");

		public Action<NotificationCompleteEnum, DNotification> NotificationDismissed;

		public Action<DNotification> NotificationStart;

		private RectTransform notificationParentTransform;

		private CellPhoneNotificationHandler cellPhoneNotificationHandler;

		private Queue<DNotification> notificationQueue;

		private TrayNotification currentNotification;

		private DNotification currentNotificationData;

		private bool persistCurrentNotification = true;

		private bool showingNotification;

		private DNotification displayingNotificationData;

		private bool reshowPreviousNotification;

		private bool isEnabled;

		public RectTransform NotificationParentTransform
		{
			get
			{
				return notificationParentTransform;
			}
		}

		public CellPhoneNotificationHandler CellPhoneNotificationHandler
		{
			get
			{
				return cellPhoneNotificationHandler;
			}
		}

		public bool IsShowingNotification
		{
			get
			{
				return showingNotification;
			}
		}

		public DNotification DisplayingNotificationData
		{
			get
			{
				return displayingNotificationData;
			}
		}

		public TrayNotificationManager()
		{
			notificationQueue = new Queue<DNotification>();
			showingNotification = false;
			isEnabled = true;
			Service.Get<EventDispatcher>().AddListener<SceneTransitionEvents.TransitionComplete>(onSceneTransitionComplete);
		}

		public void SetParentRectTransform(RectTransform notificationParentTransform)
		{
			this.notificationParentTransform = notificationParentTransform;
			if (reshowPreviousNotification)
			{
				reshowPreviousNotification = false;
				showNotification();
			}
		}

		public void SetCellPhoneNotificationHandler(CellPhoneNotificationHandler handler)
		{
			cellPhoneNotificationHandler = handler;
			CellPhoneNotificationHandler obj = cellPhoneNotificationHandler;
			obj.NotificationDismissed = (Action<NotificationCompleteEnum, DNotification>)Delegate.Combine(obj.NotificationDismissed, new Action<NotificationCompleteEnum, DNotification>(notificationComplete));
		}

		public void ShowNotification(DNotification notification)
		{
			if (isEnabled && !(notificationParentTransform == null))
			{
				notificationQueue.Enqueue(notification);
				checkQueue();
			}
		}

		public void DismissCurrentNotification(bool playAnimation = true)
		{
			if (currentNotification != null)
			{
				currentNotification.Dismiss(playAnimation);
			}
			else if (cellPhoneNotificationHandler != null)
			{
				cellPhoneNotificationHandler.DismissNotification(playAnimation);
			}
		}

		public void DismissAllNotifications(bool playAnimation = true)
		{
			DismissCurrentNotification(playAnimation);
			notificationQueue.Clear();
		}

		public void HideNotification()
		{
			if (currentNotification != null)
			{
				currentNotification.gameObject.SetActive(false);
			}
		}

		public void UnhideNotification()
		{
			if (currentNotification != null)
			{
				currentNotification.gameObject.SetActive(true);
			}
		}

		private void checkQueue()
		{
			if (!showingNotification && notificationQueue.Count > 0)
			{
				showingNotification = true;
				displayingNotificationData = notificationQueue.Dequeue();
				persistCurrentNotification = displayingNotificationData.PersistBetweenScenes;
				showNotification();
			}
		}

		private void showNotification()
		{
			bool flag = false;
			if (displayingNotificationData.Type == DNotification.NotificationType.DailyComplete || displayingNotificationData.Type == DNotification.NotificationType.ActivityTracker)
			{
				GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_HUD);
				if (gameObject != null)
				{
					CellPhoneHud componentInChildren = gameObject.GetComponentInChildren<CellPhoneHud>();
					if (componentInChildren != null && componentInChildren.gameObject.activeInHierarchy && !Service.Get<UIElementDisablerManager>().IsUIElementDisabled("CellphoneButton"))
					{
						cellPhoneNotificationHandler.ShowNotification(displayingNotificationData);
						Service.Get<iOSHapticFeedback>().TriggerNotificationFeedback(iOSHapticFeedback.NotificationFeedbackType.Success);
						flag = true;
					}
				}
			}
			if (!flag)
			{
				ShowTrayNotificationCommand showTrayNotificationCommand = new ShowTrayNotificationCommand(displayingNotificationData, notificationParentTransform);
				showTrayNotificationCommand.NotificationCreated = (Action<TrayNotification>)Delegate.Combine(showTrayNotificationCommand.NotificationCreated, new Action<TrayNotification>(onNotificationCreated));
				showTrayNotificationCommand.NotificationDismissed = (Action<NotificationCompleteEnum, DNotification>)Delegate.Combine(showTrayNotificationCommand.NotificationDismissed, new Action<NotificationCompleteEnum, DNotification>(notificationComplete));
				showTrayNotificationCommand.Execute();
				if (NotificationStart != null)
				{
					NotificationStart(displayingNotificationData);
				}
			}
		}

		private void onNotificationCreated(TrayNotification notification)
		{
			currentNotification = notification;
		}

		private void notificationComplete(NotificationCompleteEnum notificationOutcome, DNotification notificationData)
		{
			currentNotification = null;
			if (notificationOutcome == NotificationCompleteEnum.leftRoom && persistCurrentNotification)
			{
				reshowPreviousNotification = true;
				return;
			}
			if (NotificationDismissed != null)
			{
				NotificationDismissed(notificationOutcome, notificationData);
			}
			showingNotification = false;
			checkQueue();
		}

		private bool onSceneTransitionComplete(SceneTransitionEvents.TransitionComplete evt)
		{
			if (reshowPreviousNotification)
			{
				reshowPreviousNotification = false;
				showNotification();
			}
			return false;
		}

		[Invokable("UI.Notifications.ShowTestNotification")]
		public static void ShowTestNotification(string message = "Test message", float delayTime = 5f, bool containsButtons = false, DNotification.NotificationType type = DNotification.NotificationType.Generic)
		{
			DNotification dNotification = new DNotification();
			dNotification.ContainsButtons = containsButtons;
			dNotification.Message = message;
			dNotification.PopUpDelayTime = delayTime;
			dNotification.Type = type;
			if (containsButtons)
			{
				TrayNotificationManager trayNotificationManager = Service.Get<TrayNotificationManager>();
				trayNotificationManager.NotificationDismissed = (Action<NotificationCompleteEnum, DNotification>)Delegate.Combine(trayNotificationManager.NotificationDismissed, new Action<NotificationCompleteEnum, DNotification>(onNotificationDismissed));
			}
			Service.Get<TrayNotificationManager>().ShowNotification(dNotification);
		}

		private static void onNotificationDismissed(NotificationCompleteEnum notificationOutcome, DNotification notification)
		{
			TrayNotificationManager trayNotificationManager = Service.Get<TrayNotificationManager>();
			trayNotificationManager.NotificationDismissed = (Action<NotificationCompleteEnum, DNotification>)Delegate.Remove(trayNotificationManager.NotificationDismissed, new Action<NotificationCompleteEnum, DNotification>(onNotificationDismissed));
			DNotification dNotification = new DNotification();
			dNotification.PopUpDelayTime = 5f;
			switch (notificationOutcome)
			{
			case NotificationCompleteEnum.acceptButton:
				dNotification.Message = "Accepted";
				Service.Get<TrayNotificationManager>().ShowNotification(dNotification);
				break;
			case NotificationCompleteEnum.declineButton:
				dNotification.Message = "Declined";
				Service.Get<TrayNotificationManager>().ShowNotification(dNotification);
				break;
			}
		}
	}
}
