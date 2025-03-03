using ClubPenguin.Net.Domain.Decoration;
using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class CreateDecorationResponse : CPResponse
	{
		public DecorationId decorationId;

		public PlayerAssets assets;
	}
}
