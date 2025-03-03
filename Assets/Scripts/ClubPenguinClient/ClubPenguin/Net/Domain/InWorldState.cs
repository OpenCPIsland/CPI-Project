using System;
using UnityEngine;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct InWorldState
	{
		public RoomIdentifier location;

		public Vector3 position;

		public string equippedItem;
	}
}
