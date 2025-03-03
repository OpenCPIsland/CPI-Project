using NUnit.Framework.Api;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Builders
{
	public abstract class CombiningStrategy
	{
		private IEnumerable[] sources;

		private IEnumerator[] enumerators;

		public IEnumerable[] Sources
		{
			get
			{
				return sources;
			}
		}

		public IEnumerator[] Enumerators
		{
			get
			{
				if (enumerators == null)
				{
					enumerators = new IEnumerator[Sources.Length];
					for (int i = 0; i < Sources.Length; i++)
					{
						enumerators[i] = Sources[i].GetEnumerator();
					}
				}
				return enumerators;
			}
		}

		public CombiningStrategy(IEnumerable[] sources)
		{
			this.sources = sources;
		}

		public abstract IEnumerable<ITestCaseData> GetTestCases();
	}
}
