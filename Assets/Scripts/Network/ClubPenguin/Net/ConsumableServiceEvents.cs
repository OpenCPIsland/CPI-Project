using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net
{
	public static class ConsumableServiceEvents
	{
		public struct InventoryRecieved
		{
			public readonly ConsumableInventory Inventory;

			public InventoryRecieved(ConsumableInventory inventory)
			{
				Inventory = inventory;
			}
		}

		public struct ConsumableEquipped
		{
			public readonly long SessionId;

			public readonly string Type;

			public ConsumableEquipped(long sessionId, string type)
			{
				SessionId = sessionId;
				Type = type;
			}
		}

		public struct ConsumableUsed
		{
			public readonly long SessionId;

			public readonly string Type;

			public ConsumableUsed(long sessionId, string type)
			{
				SessionId = sessionId;
				Type = type;
			}
		}

		public struct ConsumableMMODeployed
		{
			public readonly long SessionId;

			public readonly long ExperienceId;

			public ConsumableMMODeployed(long sessionId, long experienceId)
			{
				SessionId = sessionId;
				ExperienceId = experienceId;
			}
		}
	}
}
