using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain.Igloo
{
	[Serializable]
	public class SavedIglooLayoutsSummary
	{
		public IglooVisibility visibility;

		public long? activeLayoutId;

		public List<SavedIglooLayoutSummary> layouts;

		public ActiveLayoutServerChangeNotification activeLayoutServerChangeNotification;
	}
}
