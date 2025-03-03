using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin.Net.Offline
{
	public struct BreadcrumbCollection : IOfflineData
	{
		public List<Breadcrumb> breadcrumbs;

		public void Init()
		{
			breadcrumbs = new List<Breadcrumb>();
		}
	}
}
