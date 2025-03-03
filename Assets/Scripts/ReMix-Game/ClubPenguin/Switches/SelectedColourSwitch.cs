using ClubPenguin.Core;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Switches
{
	public class SelectedColourSwitch : Switch
	{
		[Tooltip("List of colours that will enable this switch. If empty matches anything")]
		public AvatarColorDefinition[] Colours;

		public override object GetSwitchParameters()
		{
			List<int> list = new List<int>();
			AvatarColorDefinition[] colours = Colours;
			foreach (AvatarColorDefinition avatarColorDefinition in colours)
			{
				list.Add(avatarColorDefinition.ColorId);
			}
			return list;
		}

		public override string GetSwitchType()
		{
			return "selectedColour";
		}
	}
}
