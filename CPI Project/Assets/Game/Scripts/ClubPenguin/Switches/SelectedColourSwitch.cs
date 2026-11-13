using ClubPenguin.Core;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Switches
{
	public class SelectedColourSwitch : Switch
	{
		[Tooltip("List of colours that will enable this switch. If empty matches anything")]
		public AvatarColorDefinition[] Colours;

		private AvatarDetailsData avatarDetailsData;

		public void Start()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out avatarDetailsData))
			{
				avatarDetailsData.PlayerColorChanged += onColorChanged;
				onColorChanged(avatarDetailsData.BodyColor);
			}
		}

		public void OnDestroy()
		{
			if (avatarDetailsData != null)
			{
				avatarDetailsData.PlayerColorChanged -= onColorChanged;
			}
		}

		private void onColorChanged(Color color)
		{
			Change(IsMatch(color, Colours));
		}

		public static bool IsMatch(Color bodyColor, AvatarColorDefinition[] colours)
		{
			if (colours == null || colours.Length == 0)
			{
				return true;
			}
			for (int i = 0; i < colours.Length; i++)
			{
				Color color;
				if (colours[i] != null && ColorUtility.TryParseHtmlString("#" + colours[i].Color, out color) && bodyColor == color)
				{
					return true;
				}
			}
			return false;
		}

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
