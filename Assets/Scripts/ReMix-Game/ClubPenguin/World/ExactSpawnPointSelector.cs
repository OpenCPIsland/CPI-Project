using UnityEngine;

namespace ClubPenguin.World
{
	public class ExactSpawnPointSelector : SpawnPointSelector
	{
		public Transform SpawnPoint;

		public override Vector3 SelectSpawnPosition(CoordinateSpace cs)
		{
			Transform spawnPointTransform = getSpawnPointTransform();
			return (cs == CoordinateSpace.Local) ? spawnPointTransform.localPosition : spawnPointTransform.position;
		}

		public override Quaternion SelectSpawnRotation(CoordinateSpace cs)
		{
			Transform spawnPointTransform = getSpawnPointTransform();
			return (cs == CoordinateSpace.Local) ? spawnPointTransform.localRotation : spawnPointTransform.rotation;
		}

		private Transform getSpawnPointTransform()
		{
			Transform result = base.transform;
			if (SpawnPoint != null)
			{
				result = SpawnPoint;
			}
			return result;
		}
	}
}
