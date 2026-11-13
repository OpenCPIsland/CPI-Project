using ClubPenguin.Net.Client.Smartfox;
using Sfs3.Client.Entities;
using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class IslandTargetGroupMMOItem : CPMMOItem
	{
		public string Path;

		public long Expires;

		public long Starts;

		public IslandTargetGroupMMOItem(IMMOItem sfsItem)
		{
			Path = sfsItem.GetVariable(SocketItemVars.GAME_OBJECT_PATH.GetKey()).StringValue;
			Expires = long.Parse(sfsItem.GetVariable(SocketItemVars.STATE_TIMESTAMP.GetKey()).StringValue);
			if (sfsItem.ContainsVariable(SocketItemVars.TIMESTAMP.GetKey()))
			{
				Starts = long.Parse(sfsItem.GetVariable(SocketItemVars.TIMESTAMP.GetKey()).StringValue);
			}
		}
	}
}
