using UnityEngine;

namespace Foundation.Unity
{
	public static class MeshExtensions
	{
		public static int GetHash(this Mesh mesh)
		{
			StructHash sh = default(StructHash);
			sh.Combine(mesh.bindposes);
			sh.Combine(mesh.blendShapeCount);
			sh.Combine(mesh.boneWeights);
			sh.Combine(mesh.bounds);
			sh.Combine(mesh.colors32);
			sh.Combine(mesh.normals);
			sh.Combine(mesh.subMeshCount);
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				sh.Combine(mesh.GetIndices(i));
			}
			sh.Combine(mesh.tangents);
			sh.Combine(mesh.triangles);
			sh.Combine(mesh.uv);
			sh.Combine(mesh.uv2);
			sh.Combine(mesh.uv3);
			sh.Combine(mesh.uv4);
			sh.Combine(mesh.vertexCount);
			sh.Combine(mesh.vertices);
			return sh;
		}
	}
}
