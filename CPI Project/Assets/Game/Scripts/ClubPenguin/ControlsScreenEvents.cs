using ClubPenguin.UI;
using Disney.Kelowna.Common;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin
{
	public static class ControlsScreenEvents
	{
		public struct SetLeftOption
		{
			public readonly PrefabContentKey LeftOptionContentKey;

			public SetLeftOption(PrefabContentKey leftOptionContentKey)
			{
				LeftOptionContentKey = leftOptionContentKey;
			}
		}

		public struct SetDefaultLeftOption
		{
			public readonly GameObject DefaultLeftOptionPrefab;

			public SetDefaultLeftOption(GameObject defaultLeftOptionPrefab)
			{
				DefaultLeftOptionPrefab = defaultLeftOptionPrefab;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ReturnToDefaultLeftOption
		{
		}

		public struct SetRightOption
		{
			public readonly InputButtonGroupContentKey RightButtonGroupContentKey;

			public SetRightOption(InputButtonGroupContentKey rightButtonGroupContentKey)
			{
				RightButtonGroupContentKey = rightButtonGroupContentKey;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ReturnToDefaultRightOption
		{
		}

		public struct SetButton
		{
			public readonly InputButtonContentKey ButtonContentKey;

			public readonly int ButtonIndex;

			public SetButton(InputButtonContentKey buttonContentKey, int buttonIndex)
			{
				ButtonContentKey = buttonContentKey;
				ButtonIndex = buttonIndex;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ControlLayoutLoadComplete
		{
		}
	}
}
