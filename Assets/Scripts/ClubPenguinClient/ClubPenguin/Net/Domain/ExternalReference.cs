using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct ExternalReference
	{
		public string ExternalId;

		public long Id;

		public string Value;
	}
}
