using System.Collections.Generic;

namespace Disney.LaunchPadFramework.Utility.Algorithms
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
