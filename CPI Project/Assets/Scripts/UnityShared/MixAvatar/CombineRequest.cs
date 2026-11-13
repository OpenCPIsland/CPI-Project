using System.Collections.Generic;
using UnityEngine;

namespace MixAvatar
{
	public class CombineRequest
	{
		public readonly AvatarMeshCombineData MeshCombineData;

		public readonly GameObject TargetMeshGameObject;

		public readonly GameObject TargetBonesGameObject;

		public readonly IMaterialToTextureBaker TextureBaker;

		public readonly Shader CombinedMeshShader;

		public readonly MipMapping UseMipMaps;

		public int[] NumVerticesPerSubMesh;

		public List<SkinnedMeshRenderer> SubMeshes;

		public SkinnedMeshRenderer CombinedSkinMeshRenderer;

		public List<Texture2D> BakedTextures;

		public CombinedMeshAtlasData AtlasData;

		public Transform RootBone;

		public bool Finished;

		public CombineRequest(AvatarMeshCombineData meshCombineData, GameObject targetMeshGameObject, GameObject targetBonesGameObject, IMaterialToTextureBaker textureBaker, Shader combinedMeshShader, MipMapping useMipMaps)
		{
			MeshCombineData = meshCombineData;
			TargetMeshGameObject = targetMeshGameObject;
			TargetBonesGameObject = targetBonesGameObject;
			TextureBaker = textureBaker;
			CombinedMeshShader = combinedMeshShader;
			UseMipMaps = useMipMaps;
		}
	}
}
