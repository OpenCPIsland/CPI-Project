using Foundation.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	[Serializable]
	public class MeshDefinition
	{
		public string Name;

		public Mesh Mesh;

		public virtual Renderer CreateRenderer(GameObject go)
		{
			MeshRenderer result = go.AddComponent<MeshRenderer>();
			go.AddComponent<MeshFilter>();
			return result;
		}

		public virtual void ApplyMesh(GameObject go, Mesh overrideMesh = null)
		{
			SetSharedMesh(go, (overrideMesh != null) ? overrideMesh : Mesh);
		}

		public virtual void CleanUp(GameObject go)
		{
			SetSharedMaterial(go, null);
			SetSharedMesh(go, null);
		}

		public override string ToString()
		{
			return string.Format("[SkinnedMesh] '{0}' Mesh: {1} [{2:x8}]", Name, (Mesh != null) ? Mesh.name : "-", (Mesh != null) ? Mesh.GetHash() : 0);
		}

		public virtual Material CreateCombinedMaterial(Texture atlas)
		{
			Material material = new Material(AvatarService.CombinedMeshShader);
			material.mainTexture = atlas;
			return material;
		}

		protected virtual void SetSharedMesh(GameObject go, Mesh mesh)
		{
			MeshFilter component = go.GetComponent<MeshFilter>();
			component.sharedMesh = mesh;
		}

		protected virtual Mesh GetSharedMesh(GameObject go)
		{
			MeshFilter component = go.GetComponent<MeshFilter>();
			return component.sharedMesh;
		}

		protected virtual void SetSharedMaterial(GameObject go, Material material)
		{
			Renderer component = go.GetComponent<Renderer>();
			component.sharedMaterial = material;
		}

		protected virtual Material GetSharedMaterial(GameObject go)
		{
			Renderer component = go.GetComponent<Renderer>();
			return component.sharedMaterial;
		}

		public virtual List<UnityEngine.Object> InternalReferences()
		{
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			list.Add(Mesh);
			return list;
		}
	}
}
