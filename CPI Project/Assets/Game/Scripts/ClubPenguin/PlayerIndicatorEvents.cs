using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin
{
	public class PlayerIndicatorEvents
	{
		public struct ShowPlayerIndicator
		{
			public GameObject IndicatorObject;

			public long PlayerId;

			public ShowPlayerIndicator(GameObject indicatorObject, long playerId)
			{
				IndicatorObject = indicatorObject;
				PlayerId = playerId;
			}
		}

		public struct RemovePlayerIndicator
		{
			public long PlayerId;

			public bool IsStored;

			public bool Destroy;

			public RemovePlayerIndicator(long playerId, bool isStored, bool destroy = true)
			{
				PlayerId = playerId;
				IsStored = isStored;
				Destroy = destroy;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HidePlayerIndicators
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowPlayerIndicators
		{
		}
	}
}
