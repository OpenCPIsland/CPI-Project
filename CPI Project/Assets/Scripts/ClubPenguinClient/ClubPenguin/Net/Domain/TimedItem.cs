using ClubPenguin.Net.Client.Smartfox;
using Sfs3.Client.Entities;
using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class TimedItem : ConsumableItem
	{
		public float TimeToLive;

		public TimedItem(IMMOItem sfsItem)
		{
			TimeToLive = (float)sfsItem.GetVariable(SocketItemVars.TIME_TO_LIVE.GetKey()).DoubleValue.Value;
		}

		public TimedItem(float timeToLive)
		{
			TimeToLive = timeToLive;
		}
	}
}
