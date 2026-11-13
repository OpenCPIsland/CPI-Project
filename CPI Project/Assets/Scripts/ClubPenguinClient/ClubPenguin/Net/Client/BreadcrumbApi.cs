using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	public class BreadcrumbApi
	{
		private ClubPenguinClient clubPenguinClient;

		public BreadcrumbApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<AddBreadcrumbIdsOperation> AddBreadcrumbIds(List<Breadcrumb> breadcrumbPersistentIds)
		{
			AddBreadcrumbIdsOperation operation = new AddBreadcrumbIdsOperation(breadcrumbPersistentIds);
			return new APICall<AddBreadcrumbIdsOperation>(clubPenguinClient, operation);
		}

		public APICall<RemoveBreadcrumbIdsOperation> RemoveBreadcrumbIds(List<Breadcrumb> breadcrumbPersistentIds)
		{
			RemoveBreadcrumbIdsOperation operation = new RemoveBreadcrumbIdsOperation(breadcrumbPersistentIds);
			return new APICall<RemoveBreadcrumbIdsOperation>(clubPenguinClient, operation);
		}
	}
}
