using System.Collections.Generic;

namespace ClubPenguin.Breadcrumbs
{
	public class MultipleNotificationBreadcrumb : AbstractNotificationBreadcrumb
	{
		public string[] BreadcrumbIds;

		private Dictionary<string, int> counts;

		protected override void init()
		{
			counts = new Dictionary<string, int>();
			for (int i = 0; i < BreadcrumbIds.Length; i++)
			{
				counts.Add(BreadcrumbIds[i], notificationBreadcrumbController.GetBreadcrumbCount(BreadcrumbIds[i]));
			}
			updateBreadcrumbVisibility();
		}

		private string getRelevantBreadcrumbId(string breadcrumbId)
		{
			string result = null;
			if (breadcrumbId.IndexOf('/') == -1)
			{
				for (int i = 0; i < BreadcrumbIds.Length; i++)
				{
					if (breadcrumbId == BreadcrumbIds[i])
					{
						result = breadcrumbId;
					}
				}
			}
			else
			{
				string[] array = breadcrumbId.Split('/');
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < BreadcrumbIds.Length; j++)
					{
						if (array[i] == BreadcrumbIds[j])
						{
							result = array[i];
						}
					}
				}
			}
			return result;
		}

		protected override void onBreadcrumbAdded(string breadcrumbId, int count)
		{
			string relevantBreadcrumbId = getRelevantBreadcrumbId(breadcrumbId);
			if (relevantBreadcrumbId != null)
			{
				updateBreadcrumbCount(relevantBreadcrumbId, count);
				updateBreadcrumbVisibility();
			}
		}

		protected override void onBreadcrumbRemoved(string breadcrumbId, int count)
		{
			string relevantBreadcrumbId = getRelevantBreadcrumbId(breadcrumbId);
			if (relevantBreadcrumbId == null)
			{
				return;
			}
			if (breadcrumbId.IndexOf('/') == -1)
			{
				if (breadcrumbId == relevantBreadcrumbId)
				{
					updateBreadcrumbCount(breadcrumbId, count);
					updateBreadcrumbVisibility();
				}
				return;
			}
			string[] array = breadcrumbId.Split('/');
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == relevantBreadcrumbId)
				{
					updateBreadcrumbCount(relevantBreadcrumbId, count);
					updateBreadcrumbVisibility();
				}
			}
		}

		protected override void onBreadcrumbReset(string breadcrumbId)
		{
			string relevantBreadcrumbId = getRelevantBreadcrumbId(breadcrumbId);
			if (relevantBreadcrumbId == null)
			{
				return;
			}
			if (breadcrumbId.IndexOf('/') == -1)
			{
				if (breadcrumbId == relevantBreadcrumbId)
				{
					updateBreadcrumbCount(breadcrumbId, 0);
					updateBreadcrumbVisibility();
				}
				return;
			}
			string[] array = breadcrumbId.Split('/');
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == relevantBreadcrumbId)
				{
					updateBreadcrumbCount(relevantBreadcrumbId, 0);
					updateBreadcrumbVisibility();
				}
			}
		}

		private void updateBreadcrumbCount(string breadcrumbId, int count)
		{
			if (counts.ContainsKey(breadcrumbId))
			{
				counts[breadcrumbId] = count;
			}
		}

		private void updateBreadcrumbVisibility()
		{
			bool flag = false;
			foreach (KeyValuePair<string, int> count in counts)
			{
				if (count.Value > 0)
				{
					flag = true;
					break;
				}
			}
			if (flag)
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
