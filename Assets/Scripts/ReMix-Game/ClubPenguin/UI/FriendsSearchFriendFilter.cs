using System;
using System.Collections.Generic;

namespace ClubPenguin.UI
{
	public class FriendsSearchFriendFilter
	{
		private StringComparison comparisonType = StringComparison.OrdinalIgnoreCase;

		public List<string> GetMatches(string substring, List<string> displayNames)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < displayNames.Count; i++)
			{
				if (displayNames[i].IndexOf(substring, comparisonType) > -1)
				{
					list.Add(displayNames[i]);
				}
			}
			return list;
		}
	}
}
