using ClubPenguin.Net.Domain;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	public class GetCatalogCategoryOperation : BaseCatalogSectionOperation<CatalogCategoryRequest>
	{
		[HttpRequestJsonBody(VariableName = "category")]
		public int Category;

		public GetCatalogCategoryOperation(CatalogCategoryRequest categoryRequest)
			: base(categoryRequest)
		{
			CatalogSection = "category";
		}
	}
}
