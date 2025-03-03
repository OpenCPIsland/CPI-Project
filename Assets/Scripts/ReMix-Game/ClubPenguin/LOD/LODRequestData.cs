using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin.LOD
{
	[Serializable]
	public struct LODRequestData
	{
		public readonly string Type;

		public readonly DataEntityHandle PenguinHandle;

		public readonly PositionData PositionData;

		public event Action<GameObject, DataEntityHandle, LODRequestData> OnGameObjectGeneratedEvent;

		public event Action<GameObject, DataEntityHandle, LODRequestData> OnGameObjectRevokedEvent;

		public LODRequestData(string type, DataEntityHandle penguinHandle, PositionData positionData)
		{
			this = default(LODRequestData);
			Type = type;
			PenguinHandle = penguinHandle;
			PositionData = positionData;
		}

		public void OnGameObjectGenerated(GameObject generatedObject)
		{
			if (this.OnGameObjectGeneratedEvent != null)
			{
				this.OnGameObjectGeneratedEvent(generatedObject, PenguinHandle, this);
			}
		}

		public void OnGameObjectRevoked(GameObject revokedObject)
		{
			if (this.OnGameObjectRevokedEvent != null)
			{
				this.OnGameObjectRevokedEvent(revokedObject, PenguinHandle, this);
			}
		}
	}
}
