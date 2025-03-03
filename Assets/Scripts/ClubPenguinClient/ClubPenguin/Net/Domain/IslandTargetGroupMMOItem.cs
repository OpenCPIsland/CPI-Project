using ClubPenguin.Net.Client.Smartfox;
using Sfs2X.Entities;
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
			Path = sfsItem.GetVariable(SocketItemVars.GAME_OBJECT_PATH.GetKey()).GetStringValue();
			Expires = long.Parse(sfsItem.GetVariable(SocketItemVars.STATE_TIMESTAMP.GetKey()).GetStringValue());
			if (sfsItem.ContainsVariable(SocketItemVars.TIMESTAMP.GetKey()))
			{
				Starts = long.Parse(sfsItem.GetVariable(SocketItemVars.TIMESTAMP.GetKey()).GetStringValue());
			}
		}
	}
}
