using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Builders
{
	public class PairwiseStrategy : CombiningStrategy
	{
		internal class FleaRand
		{
			private const int FleaRandSize = 256;

			private uint b;

			private uint c;

			private uint d;

			private uint z;

			private uint[] m = new uint[256];

			private uint[] r = new uint[256];

			private uint q;

			public FleaRand(uint seed)
			{
				b = seed;
				c = seed;
				d = seed;
				z = seed;
				for (int i = 0; i < m.Length; i++)
				{
					m[i] = seed;
				}
				for (int i = 0; i < 10; i++)
				{
					Batch();
				}
				q = 0u;
			}

			public uint Next()
			{
				if (q == 0)
				{
					Batch();
					q = (uint)(r.Length - 1);
				}
				else
				{
					q--;
				}
				return r[q];
			}

			private void Batch()
			{
				uint num = b;
				uint num2 = c + ++z;
				uint num3 = d;
				for (int i = 0; i < r.Length; i++)
				{
					uint num4 = m[(long)num % (long)m.Length];
					m[(long)num % (long)m.Length] = num3;
					num3 = (num2 << 19) + (num2 >> 13) + num;
					num2 = (num ^ m[i]);
					num = num4 + num3;
					r[i] = num2;
				}
				b = num;
				c = num2;
				d = num3;
			}
		}

		internal class FeatureInfo
		{
			public const string Names = "abcdefghijklmnopqrstuvwxyz";

			public readonly int Dimension;

			public readonly int Feature;

			public FeatureInfo(int dimension, int feature)
			{
				Dimension = dimension;
				Feature = feature;
			}

			public override string ToString()
			{
				return (Dimension + 1).ToString() + "abcdefghijklmnopqrstuvwxyz"[Feature];
			}
		}

		internal class Tuple
		{
			private readonly List<FeatureInfo> features = new List<FeatureInfo>();

			public int Count
			{
				get
				{
					return features.Count;
				}
			}

			public FeatureInfo this[int index]
			{
				get
				{
					return features[index];
				}
			}

			public void Add(FeatureInfo feature)
			{
				features.Add(feature);
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append('(');
				for (int i = 0; i < features.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(' ');
					}
					stringBuilder.Append(features[i].ToString());
				}
				stringBuilder.Append(')');
				return stringBuilder.ToString();
			}
		}

		internal class TupleCollection
		{
			private readonly List<Tuple> tuples = new List<Tuple>();

			public int Count
			{
				get
				{
					return tuples.Count;
				}
			}

			public Tuple this[int index]
			{
				get
				{
					return tuples[index];
				}
			}

			public void Add(Tuple tuple)
			{
				tuples.Add(tuple);
			}

			public void RemoveAt(int index)
			{
				tuples.RemoveAt(index);
			}
		}

		internal class TestCase
		{
			public readonly int[] Features;

			public TestCase(int numberOfDimensions)
			{
				Features = new int[numberOfDimensions];
			}

			public bool IsTupleCovered(Tuple tuple)
			{
				for (int i = 0; i < tuple.Count; i++)
				{
					if (Features[tuple[i].Dimension] != tuple[i].Feature)
					{
						return false;
					}
				}
				return true;
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < Features.Length; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(' ');
					}
					stringBuilder.Append(i + 1);
					stringBuilder.Append("abcdefghijklmnopqrstuvwxyz"[Features[i]]);
				}
				return stringBuilder.ToString();
			}
		}

		internal class TestCaseCollection : IEnumerable
		{
			private readonly List<TestCase> testCases = new List<TestCase>();

			public void Add(TestCase testCase)
			{
				testCases.Add(testCase);
			}

			public IEnumerator GetEnumerator()
			{
				return testCases.GetEnumerator();
			}

			public bool IsTupleCovered(Tuple tuple)
			{
				foreach (TestCase testCase in testCases)
				{
					if (testCase.IsTupleCovered(tuple))
					{
						return true;
					}
				}
				return false;
			}
		}

		internal class PairwiseTestCaseGenerator
		{
			private const int MaxTupleLength = 2;

			private readonly FleaRand random = new FleaRand(0u);

			private readonly int[] dimensions;

			private readonly TupleCollection[][] uncoveredTuples;

			private readonly int[][] currentTupleLength;

			private readonly TestCaseCollection testCases = new TestCaseCollection();

			public PairwiseTestCaseGenerator(int[] dimensions)
			{
				this.dimensions = dimensions;
				uncoveredTuples = new TupleCollection[this.dimensions.Length][];
				for (int i = 0; i < uncoveredTuples.Length; i++)
				{
					uncoveredTuples[i] = new TupleCollection[this.dimensions[i]];
					for (int j = 0; j < this.dimensions[i]; j++)
					{
						uncoveredTuples[i][j] = new TupleCollection();
					}
				}
				currentTupleLength = new int[this.dimensions.Length][];
				for (int i = 0; i < this.dimensions.Length; i++)
				{
					currentTupleLength[i] = new int[this.dimensions[i]];
				}
			}

			public IEnumerable GetTestCases()
			{
				CreateTestCases();
				SelfTest();
				return testCases;
			}

			private void CreateTestCases()
			{
				while (true)
				{
					bool flag = true;
					ExtendTupleSet();
					Tuple tuple = FindTupleToCover();
					if (tuple == null)
					{
						break;
					}
					TestCase testCase = FindGoodTestCase(tuple);
					RemoveTuplesCoveredBy(testCase);
					testCases.Add(testCase);
				}
			}

			private void ExtendTupleSet()
			{
				for (int i = 0; i < dimensions.Length; i++)
				{
					for (int j = 0; j < dimensions[i]; j++)
					{
						ExtendTupleSet(i, j);
					}
				}
			}

			private void ExtendTupleSet(int dimension, int feature)
			{
				if (uncoveredTuples[dimension][feature].Count > 0 || currentTupleLength[dimension][feature] == 2)
				{
					return;
				}
				currentTupleLength[dimension][feature]++;
				int num = currentTupleLength[dimension][feature];
				if (num == 1)
				{
					Tuple tuple = new Tuple();
					tuple.Add(new FeatureInfo(dimension, feature));
					if (!testCases.IsTupleCovered(tuple))
					{
						uncoveredTuples[dimension][feature].Add(tuple);
					}
					return;
				}
				for (int i = 0; i < dimensions.Length; i++)
				{
					for (int j = 0; j < dimensions[i]; j++)
					{
						Tuple tuple = new Tuple();
						tuple.Add(new FeatureInfo(i, j));
						if (tuple[0].Dimension != dimension)
						{
							tuple.Add(new FeatureInfo(dimension, feature));
							if (!testCases.IsTupleCovered(tuple))
							{
								uncoveredTuples[dimension][feature].Add(tuple);
							}
						}
					}
				}
			}

			private Tuple FindTupleToCover()
			{
				int num = 2;
				int num2 = 0;
				Tuple result = null;
				for (int i = 0; i < dimensions.Length; i++)
				{
					for (int j = 0; j < dimensions[i]; j++)
					{
						if (currentTupleLength[i][j] < num)
						{
							num = currentTupleLength[i][j];
							num2 = uncoveredTuples[i][j].Count;
							result = uncoveredTuples[i][j][0];
						}
						else if (currentTupleLength[i][j] == num && uncoveredTuples[i][j].Count > num2)
						{
							num2 = uncoveredTuples[i][j].Count;
							result = uncoveredTuples[i][j][0];
						}
					}
				}
				return result;
			}

			private TestCase FindGoodTestCase(Tuple tuple)
			{
				TestCase result = null;
				int num = -1;
				for (int i = 0; i < 5; i++)
				{
					TestCase testCase = new TestCase(dimensions.Length);
					int num2 = CreateTestCase(tuple, testCase);
					if (num2 > num)
					{
						result = testCase;
						num = num2;
					}
				}
				return result;
			}

			private int CreateTestCase(Tuple tuple, TestCase test)
			{
				for (int i = 0; i < test.Features.Length; i++)
				{
					test.Features[i] = (int)((long)random.Next() % (long)dimensions[i]);
				}
				for (int i = 0; i < tuple.Count; i++)
				{
					test.Features[tuple[i].Dimension] = tuple[i].Feature;
				}
				return MaximizeCoverage(test, tuple);
			}

			private int MaximizeCoverage(TestCase test, Tuple tuple)
			{
				int[] mutableDimensions = GetMutableDimensions(tuple);
				bool flag2;
				int num;
				do
				{
					bool flag = true;
					flag2 = false;
					num = 1;
					for (int num2 = mutableDimensions.Length; num2 > 1; num2--)
					{
						int num3 = (int)((long)random.Next() % (long)num2);
						int num4 = mutableDimensions[num2 - 1];
						mutableDimensions[num2 - 1] = mutableDimensions[num3];
						mutableDimensions[num3] = num4;
					}
					foreach (int num5 in mutableDimensions)
					{
						List<int> list = new List<int>();
						int num6 = CountTuplesCovered(test, num5, test.Features[num5]);
						int num7 = currentTupleLength[num5][test.Features[num5]];
						for (int i = 0; i < dimensions[num5]; i++)
						{
							test.Features[num5] = i;
							int num8 = CountTuplesCovered(test, num5, i);
							if (currentTupleLength[num5][i] < num7)
							{
								flag2 = true;
								num7 = currentTupleLength[num5][i];
								num6 = num8;
								list.Clear();
								list.Add(i);
							}
							else if (currentTupleLength[num5][i] == num7 && num8 >= num6)
							{
								if (num8 > num6)
								{
									flag2 = true;
									num6 = num8;
									list.Clear();
								}
								list.Add(i);
							}
						}
						if (list.Count == 1)
						{
							test.Features[num5] = list[0];
						}
						else
						{
							test.Features[num5] = list[(int)((long)random.Next() % (long)list.Count)];
						}
						num += num6;
					}
				}
				while (flag2);
				return num;
			}

			private int[] GetMutableDimensions(Tuple tuple)
			{
				bool[] array = new bool[dimensions.Length];
				for (int i = 0; i < tuple.Count; i++)
				{
					array[tuple[i].Dimension] = true;
				}
				List<int> list = new List<int>();
				for (int i = 0; i < dimensions.Length; i++)
				{
					if (!array[i])
					{
						list.Add(i);
					}
				}
				return list.ToArray();
			}

			private int CountTuplesCovered(TestCase test, int dimension, int feature)
			{
				int num = 0;
				TupleCollection tupleCollection = uncoveredTuples[dimension][feature];
				for (int i = 0; i < tupleCollection.Count; i++)
				{
					if (test.IsTupleCovered(tupleCollection[i]))
					{
						num++;
					}
				}
				return num;
			}

			private void RemoveTuplesCoveredBy(TestCase testCase)
			{
				for (int i = 0; i < uncoveredTuples.Length; i++)
				{
					for (int j = 0; j < uncoveredTuples[i].Length; j++)
					{
						TupleCollection tupleCollection = uncoveredTuples[i][j];
						for (int num = tupleCollection.Count - 1; num >= 0; num--)
						{
							if (testCase.IsTupleCovered(tupleCollection[num]))
							{
								tupleCollection.RemoveAt(num);
							}
						}
					}
				}
			}

			private void SelfTest()
			{
				for (int i = 0; i < dimensions.Length - 1; i++)
				{
					for (int j = i + 1; j < dimensions.Length; j++)
					{
						for (int k = 0; k < dimensions[i]; k++)
						{
							for (int l = 0; l < dimensions[j]; l++)
							{
								Tuple tuple = new Tuple();
								tuple.Add(new FeatureInfo(i, k));
								tuple.Add(new FeatureInfo(j, l));
								if (!testCases.IsTupleCovered(tuple))
								{
									throw new ApplicationException("PairwiseStrategy self-test failed : Not all pairs are covered!");
								}
							}
						}
					}
				}
			}
		}

		public PairwiseStrategy(IEnumerable[] sources)
			: base(sources)
		{
		}

		public override IEnumerable<ITestCaseData> GetTestCases()
		{
			List<ITestCaseData> list = new List<ITestCaseData>();
			ObjectList[] array = CreateValueSet();
			int[] dimensions = CreateDimensions(array);
			IEnumerable testCases = new PairwiseTestCaseGenerator(dimensions).GetTestCases();
			foreach (TestCase item in testCases)
			{
				object[] array2 = new object[item.Features.Length];
				for (int i = 0; i < item.Features.Length; i++)
				{
					array2[i] = array[i][item.Features[i]];
				}
				ParameterSet parameterSet = new ParameterSet();
				parameterSet.Arguments = array2;
				list.Add(parameterSet);
			}
			return list;
		}

		private ObjectList[] CreateValueSet()
		{
			ObjectList[] array = new ObjectList[base.Sources.Length];
			for (int i = 0; i < array.Length; i++)
			{
				ObjectList objectList = new ObjectList();
				foreach (object item in base.Sources[i])
				{
					objectList.Add(item);
				}
				array[i] = objectList;
			}
			return array;
		}

		private int[] CreateDimensions(ObjectList[] valueSet)
		{
			int[] array = new int[valueSet.Length];
			for (int i = 0; i < valueSet.Length; i++)
			{
				array[i] = valueSet[i].Count;
			}
			return array;
		}
	}
}
