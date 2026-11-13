using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public static class SequenceNumberUtils
	{
		public static IDictionary<long, long> CreateSequenceNumberDictionary(IList<long?> sequenceNumbers)
		{
			Dictionary<long, long> dictionary = new Dictionary<long, long>();
			if (sequenceNumbers != null)
			{
				for (int i = 0; i < sequenceNumbers.Count; i += 2)
				{
					if (sequenceNumbers[i].HasValue && sequenceNumbers[i + 1].HasValue)
					{
						dictionary[sequenceNumbers[i].Value] = sequenceNumbers[i + 1].Value;
					}
				}
			}
			return dictionary;
		}
	}
}
