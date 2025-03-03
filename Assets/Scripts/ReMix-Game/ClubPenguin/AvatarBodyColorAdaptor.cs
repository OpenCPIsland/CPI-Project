using ClubPenguin.Avatar;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public static class AvatarBodyColorAdaptor
	{
		public static Color GetColorFromDefinitions(Dictionary<int, AvatarColorDefinition> avatarColors, int colorIndex)
		{
			AvatarColorDefinition value;
			if (avatarColors.TryGetValue(colorIndex, out value))
			{
				Color color;
				if (!ColorUtility.TryParseHtmlString("#" + value.Color, out color))
				{
					return GetRandomColor(avatarColors);
				}
				return color;
			}
			return GetRandomColor(avatarColors);
		}

		public static Color GetRandomColor(Dictionary<int, AvatarColorDefinition> avatarColors)
		{
			System.Random random = new System.Random();
			int[] array = new int[avatarColors.Count];
			avatarColors.Keys.CopyTo(array, 0);
			AvatarColorDefinition avatarColorDefinition = avatarColors[array[random.Next(0, array.Length)]];
			Color color;
			if (!ColorUtility.TryParseHtmlString("#" + avatarColorDefinition.Color, out color))
			{
				return AvatarService.DefaultBodyColor;
			}
			return color;
		}
	}
}
