using System;

namespace ClubPenguin.Net.Domain.Igloo
{
	[Serializable]
	public class MutableIglooData
	{
		public IglooVisibility? visibility;

		public long? activeLayoutId;
	}
}
