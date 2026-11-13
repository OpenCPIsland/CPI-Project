using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.UI
{
	[Serializable]
	public class InputButtonContentKey : TypedAssetContentKey<InputButtonDefinition>
	{
		public InputButtonContentKey(string key)
			: base(key)
		{
		}
	}
}
