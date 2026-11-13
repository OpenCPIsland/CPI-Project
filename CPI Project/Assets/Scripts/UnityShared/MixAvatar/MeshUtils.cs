using System;
using System.Collections.Generic;
using UnityEngine;

namespace MixAvatar
{
	public class MeshUtils
	{
		public static SkinnedMeshRenderer MergeSkinnedMeshes(List<SkinnedMeshRenderer> skinnedMeshRenderers, List<Transform> allBones, Dictionary<string, int> boneNamesToIndices, GameObject combinedMeshOwner, Matrix4x4 SubMeshesContainerWorldToLocalMat, out int[] numVerticesPerSubMesh)
		{
			CombineInstance[] array = new CombineInstance[skinnedMeshRenderers.Count];
			int num = 0;
			for (int i = 0; i < skinnedMeshRenderers.Count; i++)
			{
				if (skinnedMeshRenderers[i].sharedMesh.subMeshCount != 1)
				{
					throw new Exception("MergeSkinnedMeshes requires SkinnedMeshRenderers to have exactly 1 sub mesh.");
				}
				num += skinnedMeshRenderers[i].sharedMesh.vertexCount;
			}
			BoneWeight[] array2 = new BoneWeight[num];
			numVerticesPerSubMesh = new int[skinnedMeshRenderers.Count];
			int num2 = 0;
			for (int j = 0; j < skinnedMeshRenderers.Count; j++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = skinnedMeshRenderers[j];
				BoneWeight[] boneWeights = skinnedMeshRenderer.sharedMesh.boneWeights;
				Transform[] bones = skinnedMeshRenderer.bones;
				int[] array3 = new int[skinnedMeshRenderer.bones.Length];
				for (int k = 0; k < bones.Length; k++)
				{
					string name = bones[k].name;
					if (boneNamesToIndices.ContainsKey(name))
					{
						array3[k] = boneNamesToIndices[name];
					}
				}
				for (int l = 0; l < boneWeights.Length; l++)
				{
					BoneWeight boneWeight = boneWeights[l];
					int boneIndex = array3[boneWeight.boneIndex0];
					int boneIndex2 = array3[boneWeight.boneIndex1];
					int boneIndex3 = array3[boneWeight.boneIndex2];
					int boneIndex4 = array3[boneWeight.boneIndex3];
					boneWeight.boneIndex0 = boneIndex;
					boneWeight.boneIndex1 = boneIndex2;
					boneWeight.boneIndex2 = boneIndex3;
					boneWeight.boneIndex3 = boneIndex4;
					array2[num2] = boneWeight;
					num2++;
				}
				CombineInstance combineInstance = new CombineInstance
				{
					mesh = skinnedMeshRenderer.sharedMesh,
					transform = skinnedMeshRenderer.transform.localToWorldMatrix * combinedMeshOwner.transform.localToWorldMatrix
				};
				array[j] = combineInstance;
				numVerticesPerSubMesh[j] = combineInstance.mesh.vertexCount;
			}
			Matrix4x4[] array4 = new Matrix4x4[allBones.Count];
			for (int m = 0; m < allBones.Count; m++)
			{
				array4[m] = allBones[m].worldToLocalMatrix * SubMeshesContainerWorldToLocalMat;
			}
			Mesh mesh = new Mesh();
			mesh.CombineMeshes(array, true, true);
			mesh.bindposes = array4;
			mesh.boneWeights = array2;
			mesh.Optimize();
			SkinnedMeshRenderer skinnedMeshRenderer2 = combinedMeshOwner.GetComponent<SkinnedMeshRenderer>();
			if (skinnedMeshRenderer2 == null)
			{
				skinnedMeshRenderer2 = combinedMeshOwner.AddComponent<SkinnedMeshRenderer>();
			}
			skinnedMeshRenderer2.rootBone = allBones[0];
			skinnedMeshRenderer2.bones = allBones.ToArray();
			skinnedMeshRenderer2.sharedMesh = mesh;
			skinnedMeshRenderer2.sharedMesh.RecalculateBounds();
			return skinnedMeshRenderer2;
		}

		public static CombinedMeshAtlasData CreateCombinedMeshTextureAtlas(SkinnedMeshRenderer combinedSkinnedMeshRenderer, List<Texture2D> subMeshTextures, int[] numVerticesPerSubMesh, Shader combinedMeshShader, bool useMipMaps, int maxAtlasDimension = 4096)
		{
			Texture2D texture2D = new Texture2D(maxAtlasDimension, maxAtlasDimension, TextureFormat.ARGB32, useMipMaps);
			Rect[] atlasUVOffsets = texture2D.PackTextures(subMeshTextures.ToArray(), 0, maxAtlasDimension, true);
			OffsetUVsToAtlas(combinedSkinnedMeshRenderer, numVerticesPerSubMesh, atlasUVOffsets);
			Material material = new Material(combinedMeshShader);
			material.enableInstancing = true;
			material.mainTexture = texture2D;
			return new CombinedMeshAtlasData(atlasUVOffsets, material);
		}

		public static void OffsetUVsToAtlas(SkinnedMeshRenderer combinedSkinnedMeshRenderer, int[] numVerticesPerSubMesh, Rect[] atlasUVOffsets)
		{
			Vector2[] uv = combinedSkinnedMeshRenderer.sharedMesh.uv;
			Vector2[] array = new Vector2[uv.Length];
			int num = atlasUVOffsets.Length;
			bool flag = false;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (num2 < num)
				{
					array[i].x = Mathf.Lerp(atlasUVOffsets[num2].xMin, atlasUVOffsets[num2].xMax, uv[i].x);
					array[i].y = Mathf.Lerp(atlasUVOffsets[num2].yMin, atlasUVOffsets[num2].yMax, uv[i].y);
				}
				else
				{
					array[i].x = Mathf.Lerp(atlasUVOffsets[num - 1].xMin, atlasUVOffsets[num - 1].xMax, uv[i].x);
					array[i].y = Mathf.Lerp(atlasUVOffsets[num - 1].yMin, atlasUVOffsets[num - 1].yMax, uv[i].y);
					flag = true;
				}
				if (i == num3 + numVerticesPerSubMesh[num2] - 1)
				{
					num3 += numVerticesPerSubMesh[num2];
					num2++;
				}
			}
			combinedSkinnedMeshRenderer.sharedMesh.uv = array;
			if (!flag)
			{
			}
		}

		public static void ResetBonesToBindPoses(List<Transform> bones, List<BindPoseTransform> bindPoses)
		{
			if (bones.Count != bindPoses.Count)
			{
				throw new IndexOutOfRangeException("bones and bindPoses Count must be the same.");
			}
			for (int i = 0; i < bones.Count; i++)
			{
				bones[i].localPosition = bindPoses[i].LocalPosition;
				bones[i].localRotation = bindPoses[i].LocalRotation;
			}
		}
	}
}
