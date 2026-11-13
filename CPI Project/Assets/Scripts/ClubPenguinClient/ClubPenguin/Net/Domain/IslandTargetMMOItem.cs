using ClubPenguin.Net.Client.Smartfox;
using Sfs3.Client.Entities;
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
			Path = sfsItem.GetVariable(SocketItemVars.GAME_OBJECT_PATH.GetKey()).StringValue;
			HitCapacity = sfsItem.GetVariable(SocketItemVars.INTEGER_A.GetKey()).IntValue.Value;
			HitCount = sfsItem.GetVariable(SocketItemVars.ACTION_COUNT.GetKey()).IntValue.Value;
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
