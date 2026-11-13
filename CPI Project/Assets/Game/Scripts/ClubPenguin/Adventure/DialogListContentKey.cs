using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.Adventure
{
	[Serializable]
	public class DialogListContentKey : TypedAssetContentKey<DialogList>
	{
		public DialogListContentKey(string key)
			: base(key)
		{
		}
	}
}
