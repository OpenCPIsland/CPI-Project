using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	public class RemoveBackFaces : HiddenSurfaceRemoval
	{
		private GameObjectData[] gameObjectDatas;

		public float CullGeoBelowDotProduct = -0.18f;

		public bool Force360FOV = false;

		public override void Begin(GameObjectData[] datas)
		{
			gameObjectDatas = datas;
		}

		public override void Execute(Visibility visibility)
		{
			GameObjectData[] array = gameObjectDatas;
			foreach (GameObjectData gameObjectData in array)
			{
				Vector3 position = visibility.Position;
				Vector3[] vertices = gameObjectData.MeshFilter.sharedMesh.vertices;
				int[] triangles = gameObjectData.MeshFilter.sharedMesh.triangles;
				bool[] vertexVisibilities = gameObjectData.VertexVisibilities;
				bool[] triangleVisibilities = gameObjectData.TriangleVisibilities;
				Transform transform = gameObjectData.transform;
				Vector3[] array2 = new Vector3[vertices.Length];
				for (int j = 0; j < vertices.Length; j++)
				{
					array2[j] = transform.TransformPoint(vertices[j]);
				}
				int num = triangles.Length / 3;
				for (int j = 0; j < num; j++)
				{
					Vector3 vector = array2[triangles[j * 3]];
					Vector3 vector2 = array2[triangles[j * 3 + 1]];
					Vector3 vector3 = array2[triangles[j * 3 + 2]];
					Vector3 lhs = vector2 - vector;
					Vector3 rhs = vector3 - vector;
					Vector3 lhs2 = Vector3.Cross(lhs, rhs);
					lhs2.Normalize();
					Vector3 b = (vector + vector2 + vector3) * 0.333333343f;
					Vector3 vector4 = position - b;
					vector4.Normalize();
					float num2 = Vector3.Dot(lhs2, vector4);
					bool flag = true;
					if (!Force360FOV)
					{
						float num3 = 1f - visibility.Fov / 180f;
						Vector3 rhs2 = -visibility.Forward;
						flag = (Vector3.Dot(vector4, rhs2) >= num3);
						if (!flag)
						{
							Vector3 lhs3 = position - vector;
							lhs3.Normalize();
							flag = (Vector3.Dot(lhs3, rhs2) >= num3);
							if (!flag)
							{
								Vector3 lhs4 = position - vector2;
								lhs4.Normalize();
								flag = (Vector3.Dot(lhs4, rhs2) >= num3);
								if (!flag)
								{
									Vector3 lhs5 = position - vector3;
									lhs5.Normalize();
									flag = (Vector3.Dot(lhs5, rhs2) >= num3);
								}
							}
						}
					}
					bool flag2 = num2 >= CullGeoBelowDotProduct && flag;
					vertexVisibilities[triangles[j * 3]] |= flag2;
					vertexVisibilities[triangles[j * 3 + 1]] |= flag2;
					vertexVisibilities[triangles[j * 3 + 2]] |= flag2;
					triangleVisibilities[j] |= flag2;
				}
			}
		}

		public override void End()
		{
		}
	}
}
