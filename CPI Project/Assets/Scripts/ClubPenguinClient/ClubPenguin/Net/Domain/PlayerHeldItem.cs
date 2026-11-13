using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class PlayerHeldItem : CPMMOItem
	{
		public string Type;

		public string Properties;

		public override string ToString()
		{
			return string.Format("PlayerHeldItem: {0}, Properties: {1}", Type, Properties);
		}
	}
}
