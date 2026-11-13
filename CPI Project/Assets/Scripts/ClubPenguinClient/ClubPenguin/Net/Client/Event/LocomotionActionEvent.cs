using ClubPenguin.Net.Domain;
using System;
using UnityEngine;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct LocomotionActionEvent
	{
		public long SessionId;

		public LocomotionAction Type;

		public long Timestamp;

		public Vector3 Position;

		public Vector3? Direction;

		public Vector3? Velocity;

		public ActionedObject Object;
	}
}
