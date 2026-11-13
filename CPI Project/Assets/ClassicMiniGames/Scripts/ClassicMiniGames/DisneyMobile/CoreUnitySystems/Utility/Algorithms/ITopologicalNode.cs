using System.Collections.Generic;

namespace DisneyMobile.CoreUnitySystems.Utility.Algorithms
{
	public interface ITopologicalNode
	{
		string TopologicalIdentifier
		{
			get;
		}

		List<string> TopologicalDependencies
		{
			get;
		}
	}
}
