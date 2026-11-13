using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.Core
{
	[Serializable]
	public class ManifestContentKey : TypedAssetContentKey<Manifest>
	{
		public ManifestContentKey()
		{
		}

		public ManifestContentKey(string key)
			: base(key)
		{
		}
	}
}
