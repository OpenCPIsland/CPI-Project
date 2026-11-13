using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	public class RemoveOccludedCPU : RemoveOccluded
	{
		private Dictionary<GameObjectData, Collider[]> goDataToPrevColliders;

		private Dictionary<GameObjectData, MeshCollider> goDataToTempMeshCollider;

		private MeshCollider[] meshColliders;

		public override void Begin(GameObjectData[] datas)
		{
			setup(datas);
			goDataToPrevColliders = new Dictionary<GameObjectData, Collider[]>();
			goDataToTempMeshCollider = new Dictionary<GameObjectData, MeshCollider>();
			meshColliders = new MeshCollider[gameObjectDatas.Length];
			setupMeshColliders();
		}

		public override void Execute(Visibility visibility)
		{
			for (int i = 0; i < gameObjectDatas.Length; i++)
			{
				FovDirectionPair fovDirPair = new FovDirectionPair(visibility.Fov, visibility.Forward, visibility.Up);
				Vector3[] raysForFovAndDirection = getRaysForFovAndDirection(fovDirPair, visibility.Right);
				for (int j = 0; j < raysForFovAndDirection.Length; j++)
				{
					RaycastHit hitInfo;
					if (Physics.Raycast(visibility.Position, raysForFovAndDirection[j], out hitInfo))
					{
						Transform transform = hitInfo.transform;
						if (transformToGoData.ContainsKey(transform))
						{
							GameObjectData gameObjectData = transformToGoData[transform];
							gameObjectData.TriangleVisibilities[hitInfo.triangleIndex] = true;
						}
					}
				}
			}
		}

		public override void End()
		{
			revertColliders();
			cleanup();
			goDataToPrevColliders = null;
			goDataToTempMeshCollider = null;
			meshColliders = null;
		}

		private void setupMeshColliders()
		{
			for (int i = 0; i < gameObjectDatas.Length; i++)
			{
				MeshCollider component = gameObjectDatas[i].GetComponent<MeshCollider>();
				if (component != null && !component.convex && component.enabled)
				{
					meshColliders[i] = component;
					continue;
				}
				Collider[] components = gameObjectDatas[i].GetComponents<Collider>();
				if (components.Length > 0)
				{
					goDataToPrevColliders.Add(gameObjectDatas[i], components);
					for (int j = 0; j < components.Length; j++)
					{
						components[j].enabled = false;
					}
				}
				MeshCollider meshCollider = gameObjectDatas[i].gameObject.AddComponent<MeshCollider>();
				meshColliders[i] = meshCollider;
				goDataToTempMeshCollider.Add(gameObjectDatas[i], meshCollider);
			}
		}

		private void revertColliders()
		{
			foreach (KeyValuePair<GameObjectData, MeshCollider> item in goDataToTempMeshCollider)
			{
				Object.DestroyImmediate(item.Value);
			}
			foreach (KeyValuePair<GameObjectData, Collider[]> goDataToPrevCollider in goDataToPrevColliders)
			{
				for (int i = 0; i < goDataToPrevCollider.Value.Length; i++)
				{
					goDataToPrevCollider.Value[i].enabled = true;
				}
			}
		}
	}
}
