using System.Collections.Generic;
using System.Text;

namespace ClubPenguin.Breadcrumbs
{
	public class TutorialBreadcrumbController
	{
		internal delegate void BreadcrumbsUpdated();

		private List<TutorialBreadcrumbPath> breadcrumbPaths;

		private List<string> onScreenBreadcrumbs;

		internal event BreadcrumbsUpdated OnBreadcrumbsUpdated;

		public void Init()
		{
			breadcrumbPaths = new List<TutorialBreadcrumbPath>();
			onScreenBreadcrumbs = new List<string>();
		}

		internal bool IsLastShowingBreadcrumb(string breadcrumbId)
		{
			int num = -1;
			int num2 = -1;
			bool result = false;
			for (int i = 0; i < breadcrumbPaths.Count; i++)
			{
				TutorialBreadcrumbPath tutorialBreadcrumbPath = breadcrumbPaths[i];
				for (int j = 0; j < tutorialBreadcrumbPath.Ids.Length; j++)
				{
					if (onScreenBreadcrumbs.Contains(tutorialBreadcrumbPath.Ids[j]))
					{
						num = j;
					}
					if (tutorialBreadcrumbPath.Ids[j] == breadcrumbId)
					{
						num2 = j;
					}
				}
				if (num2 == num && num != -1)
				{
					result = true;
					break;
				}
				num = -1;
				num2 = -1;
			}
			return result;
		}

		internal void AddBreadcrumb(TutorialBreadcrumbPath path)
		{
			int num = hasBreadcrumb(path);
			if (num == -1)
			{
				breadcrumbPaths.Add(path);
				if (this.OnBreadcrumbsUpdated != null)
				{
					this.OnBreadcrumbsUpdated();
				}
			}
		}

		public void RemoveBreadcrumb(string id)
		{
			for (int num = breadcrumbPaths.Count - 1; num >= 0; num--)
			{
				if (breadcrumbPaths[num].Ids[breadcrumbPaths[num].Ids.Length - 1] == id)
				{
					TutorialBreadcrumbPath tutorialBreadcrumbPath = breadcrumbPaths[num];
					breadcrumbPaths.RemoveAt(num);
					if (this.OnBreadcrumbsUpdated != null)
					{
						this.OnBreadcrumbsUpdated();
					}
				}
			}
		}

		internal void BreadcrumbIsOnScreen(string breadcrumbId, bool isOnScreen)
		{
			if (isOnScreen)
			{
				if (!onScreenBreadcrumbs.Contains(breadcrumbId))
				{
					onScreenBreadcrumbs.Add(breadcrumbId);
					if (this.OnBreadcrumbsUpdated != null)
					{
						this.OnBreadcrumbsUpdated();
					}
				}
			}
			else if (onScreenBreadcrumbs.Contains(breadcrumbId))
			{
				onScreenBreadcrumbs.Remove(breadcrumbId);
				if (this.OnBreadcrumbsUpdated != null)
				{
					this.OnBreadcrumbsUpdated();
				}
			}
		}

		public void RemoveAllBreadcrumbs()
		{
			breadcrumbPaths.Clear();
			if (this.OnBreadcrumbsUpdated != null)
			{
				this.OnBreadcrumbsUpdated();
			}
		}

		private string convertBreadcrumbPathToString(TutorialBreadcrumbPath path)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < path.Ids.Length; i++)
			{
				stringBuilder.Append(path.Ids[i]);
			}
			return stringBuilder.ToString();
		}

		private int hasBreadcrumb(TutorialBreadcrumbPath path)
		{
			string a = convertBreadcrumbPathToString(path);
			string text = "";
			int result = -1;
			for (int i = 0; i < breadcrumbPaths.Count; i++)
			{
				text = convertBreadcrumbPathToString(breadcrumbPaths[i]);
				if (a == text)
				{
					result = i;
					break;
				}
			}
			return result;
		}
	}
}
