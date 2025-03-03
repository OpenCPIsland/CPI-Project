namespace ClubPenguin.Catalog
{
	public static class CatalogModelEvents
	{
		public struct CatalogStateChanged
		{
			public CatalogState State;

			public CatalogStateChanged(CatalogState state)
			{
				State = state;
			}
		}
	}
}
