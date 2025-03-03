using DisneyMobile.CoreUnitySystems.Utility.Collections;
using System;
using System.Collections.Generic;

namespace DisneyMobile.CoreUnitySystems.Utility.Algorithms
{
	public static class TopologicalSort
	{
		public static List<ITopologicalNode> Sort(List<ITopologicalNode> unsortedList)
		{
			int count = unsortedList.Count;
			List<int> list = new List<int>(count);
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			Array2D<int> array2D = new Array2D<int>(count, count, 0);
			for (int i = 0; i < unsortedList.Count; i++)
			{
				list.Add(i);
				dictionary[unsortedList[i].TopologicalIdentifier] = i;
			}
			for (int i = 0; i < unsortedList.Count; i++)
			{
				if (unsortedList[i].TopologicalDependencies != null)
				{
					for (int j = 0; j < unsortedList[i].TopologicalDependencies.Count; j++)
					{
						int y = dictionary[unsortedList[i].TopologicalDependencies[j]];
						array2D.SetAt(i, y, 1);
					}
				}
			}
			List<ITopologicalNode> list2 = new List<ITopologicalNode>();
			while (list.Count > 0)
			{
				int num = FindNodeIndexWithNoDependencies(array2D);
				if (num < 0)
				{
					throw new Exception("Dependencies are cyclic!");
				}
				if (num > unsortedList.Capacity)
				{
					throw new Exception("Internal error: Node index is exceeding the maximum amount!");
				}
				list2.Add(unsortedList[list[num]]);
				list.RemoveAt(num);
				array2D.RemoveRow(num);
				array2D.RemoveColumn(num);
			}
			return list2;
		}

		private static int FindNodeIndexWithNoDependencies(Array2D<int> adjacencyMatrix)
		{
			for (int i = 0; i < adjacencyMatrix.Width; i++)
			{
				bool flag = false;
				for (int j = 0; j < adjacencyMatrix.Height; j++)
				{
					if (adjacencyMatrix.GetAt(i, j) > 0)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
