using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Builders
{
	public class SequentialStrategy : CombiningStrategy
	{
		public SequentialStrategy(IEnumerable[] sources)
			: base(sources)
		{
		}

		public override IEnumerable<ITestCaseData> GetTestCases()
		{
			List<ITestCaseData> list = new List<ITestCaseData>();
			while (true)
			{
				bool flag = true;
				bool flag2 = false;
				object[] array = new object[base.Sources.Length];
				for (int i = 0; i < base.Sources.Length; i++)
				{
					if (base.Enumerators[i].MoveNext())
					{
						array[i] = base.Enumerators[i].Current;
						flag2 = true;
					}
					else
					{
						array[i] = null;
					}
				}
				if (!flag2)
				{
					break;
				}
				ParameterSet parameterSet = new ParameterSet();
				parameterSet.Arguments = array;
				list.Add(parameterSet);
			}
			return list;
		}
	}
}
