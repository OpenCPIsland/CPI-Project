using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Client
{
	public class GetCatalogRecentOperation : BaseCatalogSectionOperation<CatalogSectionRequest>
	{
		public GetCatalogRecentOperation(CatalogSectionRequest sectionRequest)
			: base(sectionRequest)
		{
			CatalogSection = "recent";
		}
	}
}
