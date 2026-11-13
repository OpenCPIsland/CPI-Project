using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	public struct CatalogSectionResponse
	{
		public string cursor;

		public List<CatalogItemData> items;
	}
}
