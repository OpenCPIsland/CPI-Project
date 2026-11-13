using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Breadcrumbs
{
	internal class NotificationBreadcrumbResetter : MonoBehaviour
	{
		public StaticBreadcrumbDefinitionKey Breadcrumb;

		private NotificationBreadcrumbController notificationBreadcrumbController;

		private void OnEnable()
		{
			notificationBreadcrumbController = Service.Get<NotificationBreadcrumbController>();
			notificationBreadcrumbController.ResetBreadcrumbs(Breadcrumb);
			NotificationBreadcrumbController obj = notificationBreadcrumbController;
			obj.OnBreadcrumbAdded = (Action<string, int>)Delegate.Combine(obj.OnBreadcrumbAdded, new Action<string, int>(onBreadcrumbAdded));
		}

		private void onBreadcrumbAdded(string breadcrumbId, int count)
		{
			if (breadcrumbId == Breadcrumb.Id)
			{
				notificationBreadcrumbController.ResetBreadcrumbs(Breadcrumb);
			}
		}

		private void OnDisable()
		{
			NotificationBreadcrumbController obj = notificationBreadcrumbController;
			obj.OnBreadcrumbAdded = (Action<string, int>)Delegate.Remove(obj.OnBreadcrumbAdded, new Action<string, int>(onBreadcrumbAdded));
		}
	}
}
