using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public static class CustomizerActiveSwatchEvents
	{
		public struct SetIsFabric
		{
			public readonly bool IsFabric;

			public SetIsFabric(bool isFabric)
			{
				IsFabric = isFabric;
			}
		}

		public struct SetIsVisible
		{
			public readonly bool IsVisible;

			public SetIsVisible(bool isVisible)
			{
				IsVisible = isVisible;
			}
		}

		public struct SetSwatch
		{
			public readonly Texture2D Fabric;

			public readonly Texture2D Decal;

			public SetSwatch(Texture2D fabric, Texture2D decal)
			{
				Fabric = fabric;
				Decal = decal;
			}
		}

		public struct SetIsInteractable
		{
			public readonly bool IsInteractable;

			public SetIsInteractable(bool isInteractable)
			{
				IsInteractable = isInteractable;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ToggleActiveSwatch
		{
		}
	}
}
