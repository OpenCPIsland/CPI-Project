using System.Collections.Generic;

namespace Fabric
{
	public interface IRTPPropertyListener
	{
		List<RTPProperty> CollectProperties();

		bool UpdateProperty(RTPProperty property, float value, RTPPropertyType type = RTPPropertyType.Set);
	}
}
