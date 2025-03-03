using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Builders
{
	public class CombinatorialStrategy : CombiningStrategy
	{
		public CombinatorialStrategy(IEnumerable[] sources)
			: base(sources)
		{
		}

		public override IEnumerable<ITestCaseData> GetTestCases()
		{
			List<ITestCaseData> list = new List<ITestCaseData>();
			IEnumerator[] array = new IEnumerator[base.Sources.Length];
			int num = -1;
			do
			{
				bool flag = true;
				while (++num < base.Sources.Length)
				{
					array[num] = base.Sources[num].GetEnumerator();
					if (!array[num].MoveNext())
					{
						return list;
					}
				}
				object[] array2 = new object[base.Sources.Length];
				for (int i = 0; i < base.Sources.Length; i++)
				{
					array2[i] = array[i].Current;
				}
				ParameterSet parameterSet = new ParameterSet();
				parameterSet.Arguments = array2;
				list.Add(parameterSet);
				num = base.Sources.Length;
				while (--num >= 0 && !array[num].MoveNext())
				{
				}
			}
			while (num >= 0);
			return list;
		}
	}
}
