using System.Collections.Generic;
using Tweaker.Core;

namespace Tweaker.UI.Testbed
{
	public class LogManager : ITweakerLogManager
	{
		private Dictionary<string, ITweakerLogger> loggers = new Dictionary<string, ITweakerLogger>();

		public ITweakerLogger GetLogger(string name)
		{
			ITweakerLogger value = null;
			if (!loggers.TryGetValue(name, out value))
			{
				value = new Log(name);
				loggers.Add(name, value);
			}
			return value;
		}
	}
}
