using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct BreadcrumbIdsRemoveRequest
	{
		public List<Breadcrumb> BreadcrumbIds;
	}
}
