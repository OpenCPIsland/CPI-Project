using ClubPenguin.Net.Client.Smartfox;
using Sfs2X.Entities;
using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class IslandTargetMMOItem : CPMMOItem
	{
		public string Path;

		public int HitCapacity;

		public int HitCount;

		public IslandTargetMMOItem(IMMOItem sfsItem)
		{
			Path = sfsItem.GetVariable(SocketItemVars.GAME_OBJECT_PATH.GetKey()).GetStringValue();
			HitCapacity = sfsItem.GetVariable(SocketItemVars.INTEGER_A.GetKey()).GetIntValue();
			HitCount = sfsItem.GetVariable(SocketItemVars.ACTION_COUNT.GetKey()).GetIntValue();
		}

		public bool IsAnnihilated()
		{
			return HitCount >= HitCapacity;
		}

		public string GetShortName()
		{
			int num = Path.LastIndexOf("/");
			if (num >= 0)
			{
				return Path.Substring(num);
			}
			return "";
		}
	}
}
