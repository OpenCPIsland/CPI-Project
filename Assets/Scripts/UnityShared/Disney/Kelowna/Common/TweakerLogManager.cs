using System.Collections.Generic;
using Tweaker.Core;

namespace Disney.Kelowna.Common
{
	public class TweakerLogManager : ITweakerLogManager
	{
		private Dictionary<string, ITweakerLogger> loggers = new Dictionary<string, ITweakerLogger>();

		public ITweakerLogger GetLogger(string name)
		{
			ITweakerLogger value;
			if (!loggers.TryGetValue(name, out value))
			{
				value = new TweakerLogger(name);
				loggers.Add(name, value);
			}
			return value;
		}
	}
}
