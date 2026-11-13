using Disney.LaunchPadFramework.Utility.Collections;
using System;
using System.Collections.Generic;

namespace Disney.LaunchPadFramework.Utility.Algorithms
{
	public static class TopologicalSort
	{
		public static List<ITopologicalNode> Sort(List<ITopologicalNode> unsortedList)
		{
			unsortedList.Sort(delegate(ITopologicalNode node1, ITopologicalNode node2)
			{
				if (node1 == null || node1.TopologicalIdentifier == null)
				{
					return -1;
				}
				return (node2 == null || node2.TopologicalIdentifier == null) ? 1 : node1.TopologicalIdentifier.CompareTo(node2.TopologicalIdentifier);
			});
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
				if (unsortedList[i].TopologicalDependencies == null)
				{
					continue;
				}
				for (int j = 0; j < unsortedList[i].TopologicalDependencies.Count; j++)
				{
					string text = unsortedList[i].TopologicalDependencies[j];
					int value;
					if (dictionary.TryGetValue(text, out value))
					{
						array2D.SetAt(i, value, 1);
						continue;
					}
					throw new KeyNotFoundException("Missing dependency: " + text);
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
