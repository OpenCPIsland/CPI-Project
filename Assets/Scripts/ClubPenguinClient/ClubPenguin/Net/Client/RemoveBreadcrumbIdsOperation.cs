using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/breadcrumb/v1/removebreadcrumbs")]
	[HttpPOST]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class RemoveBreadcrumbIdsOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public List<Breadcrumb> BreadcrumbList;

		[HttpResponseJsonBody]
		public BreadcrumbCountResponse CountReponse;

		public RemoveBreadcrumbIdsOperation(List<Breadcrumb> breadcrumbList)
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
			foreach (Breadcrumb breadcrumb in BreadcrumbList)
			{
				breadcrumbCollection.breadcrumbs.Remove(breadcrumb);
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
