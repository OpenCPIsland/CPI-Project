using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpContentType("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/breadcrumb/v1/breadcrumb")]
	public class AddBreadcrumbIdsOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public List<Breadcrumb> BreadcrumbList;

		[HttpResponseJsonBody]
		public BreadcrumbCountResponse CountReponse;

		public AddBreadcrumbIdsOperation(List<Breadcrumb> breadcrumbList)
		{
			BreadcrumbList = breadcrumbList;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			BreadcrumbCollection breadcrumbCollection = updateBreadCrumbs(offlineDatabase);
			CountReponse = new BreadcrumbCountResponse
			{
				breadcrumbCount = breadcrumbCollection.breadcrumbs.Count
			};
		}

		private BreadcrumbCollection updateBreadCrumbs(OfflineDatabase offlineDatabase)
		{
			BreadcrumbCollection breadcrumbCollection = offlineDatabase.Read<BreadcrumbCollection>();
			breadcrumbCollection.breadcrumbs.AddRange(BreadcrumbList);
			if (breadcrumbCollection.breadcrumbs.Count > 50)
			{
				breadcrumbCollection.breadcrumbs.RemoveRange(0, breadcrumbCollection.breadcrumbs.Count - 50);
			}
			offlineDatabase.Write(breadcrumbCollection);
			return breadcrumbCollection;
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			updateBreadCrumbs(offlineDatabase);
		}
	}
}
