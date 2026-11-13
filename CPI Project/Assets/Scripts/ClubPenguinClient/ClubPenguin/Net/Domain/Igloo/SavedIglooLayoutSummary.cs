using System;

namespace ClubPenguin.Net.Domain.Igloo
{
	[Serializable]
	public class SavedIglooLayoutSummary
	{
		public long layoutId;

		public long createdDate;

		public long lastUpdatedDate;

		public string lot;

		public string name;

		public bool memberOnly;
	}
}
