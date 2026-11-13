using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.UI
{
	[Serializable]
	public class InputButtonGroupContentKey : TypedAssetContentKey<InputButtonGroupDefinition>
	{
		public InputButtonGroupContentKey(string key)
			: base(key)
		{
		}
	}
}
