namespace ClubPenguin.Catalog
{
	public class CatalogModel
	{
		private CatalogState state;

		public string HeaderText;

		public CatalogState State
		{
			get
			{
				return state;
			}
			set
			{
				if (state != value)
				{
					state = value;
					CatalogContext.EventBus.DispatchEvent(new CatalogModelEvents.CatalogStateChanged(state));
				}
			}
		}

		public CatalogModel(CatalogState state)
		{
			this.state = state;
		}
	}
}
