using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Client
{
	public class GetCatalogPopularOperation : BaseCatalogSectionOperation<CatalogSectionRequest>
	{
		public GetCatalogPopularOperation(CatalogSectionRequest sectionRequest)
			: base(sectionRequest)
		{
			CatalogSection = "popular";
		}
	}
}
