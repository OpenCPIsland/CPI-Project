using System;

namespace ClubPenguin
{
	[Serializable]
	public struct ZonePathingNode
	{
		public ZoneDefinition Zone;

		public ZoneDefinition[] ZoneTransitions;
	}
}
