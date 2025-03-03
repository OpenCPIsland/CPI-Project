using System.Collections.Generic;

namespace Disney.LaunchPadFramework
{
	public interface IConfigurable
	{
		void Configure(IDictionary<string, object> dictionary);
	}
}
