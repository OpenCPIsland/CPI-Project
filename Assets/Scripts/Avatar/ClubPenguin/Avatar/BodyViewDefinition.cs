using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	[Serializable]
	public class BodyViewDefinition : BaseViewDefinition
	{
		public SkinnedMeshDefinition SkinnedMesh;

		public BodyMaterialProperties BodyMaterial;

		public override void ApplyToViewPart(ViewPart partView)
		{
			partView.SetMeshDefinition(SkinnedMesh);
			partView.InitMaterialProps();
			partView.AddMaterialProps(BodyMaterial);
		}

		public override List<UnityEngine.Object> InternalReferences()
		{
			List<UnityEngine.Object> list = SkinnedMesh.InternalReferences();
			list.AddRange(BodyMaterial.InternalReferences());
			return list;
		}
	}
}
