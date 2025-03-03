using ClubPenguin.Net.Client.Smartfox;
using Sfs2X.Entities;
using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class ActionedItem : TimedItem
	{
		public int ActionCount;

		public ActionedItem(IMMOItem sfsItem)
			: base(sfsItem)
		{
			ActionCount = sfsItem.GetVariable(SocketItemVars.ACTION_COUNT.GetKey()).GetIntValue();
		}

		public ActionedItem(float timeToLive, int actionCount)
			: base(timeToLive)
		{
			ActionCount = actionCount;
		}
	}
}
