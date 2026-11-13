using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public static class ScavengerHuntEvents
	{
		public struct StartFinderBulb
		{
			public readonly Dictionary<int, GameObject> ItemIdToItemGameObject;

			public readonly long LocalPlayerId;

			public readonly long OtherPlayerId;

			public StartFinderBulb(long localPlayerId, long otherPlayerId, Dictionary<int, GameObject> itemIdToItemGameObject)
			{
				LocalPlayerId = localPlayerId;
				OtherPlayerId = otherPlayerId;
				ItemIdToItemGameObject = itemIdToItemGameObject;
			}
		}

		public struct RemoveMarble
		{
			public readonly Transform MarbleTransform;

			public RemoveMarble(Transform marbleTransform)
			{
				MarbleTransform = marbleTransform;
			}
		}

		public struct FinderBulbBlink
		{
			public readonly bool IsOn;

			public FinderBulbBlink(bool isOn)
			{
				IsOn = isOn;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowHiderClockProp
		{
		}
	}
}
