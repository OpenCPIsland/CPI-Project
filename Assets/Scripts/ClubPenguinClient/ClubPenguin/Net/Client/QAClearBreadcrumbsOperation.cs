using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/breadcrumb/v1/qa/clearbreadcrumbs")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpAccept("application/json")]
	[HttpDELETE]
	public class QAClearBreadcrumbsOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public BreadcrumbsResponse BreadcrumbIds;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			BreadcrumbCollection breadcrumbCollection = updateBreadCrumbs(offlineDatabase);
			BreadcrumbIds = new BreadcrumbsResponse
			{
				breadcrumbs = new List<Breadcrumb>()
			};
		}

		private BreadcrumbCollection updateBreadCrumbs(OfflineDatabase offlineDatabase)
		{
			BreadcrumbCollection breadcrumbCollection = offlineDatabase.Read<BreadcrumbCollection>();
			breadcrumbCollection.breadcrumbs.Clear();
			offlineDatabase.Write(breadcrumbCollection);
			return breadcrumbCollection;
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			updateBreadCrumbs(offlineDatabase);
		}
	}
}
