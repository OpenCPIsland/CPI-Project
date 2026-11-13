using Disney.Manimal.Common.Util;
using LitJson;
using System;

namespace ClubPenguin.Net.Offline
{
	public struct Profile : IOfflineData
	{
		public int Colour;

		public long DateCreated;

		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public int DaysOld
		{
			get
			{
				return (int)(DateTime.UtcNow - DateCreated.MsToDateTime()).TotalDays;
			}
		}

		public void Init()
		{
			DateCreated = DateTime.UtcNow.GetTimeInMilliseconds();
		}
	}
}
