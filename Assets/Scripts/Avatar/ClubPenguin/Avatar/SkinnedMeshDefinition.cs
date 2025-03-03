using Disney.LaunchPadFramework;
using Foundation.Unity;
using System;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	[Serializable]
	public class SkinnedMeshDefinition : MeshDefinition
	{
		public string RootBoneName;

		public string[] BoneNames;

		private bool UseGpuSkinning = false;

		public SkinnedMeshDefinition(bool useGpuSkinning = false)
		{
			UseGpuSkinning = useGpuSkinning;
		}

		public override Renderer CreateRenderer(GameObject go)
		{
			Renderer result;
			if (UseGpuSkinning)
			{
				result = go.AddComponent<MeshRenderer>();
				go.AddComponent<MeshFilter>();
				go.AddComponent<GpuSkinnedRenderer>();
			}
			else
			{
				result = go.AddComponent<SkinnedMeshRenderer>();
			}
			return result;
		}

		public override void ApplyMesh(GameObject go, Mesh overrideMesh = null)
		{
			Rig componentInParent = go.GetComponentInParent<Rig>();
			if (BoneNames == null)
			{
				Log.LogErrorFormatted(this, "BoneNames must be set before calling ApplyMesh");
				return;
			}
			if (!componentInParent)
			{
				Log.LogErrorFormatted(this, "Rig component missing");
				return;
			}
			Transform[] array = new Transform[BoneNames.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = componentInParent[BoneNames[i]];
			}
			setSharedMesh(go, (overrideMesh != null) ? overrideMesh : Mesh);
			Transform rootBone = componentInParent[RootBoneName];
			SetBones(go, array, rootBone);
		}

		public override void CleanUp(GameObject go)
		{
			ComponentExtensions.DestroyIfInstance(getSharedMesh(go));
			setSharedMesh(go, null);
			Material sharedMaterial = getSharedMaterial(go);
			if ((bool)sharedMaterial)
			{
				if (sharedMaterial.HasProperty("_MainTex"))
				{
					ComponentExtensions.DestroyIfInstance(sharedMaterial.mainTexture);
				}
				ComponentExtensions.DestroyIfInstance(sharedMaterial);
				setSharedMaterial(go, null);
			}
		}

		public override string ToString()
		{
			return string.Format("[SkinnedMesh] '{0}' RootBone={1}, #BoneNames={2}, Mesh: {3} [{4:x8}]", Name, RootBoneName, (BoneNames != null) ? BoneNames.Length.ToString() : "-", (Mesh != null) ? Mesh.name : "-", (Mesh != null) ? Mesh.GetHash() : 0);
		}

		public override Material CreateCombinedMaterial(Texture atlas)
		{
			Material material = new Material(UseGpuSkinning ? AvatarService.GpuCombinedMeshShader : AvatarService.CombinedMeshShader);
			material.mainTexture = atlas;
			return material;
		}

		private void setSharedMesh(GameObject go, Mesh mesh)
		{
			if (UseGpuSkinning)
			{
				MeshFilter component = go.GetComponent<MeshFilter>();
				component.sharedMesh = mesh;
			}
			else
			{
				SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
				component2.sharedMesh = mesh;
			}
		}

		private Mesh getSharedMesh(GameObject go)
		{
			if (UseGpuSkinning)
			{
				MeshFilter component = go.GetComponent<MeshFilter>();
				return component.sharedMesh;
			}
			SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
			return component2.sharedMesh;
		}

		private void setSharedMaterial(GameObject go, Material mat)
		{
			if (UseGpuSkinning)
			{
				MeshRenderer component = go.GetComponent<MeshRenderer>();
				component.sharedMaterial = mat;
			}
			else
			{
				SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
				component2.sharedMaterial = mat;
			}
		}

		private Material getSharedMaterial(GameObject go)
		{
			if (UseGpuSkinning)
			{
				MeshRenderer component = go.GetComponent<MeshRenderer>();
				return component.sharedMaterial;
			}
			SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
			return component2.sharedMaterial;
		}

		protected virtual void SetBones(GameObject go, Transform[] bones, Transform rootBone)
		{
			if (UseGpuSkinning)
			{
				GpuSkinnedRenderer component = go.GetComponent<GpuSkinnedRenderer>();
				component.init(bones, true);
			}
			else
			{
				SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
				component2.rootBone = rootBone;
				component2.bones = bones;
			}
		}
	}
}
