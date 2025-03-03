using System.Collections.Generic;

namespace Disney.LaunchPadFramework
{
	public interface IDataInjection
	{
		void InjectData(Dictionary<string, object> injectedData);
	}
}
