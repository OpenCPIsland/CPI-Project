using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	public class EquipmentViewDefinition : BodyViewDefinition
	{
		public EquipmentMaterialProperties EquipmentMaterial;

		public DecalMaterialProperties[] DecalMaterials;

		public override void ApplyToViewPart(ViewPart partView)
		{
			base.ApplyToViewPart(partView);
			partView.SetDefaultProps(DecalMaterials);
			partView.AddMaterialProps(EquipmentMaterial);
		}

		public override List<Object> InternalReferences()
		{
			List<Object> list = base.InternalReferences();
			list.AddRange(EquipmentMaterial.InternalReferences());
			DecalMaterialProperties[] decalMaterials = DecalMaterials;
			foreach (DecalMaterialProperties decalMaterialProperties in decalMaterials)
			{
				list.AddRange(decalMaterialProperties.InternalReferences());
			}
			return list;
		}
	}
}
