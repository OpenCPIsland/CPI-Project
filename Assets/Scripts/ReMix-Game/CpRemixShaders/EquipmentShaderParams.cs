using System;
using UnityEngine;

namespace CpRemixShaders
{
	public class EquipmentShaderParams
	{
		public const string EQUIPMENT_PREVIEW_SHADER_NAME = "CpRemix/Equipment Preview";

		public const string EQUIPMENT_BAKE_SHADER_NAME = "CpRemix/Equipment Bake";

		public const string BODY_PREVIEW_SHADER_NAME = "CpRemix/Avatar Body Preview";

		public const string BODY_BAKE_SHADER_NAME = "CpRemix/Avatar Body Bake";

		public static readonly int DIFFUSE_TEX = Shader.PropertyToID("_Diffuse");

		public static readonly int DECALS_123_OPACITY_TEX = Shader.PropertyToID("_Decal123OpacityTex");

		public static readonly int DECAL_RED_1_TEX = Shader.PropertyToID("_Decal1Tex");

		public static readonly int DECAL_RED_1_COLOR = Shader.PropertyToID("_Decal1Color");

		public static readonly int DECAL_RED_1_SCALE = Shader.PropertyToID("_Decal1Scale");

		public static readonly int DECAL_RED_1_U_OFFSET = Shader.PropertyToID("_Decal1UOffset");

		public static readonly int DECAL_RED_1_V_OFFSET = Shader.PropertyToID("_Decal1VOffset");

		public static readonly int DECAL_RED_1_REPEAT = Shader.PropertyToID("_Decal1Repeat");

		public static readonly int DECAL_RED_1_ROTATION_RADS = Shader.PropertyToID("_Decal1RotationRads");

		public static readonly int DECAL_GREEN_2_TEX = Shader.PropertyToID("_Decal2Tex");

		public static readonly int DECAL_GREEN_2_COLOR = Shader.PropertyToID("_Decal2Color");

		public static readonly int DECAL_GREEN_2_SCALE = Shader.PropertyToID("_Decal2Scale");

		public static readonly int DECAL_GREEN_2_U_OFFSET = Shader.PropertyToID("_Decal2UOffset");

		public static readonly int DECAL_GREEN_2_V_OFFSET = Shader.PropertyToID("_Decal2VOffset");

		public static readonly int DECAL_GREEN_2_REPEAT = Shader.PropertyToID("_Decal2Repeat");

		public static readonly int DECAL_GREEN_2_ROTATION_RADS = Shader.PropertyToID("_Decal2RotationRads");

		public static readonly int DECAL_BLUE_3_TEX = Shader.PropertyToID("_Decal3Tex");

		public static readonly int DECAL_BLUE_3_COLOR = Shader.PropertyToID("_Decal3Color");

		public static readonly int DECAL_BLUE_3_SCALE = Shader.PropertyToID("_Decal3Scale");

		public static readonly int DECAL_BLUE_3_U_OFFSET = Shader.PropertyToID("_Decal3UOffset");

		public static readonly int DECAL_BLUE_3_V_OFFSET = Shader.PropertyToID("_Decal3VOffset");

		public static readonly int DECAL_BLUE_3_REPEAT = Shader.PropertyToID("_Decal3Repeat");

		public static readonly int DECAL_BLUE_3_ROTATION_RADS = Shader.PropertyToID("_Decal3RotationRads");

		public static readonly int DECAL_RED_4_TEX = Shader.PropertyToID("_Decal4Tex");

		public static readonly int DECAL_RED_4_COLOR = Shader.PropertyToID("_Decal4Color");

		public static readonly int DECAL_RED_4_SCALE = Shader.PropertyToID("_Decal4Scale");

		public static readonly int DECAL_RED_4_U_OFFSET = Shader.PropertyToID("_Decal4UOffset");

		public static readonly int DECAL_RED_4_V_OFFSET = Shader.PropertyToID("_Decal4VOffset");

		public static readonly int DECAL_RED_4_REPEAT = Shader.PropertyToID("_Decal4Repeat");

		public static readonly int DECAL_RED_4_ROTATION_RADS = Shader.PropertyToID("_Decal4RotationRads");

		public static readonly int DECAL_GREEN_5_TEX = Shader.PropertyToID("_Decal5Tex");

		public static readonly int DECAL_GREEN_5_COLOR = Shader.PropertyToID("_Decal5Color");

		public static readonly int DECAL_GREEN_5_SCALE = Shader.PropertyToID("_Decal5Scale");

		public static readonly int DECAL_GREEN_5_U_OFFSET = Shader.PropertyToID("_Decal5UOffset");

		public static readonly int DECAL_GREEN_5_V_OFFSET = Shader.PropertyToID("_Decal5VOffset");

		public static readonly int DECAL_GREEN_5_REPEAT = Shader.PropertyToID("_Decal5Repeat");

		public static readonly int DECAL_GREEN_5_ROTATION_RADS = Shader.PropertyToID("_Decal5RotationRads");

		public static readonly int DECAL_BLUE_6_TEX = Shader.PropertyToID("_Decal6Tex");

		public static readonly int DECAL_BLUE_6_COLOR = Shader.PropertyToID("_Decal6Color");

		public static readonly int DECAL_BLUE_6_SCALE = Shader.PropertyToID("_Decal6Scale");

		public static readonly int DECAL_BLUE_6_U_OFFSET = Shader.PropertyToID("_Decal6UOffset");

		public static readonly int DECAL_BLUE_6_V_OFFSET = Shader.PropertyToID("_Decal6VOffset");

		public static readonly int DECAL_BLUE_6_REPEAT = Shader.PropertyToID("_Decal6Repeat");

		public static readonly int DECAL_BLUE_6_ROTATION_RADS = Shader.PropertyToID("_Decal6RotationRads");

		public static readonly int BODY_COLORS_MASK_TEX = Shader.PropertyToID("_BodyColorsMaskTex");

		public static readonly int BODY_RED_CHANNEL_COLOR = Shader.PropertyToID("_BodyRedChannelColor");

		public static readonly int BODY_GREEN_CHANNEL_COLOR = Shader.PropertyToID("_BodyGreenChannelColor");

		public static readonly int BODY_BLUE_CHANNEL_COLOR = Shader.PropertyToID("_BodyBlueChannelColor");

		public static readonly int EMISSIVE_COLOR_TINT = Shader.PropertyToID("_EmissiveColorTint");

		public static readonly int DETAIL_MATCAPMASK_EMISSIVE_TEX = Shader.PropertyToID("_DetailAndMatcapMaskAndEmissive");

		public static readonly int MATCAP_RGB = Shader.PropertyToID("_MatCapRGB");

		public static readonly int ATLAS_OFFSET_U = Shader.PropertyToID("_AtlasOffsetU");

		public static readonly int ATLAS_OFFSET_V = Shader.PropertyToID("_AtlasOffsetV");

		public static readonly int ATLAS_SCALE_U = Shader.PropertyToID("_AtlasOffsetScaleU");

		public static readonly int ATLAS_SCALE_V = Shader.PropertyToID("_AtlasOffsetScaleV");

		public Texture DiffuseTexture;

		public Texture Decals123OpacityTexture;

		public Texture DecalRed1Texture;

		public Color DecalRed1Color;

		public float DecalRed1Scale;

		public float DecalRed1UOffset;

		public float DecalRed1VOffset;

		public bool DecalRed1Repeat;

		public float DecalRed1RotationRads;

		public Texture DecalGreen2Texture;

		public Color DecalGreen2Color;

		public float DecalGreen2Scale;

		public float DecalGreen2UOffset;

		public float DecalGreen2VOffset;

		public bool DecalGreen2Repeat;

		public float DecalGreen2RotationRads;

		public Texture DecalBlue3Texture;

		public Color DecalBlue3Color;

		public float DecalBlue3Scale;

		public float DecalBlue3UOffset;

		public float DecalBlue3VOffset;

		public bool DecalBlue3Repeat;

		public float DecalBlue3RotationRads;

		public Texture DecalRed4Texture;

		public Color DecalRed4Color;

		public float DecalRed4Scale;

		public float DecalRed4UOffset;

		public float DecalRed4VOffset;

		public bool DecalRed4Repeat;

		public float DecalRed4RotationRads;

		public Texture DecalGreen5Texture;

		public Color DecalGreen5Color;

		public float DecalGreen5Scale;

		public float DecalGreen5UOffset;

		public float DecalGreen5VOffset;

		public bool DecalGreen5Repeat;

		public float DecalGreen5RotationRads;

		public Texture DecalBlue6Texture;

		public Color DecalBlue6Color;

		public float DecalBlue6Scale;

		public float DecalBlue6UOffset;

		public float DecalBlue6VOffset;

		public bool DecalBlue6Repeat;

		public float DecalBlue6RotationRads;

		public Texture BodyColorsMaskTexture;

		public Color BodyRedChannelColor;

		public Color BodyGreenChannelColor;

		public Color BodyBlueChannelColor;

		public Color EmissiveColorTint;

		public Texture DetailMatcapmaslEmissiveTex;

		public float AtlasOffsetU;

		public float AtlasOffsetV;

		public float AtlasScaleU;

		public float AtlasScaleV;

		private static void validateShader(Shader shader)
		{
			if (shader.name != "CpRemix/Equipment Preview" && shader.name != "CpRemix/Equipment Bake" && shader.name != "CpRemix/Avatar Body Preview" && shader.name != "CpRemix/Avatar Body Bake")
			{
				throw new Exception("Material must use one of following shaders: CpRemix/Equipment Preview, CpRemix/Equipment Bake, CpRemix/Avatar Body Preview, CpRemix/Avatar Body Bake");
			}
		}

		public void ApplyToMaterial(Material material)
		{
			validateShader(material.shader);
			material.SetTexture(DIFFUSE_TEX, DiffuseTexture);
			if (material.shader.name == "CpRemix/Equipment Preview" || material.shader.name == "CpRemix/Equipment Bake")
			{
				material.SetTexture(DECALS_123_OPACITY_TEX, Decals123OpacityTexture);
				material.SetTexture(DECAL_RED_1_TEX, DecalRed1Texture);
				material.SetColor(DECAL_RED_1_COLOR, DecalRed1Color);
				material.SetFloat(DECAL_RED_1_SCALE, DecalRed1Scale);
				material.SetFloat(DECAL_RED_1_U_OFFSET, DecalRed1UOffset);
				material.SetFloat(DECAL_RED_1_V_OFFSET, DecalRed1VOffset);
				material.SetFloat(DECAL_RED_1_REPEAT, DecalRed1Repeat ? 1f : 0f);
				material.SetFloat(DECAL_RED_1_ROTATION_RADS, DecalRed1RotationRads);
				material.SetTexture(DECAL_GREEN_2_TEX, DecalGreen2Texture);
				material.SetColor(DECAL_GREEN_2_COLOR, DecalGreen2Color);
				material.SetFloat(DECAL_GREEN_2_SCALE, DecalGreen2Scale);
				material.SetFloat(DECAL_GREEN_2_U_OFFSET, DecalGreen2UOffset);
				material.SetFloat(DECAL_GREEN_2_V_OFFSET, DecalGreen2VOffset);
				material.SetFloat(DECAL_GREEN_2_REPEAT, DecalGreen2Repeat ? 1f : 0f);
				material.SetFloat(DECAL_GREEN_2_ROTATION_RADS, DecalGreen2RotationRads);
				material.SetTexture(DECAL_BLUE_3_TEX, DecalBlue3Texture);
				material.SetColor(DECAL_BLUE_3_COLOR, DecalBlue3Color);
				material.SetFloat(DECAL_BLUE_3_SCALE, DecalBlue3Scale);
				material.SetFloat(DECAL_BLUE_3_U_OFFSET, DecalBlue3UOffset);
				material.SetFloat(DECAL_BLUE_3_V_OFFSET, DecalBlue3VOffset);
				material.SetFloat(DECAL_BLUE_3_REPEAT, DecalBlue3Repeat ? 1f : 0f);
				material.SetFloat(DECAL_BLUE_3_ROTATION_RADS, DecalBlue3RotationRads);
				material.SetTexture(DECAL_RED_4_TEX, DecalRed4Texture);
				material.SetColor(DECAL_RED_4_COLOR, DecalRed4Color);
				material.SetFloat(DECAL_RED_4_SCALE, DecalRed4Scale);
				material.SetFloat(DECAL_RED_4_U_OFFSET, DecalRed4UOffset);
				material.SetFloat(DECAL_RED_4_V_OFFSET, DecalRed4VOffset);
				material.SetFloat(DECAL_RED_4_REPEAT, DecalRed4Repeat ? 1f : 0f);
				material.SetFloat(DECAL_RED_4_ROTATION_RADS, DecalRed4RotationRads);
				material.SetTexture(DECAL_GREEN_5_TEX, DecalGreen5Texture);
				material.SetColor(DECAL_GREEN_5_COLOR, DecalGreen5Color);
				material.SetFloat(DECAL_GREEN_5_SCALE, DecalGreen5Scale);
				material.SetFloat(DECAL_GREEN_5_U_OFFSET, DecalGreen5UOffset);
				material.SetFloat(DECAL_GREEN_5_V_OFFSET, DecalGreen5VOffset);
				material.SetFloat(DECAL_GREEN_5_REPEAT, DecalGreen5Repeat ? 1f : 0f);
				material.SetFloat(DECAL_GREEN_5_ROTATION_RADS, DecalGreen5RotationRads);
				material.SetTexture(DECAL_BLUE_6_TEX, DecalBlue6Texture);
				material.SetColor(DECAL_BLUE_6_COLOR, DecalBlue6Color);
				material.SetFloat(DECAL_BLUE_6_SCALE, DecalBlue6Scale);
				material.SetFloat(DECAL_BLUE_6_U_OFFSET, DecalBlue6UOffset);
				material.SetFloat(DECAL_BLUE_6_V_OFFSET, DecalBlue6VOffset);
				material.SetFloat(DECAL_BLUE_6_REPEAT, DecalBlue6Repeat ? 1f : 0f);
				material.SetFloat(DECAL_BLUE_6_ROTATION_RADS, DecalBlue6RotationRads);
				material.SetColor(EMISSIVE_COLOR_TINT, EmissiveColorTint);
			}
			material.SetTexture(BODY_COLORS_MASK_TEX, BodyColorsMaskTexture);
			material.SetColor(BODY_RED_CHANNEL_COLOR, BodyRedChannelColor);
			material.SetColor(BODY_GREEN_CHANNEL_COLOR, BodyGreenChannelColor);
			material.SetColor(BODY_BLUE_CHANNEL_COLOR, BodyBlueChannelColor);
			material.SetTexture(DETAIL_MATCAPMASK_EMISSIVE_TEX, DetailMatcapmaslEmissiveTex);
			if (material.shader.name == "CpRemix/Equipment Bake" || material.shader.name == "CpRemix/Avatar Body Bake")
			{
				material.SetFloat(ATLAS_OFFSET_U, AtlasOffsetU);
				material.SetFloat(ATLAS_OFFSET_V, AtlasOffsetV);
				material.SetFloat(ATLAS_SCALE_U, AtlasScaleU);
				material.SetFloat(ATLAS_SCALE_V, AtlasScaleV);
			}
		}

		public static EquipmentShaderParams FromMaterial(Material material)
		{
			validateShader(material.shader);
			EquipmentShaderParams equipmentShaderParams = new EquipmentShaderParams();
			equipmentShaderParams.DiffuseTexture = material.GetTexture(DIFFUSE_TEX);
			if (material.shader.name == "CpRemix/Equipment Preview" || material.shader.name == "CpRemix/Equipment Bake")
			{
				equipmentShaderParams.Decals123OpacityTexture = material.GetTexture(DECALS_123_OPACITY_TEX);
				equipmentShaderParams.DecalRed1Texture = material.GetTexture(DECAL_RED_1_TEX);
				equipmentShaderParams.DecalRed1Color = material.GetColor(DECAL_RED_1_COLOR);
				equipmentShaderParams.DecalRed1Scale = material.GetFloat(DECAL_RED_1_SCALE);
				equipmentShaderParams.DecalRed1UOffset = material.GetFloat(DECAL_RED_1_U_OFFSET);
				equipmentShaderParams.DecalRed1VOffset = material.GetFloat(DECAL_RED_1_V_OFFSET);
				equipmentShaderParams.DecalRed1Repeat = (material.GetFloat(DECAL_RED_1_REPEAT) == 1f);
				equipmentShaderParams.DecalRed1RotationRads = material.GetFloat(DECAL_RED_1_ROTATION_RADS);
				equipmentShaderParams.DecalGreen2Texture = material.GetTexture(DECAL_GREEN_2_TEX);
				equipmentShaderParams.DecalGreen2Color = material.GetColor(DECAL_GREEN_2_COLOR);
				equipmentShaderParams.DecalGreen2Scale = material.GetFloat(DECAL_GREEN_2_SCALE);
				equipmentShaderParams.DecalGreen2UOffset = material.GetFloat(DECAL_GREEN_2_U_OFFSET);
				equipmentShaderParams.DecalGreen2VOffset = material.GetFloat(DECAL_GREEN_2_V_OFFSET);
				equipmentShaderParams.DecalGreen2Repeat = (material.GetFloat(DECAL_GREEN_2_REPEAT) == 1f);
				equipmentShaderParams.DecalGreen2RotationRads = material.GetFloat(DECAL_GREEN_2_ROTATION_RADS);
				equipmentShaderParams.DecalBlue3Texture = material.GetTexture(DECAL_BLUE_3_TEX);
				equipmentShaderParams.DecalBlue3Color = material.GetColor(DECAL_BLUE_3_COLOR);
				equipmentShaderParams.DecalBlue3Scale = material.GetFloat(DECAL_BLUE_3_SCALE);
				equipmentShaderParams.DecalBlue3UOffset = material.GetFloat(DECAL_BLUE_3_U_OFFSET);
				equipmentShaderParams.DecalBlue3VOffset = material.GetFloat(DECAL_BLUE_3_V_OFFSET);
				equipmentShaderParams.DecalBlue3Repeat = (material.GetFloat(DECAL_BLUE_3_REPEAT) == 1f);
				equipmentShaderParams.DecalBlue3RotationRads = material.GetFloat(DECAL_BLUE_3_ROTATION_RADS);
				equipmentShaderParams.DecalRed4Texture = material.GetTexture(DECAL_RED_4_TEX);
				equipmentShaderParams.DecalRed4Color = material.GetColor(DECAL_RED_4_COLOR);
				equipmentShaderParams.DecalRed4Scale = material.GetFloat(DECAL_RED_4_SCALE);
				equipmentShaderParams.DecalRed4UOffset = material.GetFloat(DECAL_RED_4_U_OFFSET);
				equipmentShaderParams.DecalRed4VOffset = material.GetFloat(DECAL_RED_4_V_OFFSET);
				equipmentShaderParams.DecalRed4Repeat = (material.GetFloat(DECAL_RED_4_REPEAT) == 1f);
				equipmentShaderParams.DecalRed4RotationRads = material.GetFloat(DECAL_RED_4_ROTATION_RADS);
				equipmentShaderParams.DecalGreen5Texture = material.GetTexture(DECAL_GREEN_5_TEX);
				equipmentShaderParams.DecalGreen5Color = material.GetColor(DECAL_GREEN_5_COLOR);
				equipmentShaderParams.DecalGreen5Scale = material.GetFloat(DECAL_GREEN_5_SCALE);
				equipmentShaderParams.DecalGreen5UOffset = material.GetFloat(DECAL_GREEN_5_U_OFFSET);
				equipmentShaderParams.DecalGreen5VOffset = material.GetFloat(DECAL_GREEN_5_V_OFFSET);
				equipmentShaderParams.DecalGreen5Repeat = (material.GetFloat(DECAL_GREEN_5_REPEAT) == 1f);
				equipmentShaderParams.DecalGreen5RotationRads = material.GetFloat(DECAL_GREEN_5_ROTATION_RADS);
				equipmentShaderParams.DecalBlue6Texture = material.GetTexture(DECAL_BLUE_6_TEX);
				equipmentShaderParams.DecalBlue6Color = material.GetColor(DECAL_BLUE_6_COLOR);
				equipmentShaderParams.DecalBlue6Scale = material.GetFloat(DECAL_BLUE_6_SCALE);
				equipmentShaderParams.DecalBlue6UOffset = material.GetFloat(DECAL_BLUE_6_U_OFFSET);
				equipmentShaderParams.DecalBlue6VOffset = material.GetFloat(DECAL_BLUE_6_V_OFFSET);
				equipmentShaderParams.DecalBlue6Repeat = (material.GetFloat(DECAL_BLUE_6_REPEAT) == 1f);
				equipmentShaderParams.DecalBlue6RotationRads = material.GetFloat(DECAL_BLUE_6_ROTATION_RADS);
				equipmentShaderParams.EmissiveColorTint = material.GetColor(EMISSIVE_COLOR_TINT);
			}
			equipmentShaderParams.BodyColorsMaskTexture = material.GetTexture(BODY_COLORS_MASK_TEX);
			equipmentShaderParams.BodyRedChannelColor = material.GetColor(BODY_RED_CHANNEL_COLOR);
			equipmentShaderParams.BodyGreenChannelColor = material.GetColor(BODY_GREEN_CHANNEL_COLOR);
			equipmentShaderParams.BodyBlueChannelColor = material.GetColor(BODY_BLUE_CHANNEL_COLOR);
			equipmentShaderParams.DetailMatcapmaslEmissiveTex = material.GetTexture(DETAIL_MATCAPMASK_EMISSIVE_TEX);
			if (material.shader.name == "CpRemix/Equipment Bake" || material.shader.name == "CpRemix/Avatar Body Bake")
			{
				equipmentShaderParams.AtlasOffsetU = material.GetFloat(ATLAS_OFFSET_U);
				equipmentShaderParams.AtlasOffsetV = material.GetFloat(ATLAS_OFFSET_V);
				equipmentShaderParams.AtlasScaleU = material.GetFloat(ATLAS_SCALE_U);
				equipmentShaderParams.AtlasScaleV = material.GetFloat(ATLAS_SCALE_V);
			}
			return equipmentShaderParams;
		}
	}
}
