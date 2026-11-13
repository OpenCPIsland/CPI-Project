using UnityEngine;

namespace CpRemixShaders
{
	public class EquipPartMatPropsOverride
	{
		public Material Material;

		public EquipmentShaderParams PropertiesOverride;

		public EquipPartMatPropsOverride(Material material, EquipmentShaderParams propertiesOverride)
		{
			Material = material;
			PropertiesOverride = propertiesOverride;
		}
	}
}
