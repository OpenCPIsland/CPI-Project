using System.Runtime.InteropServices;

namespace ClubPenguin.World.Activities.Diving
{
	public static class DivingEvents
	{
		public enum EventTypes
		{
			AirSupplyUpdated,
			DepthUpdated,
			ShowHud,
			HideHud,
			CollisionEffects,
			AirBubbleBurstEffects,
			FreeAirEffects
		}

		public struct DepthUpdated
		{
			public readonly int CurrentDepth;

			public DepthUpdated(int currentDepth)
			{
				CurrentDepth = currentDepth;
			}
		}

		public struct AirSupplyUpdated
		{
			public readonly float AirSupply;

			public AirSupplyUpdated(float airSupply)
			{
				AirSupply = airSupply;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowHud
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideHud
		{
		}

		public struct CollisionEffects
		{
			public readonly string Tag;

			public CollisionEffects(string tag)
			{
				Tag = tag;
			}
		}

		public struct AirBubbleBurstEffects
		{
			public readonly string Tag;

			public AirBubbleBurstEffects(string tag)
			{
				Tag = tag;
			}
		}

		public struct FreeAirEffects
		{
			public readonly bool Enabled;

			public readonly string Tag;

			public FreeAirEffects(bool enabled, string tag)
			{
				Enabled = enabled;
				Tag = tag;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct PlayerResurfaced
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct EnableLocalInfiniteAir
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct DisableLocalInfiniteAir
		{
		}
	}
}
