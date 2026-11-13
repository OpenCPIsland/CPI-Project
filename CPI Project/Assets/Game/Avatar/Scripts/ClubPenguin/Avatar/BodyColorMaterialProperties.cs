using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	[Serializable]
	public class BodyColorMaterialProperties : BaseMaterialProperties
	{
		public Color BeakColor;

		public Color BellyColor;

		public Color BodyColor;

		public BodyColorMaterialProperties()
		{
			BeakColor = new Color(1f, 0.78f, 0f);
			BellyColor = Color.white;
			BodyColor = new Color(0.09f, 0.1f, 0.84f);
		}

		public BodyColorMaterialProperties(Color beak, Color belly, Color body)
		{
			BeakColor = beak;
			BellyColor = belly;
			BodyColor = body;
		}

		public override void Apply(Material mat)
		{
			mat.SetColor(ShaderParams.BODY_RED_CHANNEL_COLOR, BeakColor);
			mat.SetColor(ShaderParams.BODY_GREEN_CHANNEL_COLOR, BellyColor);
			mat.SetColor(ShaderParams.BODY_BLUE_CHANNEL_COLOR, BodyColor);
		}

		public override List<UnityEngine.Object> InternalReferences()
		{
			return new List<UnityEngine.Object>();
		}

		public override string ToString()
		{
			return string.Format("[BodyColorMaterial : BeakColor={0}, BellyColor={1}, BodyColor={2}", BeakColor, BellyColor, BodyColor);
		}
	}
}
