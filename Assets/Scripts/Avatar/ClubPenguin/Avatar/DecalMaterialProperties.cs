using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	[Serializable]
	public class DecalMaterialProperties : BaseMaterialProperties
	{
		public readonly int Channel;

		public Texture2D Texture;

		public float Scale;

		public float UOffset;

		public float VOffset;

		public bool Repeat;

		public float RotationRads;

		private static readonly DecalMaterialProperties[] DefaultProperties = new DecalMaterialProperties[6]
		{
			new DecalMaterialProperties(0),
			new DecalMaterialProperties(1),
			new DecalMaterialProperties(2),
			new DecalMaterialProperties(3),
			new DecalMaterialProperties(4),
			new DecalMaterialProperties(5)
		};

		public DecalMaterialProperties(int channel)
		{
			Channel = channel;
			Texture = null;
			Scale = 1f;
			UOffset = 0f;
			VOffset = 0f;
			Repeat = false;
			RotationRads = 0f;
		}

		public void Import(DCustomEquipmentDecal decal, Texture2D tex = null)
		{
			Texture = tex;
			Scale = decal.Scale;
			UOffset = decal.Uoffset;
			VOffset = decal.Voffset;
			Repeat = decal.Repeat;
			RotationRads = decal.Rotation;
		}

		public DCustomEquipmentDecal Export()
		{
			DCustomEquipmentDecal result = default(DCustomEquipmentDecal);
			result.TextureName = ((Texture != null) ? Texture.name : null);
			result.Scale = Scale;
			result.Uoffset = UOffset;
			result.Voffset = VOffset;
			result.Repeat = Repeat;
			result.Rotation = RotationRads;
			return result;
		}

		public override void Apply(Material mat)
		{
			mat.SetTexture(ShaderParams.DECAL_TEX[Channel], Texture);
			mat.SetFloat(ShaderParams.DECAL_SCALE[Channel], Scale);
			mat.SetFloat(ShaderParams.DECAL_U_OFFSET[Channel], UOffset);
			mat.SetFloat(ShaderParams.DECAL_V_OFFSET[Channel], VOffset);
			mat.SetFloat(ShaderParams.DECAL_REPEAT[Channel], Repeat ? 1f : 0f);
			mat.SetFloat(ShaderParams.DECAL_ROTATION_RADS[Channel], RotationRads);
		}

		public override List<UnityEngine.Object> InternalReferences()
		{
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			list.Add(Texture);
			return list;
		}

		public void ResetMaterial(Material mat)
		{
			DefaultProperties[Channel].Apply(mat);
		}

		public override string ToString()
		{
			return string.Format("[DecalMaterial: Channel={0}, Texture={1}, Scale={2}, UOffset={3}, VOffset={4}, Repeat={5}, RotationRads={6}]", Channel, Texture, Scale, UOffset, VOffset, Repeat, RotationRads);
		}
	}
}
