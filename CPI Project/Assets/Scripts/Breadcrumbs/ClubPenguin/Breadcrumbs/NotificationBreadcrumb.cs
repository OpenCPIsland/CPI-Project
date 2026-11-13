using System;

namespace ClubPenguin.Breadcrumbs
{
	public class NotificationBreadcrumb : AbstractNotificationBreadcrumb
	{
		public string BreadcrumbId;

		private int count;

		public int Count
		{
			get
			{
				return count;
			}
		}

		public void SetBreadcrumbId(string breadcrumbId)
		{
			if (string.IsNullOrEmpty(breadcrumbId))
			{
				throw new ArgumentNullException("breadcrumbId");
			}
			BreadcrumbId = breadcrumbId;
			init();
		}

		public void SetBreadcrumbId(PersistentBreadcrumbTypeDefinitionKey breadcrumbType, string instanceId)
		{
			SetBreadcrumbId(PersistentBreadcrumbTypeDefinition.ToStaticBreadcrumb(breadcrumbType.Id, instanceId));
		}

		protected override void init()
		{
			count = notificationBreadcrumbController.GetBreadcrumbCount(BreadcrumbId);
			updateBreadcrumbCount(count);
		}

		protected override void onBreadcrumbAdded(string breadcrumbId, int count)
		{
			if (breadcrumbId.IndexOf('/') == -1)
			{
				if (breadcrumbId == BreadcrumbId)
				{
					updateBreadcrumbCount(count);
				}
				return;
			}
			string[] array = breadcrumbId.Split('/');
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == BreadcrumbId)
				{
					updateBreadcrumbCount(count);
				}
			}
		}

		protected override void onBreadcrumbRemoved(string breadcrumbId, int count)
		{
			if (breadcrumbId.IndexOf('/') == -1)
			{
				if (breadcrumbId == BreadcrumbId)
				{
					updateBreadcrumbCount(count);
				}
				return;
			}
			string[] array = breadcrumbId.Split('/');
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == BreadcrumbId)
				{
					updateBreadcrumbCount(count);
				}
			}
		}

		protected override void onBreadcrumbReset(string breadcrumbId)
		{
			if (breadcrumbId.IndexOf('/') == -1)
			{
				if (breadcrumbId == BreadcrumbId)
				{
					updateBreadcrumbCount(0);
				}
				return;
			}
			string[] array = breadcrumbId.Split('/');
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == BreadcrumbId)
				{
					updateBreadcrumbCount(0);
				}
			}
		}

		private void updateBreadcrumbCount(int count)
		{
			this.count = count;
			if (count > 0)
			{
				showBreadcrumb();
			}
			else
			{
				hideBreadcrumb();
			}
		}
	}
}
