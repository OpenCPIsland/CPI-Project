using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct AdditionalPricePlanProperties
	{
		public string ExternalReference;

		public long Id;

		public string Name;

		public List<string> Values;
	}
}
