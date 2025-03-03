using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct BreadcrumbIdsAddRequest
	{
		public List<Breadcrumb> Breadcrumbs;
	}
}
