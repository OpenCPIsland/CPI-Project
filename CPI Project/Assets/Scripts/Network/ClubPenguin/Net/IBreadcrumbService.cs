using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	public interface IBreadcrumbService : INetworkService
	{
		void AddBreadcrumbIds(List<Breadcrumb> breadcrumbIds);

		void RemoveBreadcrumbIds(List<Breadcrumb> breadcrumbIds);
	}
}
