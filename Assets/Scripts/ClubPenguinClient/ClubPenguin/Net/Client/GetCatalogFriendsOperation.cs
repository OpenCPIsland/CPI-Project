using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Client
{
	public class GetCatalogFriendsOperation : BaseCatalogSectionOperation<CatalogSectionRequest>
	{
		public GetCatalogFriendsOperation(CatalogSectionRequest sectionRequest)
			: base(sectionRequest)
		{
			CatalogSection = "friends";
		}
	}
}
