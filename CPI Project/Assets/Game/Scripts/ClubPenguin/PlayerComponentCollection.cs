using ClubPenguin.Data;
using System;

namespace ClubPenguin
{
	public class PlayerComponentCollection : ComponentCollection
	{
		public string InternalId
		{
			get;
			protected set;
		}

		public PlayerComponentCollection()
		{
			InternalId = Guid.NewGuid().ToString();
			AddComponent(new PlayerIdComponent());
		}
	}
}
