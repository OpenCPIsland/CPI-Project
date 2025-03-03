using UnityEngine;

namespace ClubPenguin.DCE
{
	public static class ShaderParams
	{
		public const int MAX_DECAL_CHANNELS = 6;

		public static readonly int[] DECAL_TEX;

		public static readonly int[] DECAL_COLOR;

		public static readonly int[] DECAL_SCALE;

		public static readonly int[] DECAL_U_OFFSET;

		public static readonly int[] DECAL_V_OFFSET;

		public static readonly int[] DECAL_REPEAT;

		public static readonly int[] DECAL_ROTATION_RADS;

		public static readonly int ATLAS_OFFSET_U;

		public static readonly int ATLAS_OFFSET_V;

		public static readonly int ATLAS_OFFSET_SCALE_U;

		public static readonly int ATLAS_OFFSET_SCALE_V;

		public static readonly int BODY_RED_CHANNEL_COLOR;

		public static readonly int BODY_GREEN_CHANNEL_COLOR;

		public static readonly int BODY_BLUE_CHANNEL_COLOR;

		public static readonly int DIFFUSE_TEX;

		public static readonly int BODY_COLORS_MASK_TEX;

		public static readonly int DETAIL_MATCAPMASK_EMISSIVE_TEX;

		public static readonly int DECALS_123_OPACITY_TEX;

		public static readonly int EMISSIVE_COLOR_TINT;

		static ShaderParams()
		{
			DECAL_TEX = new int[6];
			DECAL_COLOR = new int[6];
			DECAL_SCALE = new int[6];
			DECAL_U_OFFSET = new int[6];
			DECAL_V_OFFSET = new int[6];
			DECAL_REPEAT = new int[6];
			DECAL_ROTATION_RADS = new int[6];
			ATLAS_OFFSET_U = Shader.PropertyToID("_AtlasOffsetU");
			ATLAS_OFFSET_V = Shader.PropertyToID("_AtlasOffsetV");
			ATLAS_OFFSET_SCALE_U = Shader.PropertyToID("_AtlasOffsetScaleU");
			ATLAS_OFFSET_SCALE_V = Shader.PropertyToID("_AtlasOffsetScaleV");
			BODY_RED_CHANNEL_COLOR = Shader.PropertyToID("_BodyRedChannelColor");
			BODY_GREEN_CHANNEL_COLOR = Shader.PropertyToID("_BodyGreenChannelColor");
			BODY_BLUE_CHANNEL_COLOR = Shader.PropertyToID("_BodyBlueChannelColor");
			DIFFUSE_TEX = Shader.PropertyToID("_Diffuse");
			BODY_COLORS_MASK_TEX = Shader.PropertyToID("_BodyColorsMaskTex");
			DETAIL_MATCAPMASK_EMISSIVE_TEX = Shader.PropertyToID("_DetailAndMatcapMaskAndEmissive");
			DECALS_123_OPACITY_TEX = Shader.PropertyToID("_Decal123OpacityTex");
			EMISSIVE_COLOR_TINT = Shader.PropertyToID("_EmissiveColorTint");
			for (int i = 0; i < 6; i++)
			{
				string str = "_Decal" + (i + 1);
				DECAL_TEX[i] = Shader.PropertyToID(str + "Tex");
				DECAL_COLOR[i] = Shader.PropertyToID(str + "Color");
				DECAL_SCALE[i] = Shader.PropertyToID(str + "Scale");
				DECAL_U_OFFSET[i] = Shader.PropertyToID(str + "UOffset");
				DECAL_V_OFFSET[i] = Shader.PropertyToID(str + "VOffset");
				DECAL_REPEAT[i] = Shader.PropertyToID(str + "Repeat");
				DECAL_ROTATION_RADS[i] = Shader.PropertyToID(str + "RotationRads");
			}
		}
	}
}
