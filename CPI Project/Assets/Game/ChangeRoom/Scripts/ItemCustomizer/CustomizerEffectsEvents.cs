using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public static class CustomizerEffectsEvents
	{
		public struct FabricPlaced
		{
			public readonly Vector3 PlacedPosition;

			public FabricPlaced(Vector3 placedPosition)
			{
				PlacedPosition = placedPosition;
			}
		}

		public struct DecalPlaced
		{
			public readonly Vector3 PlacedPosition;

			public DecalPlaced(Vector3 placedPosition)
			{
				PlacedPosition = placedPosition;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ItemSaving
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ItemSaved
		{
		}

		public struct FadeBackground
		{
			public readonly bool DoFade;

			public readonly float FadeAmount;

			public readonly float Duration;

			public FadeBackground(bool doFade, float fadeAmount, float duration)
			{
				DoFade = doFade;
				FadeAmount = Mathf.Clamp(fadeAmount, 0f, 1f);
				Duration = duration;
			}
		}
	}
}
