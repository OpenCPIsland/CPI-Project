using Foundation.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.DCE
{
	public class ViewPart
	{
		private List<TexturedMaterialProperties> TexturedMaterialProps;

		private MeshDefinition MeshDef;

		public void ResetMaterialProperties(Material mat)
		{
			if (TexturedMaterialProps != null)
			{
				for (int i = 0; i < TexturedMaterialProps.Count; i++)
				{
					TexturedMaterialProps[i].ResetMaterial(mat);
				}
			}
		}

		public void ApplyMaterialProperties(Material mat)
		{
			if (TexturedMaterialProps != null)
			{
				for (int i = 0; i < TexturedMaterialProps.Count; i++)
				{
					TexturedMaterialProps[i].Apply(mat);
				}
			}
		}

		public bool HasMaterialProperties(Type type)
		{
			bool flag = false;
			int num = 0;
			while (!flag && num < TexturedMaterialProps.Count)
			{
				flag = type.IsInstanceOfType(TexturedMaterialProps[num]);
				num++;
			}
			return flag;
		}

		public Material GetMaterial(bool baking = false)
		{
			int num = (TexturedMaterialProps != null) ? (TexturedMaterialProps.Count - 1) : (-1);
			Material material = null;
			int num2 = num;
			while (num2 >= 0 && !material)
			{
				material = TexturedMaterialProps[num2].GetMaterial(baking);
				num2--;
			}
			return material;
		}

		public Texture GetMaskTexture()
		{
			int num = (TexturedMaterialProps != null) ? (TexturedMaterialProps.Count - 1) : (-1);
			Texture texture = null;
			int num2 = num;
			while (num2 >= 0 && !texture)
			{
				texture = TexturedMaterialProps[num2].GetMaskTexture();
				num2--;
			}
			return texture;
		}

		public Vector2 GetTextureSize()
		{
			int num = (TexturedMaterialProps != null) ? (TexturedMaterialProps.Count - 1) : 0;
			Vector2 vector = Vector2.zero;
			int num2 = num;
			while (num2 >= 0 && Vector2.zero.Equals(vector))
			{
				vector = TexturedMaterialProps[num2].GetTextureSize();
				num2--;
			}
			return vector;
		}

		public Mesh GetMesh()
		{
			return MeshDef.Mesh;
		}

		public string[] GetBoneNames()
		{
			SkinnedMeshDefinition skinnedMeshDefinition = MeshDef as SkinnedMeshDefinition;
			if (skinnedMeshDefinition == null)
			{
				return null;
			}
			return skinnedMeshDefinition.BoneNames;
		}

		public void SetupRenderer(GameObject gameObject, DceModel model, ref Renderer rend)
		{
			if (rend == null)
			{
				rend = MeshDef.CreateRenderer(gameObject);
			}
			Material material = GetMaterial();
			ApplyMaterialProperties(material);
			ComponentExtensions.DestroyIfInstance(rend.sharedMaterial);
			rend.sharedMaterial = material;
			MeshDef.ApplyMesh(gameObject);
		}

		public void CleanUp(GameObject go)
		{
			Renderer component = go.GetComponent<Renderer>();
			if (component != null)
			{
				ComponentExtensions.DestroyIfInstance(component.sharedMaterial);
			}
			if (MeshDef != null)
			{
				MeshDef.CleanUp(go);
			}
		}

		public void SetMeshDefinition(MeshDefinition def)
		{
			MeshDef = def;
		}

		public void InitMaterialProps()
		{
			TexturedMaterialProps = new List<TexturedMaterialProperties>(2);
		}

		public void AddMaterialProps(TexturedMaterialProperties texturedMatProps)
		{
			TexturedMaterialProps.Add(texturedMatProps);
		}
	}
}
