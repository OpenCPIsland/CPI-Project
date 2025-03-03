using System.Collections.Generic;

namespace DisneyMobile.CoreUnitySystems
{
	public interface IConfigurable
	{
		void Configure(IDictionary<string, object> dictionary);

		void Reconfigure(IDictionary<string, object> dictionary);
	}
}
