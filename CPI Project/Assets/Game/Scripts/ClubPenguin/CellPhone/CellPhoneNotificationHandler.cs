using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneNotificationHandler : MonoBehaviour
	{
		public Action<NotificationCompleteEnum, DNotification> NotificationDismissed;

		public GameObject DailyNotificationPanel;

		public GameObject ActivityTrackerNotificationPanel;

		public TrayNotification DailyNotification;

		public TrayNotification ActivityTrackerNotification;

		private DNotification notificationData;

		private GameObject currentNotificationPanel;

		private TrayNotification currentNotification;

		public DNotification NotifcationData
		{
			get
			{
				return notificationData;
			}
		}

		private void Start()
		{
			Service.Get<TrayNotificationManager>().SetCellPhoneNotificationHandler(this);
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		public void ShowNotification(DNotification notificationData)
		{
			this.notificationData = notificationData;
			switch (notificationData.Type)
			{
			case DNotification.NotificationType.DailyComplete:
				currentNotificationPanel = DailyNotificationPanel;
				currentNotification = DailyNotification;
				break;
			case DNotification.NotificationType.ActivityTracker:
				currentNotificationPanel = ActivityTrackerNotificationPanel;
				currentNotification = ActivityTrackerNotification;
				break;
			}
			currentNotificationPanel.SetActive(true);
			TrayNotification trayNotification = currentNotification;
			trayNotification.ENotificationCompleted = (Action<NotificationCompleteEnum>)Delegate.Combine(trayNotification.ENotificationCompleted, new Action<NotificationCompleteEnum>(notificationComplete));
			currentNotification.Show(notificationData);
			SetParticlesActive(true);
		}

		public void DismissNotification(bool showAnimation)
		{
			if (currentNotification != null)
			{
				currentNotification.Dismiss(showAnimation);
			}
		}

		private void notificationComplete(NotificationCompleteEnum completed)
		{
			TrayNotification trayNotification = currentNotification;
			trayNotification.ENotificationCompleted = (Action<NotificationCompleteEnum>)Delegate.Remove(trayNotification.ENotificationCompleted, new Action<NotificationCompleteEnum>(notificationComplete));
			currentNotificationPanel.SetActive(false);
			DNotification arg = notificationData;
			notificationData = null;
			if (NotificationDismissed != null)
			{
				NotificationDismissed(completed, arg);
			}
			SetParticlesActive(false);
		}

		private void SetParticlesActive(bool state)
		{
			TrayNotificationCelebration component = currentNotification.GetComponent<TrayNotificationCelebration>();
			if (component != null)
			{
				ParticleSystem particlesToTint = component.ParticlesToTint;
				particlesToTint.EnableEmission(state);
				ParticleSystem component2 = particlesToTint.gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
				component2.EnableEmission(state);
			}
		}
	}
}
