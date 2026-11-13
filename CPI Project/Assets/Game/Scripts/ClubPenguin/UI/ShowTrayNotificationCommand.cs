using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ShowTrayNotificationCommand
	{
		public Action<TrayNotification> NotificationCreated;

		public Action<NotificationCompleteEnum, DNotification> NotificationDismissed;

		private DNotification notificationData;

		private RectTransform notificationParentTransform;

		private TrayNotification notification;

		public ShowTrayNotificationCommand(DNotification notificationData, RectTransform notificationParentTransform)
		{
			this.notificationData = notificationData;
			this.notificationParentTransform = notificationParentTransform;
		}

		public void Execute()
		{
			if (notificationData.WaitForLoading && Service.Get<LoadingController>().IsLoading)
			{
				Service.Get<EventDispatcher>().AddListener<LoadingController.LoadingScreenHiddenEvent>(onLoadingScreenHidden);
			}
			else if (notificationData.ShowAfterSceneLoad)
			{
				Service.Get<EventDispatcher>().AddListener<HudEvents.HudInitComplete>(onHudInitCompleted);
			}
			else
			{
				showNotification();
			}
		}

		private bool onLoadingScreenHidden(LoadingController.LoadingScreenHiddenEvent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<LoadingController.LoadingScreenHiddenEvent>(onLoadingScreenHidden);
			showNotification();
			return false;
		}

		private bool onHudInitCompleted(HudEvents.HudInitComplete evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<HudEvents.HudInitComplete>(onHudInitCompleted);
			showNotification();
			return false;
		}

		private void showNotification()
		{
			CoroutineRunner.Start(loadNotificationPrefab(), this, "loadNotificationPrefab");
		}

		private IEnumerator loadNotificationPrefab()
		{
			PrefabContentKey prefabLocation = notificationData.PrefabLocation;
			if (prefabLocation == null || string.IsNullOrEmpty(prefabLocation.Key))
			{
				prefabLocation = TrayNotificationManager.NonDisruptiveNotificationContentKey;
			}
			AssetRequest<GameObject> asset = Content.LoadAsync(prefabLocation);
			yield return asset;
			GameObject loadedGameObject = UnityEngine.Object.Instantiate(asset.Asset);
			loadedGameObject.transform.SetParent(notificationParentTransform, false);
			notification = loadedGameObject.GetComponent<TrayNotification>();
			if (notification == null)
			{
				throw new NullReferenceException("Unable to retrieve TrayNotification component in asset. Please check path: " + notificationData.PrefabLocation);
			}
			TrayNotification trayNotification = notification;
			trayNotification.ENotificationCompleted = (Action<NotificationCompleteEnum>)Delegate.Combine(trayNotification.ENotificationCompleted, new Action<NotificationCompleteEnum>(notificationTrayComplete));
			notification.Show(notificationData);
			if (NotificationCreated != null)
			{
				NotificationCreated(notification);
			}
		}

		private void notificationTrayComplete(NotificationCompleteEnum completed)
		{
			TrayNotification trayNotification = notification;
			trayNotification.ENotificationCompleted = (Action<NotificationCompleteEnum>)Delegate.Remove(trayNotification.ENotificationCompleted, new Action<NotificationCompleteEnum>(notificationTrayComplete));
			if (NotificationDismissed != null)
			{
				NotificationDismissed(completed, notificationData);
				NotificationDismissed = null;
			}
			UnityEngine.Object.Destroy(notification.gameObject);
		}
	}
}
