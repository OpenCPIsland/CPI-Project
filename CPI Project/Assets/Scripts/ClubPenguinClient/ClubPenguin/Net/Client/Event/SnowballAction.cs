using System;
using UnityEngine;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct SnowballAction
	{
		public long userid;

		public Vector3 position;

		public Vector3 velocity;

		public long timestamp;
	}
}
