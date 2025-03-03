using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct PCGetProductDetailsResponse
	{
		public PCProduct Product;

		public PCContext Context;
	}
}
