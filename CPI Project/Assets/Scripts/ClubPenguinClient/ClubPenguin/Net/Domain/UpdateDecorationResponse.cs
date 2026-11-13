using ClubPenguin.Net.Domain.Decoration;
using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class UpdateDecorationResponse : CPResponse
	{
		public DecorationId decorationId;

		public PlayerAssets assets;
	}
}
