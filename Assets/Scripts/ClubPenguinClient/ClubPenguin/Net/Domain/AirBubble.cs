using Disney.Manimal.Common.Util;
using Sfs2X.Entities.Data;
using System;
using System.Text;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class AirBubble
	{
		private const string TIME_KEY = "t";

		private const string VALUE_KEY = "v";

		private const string STATE_KEY = "s";

		public long time;

		public float value;

		public int diveState;

		public DateTime DateTime
		{
			get
			{
				return time.MsToDateTime();
			}
		}

		public static AirBubble FromSFSData(ISFSObject obj)
		{
			AirBubble airBubble = new AirBubble();
			airBubble.time = obj.GetLong("t");
			airBubble.value = obj.GetFloat("v");
			airBubble.diveState = obj.GetInt("s");
			return airBubble;
		}

		public ISFSObject ToSFSObject()
		{
			ISFSObject iSFSObject = new SFSObject();
			iSFSObject.PutLong("t", time);
			iSFSObject.PutFloat("v", value);
			iSFSObject.PutInt("s", diveState);
			return iSFSObject;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AirBubble: ").Append(DateTime).Append(": ")
				.Append(value)
				.Append(", State:")
				.Append(diveState);
			return stringBuilder.ToString();
		}
	}
}
