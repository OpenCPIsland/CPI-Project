using ClubPenguin.Net.Client.Smartfox;
using Disney.Manimal.Common.Util;
using Sfs3.Client.Entities;
using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class StatefulWorldObject : CPMMOItem
	{
		public string Path;

		public long Timestamp;

		public ScheduledWorldObjectState State;

		public DateTime DateTime
		{
			get
			{
				return Timestamp.MsToDateTime();
			}
		}

		public StatefulWorldObject()
		{
		}

		public StatefulWorldObject(IMMOItem sfsItem)
		{
			Path = sfsItem.GetVariable(SocketItemVars.GAME_OBJECT_PATH.GetKey()).StringValue;
			Timestamp = long.Parse(sfsItem.GetVariable(SocketItemVars.STATE_TIMESTAMP.GetKey()).StringValue);
			State = (ScheduledWorldObjectState)sfsItem.GetVariable(SocketItemVars.SCHEDULED_WORLD_OBJECT_STATE.GetKey()).IntValue.Value;
		}
	}
}
