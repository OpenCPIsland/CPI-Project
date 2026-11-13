using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[Serializable]
	public class ObjectComponent
	{
		public string componentName;

		public Dictionary<string, object> fields;
	}
}
