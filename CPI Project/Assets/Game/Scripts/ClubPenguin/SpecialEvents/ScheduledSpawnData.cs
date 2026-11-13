using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.SpecialEvents
{
	[Serializable]
	public class ScheduledSpawnData
	{
		public PrefabContentKey SpawnPrefabKey;

		public Transform SpawnParentTransform;

		public Vector3 SpawnOffset = Vector3.zero;

		public Vector3 SpawnRotation = Vector3.zero;

		public Vector3 SpawnScale = Vector3.zero;
	}
}
