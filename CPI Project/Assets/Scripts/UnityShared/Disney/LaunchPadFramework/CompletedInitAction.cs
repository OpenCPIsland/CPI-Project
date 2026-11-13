using Disney.LaunchPadFramework.Utility.Algorithms;
using System.Collections.Generic;

namespace Disney.LaunchPadFramework
{
	public class CompletedInitAction : ITopologicalNode
	{
		private string m_identifier;

		private List<string> m_dependencies;

		public string TopologicalIdentifier
		{
			get
			{
				return m_identifier;
			}
		}

		public List<string> TopologicalDependencies
		{
			get
			{
				return m_dependencies;
			}
		}

		public CompletedInitAction(string id, List<string> dependencies)
		{
			m_identifier = id;
			m_dependencies = dependencies;
		}
	}
}
