using System;
using UnityEngine;

namespace ClubPenguin.DCE
{
	[Serializable]
	public class BakeMaterialProperties : BaseMaterialProperties
	{
		public float AtlasOffsetU;

		public float AtlasOffsetV;

		public float AtlasOffsetScaleU;

		public float AtlasOffsetScaleV;

		public BakeMaterialProperties(Material mat = null)
		{
			if (mat != null)
			{
				AtlasOffsetU = mat.GetFloat(ShaderParams.ATLAS_OFFSET_U);
				AtlasOffsetV = mat.GetFloat(ShaderParams.ATLAS_OFFSET_V);
				AtlasOffsetScaleU = mat.GetFloat(ShaderParams.ATLAS_OFFSET_SCALE_U);
				AtlasOffsetScaleV = mat.GetFloat(ShaderParams.ATLAS_OFFSET_SCALE_V);
			}
		}

		public override void Apply(Material mat)
		{
			mat.SetFloat(ShaderParams.ATLAS_OFFSET_U, AtlasOffsetU);
			mat.SetFloat(ShaderParams.ATLAS_OFFSET_V, AtlasOffsetV);
			mat.SetFloat(ShaderParams.ATLAS_OFFSET_SCALE_U, AtlasOffsetScaleU);
			mat.SetFloat(ShaderParams.ATLAS_OFFSET_SCALE_V, AtlasOffsetScaleV);
		}

		public override string ToString()
		{
			return string.Format("[BakeMaterial : AtlasOffsetU={0}, AtlasOffsetV={1}, AtlasOffsetScaleU={2}, AtlasOffsetScaleV={3}", AtlasOffsetU, AtlasOffsetV, AtlasOffsetScaleU, AtlasOffsetScaleV);
		}
	}
}
