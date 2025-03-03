using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	public abstract class RemoveOccluded : HiddenSurfaceRemoval
	{
		public struct FovDirectionPair
		{
			public float Fov;

			public Vector3 Forward;

			public Vector3 Up;

			public FovDirectionPair(float fov, Vector3 forward, Vector3 up)
			{
				Fov = fov;
				Forward = forward;
				Up = up;
			}

			public override bool Equals(object obj)
			{
				return obj is FovDirectionPair && this == (FovDirectionPair)obj;
			}

			public override int GetHashCode()
			{
				return Fov.GetHashCode() ^ Forward.GetHashCode() ^ Up.GetHashCode();
			}

			public static bool operator ==(FovDirectionPair x, FovDirectionPair y)
			{
				bool flag = Vector3.Angle(x.Forward, y.Forward) <= 5f;
				bool flag2 = Vector3.Angle(x.Up, y.Up) <= 5f;
				return Math.Abs(x.Fov - y.Fov) < float.Epsilon && flag && flag2;
			}

			public static bool operator !=(FovDirectionPair x, FovDirectionPair y)
			{
				return !(x == y);
			}
		}

		public int NumHorizRays = 300;

		public int NumVertRays = 300;

		protected GameObjectData[] gameObjectDatas;

		protected Dictionary<Transform, GameObjectData> transformToGoData;

		private Dictionary<FovDirectionPair, Vector3[]> fovDirectionPairToRays;

		protected void setup(GameObjectData[] datas)
		{
			gameObjectDatas = datas;
			transformToGoData = new Dictionary<Transform, GameObjectData>();
			for (int i = 0; i < gameObjectDatas.Length; i++)
			{
				transformToGoData.Add(gameObjectDatas[i].transform, gameObjectDatas[i]);
			}
			fovDirectionPairToRays = new Dictionary<FovDirectionPair, Vector3[]>();
		}

		protected void cleanup()
		{
			for (int i = 0; i < gameObjectDatas.Length; i++)
			{
				bool[] triangleVisibilities = gameObjectDatas[i].TriangleVisibilities;
				int[] triangles = gameObjectDatas[i].MeshFilter.sharedMesh.triangles;
				bool[] vertexVisibilities = gameObjectDatas[i].VertexVisibilities;
				for (int j = 0; j < triangleVisibilities.Length; j++)
				{
					if (triangleVisibilities[j])
					{
						int num = triangles[j * 3];
						int num2 = triangles[j * 3 + 1];
						int num3 = triangles[j * 3 + 2];
						vertexVisibilities[num] = true;
						vertexVisibilities[num2] = true;
						vertexVisibilities[num3] = true;
					}
				}
			}
			gameObjectDatas = null;
			transformToGoData = null;
			fovDirectionPairToRays = null;
		}

		protected Vector3[] getRaysForFovAndDirection(FovDirectionPair fovDirPair, Vector3 right)
		{
			Vector3[] array;
			if (!fovDirectionPairToRays.ContainsKey(fovDirPair))
			{
				array = GetRaysForFOV(fovDirPair.Fov, NumHorizRays, NumVertRays, fovDirPair.Forward, fovDirPair.Up, right);
				fovDirectionPairToRays.Add(fovDirPair, array);
			}
			else
			{
				array = fovDirectionPairToRays[fovDirPair];
			}
			return array;
		}

		public static Vector3[] GetRaysForFOV(float fov, int numHorizRays, int numVertRays, Vector3 forward, Vector3 up, Vector3 right)
		{
			float num = (numHorizRays <= 1) ? 0f : (fov / (float)(numHorizRays - 1));
			float num2 = (0f - fov) / 2f;
			float num3 = (numVertRays <= 1) ? 0f : (fov / (float)(numVertRays - 1));
			float num4 = (0f - fov) / 2f;
			Vector3[] array = new Vector3[numHorizRays * numVertRays];
			Vector3 vector = default(Vector3);
			int num5 = 0;
			for (int i = 0; i < numVertRays; i++)
			{
				num5 = numHorizRays * i;
				Quaternion rotation = Quaternion.AngleAxis(num4 + num3 * (float)i, right);
				Vector3 axis = rotation * up;
				Vector3 point = rotation * forward;
				for (int j = 0; j < numHorizRays; j++)
				{
					vector = Quaternion.AngleAxis(num2 + num * (float)j, axis) * point;
					array[num5 + j] = vector;
				}
			}
			return array;
		}
	}
}
