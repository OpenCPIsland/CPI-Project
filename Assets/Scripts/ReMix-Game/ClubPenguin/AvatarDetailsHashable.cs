using ClubPenguin.Net.Utils;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class AvatarDetailsHashable
	{
		public NColor BodyColor;

		public long[] EquipmentIds;

		public string DisplayName;

		public string Context;

		public AvatarDetailsHashable(AvatarDetailsData avatarDetails, string displayName, string context = "")
		{
			BodyColor = avatarDetails.BodyColor;
			DisplayName = displayName;
			Context = context;
			EquipmentIds = new long[avatarDetails.Outfit.Length];
			for (int i = 0; i < EquipmentIds.Length; i++)
			{
				EquipmentIds[i] = avatarDetails.Outfit[i].Id;
			}
		}
	}
}
