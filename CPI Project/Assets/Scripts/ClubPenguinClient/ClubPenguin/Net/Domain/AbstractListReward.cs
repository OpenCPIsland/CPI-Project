using LitJson;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public abstract class AbstractListReward<T> : IRewardable
	{
		protected List<T> data;

		public abstract string RewardType
		{
			get;
		}

		public object Reward
		{
			get
			{
				return data;
			}
		}

		public AbstractListReward()
		{
			data = new List<T>();
		}

		public AbstractListReward(T value)
			: this()
		{
			data.Add(value);
		}

		public void FromJson(JsonData jsonData)
		{
			data = JsonMapper.ToObject<List<T>>(jsonData.ToJson());
		}

		public void Add(IRewardable reward)
		{
			data = combineLists(data, ((AbstractListReward<T>)reward).data);
		}

		public bool IsEmpty()
		{
			return data == null || data.Count == 0;
		}

		public static List<V> combineLists<V>(List<V> set1, List<V> set2)
		{
			if (set1 == null && set2 == null)
			{
				return null;
			}
			if (set1 == null)
			{
				return new List<V>(set2);
			}
			if (set2 == null)
			{
				return new List<V>(set1);
			}
			HashSet<V> hashSet = new HashSet<V>(set1);
			hashSet.UnionWith(set2);
			return new List<V>(hashSet);
		}
	}
}
