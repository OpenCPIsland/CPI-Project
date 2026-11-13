using System;
using UnityEngine;

namespace ClubPenguin.DCE
{
	[Serializable]
	public class DynamicLightingMaterialProperties : BaseMaterialProperties
	{
		public DynamicLightingMaterialProperties(Material mat = null)
		{
			if (!(mat != null))
			{
			}
		}

		public override void Apply(Material mat)
		{
		}

		public override string ToString()
		{
			return string.Format("[DynamicLightingMaterialProperties : ");
		}
	}
}
