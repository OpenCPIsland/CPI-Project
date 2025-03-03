using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Breadcrumbs
{
	public abstract class AbstractNotificationBreadcrumb : MonoBehaviour
	{
		private const string ANIMATOR_IS_SHOWING_BREADCRUMB = "isBreadcrumb";

		protected NotificationBreadcrumbController notificationBreadcrumbController;

		private Animator breadcrumbAnimator;

		private Image breadrumbImage;

		private void Awake()
		{
			breadcrumbAnimator = GetComponent<Animator>();
			breadrumbImage = GetComponent<Image>();
			breadrumbImage.enabled = false;
			notificationBreadcrumbController = Service.Get<NotificationBreadcrumbController>();
			NotificationBreadcrumbController obj = notificationBreadcrumbController;
			obj.OnBreadcrumbAdded = (Action<string, int>)Delegate.Combine(obj.OnBreadcrumbAdded, new Action<string, int>(onBreadcrumbAdded));
			NotificationBreadcrumbController obj2 = notificationBreadcrumbController;
			obj2.OnBreadcrumbRemoved = (Action<string, int>)Delegate.Combine(obj2.OnBreadcrumbRemoved, new Action<string, int>(onBreadcrumbRemoved));
			NotificationBreadcrumbController obj3 = notificationBreadcrumbController;
			obj3.OnBreadcrumbReset = (Action<string>)Delegate.Combine(obj3.OnBreadcrumbReset, new Action<string>(onBreadcrumbReset));
		}

		private void OnEnable()
		{
			init();
		}

		protected abstract void init();

		protected abstract void onBreadcrumbAdded(string breadcrumbId, int count);

		protected abstract void onBreadcrumbRemoved(string breadcrumbId, int count);

		protected abstract void onBreadcrumbReset(string breadcrumbId);

		protected void showBreadcrumb()
		{
			breadrumbImage.enabled = true;
			if (breadcrumbAnimator != null && !breadcrumbAnimator.GetBool("isBreadcrumb"))
			{
				breadcrumbAnimator.SetBool("isBreadcrumb", true);
			}
		}

		protected void hideBreadcrumb()
		{
			if (breadcrumbAnimator != null)
			{
				if (breadcrumbAnimator.GetBool("isBreadcrumb"))
				{
					breadcrumbAnimator.SetBool("isBreadcrumb", false);
				}
			}
			else
			{
				breadrumbImage.enabled = false;
			}
		}

		private void OnDestroy()
		{
			NotificationBreadcrumbController obj = notificationBreadcrumbController;
			obj.OnBreadcrumbAdded = (Action<string, int>)Delegate.Remove(obj.OnBreadcrumbAdded, new Action<string, int>(onBreadcrumbAdded));
			NotificationBreadcrumbController obj2 = notificationBreadcrumbController;
			obj2.OnBreadcrumbRemoved = (Action<string, int>)Delegate.Remove(obj2.OnBreadcrumbRemoved, new Action<string, int>(onBreadcrumbRemoved));
			NotificationBreadcrumbController obj3 = notificationBreadcrumbController;
			obj3.OnBreadcrumbReset = (Action<string>)Delegate.Remove(obj3.OnBreadcrumbReset, new Action<string>(onBreadcrumbReset));
		}
	}
}
