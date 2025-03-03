using System;

namespace ClubPenguin.Core.StaticGameData
{
	[Serializable]
	public class StaticGameDataKey
	{
		public static string GetTypeString(Type t)
		{
			return t.FullName + ", " + t.Assembly.GetName().Name;
		}
	}
}
