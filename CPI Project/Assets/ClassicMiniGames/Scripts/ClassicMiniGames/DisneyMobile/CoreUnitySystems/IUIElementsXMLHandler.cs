using System.Collections.Generic;

namespace DisneyMobile.CoreUnitySystems
{
	public interface IUIElementsXMLHandler
	{
		Dictionary<string, string> WriteAttributesToDictionary();

		void ReadAttributesFromDictionary(Dictionary<string, string> attributes);
	}
}
